# Feature Specification: Document Upload and Management

**Feature Branch**: `001-document-upload-management`

**Created**: 2026-11-19

**Status**: Draft

**Input**: Comprehensive document management system for ContosoDashboard

## User Scenarios & Testing

### User Story 1 - Upload Work Documents to Dashboard (Priority: P1)

Employees need a simple way to upload work-related documents to the dashboard instead of storing them across various locations (local drives, emails, shared drives). This eliminates search friction and security risks.

**Why this priority**: Core value proposition - central document repository. Addresses primary business need of reducing document fragmentation and security issues.

**Independent Test**: Can fully test by: 1) Selecting a file, 2) Adding metadata, 3) Verifying file appears in "My Documents". This alone delivers value - users now have one centralized upload location.

**Acceptance Scenarios**:

1. **Given** user is on document upload page, **When** user selects a supported file (PDF, Word, Excel, PowerPoint, text, images), **Then** file is uploaded with metadata stored successfully
2. **Given** file size under 25 MB, **When** user uploads, **Then** upload completes within 30 seconds
3. **Given** file size exceeds 25 MB, **When** user attempts upload, **Then** system rejects with clear error message
4. **Given** user uploads unsupported file type, **When** upload is attempted, **Then** system rejects with list of supported formats
5. **Given** valid file uploaded, **When** upload completes, **Then** success message displays with file title and category

**Metadata Requirements**:
- Document Title (required, text field)
- Description (optional, text area)
- Category (required, dropdown: Project Documents, Team Resources, Personal Files, Reports, Presentations, Other)
- Associated Project (optional, project picker)
- Tags (optional, comma-separated values)

---

### User Story 2 - Browse and Organize My Documents (Priority: P1)

Users need to easily find their uploaded documents through organization, sorting, and filtering capabilities.

**Why this priority**: Core value proposition alongside upload. Without browsing capability, upload alone doesn't solve document location problem.

**Independent Test**: Can fully test by: 1) Uploading multiple documents with different metadata, 2) Filtering by category/project, 3) Sorting by date/title, 4) Verifying correct results display. Users immediately see value in organized document view.

**Acceptance Scenarios**:

1. **Given** user has uploaded documents, **When** accessing "My Documents" view, **Then** list displays title, category, upload date, file size, associated project
2. **Given** multiple documents exist, **When** user sorts by title, **Then** documents appear in alphabetical order
3. **Given** multiple documents exist with different categories, **When** user filters by category, **Then** only documents in selected category appear
4. **Given** documents with dates spanning multiple projects, **When** user filters by project, **Then** only project documents appear
5. **Given** large document list (500+ documents), **When** page loads, **Then** initial view loads within 2 seconds

---

### User Story 3 - Download and Preview Documents (Priority: P1)

Users need to access documents they've uploaded for review and sharing with others.

**Why this priority**: Essential capability for document management. Without download/preview, stored documents have no utility.

**Independent Test**: Can fully test by: 1) Uploading document, 2) Downloading it, 3) Verifying file integrity. Standalone feature delivering immediate value.

**Acceptance Scenarios**:

1. **Given** document exists, **When** user clicks download, **Then** file downloads with original filename and format
2. **Given** PDF or image file, **When** user clicks preview, **Then** document displays in browser without download
3. **Given** user previews document, **When** viewing preview, **Then** document opens within 3 seconds
4. **Given** user lacks permission to access document, **When** attempting download, **Then** system denies access and shows permission error

---

### User Story 4 - Manage Document Metadata (Priority: P2)

Users need to update document information after upload without re-uploading.

**Why this priority**: Enhances usability after core upload/browse/download workflow established. Important for keeping document information accurate.

**Independent Test**: Can fully test by: 1) Uploading document, 2) Editing metadata fields, 3) Verifying changes persist. Testable independently but builds on Story 1.

**Acceptance Scenarios**:

1. **Given** user uploaded document, **When** accessing document details, **Then** user can edit title, description, category, tags
2. **Given** metadata is modified, **When** user saves changes, **Then** updated metadata persists and appears in document list
3. **Given** new version of document exists, **When** user replaces file, **Then** old version is replaced with new version
4. **Given** user is not document owner, **When** attempting edit, **Then** system denies access

---

### User Story 5 - Search Across All Accessible Documents (Priority: P2)

Users need to quickly find documents across all accessible documents without manual filtering.

**Why this priority**: Improves document discovery experience. Full-text search reduces time to locate documents.

**Independent Test**: Can fully test by: 1) Creating documents with varied metadata, 2) Searching by title/description/tags, 3) Verifying correct results returned within 2 seconds. Independently valuable but enhances earlier stories.

**Acceptance Scenarios**:

1. **Given** multiple documents exist, **When** user searches by document title, **Then** matching documents appear in results
2. **Given** multiple documents with descriptions, **When** user searches by description text, **Then** documents with matching descriptions appear
3. **Given** documents with tags, **When** user searches by tag, **Then** documents with matching tags appear
4. **Given** search query entered, **When** results display, **Then** search completes within 2 seconds
5. **Given** user searches, **When** results display, **Then** only documents user has permission to access appear

---

### User Story 6 - Share Documents with Team Members (Priority: P2)

Document owners need to grant specific team members access to documents outside their direct project scope.

**Why this priority**: Enables collaboration and reduces email attachment usage. Important for team workflows.

**Independent Test**: Can fully test by: 1) Uploading document, 2) Sharing with specific user, 3) Verifying recipient can access shared document. Testable independently but enhances core functionality.

**Acceptance Scenarios**:

1. **Given** user owns document, **When** user clicks share button, **Then** share dialog displays user/team selection
2. **Given** document is shared, **When** recipient opens dashboard, **Then** document appears in "Shared with Me" section
3. **Given** document is shared, **When** share occurs, **Then** recipient receives in-app notification
4. **Given** user is not document owner, **When** attempting share, **Then** system denies access

---

### User Story 7 - View Project Documents in Context (Priority: P2)

Team members need to access all documents associated with a project within the project detail view.

**Why this priority**: Integrates documents into existing project workflow. Provides context for documents.

**Independent Test**: Can fully test by: 1) Creating project, 2) Uploading documents to project, 3) Viewing project detail, 4) Verifying documents appear. Works independently with project feature.

**Acceptance Scenarios**:

1. **Given** project has associated documents, **When** viewing project detail page, **Then** documents section displays all project documents
2. **Given** project document list, **When** team members view project, **Then** all team members can access and download project documents
3. **Given** project manager views project, **When** viewing documents, **Then** project manager can upload additional project documents

---

### User Story 8 - Attach Documents to Tasks (Priority: P3)

Users need to link documents to specific tasks for context and reference.

**Why this priority**: Deepens integration with existing tasks feature. Nice-to-have after core document functionality established.

**Independent Test**: Can fully test by: 1) Creating task, 2) Attaching document from task detail, 3) Verifying document appears in task. Testable with minimal task feature interaction.

**Acceptance Scenarios**:

1. **Given** user viewing task detail, **When** clicking "Attach Document", **Then** document picker displays all accessible documents
2. **Given** document attached to task, **When** viewing task, **Then** attached documents display with download/preview options
3. **Given** document uploaded from task detail, **When** upload occurs, **Then** document automatically associates with task's project

---

### User Story 9 - Dashboard Visibility of Recent Documents (Priority: P3)

Users need quick access to their recently uploaded documents from dashboard homepage.

**Why this priority**: Improves dashboard utility for frequent users. Nice-to-have that complements core features.

**Independent Test**: Can fully test by: 1) Uploading documents, 2) Viewing dashboard, 3) Verifying recent documents widget displays last 5 uploads. Independent widget feature.

**Acceptance Scenarios**:

1. **Given** user has uploaded documents, **When** viewing dashboard homepage, **Then** "Recent Documents" widget displays last 5 documents
2. **Given** recent documents widget, **When** clicking document link, **Then** user navigates to document detail view
3. **Given** document widget exists, **When** viewing dashboard, **Then** dashboard displays document count in summary cards

---

### Edge Cases

- **Large File Upload**: What happens when user uploads 25 MB file on slow network? System should maintain upload progress and retry capability
- **Concurrent Uploads**: How does system handle multiple simultaneous file uploads? Each upload should process independently
- **Special Characters**: What happens with document titles containing special characters? System should sanitize/escape for safe display
- **Empty Files**: What happens when user attempts to upload 0-byte file? System should reject with appropriate error
- **Network Interruption**: What happens if network fails mid-upload? System should detect disconnection and allow resume/retry
- **Virus-Infected File**: What happens when virus scanning detects malware? System should reject file, log incident, and notify user
- **Permission Loss**: What happens if document owner's project membership is revoked? System should maintain existing document access but prevent new operations
- **Deleted Project**: What happens if project associated with documents is deleted? Documents should remain but project filter should handle gracefully

## Requirements

### Functional Requirements

**Upload & Storage**:
- **FR-001**: System MUST allow users to select one or more files from computer and upload them
- **FR-002**: System MUST support file types: PDF, Microsoft Office (Word, Excel, PowerPoint), text files, JPEG, PNG
- **FR-003**: System MUST enforce 25 MB maximum file size per upload with clear rejection message
- **FR-004**: System MUST require document title and category on upload; accept optional description, project, and tags
- **FR-005**: System MUST display upload progress indicator during file transfer
- **FR-006**: System MUST display success/error message after upload completes
- **FR-007**: System MUST automatically capture upload date/time, uploader name, file size, file type (MIME type)
- **FR-008**: System MUST store files outside web-accessible directory using pattern: `{userId}/{projectId or "personal"}/{GUID}.{extension}`
- **FR-009**: System MUST scan all uploaded files for viruses/malware before storage and reject infected files
- **FR-010**: System MUST validate file extensions against whitelist before accepting upload
- **FR-011**: System MUST use GUID-based filenames to prevent path traversal attacks
- **FR-012**: System MUST generate unique file paths BEFORE database insertion to prevent duplicate key violations

**Organization & Browsing**:
- **FR-013**: System MUST display user's documents in "My Documents" view with columns: title, category, upload date, file size, associated project
- **FR-014**: System MUST allow sorting by: title (A-Z), upload date (newest/oldest), category (A-Z), file size (smallest/largest)
- **FR-015**: System MUST allow filtering by: category (single or multiple), associated project, date range
- **FR-016**: System MUST display project-specific documents when viewing project detail page
- **FR-017**: System MUST allow all project team members to view and download project documents
- **FR-018**: System MUST allow Project Managers to upload documents to their projects

**Search**:
- **FR-019**: System MUST support full-text search across: document title, description, tags, uploader name, associated project
- **FR-020**: System MUST return search results within 2 seconds
- **FR-021**: System MUST only display documents in search results that user has permission to access

**Access & Management**:
- **FR-022**: System MUST allow users to download any document they have permission to access
- **FR-023**: System MUST provide in-browser preview for PDF and image files
- **FR-024**: System MUST load document preview within 3 seconds
- **FR-025**: System MUST allow document owners to edit: title, description, category, tags
- **FR-026**: System MUST allow document owners to replace document file with new version
- **FR-027**: System MUST allow document owners to delete documents with confirmation
- **FR-028**: System MUST allow Project Managers to delete any document in their projects
- **FR-029**: System MUST permanently delete document after user confirmation (no recovery)

**Sharing**:
- **FR-030**: System MUST allow document owners to share documents with specific users or teams
- **FR-031**: System MUST display shared documents in recipient's "Shared with Me" section
- **FR-032**: System MUST send in-app notification when document is shared with user
- **FR-033**: System MUST enforce document owner authorization for all share operations

**Integration**:
- **FR-034**: System MUST allow attaching documents to tasks from task detail page
- **FR-035**: System MUST automatically associate uploaded documents with task's project
- **FR-036**: System MUST display attached documents in task detail view with download/preview options
- **FR-037**: System MUST add "Recent Documents" widget to dashboard showing last 5 user uploads
- **FR-038**: System MUST include document count in dashboard summary cards
- **FR-039**: System MUST send notification when new document added to user's project
- **FR-040**: System MUST send notification when user is tagged/mentioned in document comments

**Authorization**:
- **FR-041**: System MUST implement per-user authorization for all document operations
- **FR-042**: System MUST prevent path traversal attacks by validating all file paths
- **FR-043**: System MUST prevent unauthorized access via IDOR (Insecure Direct Object Reference) attacks
- **FR-044**: System MUST require authorization checks in document download endpoint

**Audit & Reporting**:
- **FR-045**: System MUST log all document activities: uploads, downloads, deletions, shares
- **FR-046**: System MUST allow administrators to generate reports: most uploaded types, most active uploaders, access patterns

### Key Entities

**Document**:
- **DocumentId** (integer primary key, for consistency with User/Project keys)
- **Title** (string, required, searchable)
- **Description** (string, optional)
- **Category** (text, required, values: "Project Documents", "Team Resources", "Personal Files", "Reports", "Presentations", "Other")
- **FileType** (string, 255 characters max, stores MIME type like "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
- **FilePath** (string, stores relative path pattern: `{userId}/{projectId or "personal"}/{GUID}.{extension}`)
- **FileSize** (long, bytes)
- **UploadedBy** (UserId foreign key)
- **UploadedDate** (DateTime UTC)
- **AssociatedProject** (ProjectId foreign key, optional)
- **Tags** (string, comma-separated or relationship table)

**DocumentShare**:
- **ShareId** (integer primary key)
- **DocumentId** (foreign key to Document)
- **SharedWithUserId** (foreign key to User)
- **SharedByUserId** (foreign key to User)
- **SharedDate** (DateTime UTC)

**DocumentActivity Log** (optional for detailed audit):
- **ActivityId** (integer primary key)
- **DocumentId** (foreign key to Document)
- **UserId** (foreign key to User)
- **ActivityType** (enum: Upload, Download, Delete, Share, Edit)
- **Timestamp** (DateTime UTC)
- **IpAddress** (string, optional)
- **Details** (string, optional JSON data)

## Success Criteria

### Measurable Outcomes

- **SC-001**: Users can complete upload within 30 seconds for files up to 25 MB on typical network (measured average upload time)
- **SC-002**: Document list pages load within 2 seconds for up to 500 documents (measured page load time)
- **SC-003**: Document search returns results within 2 seconds (measured query response time)
- **SC-004**: Document preview loads within 3 seconds for supported formats (measured preview open time)
- **SC-005**: Within 3 months of launch, 70% of active dashboard users have uploaded at least one document (measured adoption rate)
- **SC-006**: Average time for users to locate document reduced to under 30 seconds (measured via user testing or activity logs)
- **SC-007**: 90% of uploaded documents are properly categorized (measured via document database analysis)
- **SC-008**: Zero security incidents related to unauthorized document access (measured via audit logs)
- **SC-009**: Users can upload document with 3 clicks or fewer (measured through UX testing)
- **SC-010**: System successfully scans 100% of uploads for viruses without false positives (measured via virus scanning logs)

## Technical Architecture

### Storage Layer Design

**File Storage Pattern**:
```
Local: AppData/uploads/{userId}/{projectId or "personal"}/{GUID}.{extension}
Example: AppData/uploads/user123/proj456/a1b2c3d4-e5f6-4a7b-9c8d-e1f2g3h4i5j6.pdf
```

**Upload Sequence** (prevents orphaned records and duplicate keys):
1. Validate file (size, extension whitelist, virus scan)
2. Authorize user (project membership if applicable)
3. Generate unique GUID-based filename with extension
4. **Save file to disk**
5. **Create database record with verified file path**
6. Send notifications to project members

**Security Implementation**:
- Files stored outside `wwwroot` for access control
- File download requires controller endpoint with authorization checks
- GUID-based filenames prevent path traversal attacks
- Never use user-supplied filenames directly in paths
- Whitelist file extensions before saving
- Implement IDOR prevention in all authorization checks

### Service Layer Architecture

**IFileStorageService Interface** (enables cloud migration):
```csharp
public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteAsync(string filePath);
    Task<Stream> DownloadAsync(string filePath);
    Task<string> GetUrlAsync(string filePath, TimeSpan expiration);
}
```

**Local Implementation**:
- `LocalFileStorageService : IFileStorageService`
- Uses `System.IO` for file operations
- Implements upload/delete/download with local filesystem
- No Azure SDK dependencies

**Future Azure Implementation**:
- `AzureBlobStorageService : IFileStorageService`
- Uses `Azure.Storage.Blobs` SDK
- Same path pattern works as blob names
- Swap via dependency injection configuration
- No code changes to controllers or business logic required

### Blazor-Specific Implementation

**InputFile Component Pattern**:
```csharp
// Extract metadata BEFORE opening stream
var fileName = SelectedFile.Name;
var fileSize = SelectedFile.Size;
var contentType = SelectedFile.ContentType;

// Copy to MemoryStream immediately (prevents disposal issues)
using var memoryStream = new MemoryStream();
using (var fileStream = SelectedFile.OpenReadStream(maxFileSize))
{
    await fileStream.CopyToAsync(memoryStream);
}
memoryStream.Position = 0;

// Clear reference to prevent reuse errors
SelectedFile = null;
StateHasChanged();
```

**Key Requirements**:
- Use `@key` attribute on InputFile to force re-render after upload
- Extract all file metadata into local variables BEFORE opening stream
- Copy IBrowserFile stream to MemoryStream to prevent disposal
- Clear SelectedFile reference after processing
- Ensure Login flow includes all required claims: NameIdentifier, Name, Email, Role, Department
- Department claim required for team-based authorization

### Database Schema

**Document Table**:
```sql
CREATE TABLE Document (
    DocumentId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Category NVARCHAR(50) NOT NULL, -- text values, not integer enum
    FileType NVARCHAR(255), -- accommodates long MIME types
    FilePath NVARCHAR(500) NOT NULL, -- {userId}/{projectId}/{GUID}.ext
    FileSize BIGINT NOT NULL,
    UploadedBy INT NOT NULL FOREIGN KEY (User),
    UploadedDate DATETIME2 NOT NULL,
    AssociatedProject INT FOREIGN KEY (Project),
    Tags NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    ModifiedAt DATETIME2 DEFAULT GETUTCDATE(),
    IsDeleted BIT DEFAULT 0
);
```

**DocumentShare Table**:
```sql
CREATE TABLE DocumentShare (
    ShareId INT PRIMARY KEY IDENTITY(1,1),
    DocumentId INT NOT NULL FOREIGN KEY (Document),
    SharedWithUserId INT NOT NULL FOREIGN KEY (User),
    SharedByUserId INT NOT NULL FOREIGN KEY (User),
    SharedDate DATETIME2 NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);
```

## Assumptions

- Training environment has local disk storage available with sufficient capacity for typical document volumes
- Most documents expected to be under 10 MB in size
- Users are familiar with basic file management concepts and drag-drop interaction
- Local filesystem storage is acceptable for training/development purposes
- Cloud migration to Azure Blob Storage is planned for production deployment
- Users may work offline; core document functionality does not require internet connection
- Project membership determines document authorization (existing role system will be reused)
- Virus scanning can use standard Windows Defender or similar local scanning
- Database will be initialized fresh for training/testing scenarios
- File naming and path conventions are consistent across all environments (Windows/Unix)

## Out of Scope

The following features are NOT included in this initial release:

- Real-time collaborative editing of documents
- Version history and rollback capabilities
- Advanced document workflows (approval processes, document routing)
- Integration with external systems (SharePoint, OneDrive, Google Drive)
- Mobile app support (web-only for initial release)
- Document templates or document generation features
- Storage quotas and quota management per user/project
- Soft delete/trash functionality with recovery capability
- Document watermarking or DRM
- Full-text optical character recognition (OCR) of document content
- Document fingerprinting or duplicate detection
- Advanced commenting and collaboration features
- Document versioning and history
- Document approval workflows

