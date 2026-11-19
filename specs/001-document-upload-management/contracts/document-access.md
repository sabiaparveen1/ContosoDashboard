# Document Access Contract

The application currently uses Blazor Server and has no separate public document API. The following same-origin endpoints provide authorized file-byte access for preview and download; metadata operations remain behind the document service and authenticated Blazor UI.

## File endpoints

### `GET /documents/{documentId:int}/download`

- Requires an authenticated user and a document-level authorization decision.
- Permits an authorized owner, an active individually shared recipient, a current project member, or a project manager according to the document's scope. Whether a former project-member owner retains read/download access is unresolved in the specification and must be decided before implementation.
- Returns the file bytes as an attachment, with server-detected allowlisted `Content-Type`, a sanitized original filename in `Content-Disposition`, and `X-Content-Type-Options: nosniff`.
- Uses a stream from private storage; never resolves a client-supplied path or serves through static-file middleware.
- Returns not found for a nonexistent or unauthorized document to avoid confirming another user's document IDs. An authenticated but visible operation denied by a clearly defined business rule may use the application's standard access-denied response.
- Records a download activity only after an authorized download is initiated.

### `GET /documents/{documentId:int}/preview`

- Uses the same authentication and access rules as download.
- Permits inline preview only for detected PDF, JPEG, or PNG content. All other supported types are rejected from this endpoint and remain available through download.
- Returns an inline response with the server-derived media type, `X-Content-Type-Options: nosniff`, and a restrictive same-origin policy. Does not expose a permanent storage URL.
- Records an authorized preview as a download/access activity using the audit schema's defined activity classification.

## Service boundary

All document list, search, metadata, share, project/task link, upload, deletion, and report operations go through `IDocumentService`. Each operation receives the authenticated actor identity and enforces authorization in the service, not only in the component or endpoint. The service returns metadata/streams only after the access decision succeeds.

## Error behavior

- Invalid size, unsupported or indeterminate content, invalid content, malware detection, and scanner failure produce a user-safe validation result with no file bytes returned. A filename-extension mismatch alone does not reject supported detected content.
- Missing document and unauthorized byte-access requests do not disclose the storage key, file existence, or other users' metadata.
- Storage/configuration failure and scan timeout fail closed. Internal exception text and filesystem paths are logged server-side only and are not returned to the browser.
- Concurrent metadata updates return a conflict/reload result; they do not silently overwrite a newer rowversion.

