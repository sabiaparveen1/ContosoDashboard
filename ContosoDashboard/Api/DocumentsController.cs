using ContosoDashboard.Services.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContosoDashboard.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(IDocumentService documentService, ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid user context");
        }
        return userId;
    }

    [HttpGet("{documentId}/download")]
    public async Task<IActionResult> Download(int documentId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var stream = await _documentService.DownloadAsync(documentId, userId, cancellationToken);

            var document = await _documentService.GetDocumentByIdAsync(documentId, userId, cancellationToken);
            if (document == null)
            {
                return NotFound("Document not found");
            }

            return File(stream, document.MimeType, document.FileName);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading document {DocumentId}", documentId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{documentId}/preview")]
    public async Task<IActionResult> Preview(int documentId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var document = await _documentService.GetDocumentByIdAsync(documentId, userId, cancellationToken);
            if (document == null)
            {
                return NotFound("Document not found");
            }

            // Only allow preview for certain file types
            var allowedPreviewTypes = new[] { ".pdf", ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".txt" };
            if (!allowedPreviewTypes.Contains(document.FileExtension.ToLowerInvariant()))
            {
                return BadRequest("Preview not available for this file type");
            }

            var stream = await _documentService.DownloadAsync(documentId, userId, cancellationToken);
            return File(stream, document.MimeType);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing document {DocumentId}", documentId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
        [FromForm] string title,
        [FromForm] string category,
        [FromForm] string? description,
        [FromForm] int? projectId,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Title is required");
            }

            var userId = GetCurrentUserId();

            using (var stream = file.OpenReadStream())
            {
                var document = await _documentService.UploadAsync(
                    stream,
                    file.FileName,
                    title,
                    category,
                    description,
                    projectId,
                    userId,
                    cancellationToken);

                return CreatedAtAction(nameof(Download), new { documentId = document.DocumentId }, document);
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{documentId}/metadata")]
    public async Task<IActionResult> UpdateMetadata(
        int documentId,
        [FromBody] UpdateMetadataRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var document = await _documentService.UpdateMetadataAsync(
                documentId,
                request.Title,
                request.Description,
                request.Tags,
                userId,
                cancellationToken);

            return Ok(document);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document metadata for {DocumentId}", documentId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{documentId}")]
    public async Task<IActionResult> Delete(int documentId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _documentService.DeleteAsync(documentId, userId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}", documentId);
            return StatusCode(500, "Internal server error");
        }
    }
}

public class UpdateMetadataRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
}
