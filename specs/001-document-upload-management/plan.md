# Implementation Plan: Document Upload and Management

**Branch**: `001-document-upload-management` | **Date**: 2026-11-19 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-document-upload-management/spec.md`

## Summary

Add private, locally stored document management to the existing ASP.NET Core 8 Blazor Server dashboard. Users will upload, browse, search, preview/download, edit, delete, share, and associate documents with projects and tasks. A document service will enforce owner, project-member, project-manager, recipient, and administrator boundaries; a local file-storage abstraction and fail-closed malware-scanning adapter keep file access outside `wwwroot` and the baseline offline.

## Technical Context

**Language/Version**: C# / .NET 8 (nullable reference types and implicit usings enabled)

**Primary Dependencies**: ASP.NET Core 8 Blazor Server; Entity Framework Core 8; SQL Server LocalDB. Add no cloud dependency. Use built-in `System.IO` and `System.IO.Compression` for local storage and bounded OOXML inspection; integrate malware scanning behind an injectable local-scanner interface.

**Storage**: SQL Server LocalDB for document metadata, access/share/tag/task-link/audit records; private local filesystem outside `wwwroot` for file bytes. Persist a generated relative storage key, not an absolute path or a user-supplied filename.

**Testing**: No test project currently exists. Add automated unit/service tests in a .NET 8 test project and use temporary storage plus fake scanner implementations for deterministic tests; manual LocalDB/Defender checks are in `quickstart.md`.

**Target Platform**: Local, offline training deployment on the documented ASP.NET Core 8 / SQL Server LocalDB stack; LocalDB and the selected local malware scanner require a supported Windows host.

**Project Type**: Existing server-rendered Blazor web application with EF Core persistence and Razor Pages for authentication.

**Performance Goals**: Enforce a 25 MiB per-file limit; complete uploads up to the limit within 30 seconds under typical local-network conditions; return paged lists and metadata searches for 500 documents within 2 seconds; preview supported PDFs/images within 3 seconds.

**Constraints**: Do not save uploads under `wwwroot`; do not trust browser filename, extension, or MIME type; validate authorization in services and file endpoints; scan every upload before making it available; fail closed if the scanner is unavailable or errors; avoid buffering a full upload in memory; no cloud services; preserve the training-only security boundary.

**Scale/Scope**: Nine user stories, from core personal uploads and browsing through project/task integration, sharing, recent-document dashboard visibility, and administrative activity reports. Expected list/search scale is hundreds of records per user.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Gate assessment |
|---|---|
| I. Training-First, Offline-Ready Learning | **Pass.** LocalDB, local filesystem, and a local scanner preserve offline operation. Scanner configuration/availability is a local prerequisite; no hosted scanning service is introduced. Document remaining training limitations. |
| II. Security Concepts With Honest Boundaries | **Pass.** Protect every operation at the service and download/preview endpoint, isolate personal documents, derive project access from current membership, use opaque storage keys, validate file contents, and fail closed on malware-scan failure. Keep the feature explicitly training-only. |
| III. Layered, Testable Design | **Pass.** Keep UI, document authorization/business service, scanner/storage abstractions, and EF Core persistence separate. Add automated tests for authorization, file validation, and cleanup/error paths. |
| IV. Spec-Driven, Verifiable Changes | **Pass.** This plan and its design artifacts precede the separate task-generation and implementation workflows. The quickstart defines validation scenarios; the implementation must report checks actually run. |
| V. Simplicity and Documented Limitations | **Pass with documented scope decisions.** Reuse the current stack and notification/project services; do not build cloud storage, full-text file-content indexing, version history, or a team/group directory. |

**Pre-design gate result**: No unjustified constitutional violations identified. A specification inconsistency concerning comment mentions is recorded under risks and in `research.md`; this plan does not silently add an out-of-scope commenting system.

**Post-design gate result**: Pass. The selected storage, scanner, authorization, persistence, and test boundaries preserve all five principles. The comment-mention mismatch and local-scanner prerequisite remain explicit risks, not hidden exceptions or claims of production readiness.

## Design Decisions

- **Persistence**: Add EF Core entities and indexes to the existing `ApplicationDbContext`. Use a migration for the schema change and switch database initialization from `EnsureCreated()` to applying migrations. The current database is created with `EnsureCreated()` and has no migrations history; a one-time LocalDB reset is expected for a clean migration baseline, consistent with the specification's fresh-training-database assumption.
- **File lifecycle**: Stream to a private staging location, validate size and detected type, scan locally, then atomically move to the generated final key and persist metadata. If the database write fails, remove the staged/final file as compensation. Keep files unavailable until a clean scan result is established. Treat delete as irreversible to users; if filesystem cleanup fails, keep access denied and retry cleanup rather than offering a recovery/trash UI.
- **Content validation**: Allow only PDF, legacy and OOXML Word/Excel/PowerPoint, plain text, JPEG, and PNG. Determine format from content, accept supported content regardless of the filename extension, and derive MIME type and storage extension from the detected format. Reject unsupported or indeterminate content, macro-enabled formats, invalid text, and malformed/unsafe OOXML packages. Store the sanitized original display filename separately. Use a bounded signature/OOXML inspector without a new runtime cloud or parsing-service dependency.
- **Malware scanning**: Define `IFileMalwareScanner` and a local Windows scanner adapter, configured explicitly for the host's supported scanner (Microsoft Defender on the documented Windows baseline). Any missing scanner, timeout, or indeterminate result rejects the upload. A test fake is used only in automated tests, never as the production/default runtime scanner. Scanner definitions must be kept current by the local environment.
- **Authorization**: Personal documents are available to their owner and explicitly shared recipients. Project documents remain available to current project members and managers; the former owner loses management rights when removed from the project. The clarification does not specify whether the former owner retains read/download access, so resolve that rule before implementation. The project's manager can manage project documents. A shared recipient can read/download but cannot re-share. Derive the actor ID from the authenticated `NameIdentifier` claim and repeat the access check inside every service operation and file endpoint.
- **Organization and search**: Normalize tags into `Tag` and `DocumentTag` tables. Use parameterized EF Core metadata search over title, description, tags, uploader display name, and project name, always composed with the access predicate; do not search file contents. Provide paging and `AsNoTracking` list queries. Index owner, project, category, upload date, share recipient, and relationship keys; treat the 2-second target as an acceptance check on the stated training scale.
- **Concurrency and audit**: Use SQL Server `rowversion` for metadata updates and report a reload/conflict message on `DbUpdateConcurrencyException`. Record actor, document, activity type, and UTC time for upload, download, edit, delete, and share. Do not retain IP addresses by default; the spec makes them optional and no retention/privacy policy is defined.
- **Sharing scope**: Share records target individual users. Project membership is the existing notion of a team for project documents; no arbitrary team/group share model is introduced because the repository has no team entity or group authorization model.

## Project Structure

### Documentation (this feature)

```text
specs/001-document-upload-management/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── contracts/
    ├── document-ui.md
    └── document-access.md
```

### Source Code (repository root)

```text
ContosoDashboard/
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── Document.cs
│   ├── DocumentShare.cs
│   ├── Tag.cs
│   ├── DocumentTag.cs
│   ├── TaskDocument.cs
│   └── DocumentActivity.cs
├── Services/
│   ├── IDocumentService.cs / DocumentService.cs
│   ├── IFileStorageService.cs / LocalFileStorageService.cs
│   ├── IFileMalwareScanner.cs / configured local scanner adapter
│   └── upload content inspection and authorization helpers
├── Pages/
│   ├── Documents.razor
│   ├── UploadDocument.razor
│   ├── DocumentDetails.razor
│   ├── ProjectDetails.razor           # project-document section
│   ├── Tasks.razor                    # task attachment entry point
│   └── Index.razor                    # recent-document widget/count
├── Endpoints/
│   └── DocumentAccessEndpoints.cs     # authorized preview/download
└── Program.cs                         # DI, endpoint, and migration wiring
ContosoDashboard.Tests/
└── document service, authorization, validation, and storage tests
```

**Structure Decision**: Extend the existing single Blazor Server application and its `Models`, `Data`, `Services`, `Pages`, and startup composition root. Keep file access in narrowly scoped services/endpoints; add a separate test project because the repository currently contains no automated test project. The specified actual folders remain under `ContosoDashboard/`; `Endpoints/` and `ContosoDashboard.Tests/` are new.

## Complexity Tracking

No constitutional gate violations require an exception.

## Risks and Specification Gaps

1. **FR-040 mentions document comments that otherwise do not exist.** The feature has no document-comment user story/entity, and advanced commenting is explicitly out of scope. The implementation must not invent a comments feature solely to satisfy the mention-notification clause. Clarify or revise FR-040 before task generation/implementation.
2. **Virus scanning is environment-dependent.** Uploads cannot succeed on a host without a supported, configured, functioning local scanner and current definitions. Document this prerequisite and fail closed; do not substitute a no-op scanner.
3. **Migration baseline**: A database previously created by `EnsureCreated()` cannot be incrementally upgraded by a first migration without a one-time reset/baseline strategy. The planned training path resets LocalDB before applying the new migration.
4. **Team sharing terminology**: The repository has project membership, not arbitrary team/group identities. This plan limits explicit sharing to individual users and uses project membership for project-wide access; broader group sharing requires a separately specified group model.
5. **Former-owner read access**: The clarification removes the former owner's management rights and preserves access for current project members, but does not state whether the former owner can still read/download the project document. Resolve this authorization rule before implementation; do not infer it from ownership alone.

