# Decision Record #8 - Change Request as the Unit of Editorial Workflow (25-05-2025)

## Status: Accepted

## Summary
> *In the context of **representing a set of changes that travels within the editorial workflow**, we decided for **the Change Request as a first-class domain entity, encapsulating content, workflow, stage, and audit information** to achieve **traceability, auditability, and robust workflow orchestration**, accepting **the added complexity of managing a new aggregate and its lifecycle**.*

## Context

The editorial workflow extension for PiranhaCMS required a mechanism to represent and manage sets of content changes as they progress through multiple workflow stages (e.g., Draft, Review, Legal, Approved, Published). The system needed to:
- Track the state and history of content as it moves through the workflow.
- Support multi-stage approvals, role-based transitions, and audit trails.
- Enable collaboration, comments, and conflict resolution for concurrent edits.
- Integrate with existing PiranhaCMS content and user models without breaking core functionality.

Several approaches were considered:
- **Direct Content State Mutation**: Modifying the content entity directly as it moves through stages. This approach risks data loss, complicates rollback, and makes audit trails difficult.
- **Flat Versioning**: Creating a new content version for each change, with workflow state as a property. This approach lacks explicit grouping of related changes and does not support collaborative review or comments.
- **Change Request Aggregate**: Introducing a dedicated Change Request entity to encapsulate a set of changes, workflow state, creator, timestamps, and associated metadata. This approach aligns with DDD principles and supports extensibility, audit, and collaboration.

## Decision

We have decided to implement the **Change Request** as the core unit of change in the editorial workflow. The Change Request is a domain aggregate that:
- **Encapsulates a snapshot of content** at the time of request creation, ensuring that changes are reviewable and auditable.
- **Tracks workflow association** (`WorkflowId`), current stage (`StageId`), and status (`ChangeRequestStatus`), supporting multi-stage, role-based transitions.
- **Records creator and timestamps** (`CreatedById`, `CreatedAt`, `LastModified`) for accountability and audit.
- **Links to the target content** (`ContentId`), enabling integration with PiranhaCMS content entities.
- **Supports notes and comments** for collaborative review and decision-making.
- **Maintains a status enum** (Draft, Submitted, InReview, Approved, Rejected, Published) to drive UI and business logic.

This model is implemented in `core/Piranha/Models/ChangeRequest.cs` and is supported by API models, services, and UI components for creation, transition, and review. The Change Request is the only entity allowed to move between workflow stages, and all transitions, approvals, and rejections are recorded against it, providing a complete audit trail.

## Rationale
- **Traceability & Auditability**: By encapsulating all relevant data, the Change Request enables full traceability of who made what change, when, and why, supporting compliance and governance requirements.
- **Separation of Concerns**: Decouples editorial workflow logic from core content entities, minimizing risk to existing CMS functionality and simplifying future upgrades.
- **Extensibility**: The aggregate can be extended with comments, transition history, and custom metadata without breaking existing workflows.
- **Conflict Resolution**: Supports collaborative editing and conflict management by isolating changes until they are approved and merged.
- **Alignment with DDD**: Follows Domain-Driven Design best practices by modeling the editorial workflow as a bounded context with Change Request as the aggregate root.
- **UI/UX Consistency**: Enables clear interfaces for managers, reviewers, and administrators to view, comment, and act on change requests at each stage.

## Consequences
- All editorial changes must be encapsulated in a Change Request to enter the workflow.
- The system must manage the lifecycle of Change Requests, including creation, transition, approval, rejection, and publication.
- Additional infrastructure is required for storing content snapshots, comments, and transition history.
- The approach enables robust audit trails, rollback, and compliance reporting, at the cost of increased model and service complexity.
- Future features (e.g., batch operations, advanced conflict resolution, notification hooks) can be built on top of the Change Request aggregate.

## Associated Drivers
*UC-1, UC-2, UC-3, UC-9, UC-10, CON-2*

## References
- [ChangeRequest.cs](../../core/Piranha/Models/ChangeRequest.cs)
