# ADD Drivers

## **Terminology**

*Editorial Workflow* \- name given to the chain of operations that change requests are required to pass from the moment they are created to the moment they are published in the project.

*Manager* \- name given to users with authentication capabilities that have permissions associated with the editorial workflow (which includes the standard roles such as author, reviewer and system administrator).

*Transition Manager* \- name given to managers whose role allows them to perform a transition of a change request from one stage to another. Reviewers, legal analysts and publishers are examples of this type of manager.

*Author\** \- system users that can create content in a project and submit it to be approved. (Draft, Draft \-\> Review)

*Reviewer\** \- system users that can approve content batches coming from authors and move it along the editorial workflow, either to legal analysts or final approvers. (Review \-\> Legal, Review \-\> Approved)

*Legal Analyst\** \- system users that can analyze content changes for compliance to legal standards and move it along the editorial workflow towards the final approvers. (Legal \-\> Approved)

*Publisher\** \- system users that sit at the top of the editorial workflow and can publish approved content batches. (Approved \-\> Published)

*System Administrator (SysAdmin for short)* \- system users that have the highest level of privilege and can perform any role’s actions, alongside other administrator functions such as creating and assigning roles, or making alterations to the workflow.

*End User* \- user without authentication capabilities that accesses and navigates the published product.

*Change Request* \- request made by a manager to pass their draft through the editorial workflow, with the objective of publishing their changes.

*Change Request Comment* \- comment made on any change request by a manager (the creator of the request or transition managers) that can be used for discussion or indicative of stage transitions

*Content Batch* \- a set of content changes made by managers.

*Stage* \- step in the editorial workflow at which a change request can be, which, for example can be Draft, Review, Legal, Approved or Published. A change request can only be at one stage at a time.

*(Stage) Transition* \- name given to a transition of a change request from one stage to another, made by a transition manager.

*Draft* \- State in which a content request is being created and saved ‘locally’ in a author’s workspace.

*Review\** \- State in which a content request is under review, which can be approved by a reviewer.

*Legal\** \- State in which a content request is under review by legal analysts, which check for compliance to legal standards.

*Approved\** \- State in which a content request is approved and ready to be published, only needing the approval of a publisher.

*Published\** \- State in which a content batch is published and accessible for end users.

\* *\- terms that are not generalized and are associated with the use case indicated in the scenario given by the professor*

## **Primary Functionalities**

### UC-1 : Create Change Request

> A manager should be able to create a change request with their drafted changes and submit it to the next stage, if it exists. An interface should be in place to facilitate the submission of new change requests.

**As a** Manager
**I want** to create a change request with my drafted changes and submit it to the next workflow stage
**So that** I can formally initiate the editorial review process

**Acceptance Criteria**

- **Happy Path**

    Given I have drafted content and valid permissions,
    when I click “Submit Change Request,”
    then a new change request is created in the Draft stage and I see confirmation.

-     Unhappy Paths

        If required fields (e.g. title, content) are missing,
        then submission is blocked and validation messages display.

        If I lack “create” permission,
        then I receive an “Unauthorized” error and no request is created.

        If the backend service is unavailable,
        then I see a retry option and a clear error message.

### UC-2 : Transition Forwards

> A transition manager should be able to transition a change request to one of the next available stages. This transition should notify the involved roles/users.

**As a** Transition Manager
**I want** to move a change request forward to the next available stage
**So that** the appropriate stakeholders are notified and the workflow progresses smoothly

**Acceptance Criteria**

- **Happy Path**

    Given a request in stage “Review”,
    When I select “Approve” and confirm,
    Then the request advances to “Legal” (or “Approved” if no legal step) and assigned users are notified.

-     Unhappy Paths

        If the request has unresolved conflicts,
        Then the “Approve” button is disabled with tooltip “Resolve all conflicts first.”

        If I try to advance beyond the final stage,
        Then I receive “No further stages” and no action is taken.

        If notification dispatch fails,
        Then the transition still completes, an error log entry is created and I am alerted that there will be another communication attempt.

### UC-3 : Transition Backwards

> A transition manager should be able to transition a change request to one of the previous available stages. This transition should notify the involved roles/users.

**As a** Transition Manager
**I want** to move a change request back to a previous stage
**So that** I can request revisions or corrections when necessary

**Acceptance Criteria**

- **Happy Path**

    Given a request in “Legal”,
    When I choose “Request Changes”, enter a reason, and confirm,
    Then the request returns to “Review” and the creator receives a notification.

-     Unhappy Paths

        If I do not supply a reason,
        Then the “Request Changes” action is blocked with “Reason required.”

        If the target previous stage no longer exists (deleted),
        Then I see “Cannot revert: stage missing” and the request remains unchanged.

### UC-4 : Workflow Creation

> A system administrator should be able to create a new editorial workflow.

**As a** System Administrator
**I want** to create a new editorial workflow
**So that** I can define the overall approval process tailored to our organization’s needs

**Acceptance Criteria**

- **Happy Path**

    Given I open “New Workflow”,
    When I name it “Press Release”, choose standard stages, and save,
    Then the workflow appears in the list and is available for assignment.

-     Unhappy Paths

        If I choose a duplicate workflow name,
        Then I receive “Name already in use” and must pick another.

        If I leave it blank,
        Then validation prevents saving with “Name is required.”


### UC-5 : Stage Creation

> A system administrator should be able to create a new stage within an editorial workflow, with rules regarding the user/roles that can perform stage transitions and other rules.

**As a** System Administrator
**I want** to add a new stage to an existing editorial workflow with rules for who can perform transitions
**So that** I can customize and extend the workflow’s steps as requirements evolve

**Acceptance Criteria**

- **Happy Path**

    Given an existing workflow,
    When I click “Add Stage,” define “SEO Review,” assign roles “SEO Analyst,” and save,
    Then the stage is inserted in the flow and visible in the UI.

-     Unhappy Paths

        If the stage name duplicates another in the same workflow,
        Then I get “Stage name must be unique.”

        If no roles are assigned,
        Then I see “At least one role required” and cannot save.

### UC-6 : Stage Edition

> A system administrator should be able to edit an existing stage within an editorial workflow, including rules regarding the user/roles that can perform stage transitions and other rules.

**As a** System Administrator
**I want** to edit an existing workflow stage’s properties and transition rules
**So that** I can keep the workflow aligned with changing policies or team structures

**Acceptance Criteria**

- **Happy Path**

    Given stage “Legal” exists,
    When I change its name to “Compliance Review” and update allowed roles,
    Then changes persist and subsequent transitions respect the new rules.

-     Unhappy Paths

        If I remove all roles,
        Then the UI blocks the update with “Must assign at least one role.”

        If someone is mid-transition into that stage,
        Then I receive a warning “Active requests exist—confirm to proceed”, and can cancel or force.

### UC-7 : Stage Deletion

> A system administrator should be able to delete an existing stage within an editorial workflow, raising warnings on stages that are associated with the now deleted stage.

**As a** System Administrator
**I want** to delete an existing stage from an editorial workflow and receive warnings about any dependencies
**So that** I can safely remove obsolete steps without breaking existing workflows

**Acceptance Criteria**

- **Happy Path**

    Given stage “Draft Copyediting” has no in‑flight requests,
    When I press “Delete” and confirm,
    Then the stage is removed from the workflow.

-     Unhappy Paths

        If there are transition requests aimed at that stage,
        Then a warning will show "There are pending transition requests."
        And deletion is reliant upon accepting the removal of all transition requests
        And all affected users are notified of the request's removal.

        If requests are currently in that stage,
        Then deletion is blocked with “Cannot delete stage with active requests.”

        If the stage is terminal (e.g. “Published”),
        Then deletion is disallowed with “Cannot remove terminal stage.”

### UC-8 : Workflow Dashboard

> A manager should be able to access a dashboard that documents the changes to content overtime.

**As a** Manager  
**I want** to view a dashboard listing change requests and their current workflow stage  
**So that** I can track the progress of each request and follow up as needed  

**Acceptance Criteria**

- **Happy Path**

    When I navigate to “Dashboard”,  
    Then I see a searchable and sortable list of recent change requests,  
    And for each request, I can view its current stage in the workflow.

- **Unhappy Paths**

    If the data service fails,  
    Then I see “Unable to load change requests” and a “Retry” button.

### UC-9 : Change Request Interface

> A transition manager should be able to view information regarding a particular change request, including the changes, roles involved, author, comments, stage and others.

**As a** Transition Manager
**I want** to inspect detailed information about a specific change request (including content changes, involved roles, creator, comments, and current stage)
**So that** I can make informed decisions during reviews and approvals

**Acceptance Criteria**

- **Happy Path**

    When I click on a request ID,
    Then a detail pane opens showing diff-view, metadata (creator, timestamps), assigned roles, and comments.

- **Unhappy Paths**

    If I lack “view” permission,
    Then access is denied with “Unauthorized.”

    If the request was deleted,
    Then I see “Request not found” and return to the list.

### UC-10 : Conflict Between Content Changes

> A transition manager should be able to solve conflicts between new changes that were made for older (overwritten) content.

**As a** Transition Manager
**I want** to detect and resolve conflicts between new changes and earlier content versions
**So that** I ensure content integrity and consistency before approval or publication

**Acceptance Criteria**

- **Happy Path**

    When I open a request whose draft conflicts with the current live version,
    Then I see inline conflict markers and options to accept one side or merge manually.

- **Unhappy Paths**

    If automatic merge fails,
    Then I get “Merge conflict—please resolve manually” and conflict highlighting.

    If I attempt to publish without resolving conflicts,
    Then the “Publish” button is disabled with “Resolve all conflicts first.”

### UC-11 : Workflow Edition

> A system administrator should be able to edit an existing editorial workflow (rename or delete it).

**As a** System Administrator  
**I want** to edit an existing editorial workflow (rename or delete it)  
**So that** I can update approval processes as organizational needs evolve  

**Acceptance Criteria**

- **Happy Path**

    Given I open an existing workflow named “Press Release”,  
    When I rename it to “Product Launch Approval”, modify the stages, and save,  
    Then the updated workflow is reflected in the list and used in future assignments.

- **Unhappy Paths**

    If I rename it to a name already in use,  
    Then I receive “Name already in use” and must choose a unique name.

    If I clear the name field and try to save,  
    Then validation prevents saving with “Name is required.”

    If I remove all stages,  
    Then validation prevents saving with “At least one stage is required.”

### UC-12 : Analytics Dashboard

> A manager should be able to view workflow performance analytics on the dashboard.

**As a** Manager  
**I want** to view workflow performance analytics on the dashboard  
**So that** I can monitor workflow health and identify bottlenecks or delays  

**Acceptance Criteria**

- **Happy Path**

    When I navigate to “Dashboard”,  
    Then I see charts displaying:
    - Number of requests per workflow stage  
    - Average acceptance and refusal wait times  
    - Trends over time  

- **Unhappy Paths**

    If the data service fails,  
    Then I see “Unable to load metrics” and a “Retry” button.

    If I filter by an invalid date range (e.g., end date before start date),  
    Then I receive “Please select a valid date range.”

### UC-13 : Telemetry

> A system administrator should be able to view telemetry data regarding the performance of the editorial workflow.

**As a** Manager  
**I want** to view system telemetry and trace data on the observability dashboard
**So that** I can monitor  system health, performance, and troubleshoot issues effectively

**Acceptance Criteria**

- **Happy Path**

    **Given** I navigate to the **"Observability Dashboard"**,
    Then I see the following panels populated with telemetry data:
    - CPU, memory, and I/O usage metrics from Prometheus
    - HTTP request rates, error rates, and latencies (RED metrics)
    - Distributed trace visualization for recent requests via Jaeger
    - Service dependency graph and latency breakdown
   **And** the metrics auto-refresh every 30 seconds

- **Unhappy Paths**

   If Prometheus is unreachable,
   Then I see an error message: "Metrics temporarily unavailable" and a "Retry" button

   If Jaeger fails to return traces,
   Then the trace panel shows "Unable to fetch trace data" with a "Refresh" option

   If the user applies a filter with invalid parameters (e.g., duration < 0),
   Then display a validation error: "Please enter a valid time range."

   If no data is available for the selected time range,
   Then show "No telemetry data available for this period." instead of empty charts


## **Quality Attributes**

### QA-1 : Stage Transitions

*Performance*

Stage transitions complete within 3 seconds, even when triggering complex validations and notifications.

*Associated Use Case: UC-2 and UC-3*

### QA-2 : Read Access

*Performance, Usability*

The system can handle 1000 concurrent read operations while maintaining sub-second (\<1000ms) response times.

*Associated Use Case: All*

### QA-3 : Workflow Dashboard Load

*Performance*

Dashboard views displaying workflow status load within 3 seconds, even when showing 500+ content items.

*Associated Use Case: UC-10*

### QA-4 : Number of Stages

*Extensibility, Scalability, Maintainability*

The system should be able to handle a large number of stages (10+) for a single workflow.

*Associated Use Case: UC-5*

### QA-5 : Notification Time

*Performance, Usability*

When a stage transition is performed, a notification should appear to the appropriate managers within 5 seconds.

*Associated Use Case: UC-2 and UC-3*

### QA-6 : Permission for Transition

*Security*

The system can handle 100 concurrent read operations while maintaining sub-second (\<1000ms) response times.

*Associated Use Case: UC-2 and UC-3*

### QA-7 : Transition Load

*Performance, Security*

There should be 0 cases where unauthorized transitions are performed by underprivileged managers/users.

*Associated Use Case: UC-2 and UC-3*

## **Constraints**

### CON-1 : Deadline

Implementation must be completed within the project timeline (by June 3rd-4th deadline).

### CON-2 : Compatibility

Must integrate with existing Piranha CMS content model without breaking core functionality.

### CON-3 : Stakeholder

Must align with Content'r'us commercial strategy for CMS offerings.

### CON-4 : Large Teams

Must handle large teams with at least 100 members, all working concurrently, while maintaining high performance.

## **Architectural Concerns**

### AC-1 : Study Existing Architecture

Explore the current PiranhaCMS implementation and pinpoint the systems where integration will be required.

### AC-2 : Decide Between Fork and Module

Discuss about how we will integrate the new feature with the current framework, one option being the forking of the repository and performing alterations to the source code, and the other being the development of a separate module that can be optionally added by the users.

### AC-3 : Allocate Work

Allocate work to the members of the development team, ensuring that each member stays informed of all components of the project.

### AC-4 : Use of PiranhaCMS

Ensure that the new feature is compatible with the existing PiranhaCMS architecture and does not break any core functionalities.

### AC-5 : Documentation

Ensure that all components of the project are well documented, including the architecture, design decisions, and implementation details.

### AC-6 : Usability
Ensure that the new feature is easy to use and understand for both developers and end users, with clear documentation and user interfaces.