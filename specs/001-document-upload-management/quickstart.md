# Quickstart: Document Upload and Management

This guide validates the planned feature after implementation. It does not claim that the current repository already contains these routes or services.

## Prerequisites

- Windows host with .NET 8 SDK, SQL Server LocalDB, and the repository's NuGet dependencies available.
- A configured, enabled local malware scanner with current signatures. Uploads must fail closed if the scanner cannot be used.
- Writable private storage root configured outside `ContosoDashboard/wwwroot`.
- The mock login user accounts seeded by the application. This authentication is training-only.

## Build and automated tests

From the repository root:

```powershell
dotnet test .\ContosoDashboard.Tests\ContosoDashboard.Tests.csproj
dotnet build .\ContosoDashboard\ContosoDashboard.csproj
```

Expected: test project passes, application builds without errors, and all test scanner/storage dependencies are local fakes or temporary directories; no hosted service is required.

If a LocalDB database was previously initialized by the existing `EnsureCreated()` startup path, follow the feature's one-time reset/migration instructions before starting the migrated schema. Do not delete any database with user data without confirming it is disposable.

## Run locally

```powershell
dotnet run --project .\ContosoDashboard\ContosoDashboard.csproj
```

Sign in through the existing mock `/login` flow. Open `/documents` to browse and `/documents/upload` to upload. Expected: authorized users see only documents they can access; an unconfigured scanner prevents a successful upload and returns a clear training-environment message.

## End-to-end validation scenarios

1. **Valid upload and metadata**: Upload a non-empty allowlisted PDF under 25 MiB with title/category and optional project/tags. Expected: progress is visible, the clean file is stored outside `wwwroot`, generated storage key is used, metadata appears in the document list, and a success message includes the title/category.
2. **Validation and scanner failure**: Try a zero-byte file, an unsupported/mismatched extension and content, a file over 25 MiB, and a host with scanner unavailable. Expected: each is rejected with a clear message; no rejected file is downloadable and no partial public file is created.
3. **List, search, filters, and paging**: Create records with distinct titles, categories, tags, owners, and projects. Test title/date/category/size sort, category/project/date filters, and title/description/tag/uploader/project search. Expected: results are paged, match the filter/query, exclude unauthorized records, and satisfy the 500-document / 2-second target in the intended local test environment.
4. **Download and preview**: Download an owned file, preview a PDF or image, then attempt both actions with an unrelated user's document ID. Expected: authorized downloads retain the original display filename and bytes; only PDF/images preview inline; unauthorized or nonexistent records do not reveal file bytes.
5. **Owner, recipient, and project boundaries**: Share a personal document with another seeded user, revoke access, and test project documents as a current member, former/non-member, and project manager. Expected: recipient can read/download but not edit/re-share; project team access follows current membership; managers can manage project documents; personal documents remain isolated.
6. **Metadata and concurrency**: Edit metadata, then submit an edit using a stale rowversion. Expected: the first save persists; the stale edit receives a conflict/reload message without overwriting current metadata.
7. **Task/project/dashboard integration**: Upload or attach a document from a task; inspect project details and dashboard. Expected: task uploads inherit the task's project, links enforce existing access, project document lists are visible to eligible members, recent uploads show the latest five, and the document count reflects accessible records.
8. **Audit and deletion**: Upload, download, edit, share/revoke, and permanently delete a disposable document; inspect the activity report as an administrator and non-administrator. Expected: the actions are recorded with actor and UTC time, the non-admin report is denied, deletion has no recovery UI, and audit history remains after document metadata is removed.

## Contract and model references

- User-facing routes and behavior: [contracts/document-ui.md](contracts/document-ui.md)
- Authorized file access: [contracts/document-access.md](contracts/document-access.md)
- Entity constraints and access state: [data-model.md](data-model.md)

