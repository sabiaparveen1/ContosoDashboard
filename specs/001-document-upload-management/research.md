# Research: Document Upload and Management

**Date**: 2026-11-19  
**Inputs**: `spec.md`, repository `README.md`, `.specify/memory/constitution.md`, current project and data-access layout.

## Decisions

### 1. Retain the current offline application stack

- **Decision**: Implement in the existing .NET 8 Blazor Server app with EF Core 8, SQL Server LocalDB, and local filesystem storage. Add no hosted services or cloud packages.
- **Rationale**: This is the documented and constitutionally required baseline. The repository already has `ApplicationDbContext`, scoped services, cookie authentication, project membership, notifications, and project/task/dashboard pages to integrate with.
- **Alternatives considered**: A separate API/service, cloud object storage, and a hosted antivirus API. These would increase deployment and network requirements and violate the offline-first training baseline.

### 2. Stream to private staging, validate, scan, then publish

- **Decision**: Use Blazor `InputFile` with the explicit 25 MiB maximum. Copy bounded chunks to a private staging file rather than buffering the whole upload. Check actual bytes read in addition to the client-reported length. Keep staging and final storage outside `wwwroot`; make a file available only after validation and a clean local scan.
- **Rationale**: Avoids trusting browser metadata, reduces memory pressure for concurrent uploads, and keeps static-file hosting from bypassing document authorization. Staging permits cleanup on rejection or persistence failure.
- **Alternatives considered**: Reading all bytes into a `MemoryStream` (memory scales with size and concurrent uploads); direct static-file hosting under `wwwroot` (bypasses per-document checks); upload to a remote scanner/storage service (not offline).

### 3. Use a configured local malware scanner and fail closed

- **Decision**: Introduce `IFileMalwareScanner` with a Windows-local scanner adapter for the documented LocalDB host. Scanner path/configuration, availability, timeout, and error results are explicit. Reject uploads when scanning is unavailable, times out, or returns an indeterminate result. Tests may inject a fake scanner; runtime registration must not default to a no-op.
- **Rationale**: The specification requires scanning every upload and assumes Windows Defender or a similar local scanner. A narrow interface isolates OS-specific behavior and allows repeatable tests without requiring cloud access.
- **Alternatives considered**: No-op scanning (violates FR-009); a cloud scanning API (violates offline operation); ClamAV (adds installation, signature-update, and resource requirements not present in the baseline); embedding a scanner implementation (not justified for a training app).
- **Operational caveat**: The host must have a supported scanner enabled and its signatures updated through its local administration process. This design does not claim that signature scanning guarantees safety or makes the app production-ready.

### 4. Validate allowlisted file content, not client declarations alone

- **Decision**: Allow only PDF, Word/Excel/PowerPoint legacy and OOXML formats, plain text, JPEG, and PNG. Determine format from content and accept supported content regardless of filename extension; derive the MIME type and generated storage extension from the detected format. Reject unsupported or indeterminate content and macro-enabled formats. For OOXML, inspect the ZIP container and expected package entries with bounded entry count/expanded size; do not extract arbitrary archive paths. Store the sanitized original filename separately from the generated storage key.
- **Rationale**: The resolved Q2 explicitly selects actual-type validation regardless of extension, and FR-010 is updated to require detected content validation. The browser-provided MIME value and filename extension are not authoritative. The original filename remains display metadata, never a path component.
- **Alternatives considered**: Extension-only checks (can accept spoofed content); requiring extension/content agreement (would reject supported, legitimately renamed content contrary to Q2); trusting browser MIME metadata (client controlled); adding a third-party detector package (offline package provenance and maintenance are unnecessary for this bounded allowlist).
- **Text caveat**: Plain text has no reliable magic number. Accept only valid text under the selected encoding/control-character policy; reject binary-looking or undecodable input.

### 5. Separate metadata access from file-byte access

- **Decision**: Keep metadata and authorization in `IDocumentService`/`DocumentService`; keep filesystem operations in `IFileStorageService`/`LocalFileStorageService`. Both Blazor views and download/preview endpoints call the service with the authenticated actor ID. The local adapter resolves generated relative keys beneath one configured private root and verifies the resolved path remains inside that root.
- **Rationale**: This follows the repository's existing interface/service/DI pattern and prevents UI-only checks from becoming the access boundary. It retains a future seam for alternative storage without implementing cloud migration now.
- **Alternatives considered**: Direct `System.IO` calls from Razor components (mixes layers and duplicates checks); returning permanent public file URLs (bypasses per-request authorization); storing absolute host paths in the database (ties records to one machine).

### 6. Keep metadata search relational and bounded

- **Decision**: Normalize tags into `Tag` and `DocumentTag`. Search metadata fields (title, description, tag, uploader name, project name) with parameterized EF Core queries composed with the document access predicate. Use paging, `AsNoTracking`, selected columns for list rows, and indexes for owner/project/category/date and relationships.
- **Rationale**: This meets the described search fields and 500-document training scale without adding a full-text service or indexing document contents. Normalized tags follow clarification Q3 and avoid duplicate tag spellings from repeated uploads.
- **Alternatives considered**: Comma-delimited tag strings (fragile filtering and duplicate values); SQL full-text catalog (extra DB-specific setup and uncertain LocalDB availability); an external search engine (offline/deployment cost not justified).

### 7. Use optimistic metadata concurrency and activity auditing

- **Decision**: Add a SQL Server `rowversion` concurrency token to document metadata. Return a clear reload/conflict result on concurrent edits. Record upload, download, edit, delete, share, and rejected-scan activity with actor ID and UTC time. Omit IP addresses by default.
- **Rationale**: Matches Q4's optimistic-concurrency choice and preserves an audit trail without locking downloads. Existing SQL Server EF Core supports the rowversion pattern; the project already uses LocalDB.
- **Alternatives considered**: Pessimistic locks/queues (unnecessary for independent metadata and file reads); last-write-wins without a token (silently hides conflicting edits); retaining IP addresses (not required and no privacy/retention policy is specified).

### 8. Use existing membership and notification concepts; avoid inventing adjacent features

- **Decision**: Project membership grants project-document read/download access; project managers manage project documents as specified. Explicit shares target individual users and are read/download-only for recipients. Reuse the current notification service for shares and project-document events, and integrate document counts/recent uploads with the dashboard. Use a join entity for task attachments.
- **Rationale**: The codebase has `ProjectMember`, project manager IDs, `INotificationService`, tasks, and dashboard services, but no arbitrary team/group model. Individual shares give an unambiguous owner-controlled boundary.
- **Alternatives considered**: Add arbitrary teams/groups (unjustified new identity/authorization model); allow recipients to re-share (contradicts resolved Q1); duplicate project membership inside share records (creates stale access after membership changes).
- **Scope gap**: FR-040 requires mentions in document comments, but neither a comment story nor comment entity exists and advanced commenting is out of scope. Do not invent comments silently; clarify or revise that requirement before implementation planning.
- **Authorization gap**: Q5 revokes the former owner's management rights and preserves current project-team access, but does not say whether the former owner retains read/download access. Resolve before implementation.

### 9. Apply an EF Core schema migration, with a clean LocalDB baseline

- **Decision**: Add the document schema through EF Core migrations and apply migrations during startup instead of relying on `EnsureCreated()`. Reset a training LocalDB instance created previously by `EnsureCreated()` before adopting the first migration.
- **Rationale**: `EnsureCreated()` does not evolve an existing schema and is not a migration mechanism. The feature adds several related entities, indexes, and concurrency configuration. The spec explicitly assumes a fresh database for training/testing.
- **Alternatives considered**: Continue `EnsureCreated()` and require all existing users to hand-delete/replace their database on each schema change (not repeatable); create ad hoc startup SQL (bypasses EF model/schema history).

## Source References

Repository sources used for project-specific choices:

- `README.md` — offline-training scope, .NET 8/Blazor Server/LocalDB stack, current page/service/model structure, local-file-storage direction.
- `.specify/memory/constitution.md` — offline-first, layered/testable design, mock-auth boundaries, Spec Kit workflow, documented limitations.
- `ContosoDashboard/Program.cs` — DI, cookie authentication, authorization policies, current `EnsureCreated()` startup behavior.
- `ContosoDashboard/Data/ApplicationDbContext.cs` — existing EF Core entities, relationships, indexes, and seed data.
- `ContosoDashboard/Services/ProjectService.cs`, `NotificationService.cs`, `TaskService.cs`, and `DashboardService.cs` — current access, notification, task, and dashboard integration patterns.
- `ContosoDashboard/ContosoDashboard.csproj` — .NET 8 and EF Core SQL Server dependencies.

General technical references for implementation:

- Microsoft, [Blazor file uploads](https://learn.microsoft.com/aspnet/core/blazor/file-uploads)
- Microsoft, [Upload files in ASP.NET Core](https://learn.microsoft.com/aspnet/core/mvc/models/file-uploads)
- Microsoft, [Blazor Server with EF Core](https://learn.microsoft.com/aspnet/core/blazor/blazor-server-ef-core)
- Microsoft, [EF Core optimistic concurrency](https://learn.microsoft.com/ef/core/saving/concurrency)
- Microsoft, [EF Core efficient querying](https://learn.microsoft.com/ef/core/performance/efficient-querying)
- OWASP, [File Upload Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/File_Upload_Cheat_Sheet.html)
- Microsoft, [Microsoft Defender Antivirus command-line options](https://learn.microsoft.com/microsoft-365/security/defender-endpoint/command-line-arguments-microsoft-defender-antivirus)

