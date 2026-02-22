# ProQuote Architecture

## Layers

- `ProQuote.Domain`
  - Core business entities, enums, and domain rules.
  - No dependency on `Application`, `Infrastructure`, `Web`, or `UI`.
- `ProQuote.Application`
  - Use-cases, DTOs, and service contracts.
  - Depends only on `Domain`.
  - Defines orchestration boundaries for business workflows.
- `ProQuote.Infrastructure`
  - EF Core, repositories, external integrations, and concrete service implementations.
  - Implements `Application` contracts.
  - Depends on `Application` and `Domain`.
- `ProQuote.Web`
  - API controllers and Blazor pages/layouts.
  - Handles HTTP/UI concerns only.
  - Depends on `Application`, `Infrastructure`, and `UI`.
- `ProQuote.UI`
  - Reusable visual components, tokens, and presentation behavior.
  - No business logic.

## Dependency Rules

- Allowed direction: `Web -> Application -> Domain` and `Web -> Infrastructure`.
- `Infrastructure` may depend on `Application` and `Domain`.
- `Domain` must remain persistence-agnostic and framework-light.
- Controllers/pages should not contain workflow logic; they call application use-cases/services.
- Shared styling and component behavior should live in `ProQuote.UI`, not feature pages.

## Current Architectural Pattern

- Query and command concerns are currently mixed in some services.
- New work should separate:
  - Read paths: query-focused services/projections.
  - Write paths: explicit application use-cases.

## First Use-Case Migrations

- Introduced `Award Buyer Quote` as an application use-case:
  - Command model for request payload at application boundary.
  - Use-case implementation in `Application`.
  - Existing infrastructure service still executes domain updates.
  - API endpoint now depends on the use-case instead of directly orchestrating.
- Introduced `Publish Buyer RFQ` as an application use-case:
  - Centralized draft-to-published workflow, validation, and audit logging.
  - Buyer RFQ create/update API flows call the publish use-case when `Publish=true`.
  - Added dedicated publish endpoint (`POST /api/v1/buyer/rfqs/{id}/publish`).
- Introduced `Send RFQ Invitations` as an application use-case:
  - Centralized invitation command handling at the application boundary.
  - Buyer invitation send API flow now delegates to the use-case.
  - Existing invitation service remains the infrastructure workflow executor.
- Introduced `Close RFQ (Admin)` as an application use-case:
  - Centralized admin close validation and audit logging.
  - Admin RFQ list page now delegates close action to the use-case.
- Introduced `Review Supplier Registration (Admin)` as an application use-case:
  - Centralized approve/reject validation for pending suppliers.
  - Pending suppliers admin page now delegates approve/reject actions to the use-case.
- Introduced `Move RFQ To Under Evaluation (Admin)` as an application use-case:
  - Centralized state-transition validation and audit logging.
  - Admin RFQ list page now delegates the move action to the use-case.
- Introduced `Cancel RFQ (Admin)` as an application use-case:
  - Centralized cancellation rules and reason assignment.
  - Admin RFQ list page now delegates cancel action to the use-case.
- Introduced `Mark Notifications as Read` as application use-cases:
  - Added explicit use-cases for marking a single notification and marking all notifications as read.
  - Notification API endpoints and all role layouts/pages now delegate write operations to these use-cases.
- Introduced initial quote differentiation foundations:
  - Added canonical quote normalization model for line-level baselines and variance calculations.
  - Added configurable weighted scoring template (price, lead time, coverage) with buyer UI save flow.
  - Quote comparison now includes computed score breakdown per quote.

This is the baseline pattern for future workflows:
- Add new workflows as explicit command/use-case pairs before wiring UI or controllers.

## Next Recommended Steps

1. Migrate remaining buyer/admin state-changing workflows to explicit use-cases.
2. Add application-level tests around use-cases (without web host bootstrapping).
3. Introduce policy-based authorization matrix and central permission constants.
4. Add outbox pattern for notifications and audit events on critical mutations.
