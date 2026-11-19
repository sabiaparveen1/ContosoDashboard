using ContosoDashboard.Data;
using ContosoDashboard.Models;
using ContosoDashboard.Services.Storage;
using ContosoDashboard.Services.Scanning;
using Microsoft.EntityFrameworkCore;

namespace ContosoDashboard.Services.Documents;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _storageService;
    private readonly IFileMalwareScanner _malwareScanner;
    private readonly ILogger<DocumentService> _logger;
    public const int MaxFileSizeBytes = 25 * 1024 * 1024;

    public DocumentService(ApplicationDbContext context, IFileStorageService storageService, IFileMalwareScanner malwareScanner, ILogger<DocumentService> logger)
    {
        _context = context;
        _storageService = storageService;
        _malwareScanner = malwareScanner;
        _logger = logger;
    }

    // Authorization Methods
    private async Task<bool> IsOwnerAsync(int documentId, int userId)
    {
        var doc = await _context.Documents.AsNoTracking().FirstOrDefaultAsync(d => d.DocumentId == documentId);
        return doc?.UploadedByUserId == userId;
    }

    private async Task<bool> IsProjectMemberAsync(int projectId, int userId)
    {
        var isMember = await _context.ProjectMembers.AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        var isManager = await _context.Projects.AnyAsync(p => p.ProjectId == projectId && p.ProjectManagerId == userId);
        return isMember || isManager;
    }

    private async Task<bool> IsProjectManagerAsync(int projectId, int userId)
    {
        return await _context.Projects.AnyAsync(p => p.ProjectId == projectId && p.ProjectManagerId == userId);
    }

    private async Task<bool> IsShareRecipientAsync(int documentId, int userId)
    {
        return await _context.DocumentShares.AnyAsync(ds => ds.DocumentId == documentId && ds.SharedWithUserId == userId && ds.IsActive);
    }

    private async Task<bool> CanAccessDocumentAsync(int documentId, int userId)
    {
        var doc = await _context.Documents.AsNoTracking().FirstOrDefaultAsync(d => d.DocumentId == documentId && !d.IsDeleted);
        if (doc == null) return false;

        // Owner can always access
        if (doc.UploadedByUserId == userId) return true;

        // Share recipient can access
        if (await IsShareRecipientAsync(documentId, userId)) return true;

        // Project members can access project documents
        if (doc.ProjectId.HasValue && await IsProjectMemberAsync(doc.ProjectId.Value, userId)) return true;

        return false;
    }

    public async Task<List<Document>> GetMyDocumentsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .AsNoTracking()
            .Where(d => d.UploadedByUserId == userId && !d.IsDeleted)
            .OrderByDescending(d => d.UploadedDate)
            .Include(d => d.DocumentTags).ThenInclude(dt => dt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Document>> GetProjectDocumentsAsync(int projectId, int userId, CancellationToken cancellationToken = default)
    {
        if (!await IsProjectMemberAsync(projectId, userId))
        {
            _logger.LogWarning("User {UserId} attempted to access documents for project {ProjectId}", userId, projectId);
            return new List<Document>();
        }

        return await _context.Documents
            .AsNoTracking()
            .Where(d => d.ProjectId == projectId && !d.IsDeleted)
            .OrderByDescending(d => d.UploadedDate)
            .Include(d => d.DocumentTags).ThenInclude(dt => dt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Document>> GetSharedWithMeAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentShares
            .AsNoTracking()
            .Where(ds => ds.SharedWithUserId == userId && ds.IsActive)
            .Select(ds => ds.Document)
            .Where(d => !d.IsDeleted)
            .OrderByDescending(d => d.UploadedDate)
            .Include(d => d.DocumentTags).ThenInclude(dt => dt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<Document?> GetDocumentByIdAsync(int documentId, int userId, CancellationToken cancellationToken = default)
    {
        if (!await CanAccessDocumentAsync(documentId, userId))
        {
            _logger.LogWarning("User {UserId} attempted to access document {DocumentId}", userId, documentId);
            return null;
        }

        return await _context.Documents
            .AsNoTracking()
            .Include(d => d.UploadedByUser)
            .Include(d => d.Project)
            .Include(d => d.DocumentTags).ThenInclude(dt => dt.Tag)
            .Include(d => d.Shares.Where(s => s.IsActive)).ThenInclude(s => s.SharedWithUser)
            .FirstOrDefaultAsync(d => d.DocumentId == documentId, cancellationToken);
    }

    public async Task<Document> UploadAsync(Stream fileStream, string fileName, string title, string category, string? description, int? projectId, int uploadedByUserId, CancellationToken cancellationToken = default)
    {
        // Validation
        if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name cannot be empty", nameof(fileName));
        if (string.IsNullOrWhiteSpace(title)) throw new InvalidOperationException("Title is required.");
        if (!DocumentCategories.All.Contains(category)) throw new InvalidOperationException("Select a valid document category.");

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var mimeType = GetMimeType(ext);
        if (mimeType is null) throw new InvalidOperationException("This file extension is not allowed.");

        // Verify project access if project document
        if (projectId.HasValue && !await IsProjectMemberAsync(projectId.Value, uploadedByUserId))
        {
            throw new UnauthorizedAccessException($"User does not have access to project {projectId}");
        }

        using var stagedFile = new MemoryStream();
        var buffer = new byte[81920];
        int bytesRead;
        while ((bytesRead = await fileStream.ReadAsync(buffer.AsMemory(), cancellationToken)) > 0)
        {
            if (stagedFile.Length + bytesRead > MaxFileSizeBytes)
                throw new InvalidOperationException("File size exceeds the maximum allowed size of 25 MB.");

            stagedFile.Write(buffer, 0, bytesRead);
        }

        var fileSizeBytes = stagedFile.Length;
        if (fileSizeBytes == 0) throw new InvalidOperationException("File cannot be empty.");

        stagedFile.Position = 0;
        if (!await DocumentFileContent.MatchesExtensionAsync(stagedFile, ext, cancellationToken))
            throw new InvalidOperationException($"File content does not match the {ext} extension.");

        stagedFile.Position = 0;
        if (!await _malwareScanner.ScanAsync(stagedFile, fileName, cancellationToken))
            throw new InvalidOperationException("File failed malware scan.");

        stagedFile.Position = 0;
        var storagePath = await _storageService.UploadAsync(stagedFile, fileName, ext, cancellationToken);

        // Create document record
        var document = new Document
        {
            FileName = fileName,
            FileExtension = ext,
            FileSizeBytes = fileSizeBytes,
            FileStoragePath = storagePath,
            Title = title,
            Category = category,
            Description = description,
            MimeType = mimeType,
            UploadedByUserId = uploadedByUserId,
            ProjectId = projectId,
            UploadedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        // Log activity
        await LogActivityAsync(document.DocumentId, uploadedByUserId, "Uploaded", $"File: {fileName}", cancellationToken);

        _logger.LogInformation("Document uploaded successfully: {DocumentId} by user {UserId}", document.DocumentId, uploadedByUserId);
        return document;
    }

    public async Task DeleteAsync(int documentId, int userId, CancellationToken cancellationToken = default)
    {
        var doc = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId, cancellationToken);
        if (doc == null) throw new KeyNotFoundException($"Document {documentId} not found");

        if (doc.UploadedByUserId != userId) throw new UnauthorizedAccessException("Only the document owner can delete it");

        // Delete file from storage
        try
        {
            await _storageService.DeleteAsync(doc.FileStoragePath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from storage: {StoragePath}", doc.FileStoragePath);
        }

        // Mark as deleted (soft delete)
        doc.IsDeleted = true;
        doc.DeletedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Log activity
        await LogActivityAsync(documentId, userId, "Deleted", null, cancellationToken);

        _logger.LogInformation("Document deleted: {DocumentId} by user {UserId}", documentId, userId);
    }

    public async Task<Stream> DownloadAsync(int documentId, int userId, CancellationToken cancellationToken = default)
    {
        if (!await CanAccessDocumentAsync(documentId, userId))
        {
            throw new UnauthorizedAccessException($"User does not have access to document {documentId}");
        }

        var doc = await _context.Documents.AsNoTracking().FirstOrDefaultAsync(d => d.DocumentId == documentId, cancellationToken);
        if (doc == null) throw new KeyNotFoundException($"Document {documentId} not found");

        // Log activity
        await LogActivityAsync(documentId, userId, "Downloaded", null, cancellationToken);

        return await _storageService.DownloadAsync(doc.FileStoragePath, cancellationToken);
    }

    public async Task<Document> UpdateMetadataAsync(int documentId, string title, string? description, List<string>? tags, int userId, CancellationToken cancellationToken = default)
    {
        var doc = await _context.Documents
            .Include(d => d.DocumentTags).ThenInclude(dt => dt.Tag)
            .FirstOrDefaultAsync(d => d.DocumentId == documentId, cancellationToken);

        if (doc == null) throw new KeyNotFoundException($"Document {documentId} not found");
        if (doc.UploadedByUserId != userId) throw new UnauthorizedAccessException("Only the document owner can update metadata");

        try
        {
            doc.Title = title;
            doc.Description = description;
            doc.UpdatedDate = DateTime.UtcNow;

            // Update tags if provided
            if (tags != null)
            {
                _context.DocumentTags.RemoveRange(doc.DocumentTags);

                foreach (var tagName in tags.Distinct())
                {
                    var normalized = tagName.Trim().ToLowerInvariant();
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.NormalizedName == normalized, cancellationToken);
                    
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName.Trim(), NormalizedName = normalized };
                        _context.Tags.Add(tag);
                    }

                    doc.DocumentTags.Add(new DocumentTag { Document = doc, Tag = tag });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            await LogActivityAsync(documentId, userId, "MetadataEdited", $"Title: {title}", cancellationToken);

            _logger.LogInformation("Document metadata updated: {DocumentId} by user {UserId}", documentId, userId);
            return doc;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict updating document {DocumentId}", documentId);
            throw new InvalidOperationException("Document was modified by another user. Please refresh and try again.");
        }
    }

    public async Task<DocumentShare> ShareAsync(int documentId, int sharedWithUserId, int sharedByUserId, CancellationToken cancellationToken = default)
    {
        var doc = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId, cancellationToken);
        if (doc == null) throw new KeyNotFoundException($"Document {documentId} not found");
        if (doc.UploadedByUserId != sharedByUserId) throw new UnauthorizedAccessException("Only the document owner can share it");

        // Check if already shared
        var existingShare = await _context.DocumentShares
            .FirstOrDefaultAsync(ds => ds.DocumentId == documentId && ds.SharedWithUserId == sharedWithUserId && ds.IsActive, cancellationToken);

        if (existingShare != null) return existingShare;

        var share = new DocumentShare
        {
            DocumentId = documentId,
            SharedWithUserId = sharedWithUserId,
            SharedByUserId = sharedByUserId,
            SharedDate = DateTime.UtcNow,
            IsActive = true
        };

        _context.DocumentShares.Add(share);
        await _context.SaveChangesAsync(cancellationToken);

        await LogActivityAsync(documentId, sharedByUserId, "Shared", $"Shared with user {sharedWithUserId}", cancellationToken);

        _logger.LogInformation("Document shared: {DocumentId} with user {SharedWithUserId} by {SharedByUserId}", documentId, sharedWithUserId, sharedByUserId);
        return share;
    }

    public async Task RevokeShareAsync(int shareId, int userId, CancellationToken cancellationToken = default)
    {
        var share = await _context.DocumentShares
            .Include(ds => ds.Document)
            .FirstOrDefaultAsync(ds => ds.DocumentShareId == shareId, cancellationToken);

        if (share == null) throw new KeyNotFoundException($"Share {shareId} not found");
        if (share.Document.UploadedByUserId != userId) throw new UnauthorizedAccessException("Only the document owner can revoke shares");

        try
        {
            share.IsActive = false;
            share.RevokedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            await LogActivityAsync(share.DocumentId, userId, "ShareRevoked", $"Revoked from user {share.SharedWithUserId}", cancellationToken);

            _logger.LogInformation("Document share revoked: {ShareId}", shareId);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict revoking share {ShareId}", shareId);
            throw new InvalidOperationException("Share was modified by another user. Please refresh and try again.");
        }
    }

    public async Task<List<Document>> SearchAsync(int userId, string? searchQuery, int? projectId, List<string>? tags, DateTime? dateFrom, DateTime? dateTo, CancellationToken cancellationToken = default)
    {
        var query = GetAccessibleDocumentsQuery(userId);

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var searchLower = searchQuery.ToLowerInvariant();
            query = query.Where(d => d.Title.ToLower().Contains(searchLower) || d.Description!.ToLower().Contains(searchLower) || d.UploadedByUser.DisplayName.ToLower().Contains(searchLower));
        }

        if (projectId.HasValue)
        {
            query = query.Where(d => d.ProjectId == projectId.Value);
        }

        if (tags != null && tags.Count > 0)
        {
            var normalizedTags = tags.Select(t => t.ToLowerInvariant()).ToList();
            query = query.Where(d => d.DocumentTags.Any(dt => normalizedTags.Contains(dt.Tag.NormalizedName)));
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(d => d.UploadedDate >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(d => d.UploadedDate <= dateTo.Value.AddDays(1));
        }

        return await query
            .OrderByDescending(d => d.UploadedDate)
            .Include(d => d.DocumentTags).ThenInclude(dt => dt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Document>> GetListAsync(int userId, int? projectId, string? sortBy = "date", bool descending = true, CancellationToken cancellationToken = default)
    {
        var query = GetAccessibleDocumentsQuery(userId);

        if (projectId.HasValue)
        {
            query = query.Where(d => d.ProjectId == projectId.Value);
        }

        query = sortBy switch
        {
            "title" => descending ? query.OrderByDescending(d => d.Title) : query.OrderBy(d => d.Title),
            "size" => descending ? query.OrderByDescending(d => d.FileSizeBytes) : query.OrderBy(d => d.FileSizeBytes),
            "category" => descending ? query.OrderByDescending(d => d.FileExtension) : query.OrderBy(d => d.FileExtension),
            _ => descending ? query.OrderByDescending(d => d.UploadedDate) : query.OrderBy(d => d.UploadedDate)
        };

        return await query
            .Include(d => d.DocumentTags).ThenInclude(dt => dt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskDocument> AttachToTaskAsync(int taskId, int documentId, int attachedByUserId, CancellationToken cancellationToken = default)
    {
        if (!await CanAccessDocumentAsync(documentId, attachedByUserId))
        {
            throw new UnauthorizedAccessException($"User does not have access to document {documentId}");
        }

        var attachment = new TaskDocument
        {
            TaskId = taskId,
            DocumentId = documentId,
            AttachedDate = DateTime.UtcNow,
            AttachedByUserId = attachedByUserId
        };

        _context.TaskDocuments.Add(attachment);
        await _context.SaveChangesAsync(cancellationToken);

        await LogActivityAsync(documentId, attachedByUserId, "Attached", $"Attached to task {taskId}", cancellationToken);

        return attachment;
    }

    public async Task<List<Document>> GetTaskDocumentsAsync(int taskId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.TaskDocuments
            .AsNoTracking()
            .Where(td => td.TaskId == taskId)
            .Select(td => td.Document)
            .Where(d => !d.IsDeleted)
            .Include(d => d.DocumentTags).ThenInclude(dt => dt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Document>> GetRecentDocumentsAsync(int userId, int count = 5, CancellationToken cancellationToken = default)
    {
        return await GetAccessibleDocumentsQuery(userId)
            .OrderByDescending(d => d.UploadedDate)
            .Take(count)
            .Include(d => d.DocumentTags).ThenInclude(dt => dt.Tag)
            .ToListAsync(cancellationToken);
    }

    // Helper Methods
    private IQueryable<Document> GetAccessibleDocumentsQuery(int userId)
    {
        return _context.Documents
            .AsNoTracking()
            .Where(d => !d.IsDeleted && (
                d.UploadedByUserId == userId ||
                _context.DocumentShares.Any(ds => ds.DocumentId == d.DocumentId && ds.SharedWithUserId == userId && ds.IsActive) ||
                (d.ProjectId != null && _context.ProjectMembers.Any(pm => pm.ProjectId == d.ProjectId && pm.UserId == userId)) ||
                (d.ProjectId != null && _context.Projects.Any(p => p.ProjectId == d.ProjectId && p.ProjectManagerId == userId))
            ))
            .Include(d => d.UploadedByUser);
    }

    private async Task LogActivityAsync(int documentId, int performedByUserId, string action, string? details, CancellationToken cancellationToken)
    {
        var activity = new DocumentActivity
        {
            DocumentId = documentId,
            PerformedByUserId = performedByUserId,
            Action = action,
            Details = details,
            PerformedDate = DateTime.UtcNow
        };

        _context.DocumentActivities.Add(activity);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging document activity: {Action} for document {DocumentId}", action, documentId);
        }
    }

    private static string? GetMimeType(string extension)
    {
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".doc" => "application/msword",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xls" => "application/vnd.ms-excel",
            ".txt" => "text/plain",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".ppt" => "application/vnd.ms-powerpoint",
            _ => null
        };
    }
}
