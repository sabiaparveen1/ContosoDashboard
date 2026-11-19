# Document Upload & Management MVP - Quick Start Guide

## Getting Started

The Document Upload and Management feature is now live in ContosoDashboard!

### For Users

#### 1. Upload a Document

1. Click **Documents** in the navigation menu
2. Click **Upload Document** button
3. Enter a title (required)
4. Choose a category (required)
5. Optionally add a description
6. Select a file (PDF, Word, Excel, images, PowerPoint, UTF-8 text), up to 25 MB
7. Click **Upload Document**. On success, you'll be redirected to My Documents.

#### 2. View Your Documents

1. Click **Documents** in the navigation menu → **My Documents**
2. See all your uploaded documents in a table
3. Documents show: Title, Upload Date, File Size, File Type
4. Click the **eye icon** to view document details
5. Click the **download icon** to download the file

#### 3. View Document Details

1. From My Documents, click on a document title or the eye icon
2. View complete metadata:
   - File information (name, size, type)
   - Who uploaded it and when
   - Associated tags
   - Share status

#### 4. Download a File

**Option A**: From My Documents table
- Click the download icon (↓)

**Option B**: From Document Detail
- Click the **Download** button
- For PDF/images/text: Click **Preview** to view in browser

#### 5. Edit Document Metadata

1. Open a document detail page
2. Click **Edit** button
3. Update:
   - Title
   - Description
   - Tags (comma-separated, e.g., "important, project-x, urgent")
4. Click **Save Changes**

#### 6. Delete a Document

1. Open a document detail page
2. Click **Delete** button
3. Confirm deletion in the modal
4. Document is permanently deleted (no restore available)

#### 7. Share a Document (Coming Soon - Phase 4)

Currently, documents are accessible via:
- **Personal documents**: Only you can access
- **Project documents**: Project members can access
- Full sharing features coming in Phase 4

---

## For Administrators

### Configuration

**File Storage Location**  
Edit `appsettings.json`:
```json
{
  "FileStorage": {
    "LocalPath": "C:\\DocumentStore"  // or any path
  }
}
```

**Allowed File Types** (hardcoded - change in DocumentService.cs)
- `.pdf` - PDFs
- `.docx`, `.doc` - Microsoft Word
- `.xlsx`, `.xls` - Microsoft Excel
- `.txt` - Text files
- `.png`, `.jpg`, `.jpeg`, `.gif`, `.bmp` - Images
- `.pptx`, `.ppt` - PowerPoint

**Max File Size** (hardcoded - change in DocumentService.cs)
- 25 MB (26,214,400 bytes)

### Monitoring

**Database Tables to Monitor**
- `Documents` - Document metadata and status
- `DocumentActivities` - Audit log of all operations
- `DocumentShares` - Share records (Phase 4)
- `DocumentTags` - Searchable tags

**Key Audit Log Actions**
- `Uploaded` - File uploaded
- `Downloaded` - File downloaded (tracks access)
- `MetadataEdited` - Metadata updated
- `Deleted` - Document deleted
- `Shared` - Shared with another user (Phase 4)
- `ShareRevoked` - Share revoked (Phase 4)

**Example Query - Recent Activity**
```sql
SELECT TOP 100 
  da.DocumentActivityId,
  da.Action,
  da.PerformedDate,
  u.DisplayName,
  da.Details
FROM DocumentActivities da
JOIN Users u ON da.PerformedByUserId = u.UserId
ORDER BY da.PerformedDate DESC;
```

### Support Troubleshooting

**Q: User reports "File failed malware scan"**  
A: The app uses a test scanner that detects the EICAR test string; it does not call Windows Defender or establish that a file is safe. For local training only, set `FileScanning__Enabled=false` before starting the app with the `http` launch profile. This disables the test scanner in Development but leaves file-size and content checks enabled.

**Q: Upload shows "File size exceeds maximum"**  
A: The file is larger than 25 MB. Compress or split the file before uploading.

**Q: Document appears in audit log but not in list**  
A: Document may be soft-deleted. Check `IsDeleted` flag in Documents table.

**Q: "Concurrency conflict" error on metadata edit**  
A: Document was edited by another user simultaneously. User should refresh and try again.

**Q: Share not working (Phase 4+)**  
A: Verify recipient user exists in Users table and is active. Check DocumentShares table for active share record (IsActive = 1).

---

## API Usage (For Developers)

### Authentication
All endpoints require authenticated user (cookie-based, training mode).

### Upload File
```http
POST /api/documents/upload HTTP/1.1
Content-Type: multipart/form-data

title=My Document
description=A sample document
projectId=1  (optional)
file=@document.pdf
```

**Response (201 Created)**
```json
{
  "documentId": 42,
  "fileName": "document.pdf",
  "title": "My Document",
  "fileExtension": ".pdf",
  "fileSizeBytes": 102400,
  "uploadedDate": "2026-11-19T15:35:00Z"
}
```

### Download File
```http
GET /api/documents/42/download HTTP/1.1
```

**Response**: File stream with appropriate MIME type

### Preview File
```http
GET /api/documents/42/preview HTTP/1.1
```

**Supported types**: .pdf, .png, .jpg, .jpeg, .gif, .bmp, .txt

### Update Metadata
```http
PUT /api/documents/42/metadata HTTP/1.1
Content-Type: application/json

{
  "title": "Updated Title",
  "description": "Updated description",
  "tags": ["important", "project-x", "urgent"]
}
```

### Delete Document
```http
DELETE /api/documents/42 HTTP/1.1
```

**Response (204 No Content)**

---

## Security Features

✅ **Implemented**
- Authorization checks on every operation
- File type validation (extension + MIME type)
- File size limits
- Pre-upload malware scanning
- Optimistic concurrency for safe updates
- Soft delete for audit preservation
- Security headers (X-Content-Type-Options: nosniff)
- Audit log of all operations

---

## Known Limitations (MVP Only)

- Sharing features coming in Phase 4
- Search/filtering basic (advanced in Phase 5)
- No task integration yet (Phase 6)
- Preview limited to certain file types
- No versioning/restore features
- Local filesystem storage only (Phase 2 uses local storage)

---

## Support & Feedback

**Issues or Feature Requests**
Contact: Documentation in `/docs/PERFORMANCE.md`

**MVP Status**: Ready for UAT  
**Last Updated**: 2026-11-19  
**Version**: 1.0.0 (MVP)
