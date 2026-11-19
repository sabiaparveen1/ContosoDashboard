namespace ContosoDashboard.Models;

public class Document
{
    public int DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileStoragePath { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = DocumentCategories.Other;
    public string? Description { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    
    public int UploadedByUserId { get; set; }
    public int? ProjectId { get; set; }
    
    public DateTime UploadedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    
    // Navigation properties
    public virtual User UploadedByUser { get; set; } = null!;
    public virtual Project? Project { get; set; }
    public virtual ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
    public virtual ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
    public virtual ICollection<TaskDocument> TaskDocuments { get; set; } = new List<TaskDocument>();
    public virtual ICollection<DocumentActivity> ActivityLog { get; set; } = new List<DocumentActivity>();
}

public class DocumentTag
{
    public int DocumentTagId { get; set; }
    public int DocumentId { get; set; }
    public int TagId { get; set; }
    
    public virtual Document Document { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}

public class Tag
{
    public int TagId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    
    public virtual ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
}

public class DocumentShare
{
    public int DocumentShareId { get; set; }
    public int DocumentId { get; set; }
    public int SharedWithUserId { get; set; }
    public int SharedByUserId { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    
    public DateTime SharedDate { get; set; }
    public DateTime? RevokedDate { get; set; }
    public bool IsActive { get; set; }
    
    public virtual Document Document { get; set; } = null!;
    public virtual User SharedWithUser { get; set; } = null!;
    public virtual User SharedByUser { get; set; } = null!;
}

public class TaskDocument
{
    public int TaskDocumentId { get; set; }
    public int TaskId { get; set; }
    public int DocumentId { get; set; }
    
    public DateTime AttachedDate { get; set; }
    public int AttachedByUserId { get; set; }
    
    public virtual TaskItem Task { get; set; } = null!;
    public virtual Document Document { get; set; } = null!;
    public virtual User AttachedByUser { get; set; } = null!;
}

public class DocumentActivity
{
    public int DocumentActivityId { get; set; }
    public int DocumentId { get; set; }
    public int PerformedByUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime PerformedDate { get; set; }
    
    public virtual Document? Document { get; set; }
    public virtual User PerformedByUser { get; set; } = null!;
}

public enum DocumentAction
{
    Uploaded,
    Downloaded,
    MetadataEdited,
    Tagged,
    Shared,
    ShareRevoked,
    Deleted,
    Restored
}
