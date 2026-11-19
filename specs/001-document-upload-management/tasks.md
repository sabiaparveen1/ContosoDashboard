# Tasks: Document Upload and Management Implementation

**Feature**: 001-document-upload-management  
**Branch**: 001-document-upload-management  
**Status**: Ready for implementation  
**Date**: 2026-11-19

## Phase 1: Setup & Infrastructure

**Purpose**: Project initialization and core infrastructure setup

- [ ] T001 Create EF Core migration for document schema (Document, DocumentShare, Tag, DocumentTag, TaskDocument, DocumentActivity) in `src/Data/Migrations/`
- [ ] T002 [P] Configure LocalDB baseline and migration startup in `Program.cs`
- [ ] T003 Create `IFileStorageService` interface in `src/Services/Storage/`
- [ ] T004 [P] Implement `LocalFileStorageService` with upload/download/delete in `src/Services/Storage/`
- [ ] T005 [P] Create `IFileMalwareScanner` interface and test implementation in `src/Services/Scanning/`
- [ ] T006 [P] Implement Windows Defender scanner adapter in `src/Services/Scanning/`
- [ ] T007 Register storage and scanner services in DI container (`Program.cs`)

**Checkpoint**: Infrastructure ready for document service implementation

---

## Phase 2: Foundational - Document Service & Core Operations

**Purpose**: Core authorization, service layer, and document operations that MUST be complete before UI implementation

**⚠️ CRITICAL**: No user story work (UI, search, sharing) can begin until this phase is complete

- [ ] T008 Create `IDocumentService` interface with authorization methods in `src/Services/Documents/`
- [ ] T009 Implement `DocumentService` class with GetMyDocuments, GetProjectDocuments, GetSharedWithMe methods in `src/Services/Documents/`
- [ ] T010 Implement authorization checks (owner, project member, project manager, share recipient) in `DocumentService` (`src/Services/Documents/DocumentService.cs`)
- [ ] T011 Implement document upload pipeline with validation, scanning, and atomic storage in `src/Services/Documents/DocumentService.cs`
- [ ] T012 Implement metadata edit with optimistic concurrency (rowversion) in `src/Services/Documents/DocumentService.cs`
- [ ] T013 Implement hard delete with file cleanup and audit logging in `src/Services/Documents/DocumentService.cs`
- [ ] T014 Implement share and revoke methods in `src/Services/Documents/DocumentService.cs`
- [ ] T015 Create `DocumentController` or minimal API endpoints in `src/Api/`
- [ ] T016 Implement Download endpoint (GET /documents/{documentId}/download) in `src/Api/DocumentController.cs`
- [ ] T017 Implement Preview endpoint (GET /documents/{documentId}/preview) for PDF/image in `src/Api/DocumentController.cs`
- [ ] T018 Implement tag normalization and DocumentTag junction entity handling in `src/Data/`
- [ ] T019 Implement search method across title, description, tags, uploader, project in `src/Services/Documents/DocumentService.cs`
- [ ] T020 Implement document list with sorting (title, date, category, size) and filtering (category, project, date range) in `src/Services/Documents/DocumentService.cs`
- [ ] T021 Create `DocumentActivity` audit logging in `src/Services/Documents/DocumentService.cs` for all operations

**Checkpoint**: Foundation complete - all document operations working, authorization enforced, audit logging active

---

## Phase 3: User Story 1 - Core Document Management (MVP: Upload, View, Download)

**Goal**: Enable users to upload documents, view personal/project documents, and download files

**Independent Test**: Upload PDF → View in My Documents → Download and verify bytes → Edit metadata → Delete successfully

### Implementation for User Story 1 - Core Document Management

- [ ] T022 [P] [US1] Create Blazor Upload page component in `src/Components/Pages/Documents/Upload.razor`
- [ ] T023 [P] [US1] Create Blazor My Documents page component in `src/Components/Pages/Documents/MyDocuments.razor`
- [ ] T024 [US1] Create Blazor Document Detail page component in `src/Components/Pages/Documents/DocumentDetail.razor` (depends on T022, T023)
- [ ] T025 [US1] Wire upload page to `DocumentService.UploadAsync()` in `Upload.razor` with progress display
- [ ] T026 [US1] Wire My Documents list to `DocumentService.GetMyDocumentsAsync()` in `MyDocuments.razor` with sorting/filtering
- [ ] T027 [US1] Wire Document Detail to display metadata, preview/download, and edit controls in `DocumentDetail.razor`
- [ ] T028 [US1] Implement edit metadata form in Document Detail (`DocumentDetail.razor`)
- [ ] T029 [US1] Implement delete with confirmation in Document Detail (`DocumentDetail.razor`)
- [ ] T030 [US1] Add "My Documents" navigation link to main layout in `src/Components/Layout/NavMenu.razor`
- [ ] T031 [P] [US1] Create unit tests for `DocumentService` authorization in `tests/Services/DocumentServiceAuthorizationTests.cs`
- [ ] T032 [P] [US1] Create unit tests for file validation and scanning in `tests/Services/FileValidationTests.cs`
- [ ] T033 [US1] Create integration tests for upload-to-download flow in `tests/Integration/DocumentFlowTests.cs` (depends on T031, T032)

**Checkpoint**: User Story 1 complete and independently testable - MVP delivers upload, view, download functionality

---

## Phase 4: User Story 2 - Document Sharing & Access Control

**Goal**: Enable document owners to share documents with individuals and manage access

**Independent Test**: Upload document → Share with user → Recipient downloads shared document → Revoke access and verify 404

- [ ] T034 [P] [US2] Create Share section UI component in `src/Components/Pages/Documents/ShareManager.razor`
- [ ] T035 [P] [US2] Create Shared With Me tab/filter in My Documents in `MyDocuments.razor`
- [ ] T036 [US2] Wire Share UI to `DocumentService.ShareAsync()` and `RevokeAsync()` in `ShareManager.razor` (depends on T034, T035)
- [ ] T037 [US2] Add share notifications in `DocumentService` (send to recipient)
- [ ] T038 [US2] Create unit tests for share authorization (recipient cannot re-share/edit) in `tests/Services/ShareAuthorizationTests.cs`
- [ ] T039 [US2] Create integration tests for share flow in `tests/Integration/ShareFlowTests.cs` (depends on T038)

**Checkpoint**: User Story 2 complete - sharing and access control working independently

---

## Phase 5: User Story 3 - Search & Organization

**Goal**: Enable users to find documents by search and organize with filtering

**Independent Test**: Upload multiple documents with tags → Search by keyword/tag → Filter by category → Verify results

- [ ] T040 [P] [US3] Add search box to My Documents page in `MyDocuments.razor`
- [ ] T041 [P] [US3] Add category, project, and date range filters to My Documents in `MyDocuments.razor`
- [ ] T042 [US3] Wire filters to `DocumentService.SearchDocumentsAsync()` in `MyDocuments.razor` (depends on T040, T041)
- [ ] T043 [US3] Create unit tests for search and filtering in `tests/Services/SearchTests.cs`
- [ ] T044 [US3] Create performance tests to verify search returns within 2 seconds for 500 documents in `tests/Performance/SearchPerformanceTests.cs`

**Checkpoint**: User Story 3 complete - search and filtering working

---

## Phase 6: User Story 4 - Project & Task Integration

**Goal**: Integrate documents with projects and tasks for team collaboration

**Independent Test**: Create project → Upload project document → View in project → Attach document to task → View in task

- [ ] T045 [P] [US4] Add Documents section to Project detail page in `src/Components/Pages/Projects/ProjectDetail.razor`
- [ ] T046 [P] [US4] Add document picker and attachment section to Task detail in `src/Components/Pages/Tasks/TaskDetail.razor`
- [ ] T047 [P] [US4] Add Recent Documents widget to Dashboard in `src/Components/Pages/Dashboard.razor`
- [ ] T048 [US4] Wire project documents list to `DocumentService.GetProjectDocumentsAsync()` in `ProjectDetail.razor` (depends on T045)
- [ ] T049 [US4] Wire task document attachment to `DocumentService` in `TaskDetail.razor` (depends on T046)
- [ ] T050 [US4] Create `TaskDocument` junction entity and wire in `DocumentService` (`src/Data/ApplicationDbContext.cs`)
- [ ] T051 [US4] Implement project document upload from project page (auto-associates project)
- [ ] T052 [US4] Wire Recent Documents widget to `DocumentService.GetRecentDocumentsAsync()` in `Dashboard.razor` (depends on T047)
- [ ] T053 [US4] Create integration tests for project/task document workflows in `tests/Integration/ProjectTaskIntegrationTests.cs`

**Checkpoint**: User Story 4 complete - project and task integration working

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Performance optimization, testing, and final validation

- [ ] T054 [P] Implement comprehensive error handling and user feedback for all document operations in `src/Services/Documents/`
- [ ] T055 [P] Add security headers (X-Content-Type-Options: nosniff) to file download/preview endpoints in `DocumentController.cs`
- [ ] T056 [P] Performance validation and measurement documentation in `docs/PERFORMANCE.md`
- [ ] T057 Resolve specification gaps (former owner access on project documents, comment mentions) with decision in `spec.md`
- [ ] T058 Update quickstart.md with document upload and sharing validation steps in `docs/quickstart.md`
- [ ] T059 Code review and cleanup across all document service layers
- [ ] T060 Final end-to-end validation using quickstart.md checklist

**Checkpoint**: Implementation complete, validated, and ready for deployment

---

## Dependencies & Execution Order

### Phase Dependencies

| Phase | Purpose | Dependencies | Status |
|-------|---------|--------------|--------|
| Phase 1 | Setup & Infrastructure | None - start immediately | Blocking: Phase 2, 3+ |
| Phase 2 | Foundational Service | Requires Phase 1 complete | Blocking: Phase 3, 4, 5, 6 |
| Phase 3 | MVP: Upload/View/Download | Requires Phase 2 complete | MVP deliverable |
| Phase 4 | Sharing & Access Control | Requires Phase 2 complete | Optional for MVP; can parallel Phase 3 |
| Phase 5 | Search & Organization | Requires Phase 2 complete | Optional for MVP; can parallel Phase 3, 4 |
| Phase 6 | Project/Task Integration | Requires Phase 2 complete | Optional for MVP; can parallel Phase 3, 4, 5 |
| Phase 7 | Polish & Validation | Requires all features complete | Final phase |

### Task Dependencies Within Phases

**Phase 1 Setup**:
- T001 → T002 (schema before migrations)
- T003 → T004, T005, T006 (interfaces before implementations)
- T007 depends on T004, T006 (register after implementations)

**Phase 2 Foundational**:
- T008 → T009 (interface before implementation)
- T009 → T010, T011, T012, T013, T014, T018, T019, T020 (service class is prerequisite)
- T015 depends on T009, T021 (service complete before API layer)
- T016, T017 depend on T015 (API endpoints)
- T021 depends on T008, T009 (audit logging integrated throughout)

**Phase 3 MVP (US1)**:
- T022, T023 can run in parallel [P]
- T024 depends on T022, T023 (detail page integrates both)
- T025, T026, T027, T028, T029 depend on T024
- T030 depends on T025 or T026 (navigation setup)
- T031, T032 can run in parallel [P]
- T033 depends on T031, T032

**Phase 4 (US2)**:
- T034, T035 can run in parallel [P]
- T036 depends on T034, T035
- T037, T038, T039 follow sequential dependency chain

**Phase 5 (US3)**:
- T040, T041 can run in parallel [P]
- T042 depends on T040, T041
- T043, T044 follow

**Phase 6 (US4)**:
- T045, T046, T047 can run in parallel [P]
- T048, T049 depend on their respective parent tasks
- T050 is independent (data model)
- T051, T052, T053 follow

**Phase 7 Polish**:
- T054, T055, T056 can run in parallel [P]
- T057, T058, T059, T060 follow sequentially

### Parallel Opportunities

**Within Phase 1 (Setup)**:
- Storage interface/implementation: T003, T004 (different files)
- Scanner interface/implementation: T005, T006 (different files)

**Within Phase 2 (Foundational)**:
- Service methods: T010, T011, T012, T013, T014, T018, T019, T020 (can start after T009 complete)
- Tests can start early: T031, T032 (don't block implementation)

**Within Phase 3 (MVP)**:
- UI pages: T022, T023 (independent components)
- Tests: T031, T032 (parallel with implementation)

**Within Phase 4-6**:
- UI components: marked with [P] where independent
- Each user story can run in parallel with others (different team members)

**After Phase 2 Complete**:
- Phases 3, 4, 5, 6 can be staffed in parallel by different developers/teams
- Polish tasks (Phase 7) can run alongside Phase 6 if needed

---

## Implementation Strategy

### MVP First (User Story 1 Only) - Recommended Path

**Goal**: Deliver upload → view → download in shortest time, validate architecture, then expand

**Phases to Execute**:
1. **Phase 1: Setup & Infrastructure** (T001-T007) - 1-2 days
2. **Phase 2: Foundational - Document Service & Core Operations** (T008-T021) - 2-3 days
3. **Phase 3: User Story 1 - Core Document Management** (T022-T033) - 2-3 days

**Deliverable**: 
- Users can upload documents (PDF, Word, Excel, text, images)
- Users can view personal and project documents
- Users can download files with correct MIME types
- All operations logged in audit trail
- **Task Count**: 33 tasks (T001-T033)
- **Stop & Validate**: Deploy MVP, gather user feedback before continuing

### Incremental Expansion (Full Feature)

**After MVP validation, add incrementally**:
- **Phase 4: User Story 2 - Sharing** (T034-T039) - 1-2 days → Enables collaboration
- **Phase 5: User Story 3 - Search** (T040-T044) - 1 day → Improves discoverability
- **Phase 6: User Story 4 - Integration** (T045-T053) - 1-2 days → Deep team integration
- **Phase 7: Polish** (T054-T060) - 1 day → Final hardening

**Total**: 60 tasks across all features
**Timeline**: MVP in ~5 days; full feature in ~10-12 days

### Parallel Team Strategy (If Multiple Developers Available)

**Setup Phase**: 1 developer - Configure infrastructure (Phase 1: 1-2 days)

**Foundation Phase**: 2-3 developers in parallel:
- Dev A: Service methods (T010-T014, T018-T020)
- Dev B: API endpoints (T015-T017)
- Dev C: Tests (T031-T033)

**MVP Phase**: 2-3 developers in parallel:
- Dev A: Upload & My Documents pages (T022, T023, T025, T026)
- Dev B: Document Detail & edit/delete (T024, T027-T029)
- Dev C: Tests & validation (T031-T033)

**After MVP**: 4+ developers in parallel (Phase 4, 5, 6 by feature)

---

## Notes

- **[P]**: Task can run in parallel with other [P] tasks (different files, no cross-task dependencies)
- **[US1], [US2], [US3], [US4]**: Task belongs to User Story 1, 2, 3, or 4 (supports traceability)
- **File paths**: Exact ASP.NET Core folder structure assumed (adjust per plan.md)
- **Tests**: Unit tests (in-memory DbContext), integration tests (SQLite DbContext), performance tests measure against targets
- **MVP Stopping Point**: After Phase 3, users have full value for core scenario; can deploy and gather feedback
- **Checkpoint Pattern**: Each phase ends with a checkpoint for validation and go/no-go decision
- **Concurrency**: rowversion on Document and DocumentShare for optimistic locking; always handle DbUpdateConcurrencyException
- **Authorization**: All service methods validate actor before returning data; no metadata leaks on 404
- **Audit Trail**: DocumentActivity records every operation; preserved on document delete (FK nullable)
- **Performance Targets**: Search/list ≤ 2 sec (500 docs), preview ≤ 3 sec, upload < 30 sec (25 MiB)
