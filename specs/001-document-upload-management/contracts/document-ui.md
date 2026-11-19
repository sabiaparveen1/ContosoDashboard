# Document UI Contract

The feature adds authenticated Blazor pages and sections to the existing server-side application. These are user-facing contracts, not claims that the current code already implements them.

## Routes and views

| Route / placement | Contract |
|---|---|
| `/documents` | Authenticated "My Documents" view. Displays accessible personal, shared, and project documents with title, category, upload date, size, and associated project; supports paging, sorting, category/project/date filtering, and metadata search. |
| `/documents/upload` | Authenticated upload form. Requires title and category; accepts optional description, project, and tags. Accepts only the specified file allowlist up to 25 MiB per file; shows progress and a clear completion/rejection result. |
| `/documents/{documentId:int}` | Authenticated metadata/details view. Allows eligible owner/project manager to edit permitted metadata, share/revoke access where owner, and delete after confirmation. A read-only recipient cannot edit or re-share. |
| Project details | Adds a project-document section showing only documents accessible under current project membership/manager rules. A project manager can upload project documents. |
| Task details | Adds attached-document list and picker/upload entry point. Uploading from a task associates the task's project. |
| Dashboard `/` | Adds the latest five accessible user uploads and a document count, preserving existing authentication and dashboard behavior. |
| Shared with Me | A section/filter in the document experience lists active individual shares; recipient rights are read/download only. |
| Administrator document activity view/report | Restricted to Administrator. Reports aggregate stored activity records; non-admin access is denied. |

## Common behavior

- Every page requires authentication. Authentication uses the existing training-only mock cookie identity.
- The authenticated `NameIdentifier` claim supplies the actor ID; the client cannot select a different acting user.
- List, search, sort, filter, and pagination operations preserve the same authorization scope. Empty result sets disclose no inaccessible document metadata.
- User-controlled metadata is validated server-side and encoded on output. File paths and detected MIME values are not editable UI fields.
- Upload progress, success, scanner failure, size/type rejection, authorization failure, and stale metadata conflict are presented as actionable messages.
- Delete requires explicit confirmation and is irreversible; no recycle-bin or restore path is provided.

