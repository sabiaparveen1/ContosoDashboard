<!--
Sync Impact Report
Version change: unversioned scaffold -> 1.0.0 (initial adoption)
Modified principles: None (initial adoption)
Added principles: I. Training-First, Offline-Ready Learning; II. Security Concepts With Honest Boundaries; III. Layered, Testable Design; IV. Spec-Driven, Verifiable Changes; V. Simplicity and Documented Limitations
Added sections: Training Context and Technical Constraints; Spec-Driven Development and Quality Gates
Removed sections: None
Follow-up TODOs: Confirm the original ratification date; it is not recorded in the repository.
-->

# ContosoDashboard Constitution

## Core Principles

### I. Training-First, Offline-Ready Learning
The application and its examples MUST remain explicitly scoped to training and educational use.
Baseline development and use MUST remain local and offline, without requiring cloud accounts or
external services. Project guidance MUST identify mock features and known limitations rather than
imply production readiness. This keeps the training experience accessible and self-contained.

### II. Security Concepts With Honest Boundaries
Features involving identity or data access MUST demonstrate explicit authentication and
authorization, least privilege, user/resource isolation, and defense in depth where applicable.
Mock authentication, sample identities, and simplified controls MUST be labeled training-only and
MUST NOT be represented as production security. Specifications for such changes MUST state the
relevant access boundaries and expected behavior for unauthorized requests.

### III. Layered, Testable Design
Changes MUST preserve clear responsibilities between UI, business services, domain models, and
persistence. Application code MUST use interface abstractions and dependency injection when an
infrastructure implementation is intended to be replaceable. Services that return protected
resources MUST enforce access rules rather than rely only on UI visibility. New or changed behavior
MUST have appropriate automated tests.

### IV. Spec-Driven, Verifiable Changes
Substantial feature work MUST follow the repository's Spec Kit workflow: define a specification,
review the plan, generate ordered tasks, then implement. Specifications MUST express testable
acceptance criteria. Changes MUST run the relevant available tests and report the actual validation
performed; incomplete or unavailable validation MUST be stated rather than implied.

### V. Simplicity and Documented Limitations
Solutions MUST favor the simplest design that meets the educational objective and the requirements.
Examples and project guidance MUST remain consistent with actual behavior. Known training
limitations MUST be documented, and production-grade complexity MUST NOT be added without a
demonstrated project need.

## Training Context and Technical Constraints

- The supported baseline is ASP.NET Core 8, Blazor Server, Entity Framework Core, and SQL Server
  LocalDB, as described in the project documentation.
- The baseline MUST work locally without cloud integrations or external service dependencies.
  Changes to the baseline stack or dependencies MUST be documented in the relevant specification
  and project guidance.
- Mock authentication and seeded sample users exist for training only. Any production migration
  notes are educational guidance, not a claim that this project is production-ready.
- Sample data and examples MUST remain fictional and MUST NOT contain real credentials or personal
  data.

## Spec-Driven Development and Quality Gates

- Feature specifications MUST describe user-visible behavior, constraints, and acceptance criteria
  before implementation begins.
- Plans and task lists MUST be reviewed at the workflow's review gates before the next phase.
- Implementation MUST include or update tests appropriate to the changed behavior and MUST run the
  relevant test/build checks available in the repository.
- Reviews MUST check security boundaries, documentation accuracy, and consistency with these
  principles. Any unresolved deviation or validation gap MUST be recorded in the feature artifacts
  or review summary.

## Governance

This constitution governs project-level decisions. Proposed amendments MUST document their
rationale, affected principles or sections, and any migration implications. Repository maintainers
MUST review and approve amendments before merge. Feature specifications, plans, and code reviews
MUST check for compliance; any exception MUST be explicit, scoped, and justified in the related
feature artifacts.

Constitution versions use semantic versioning. A MAJOR increment marks a backward-incompatible
governance change or removal/redefinition of a principle. A MINOR increment marks a new principle
or section, or materially expanded guidance. A PATCH increment marks clarifications, wording
improvements, and non-semantic refinements.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): original adoption date not recorded | **Last Amended**: 2026-11-19
