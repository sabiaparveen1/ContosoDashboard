# Data Model: Document Upload and Management

This model extends the existing `User`, `Project`, `TaskItem`, and `Notification` entities. File bytes remain in private local storage; SQL Server stores metadata and authorization relationships only.

## Entities

### Document

| Field | Type / constraints | Notes |
|---|---|---|
| `DocumentId` | `int`, primary key, identity | Stable application identifier. |
| `Title` | `nvarchar(255)`, required | User-visible title; trim and reject blank values. |
| `Description` | bounded nullable text | Optional; impose a server-side maximum before persistence. |
| `Category` | `nvarchar(50)`, required | One of Project Documents, Team Resources, Personal Files, Reports, Presentations, Other. |
| `OriginalFileName` | `nvarchar(255)`, required | Sanitized base filename only; never used to build a storage path. |
| `FileType` | `nvarchar(255)`, required | Server-detected and allowlisted MIME type, not browser-supplied content type. |
| `StorageKey` | `nvarchar(500)`, required, unique | Relative generated key `{userId}/{projectId-or-personal}/{guid}.{extension}`. Resolve under configured root only. |
| `FileSize` | `bigint`, required, > 0 and <= 25 MiB | Count bytes while streaming; do not rely only on `IBrowserFile.Size`. |
| `UploadedByUserId` | `int`, FK to `User`, required | Original owner/uploader. |
| `UploadedDateUtc` | `datetime2`, required | Server-generated UTC time. |
| `AssociatedProjectId` | `int`, nullable FK to `Project` | Current project association; project access is checked dynamically. |
| `RowVersion` | SQL Server `rowversion`, concurrency token | Detect concurrent metadata edits. |

**Relationships**: many documents belong to one uploader; a document may be associated with one project; documents have many tags, shares, task links, and activity events. File content is external to this row and is not served as a static asset.

**Validation**:

- Require title and category; trim user-provided text and encode on display.
- Detect the actual format from file content and accept only allowlisted formats, regardless of filename extension. Reject unsupported or indeterminate content, macro-enabled types, empty files, and invalid or malformed content; derive the MIME type and storage-key extension from detected content.
- Preserve the original filename only as sanitized metadata. Create the storage key on the server from the authenticated user ID, optional project ID, a GUID, and the detected-format extension.
- Require project membership/manager authorization before associating a project document. A task upload inherits the task's project after task access is checked.
- Persist a document only after a successful clean malware scan and final storage placement.

### Tag

| Field | Type / constraints | Notes |
|---|---|---|
| `TagId` | `int`, primary key, identity | |
| `NormalizedName` | `nvarchar(100)`, required, unique | Trim and normalize consistently before lookup/insert. |
| `DisplayName` | `nvarchar(100)`, required | User-facing form. |

### DocumentTag

| Field | Type / constraints | Notes |
|---|---|---|
| `DocumentId` | `int`, composite primary key, FK to `Document` | |
| `TagId` | `int`, composite primary key, FK to `Tag` | |

**Relationships**: many-to-many between `Document` and `Tag`. A document cannot contain the same normalized tag twice. Remove unreferenced tags only if the service can do so without racing concurrent writes; otherwise retain them.

### DocumentShare

| Field | Type / constraints | Notes |
|---|---|---|
| `ShareId` | `int`, primary key, identity | |
| `DocumentId` | `int`, FK to `Document`, required | |
| `SharedWithUserId` | `int`, FK to `User`, required | Individual-recipient share. |
| `SharedByUserId` | `int`, FK to `User`, required | Must be the document owner. |
| `SharedDateUtc` | `datetime2`, required | Server-generated UTC time. |

**Constraints and access**: unique `(DocumentId, SharedWithUserId)`; disallow duplicate recipients. Recipients receive read/download access only and cannot re-share or edit. Revoke a share by deleting its row. Project membership is not duplicated as shares and is evaluated from current membership on every operation.

### TaskDocument

| Field | Type / constraints | Notes |
|---|---|---|
| `TaskId` | `int`, composite primary key, FK to existing `TaskItem` | |
| `DocumentId` | `int`, composite primary key, FK to `Document` | |
| `LinkedByUserId` | `int`, FK to `User`, required | Actor who attached the document. |
| `LinkedDateUtc` | `datetime2`, required | Server-generated UTC time. |

**Relationships**: many-to-many between tasks and documents. When uploading from a task, set `AssociatedProjectId` to the task's project. A task link does not grant access beyond the user's existing document/task/project authorization.

### DocumentActivity

| Field | Type / constraints | Notes |
|---|---|---|
| `ActivityId` | `long`, primary key, identity | |
| `DocumentId` | `int`, nullable, indexed scalar | Retain identifier in the audit record even if document metadata is physically deleted; no cascading FK from this audit field. |
| `UserId` | `int`, nullable FK to `User` | Nullable for system-originated events or deleted identities. |
| `ActivityType` | bounded string/enum | Upload, Download, Delete, Share, Edit, UploadRejected, ScanRejected. |
| `TimestampUtc` | `datetime2`, required | Server-generated UTC time. |
| `Details` | bounded nullable text | Minimal sanitized context; never include file bytes or secrets. |

**Retention and privacy**: keep only actor, action, document identifier, timestamp, and necessary details. Do not capture IP addresses by default because the spec makes them optional and no retention policy is defined. Ensure physical document deletion does not cascade-delete audit history.

## Existing Entities Reused

- `User`: document owner, share recipient, actor, and uploader display-name source.
- `Project` / `ProjectMember`: current project visibility and project-manager authority.
- `TaskItem`: task attachment target and source of a task upload's project association.
- `Notification`: share/project activity notifications using existing in-app notification behavior.

## Authorization State Transitions

1. **Upload**: request validated and authorized -> private staged file -> content and malware checks -> clean file moved to generated final storage key -> metadata and relationships persisted -> notification/audit recorded. Any failed validation or scan leaves no accessible document record; clean up staging/final bytes if persistence fails.
2. **Access**: each list/search/detail/preview/download operation evaluates the current actor's owner, share-recipient, project-member, or project-manager rights. Never infer permission from a client-provided document ID or storage key.
3. **Project membership loss**: other current project members retain access, and the former owner loses management rights; the project manager may manage the project document. The specification does not define whether the former owner retains read/download access, so resolve this before implementation. A separately granted personal share follows its own share record.
4. **Edit**: owner (or entitled project manager for a project document) submits metadata with the original rowversion -> update if unchanged; otherwise reject with a concurrency conflict and require reload.
5. **Delete**: after explicit confirmation and authorization, revoke access and permanently remove the file and relational metadata; no user-visible recovery path. Preserve the audit event. Filesystem cleanup failures must deny further content access and be retried/reconciled.
6. **Share**: owner creates a unique recipient grant -> recipient can read/download -> owner may revoke. A recipient cannot create another share.

## Indexes

- `Document(UploadedByUserId, UploadedDateUtc)` for "My Documents" and recent uploads.
- `Document(AssociatedProjectId, UploadedDateUtc)` for project views.
- `Document(Category, UploadedDateUtc)` for category filters/sorts.
- Unique `Document(StorageKey)`.
- Unique `Tag(NormalizedName)`.
- `DocumentTag(TagId, DocumentId)` and primary-key index `(DocumentId, TagId)`.
- Unique `DocumentShare(DocumentId, SharedWithUserId)` and index `(SharedWithUserId, SharedDateUtc)`.
- `TaskDocument(TaskId, DocumentId)` and reverse lookup `(DocumentId, TaskId)`.
- `DocumentActivity(DocumentId, TimestampUtc)` and `(UserId, TimestampUtc)`.

Search results must apply access constraints before paging. Metadata `Contains` search is appropriate for the specified small training scale; it is not document-content indexing and must not be described as a general production full-text search service.

