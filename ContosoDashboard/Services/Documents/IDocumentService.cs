using ContosoDashboard.Models;

namespace ContosoDashboard.Services.Documents;

public interface IDocumentService
{
    // Authorization and Query Methods
    Task<List<Document>> GetMyDocumentsAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<Document>> GetProjectDocumentsAsync(int projectId, int userId, CancellationToken cancellationToken = default);
    Task<List<Document>> GetSharedWithMeAsync(int userId, CancellationToken cancellationToken = default);
    Task<Document?> GetDocumentByIdAsync(int documentId, int userId, CancellationToken cancellationToken = default);
    
    // Upload and File Management
    Task<Document> UploadAsync(Stream fileStream, string fileName, string title, string category, string? description, int? projectId, int uploadedByUserId, CancellationToken cancellationToken = default);
    Task DeleteAsync(int documentId, int userId, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(int documentId, int userId, CancellationToken cancellationToken = default);
    
    // Metadata Management
    Task<Document> UpdateMetadataAsync(int documentId, string title, string? description, List<string>? tags, int userId, CancellationToken cancellationToken = default);
    
    // Sharing
    Task<DocumentShare> ShareAsync(int documentId, int sharedWithUserId, int sharedByUserId, CancellationToken cancellationToken = default);
    Task RevokeShareAsync(int shareId, int userId, CancellationToken cancellationToken = default);
    
    // Search and Filtering
    Task<List<Document>> SearchAsync(int userId, string? searchQuery, int? projectId, List<string>? tags, DateTime? dateFrom, DateTime? dateTo, CancellationToken cancellationToken = default);
    Task<List<Document>> GetListAsync(int userId, int? projectId, string? sortBy = "date", bool descending = true, CancellationToken cancellationToken = default);
    
    // Task Integration
    Task<TaskDocument> AttachToTaskAsync(int taskId, int documentId, int attachedByUserId, CancellationToken cancellationToken = default);
    Task<List<Document>> GetTaskDocumentsAsync(int taskId, int userId, CancellationToken cancellationToken = default);
    
    // Recent Documents
    Task<List<Document>> GetRecentDocumentsAsync(int userId, int count = 5, CancellationToken cancellationToken = default);
}
