 **Evolving Piranha CMS for “Contents’r’us” \- Scenario 2**

* Alexandre Pedro Ribeiro, 108122  
* Maria João Machado Sardinha, 108756  
* Miguel Aido Miragaia, 108317  
* Miguel da Silva Pinto, 107449

# **1\. System Analysis & Project Vision**

## **1.1 Alignment of Piranha CMS with Scenario Objectives**

**Strengths:**

* **Content Management:** Piranha CMS offers robust content management capabilities, including support for Sites, Pages, Posts, and Media. Its modular design allows for flexible content structuring, which aligns well with scenarios requiring diverse content types and hierarchies.  
    
* **Modular and Extensible Architecture:** Built on .NET 8, Piranha CMS is cross-platform and supports both integrated and headless modes. Its architecture is designed for extensibility, allowing developers to build custom modules and extend functionalities as needed. ​  
    
* **User-Friendly Interface:** The CMS provides a responsive and intuitive admin interface, making it accessible to non-technical users and facilitating content management tasks.  
    
* **Traceability:** Piranha CMS includes logging mechanisms that trace how requests are handled and how actions are executed throughout the system. This feature enables tracking of user and system activities, supporting accountability and providing a basis for compliance and troubleshooting.


**Limitations:**

* **Editorial Workflow:** Piranha CMS lacks a built-in, configurable editorial workflow system. While it supports basic content states (e.g., draft, published), it does not provide multi-stage approval processes or customizable workflows out of the box.

* **User & Permission Management:** The CMS includes basic user and role management features. However, it does not offer advanced permission systems or granular access controls required for complex editorial scenarios.

* **Publishing Mechanisms:** While Piranha CMS supports content publishing, it lacks advanced features such as scheduled publishing, multi-channel delivery, and publishing workflows, which are essential for scenarios involving coordinated content releases.

* **Notifications and Audit Trails:** The CMS does not provide built-in notification systems or comprehensive audit and versioning features, which are crucial for tracking content changes and ensuring accountability in editorial processes. ​

[https://piranhacms.org/docs/v7/content/introduction](https://piranhacms.org/docs/v7/content/introduction)  
[https://medevel.com/piranha-cms/](https://medevel.com/piranha-cms/)  
[https://github.com/PiranhaCMS/piranha.core/issues/14](https://github.com/PiranhaCMS/piranha.core/issues/14)

## **1.2 Vision & Strategic Goals**

**Vision:** Develop an advanced content management platform that extends Piranha CMS by incorporating robust editorial workflows, granular user permissions, sophisticated publishing mechanisms, and comprehensive notification and audit systems, thereby meeting the complex needs of modern content-driven organizations.

**Strategic Goals:**

1. **Implement Configurable Editorial Workflows:** Introduce multi-stage approval processes with customizable stages and transitions to ensure content quality and compliance.​

2. **Enhance User & Permission Management:** Develop a granular permission system that allows precise control over user actions and access levels within the CMS.​

3. **Advance Publishing Capabilities:** Enable scheduled publishing, support for multiple delivery channels, and integration with external platforms to streamline content dissemination.​

4. **Integrate Notification Systems:** Implement real-time notifications to keep users informed about content status changes, approvals, and other critical events.​

5. **Establish Comprehensive Audit & Versioning:** Provide detailed audit trails and version control to track content changes, facilitate rollbacks, and ensure accountability.

**Key Quality Attributes:**

* **Extensibility:** The system should be designed to accommodate future enhancements and integrations with minimal disruption.​

* **Scalability:** Ensure the platform can handle increasing amounts of content, users, and traffic without performance degradation.​

* **Security:** Implement robust security measures to protect content integrity and user data.​

* **Usability:** Maintain an intuitive and user-friendly interface to facilitate adoption and efficient content management.

* **Maintainability:** Design the system for ease of maintenance, allowing for quick updates and issue resolution.

* **Performance:** The platform must consistently deliver fast response times and high throughput, even under peak load, ensuring low latency for user interactions and content operations, with performance targets such as sub-second responses for common actions and efficient dashboard views maintained as content and user volumes grow

# **2\. Choose an Architectural Design Methodology**

## **2.1 Architectural Design Methods \- Pros & Cons:**

This section was made using Claude ([https://claude.ai/share/ffee8cc4-cbae-465f-a6d3-c91eb0afea2d](https://claude.ai/share/ffee8cc4-cbae-465f-a6d3-c91eb0afea2d)).

More on ADD in the next section and also in the following books: [https://learning.oreilly.com/library/view/software-architecture-in/9780136885979/ch20.xhtml\#ch20lev1sec2](https://learning.oreilly.com/library/view/software-architecture-in/9780136885979/ch20.xhtml#ch20lev1sec2)  
[https://learning.oreilly.com/library/view/designing-software-architectures/9780134390857/ch03.xhtml\#ch03lev2sec1](https://learning.oreilly.com/library/view/designing-software-architectures/9780134390857/ch03.xhtml#ch03lev2sec1)

### **2.1.1. Attribute-Driven Design (ADD)**

#### Description:

ADD is a systematic approach that designs architecture based on quality attributes (non-functional requirements) like performance, security, and scalability.

#### Advantages for Your Project:

* **Quality-Focused**: Explicitly addresses your performance and scalability requirements  
* **Systematic**: Provides clear steps for decomposing the system  
* **Traceability**: Maintains links between requirements and design decisions  
* **Iterative**: Allows incremental refinement of the architecture

#### Disadvantages for Your Project:

* **Potentially Complex**: Can be heavyweight for smaller teams  
* **Time-Consuming**: The attribute analysis phase can delay implementation  
* **Less Domain Focus**: Emphasizes quality attributes over domain structures

#### Fit with Your Current Work:

You've already done extensive domain modeling using DDD. ADD would complement this by ensuring your workflow system meets the performance needs when multiple editorial teams work simultaneously.

### **2.1.2. Domain-Driven Design (DDD)**

#### Description:

DDD focuses on modeling complex domains by connecting implementation to an evolving domain model, using a ubiquitous language shared by developers and domain experts.

#### Advantages for Your Project:

* **Domain Alignment**: Perfect for complex editorial workflows and content lifecycle management  
* **Ubiquitous Language**: Helps bridge technical and business stakeholders  
* **Strategic Design**: Bounded contexts help manage complexity  
* **Tactical Patterns**: Aggregates, entities, and value objects provide implementation guidance

#### Disadvantages for Your Project:

* **Learning Curve**: Requires team commitment to DDD concepts  
* **Less Focus on Technical Qualities**: May need supplementation for performance concerns  
* **Implementation Overhead**: Strict DDD can lead to more complex code initially

#### Fit with Your Current Work:

You've already extensively applied DDD principles in your domain modeling, so continuing with this approach would provide consistency. Your bounded contexts for Editorial Workflow, Content Management, etc., show strong DDD thinking.

### **2.1.3. Architecture-Centric Design Method (ACDM)**

#### Description:

ACDM focuses on evolving architectures incrementally through iterations, starting with a base architecture that is refined over time as requirements become clearer.

#### Advantages for Your Project:

* **Iterative Approach**: Allows refining the workflow architecture as you better understand requirements  
* **Risk Management**: Early focus on architectural risks  
* **Stakeholder Involvement**: Encourages continuous feedback  
* **Architectural Drivers**: Identifies key influences on architecture

#### Disadvantages for Your Project:

* **Less Prescriptive**: Provides fewer specific guidelines than ADD  
* **Potentially Ambiguous**: Can be hard to determine when architecture is "complete"  
* **Resource Intensive**: Requires ongoing architecture evaluation

#### Fit with Your Current Work:

ACDM could help you evolve from your current DDD model to implementation by providing structure for the refinement process, especially useful as you develop the editorial workflow features incrementally.

### **2.1.4. Test-Driven Development (TDD)**

#### Description:

TDD is a development process where tests are written before implementation code, driving the design through rapid feedback cycles.

#### Advantages for Your Project:

* **Verifiable Requirements**: Ensures workflow transitions and permissions work correctly  
* **Continuous Verification**: Catches regressions early  
* **Emergent Design**: Architecture evolves organically from requirements  
* **Maintainable Code**: Results in highly testable components

#### Disadvantages for Your Project:

* **Tactical Focus**: More code-level than architectural  
* **Learning Curve**: Requires discipline and skill to implement effectively  
* **Time Investment**: Initial development may feel slower  
* **Less Strategic**: May not address cross-cutting concerns effectively

#### Fit with Your Current Work:

TDD would be more complementary to your architectural strategy rather than forming the core methodology. It could help implement the bounded contexts you've defined, but may not provide enough architectural guidance on its own.

### **2.1.5. TOGAF Architecture Development Method (ADM)**

#### Description:

TOGAF ADM is an enterprise architecture framework that provides a comprehensive approach to planning, designing, implementing, and governing enterprise architectures.

#### Advantages for Your Project:

* **Comprehensive**: Addresses all aspects of enterprise architecture  
* **Well-Documented**: Provides detailed guidance and templates  
* **Governance-Focused**: Strong on change management and implementation  
* **Business Alignment**: Emphasizes business value

#### Disadvantages for Your Project:

* **Heavyweight**: May be overly complex for your CMS project  
* **Generic**: Not specifically tailored for software development  
* **Resource Intensive**: Requires significant documentation  
* **Slow Adoption**: Full implementation cycle can be lengthy

#### Fit with Your Current Work:

TOGAF might be too heavyweight for your specific CMS enhancement project, especially given that you're extending an existing system rather than creating an enterprise-wide architecture.

### **2.1.6. Top-Down Approach**

#### Description:

A top-down design starts with high-level architecture and progressively decomposes it into more detailed components and interactions.

#### Advantages for Your Project:

* **Strategic Vision**: Ensures the big picture of editorial workflows is understood first  
* **Consistency**: Creates a coherent overall structure  
* **Easier Communication**: Provides clear architectural vision to stakeholders  
* **Early Focus on Interfaces**: Defines component boundaries early

#### Disadvantages for Your Project:

* **Potential Rigidity**: May be less adaptable to changing requirements  
* **Late Validation**: Detailed problems might be discovered late  
* **Abstract Start**: Can delay tackling concrete implementation challenges  
* **Risk of Over-engineering**: May create unnecessary abstractions

#### Fit with Your Current Work:

Your domain modeling seems to have already taken a somewhat top-down approach by defining bounded contexts and domain interactions before implementation details, making this approach consistent with your work so far.

### **2.1.7. Bottom-Up Approach**

#### Description:

Bottom-up design begins with implementing concrete components and gradually combines them into higher-level abstractions.

#### Advantages for Your Project:

* **Early Implementation**: Quick feedback from working code  
* **Pragmatic**: Focuses on solving immediate problems  
* **Emergent Architecture**: Architecture evolves from practical experience  
* **Reduced Risk**: Early technical validation

#### Disadvantages for Your Project:

* **Integration Challenges**: May struggle with component integration later  
* **Potential Inconsistency**: Different components might follow different patterns  
* **Less Strategic**: May miss broader architectural concerns  
* **Refactoring Needs**: Often requires restructuring as system grows

#### Fit with Your Current Work:

Your project already has a well-defined domain model, so a pure bottom-up approach might not align with your current direction. However, elements of bottom-up thinking could help validate your domain model through implementation.

### **Recommendation: Hybrid ADD \+ DDD Approach**

Based on your current progress and the specific requirements of your editorial workflows scenario, I recommend a hybrid approach that combines:

1. **Domain-Driven Design (DDD) as the foundation**: Continue with your existing domain modeling approach, using bounded contexts, aggregates, and domain events to model the complex editorial workflows.  
2. **Attribute-Driven Design (ADD) for quality attributes**: Incorporate ADD's systematic approach to ensure your architecture meets the performance, scalability, and usability requirements that are critical for editorial teams.

### **Implementation Approach:**

1. **Define Quality Attribute Scenarios**: Document specific scenarios for performance (e.g., "System must handle 100 concurrent editors without response time exceeding 2 seconds") and other critical attributes.  
2. **Decompose Along Domain Boundaries**: Use your existing bounded contexts (Editorial Workflow, Content Management, etc.) as the primary decomposition strategy.  
3. **Apply ADD Within Each Context**: For each bounded context, use ADD to ensure the design meets quality attribute requirements.  
4. **Integrate Through Domain Events**: Use the domain events you've defined to integrate across contexts while maintaining loose coupling.  
5. **Iterative Refinement**: Adopt ACDM's iterative mindset, refining the architecture as you implement and test each feature.

This hybrid approach provides several benefits for your specific project:

* Maintains consistency with your existing domain modeling work  
* Ensures architectural decisions explicitly address performance and scalability  
* Provides systematic guidance for moving from domain model to implementation  
* Keeps focus on the business value of the editorial workflow features

## **2.2 Selected Architecture Design Methodology \- ADD ( with DDD )**

### **2.2.1. Why ‘ADD’?**

The project explicitly states that "performance and scalability have emerged as critical drivers." ADD is uniquely centered on quality attributes, making it the only methodology that places these non-functional requirements at the heart of the design process. While other methodologies might consider quality attributes, ADD makes them the primary driver of architectural decisions.

Like other methods such as DDD and ACDM, ADD is very flexible and follows an iterative process that can be shortened in favor of implementation if the need arises.

### **2.2.2. How will we implement it?**

### 

First, we need to determine the drivers of our architecture. That means deciding what primary functional requirements, quality attribute scenarios, constraints and concerns are most important while matching the overall design purpose.

Here are some examples tailored to our scenario:

#### 2.2.2.1. Design Purpose

The primary design purpose is to extend Piranha CMS with advanced editorial workflow capabilities that enable multi-stage content approvals while maintaining high performance and scalability.

**Core Design Purpose Statement:**

Create an extensible editorial workflow system for Piranha CMS that enables organizations to implement customizable approval processes, granular permissions, and comprehensive audit trails while maintaining high performance under concurrent editorial work.

#### 2.2.2.2. Primary Functional Requirements

##### Content Workflow Management

* Create configurable workflow definitions with multiple stages (e.g., Draft, Review, Legal ***(?)***, Approved, Published)  
* Support custom transitions between stages with validation rules  
* Enable assignment of content to specific users or roles at each stage ***(?) (Makes sense to have, but might be considered optional for our use case)***  
* Track approval status across workflow stages

##### Permissions and Role Management

* Implement granular permissions for workflow actions (create, review, approve, publish)  
* Support role-based access control for workflow stages ***(Is kind of already implemented)***  
* Enable delegation of approval authority ***(Also kind of implemented)***  
* Provide temporary permission escalation with audit tracking ***(Optional imo)***

##### Publishing Mechanisms

* Support scheduled publishing with specific dates and times ***(Optional imo)***  
* Enable publishing to multiple channels/endpoints simultaneously ***(Not sure if it’s relevant)***  
* Provide management of publishing queues with prioritization ***(Not sure if it’s relevant)***  
* Implement publishing rollback capabilities

##### Notification System

* Trigger notifications based on workflow state changes  
* Support multiple notification channels (in-app, email) ***(Depends on what is currently implemented)***  
* Allow configuration of notification rules by event type ***(Optional imo)***  
* Enable user subscription preferences for notifications ***(Optional imo)***

##### Audit and Versioning

* Track all content changes with user attribution  
* Maintain complete version history of content  
* Record workflow transitions with timestamps and approvers  
* Support compliance reporting on content lifecycle

#### 2.2.2.3. Quality Attribute Scenarios

##### Performance

* **Scenario P1:** When 100 editors are simultaneously working on content in the CMS, the system responds to user interactions within 2 seconds.  
* **Scenario P2:** Content state transitions complete within 1 second, even when triggering complex validations and notifications.  
* **Scenario P3:** The system can handle 1000 concurrent read operations while maintaining sub-second (\<1000ms) response times.  
* **Scenario P4:** Dashboard views displaying workflow status load within 3 seconds, even when showing 500+ content items. ***(Not sure if it’s relevant)***

##### Scalability

* **Scenario S1:** The system accommodates growth to 10,000 content items without degradation in performance. ***(Not really in the scope of our work, but could be useful/trivial to do)***  
* **Scenario S2:** Editorial teams can scale to 500 users across 50 simultaneous workflows. ***(Depends on what we consider to be a workflow, if it’s multi-branch or single branch)***  
* **Scenario S3:** The workflow engine handles 10,000 (\~2.8/s) ***(Maybe lower this value)*** state transitions per hour during peak publishing periods.  
* **Scenario S4:** The system scales horizontally to distribute load across multiple servers during high-demand periods. ***(Most likely not in the scope of our work)***

##### Modifiability

* **Scenario M1:** New workflow stages can be added to existing workflows without code changes.  
* **Scenario M2:** Custom validation rules can be implemented and integrated within 1 day of development effort. ***(Most likely not in the scope of our work, depending on the definition of ‘custom validation rule’)***  
* **Scenario M3:** New notification channels can be added with only 10% of the codebase affected. ***(Not sure of what this means)***  
* **Scenario M4:** The system can be extended to support new content types without modifying the workflow engine. ***(Not really in the scope of our work imo)***

##### Security

* **Scenario SE1:** The system prevents unauthorized access to content in restricted workflow stages. ***(Unsure of what this means, my guess is that users should not access drafts from other users, or lower privilege users should not access workflows in review)***  
* **Scenario SE2:** All approval actions require proper authentication and authorization.  
* **Scenario SE3:** Audit logs cannot be modified by any user, including administrators.  
* **Scenario SE4:** Content in sensitive workflow stages (e.g., Legal Review) is encrypted at rest.

##### Usability

***(This usability section is interesting, but most scenarios would require interaction from real users)***

* **Scenario U1:** Editorial users can determine content status and next required actions within 5 seconds of viewing a dashboard.  
* **Scenario U2:** Workflow configuration can be completed by administrators without developer intervention.  
* **Scenario U3:** First-time users can successfully route content through a workflow after 30 minutes of system introduction.  
* **Scenario U4:** Editorial users receive clear feedback on why content transitions are rejected.

##### Reliability

* **Scenario R1:** The system recovers from database connection failures without losing workflow state. ***(Save draft in local or session storage)***  
* **Scenario R2:** Failed publishing attempts are automatically retried up to 3 times before requiring manual intervention.  
* **Scenario R3:** The system maintains a consistent workflow state even during unexpected shutdowns.  
* **Scenario R4:** Content approval records are never lost, even under system stress conditions.

#### 2.2.2.4. Constraints

##### Technical Constraints

* Must integrate with existing Piranha CMS content model without breaking core functionality  
* Must operate within .NET 6 ecosystem  
* Must support both SQL Server and PostgreSQL databases  
* Must work in both integrated and headless Piranha CMS deployments  
* Must maintain backward compatibility with existing content APIs

##### Business Constraints

* Implementation must be completed within the project timeline (by June 3rd-4th deadline)  
* Solution must be maintainable by the existing development team  
* Must align with Content'r'us commercial strategy for CMS offerings  
* Should minimize licensing dependencies on third-party commercial components

##### Operational Constraints

* Must support cloud deployment on Azure or AWS ***(Not relevant)***  
* System administration must be possible through web interface without direct server access  
* Upgrades must be possible without extended service interruptions ***(Not sure of what this means)***  
* Database migration strategies must preserve existing content and workflows

#### 2.2.2.5. Architectural Concerns

##### Integration Concerns

* How to extend Piranha's content model to include workflow state without breaking core functionality  
* How to intercept content operations to enforce workflow rules  
* How to integrate with Piranha's existing authentication system  
* How to maintain a clean separation between core CMS and workflow functionality

##### Performance Concerns

* How to minimize performance impact when applying workflow validations  
* How to efficiently query content by workflow state  
* How to implement caching strategies that respect workflow state  
* How to handle notifications without blocking content operations

##### Extensibility Concerns

* How to design workflow definitions that can be customized by end users  
* How to support plug-in validation rules and custom actions  
* How to enable third-party integrations at workflow transition points  
* How to version workflow definitions as they evolve

##### Migration Concerns

* How to migrate existing content into workflow-enabled structures  
* How to handle in-flight content during system upgrades  
* How to roll out workflow features incrementally  
* How to provide backward compatibility for existing API consumers

##### Compliance Concerns

* How to ensure tamper-proof audit records  
* How to implement retention policies for historical content versions  
* How to support regulatory reporting requirements  
* How to maintain separation of duties in sensitive workflows

After consolidating the drivers, we perform successive design rounds with implementation until the main drivers are fully integrated. For this project, the main drivers can be:

- The **Strategic Goals** defined in section **1.2**  
- Quality attribute scenarios associated with the company’s overarching emphasis on **performance, scalability, and reliability** (section **2.2.2.3**)

## **2.3 Revision**

This section contains the drivers gathered from previous 2.X sections.

### **2.3.1 Terminology**

*Editorial Workflow* \- name given to the chain of operations that change requests are required to pass from the moment they are created to the moment they are published in the project.

*Manager* \- name given to users with authentication capabilities that have permissions associated with the editorial workflow (which includes the standard roles such as creator, reviewer and system administrator).

*Transition Manager* \- name given to managers whose role allows them to perform a transition of a change request from one stage to another. Reviewers, legal analysts and publishers are examples of this type of manager.

*Creator\** \- system users that can create content in a project and submit it to be approved. (Draft, Draft \-\> Review)

*Reviewer\** \- system users that can approve content batches coming from creators and move it along the editorial workflow, either to legal analysts or final approvers. (Review \-\> Legal, Review \-\> Approved)

*Legal Analyst\** \- system users that can analyze content changes for compliance to legal standards and move it along the editorial workflow towards the final approvers. (Legal \-\> Approved)

*Publisher\** \- system users that sit at the top of the editorial workflow and can publish approved content batches. (Approved \-\> Published)

*System Administrator (SysAdmin for short)* \- system users that have the highest level of privilege and can perform any role’s actions, alongside other administrator functions such as creating and assigning roles, or making alterations to the workflow.

*End User* \- user without authentication capabilities that accesses and navigates the published product.

*Change Request* \- request made by a manager to pass their draft through the editorial workflow, with the objective of publishing their changes.

*Change Request Comment* \- comment made on any change request by a manager (the creator of the request or transition managers) that can be used for discussion or indicative of stage transitions

*Content Batch* \- a set of content changes made by managers.

*Stage* \- step in the editorial workflow at which a change request can be, which, for example can be Draft, Review, Legal, Approved or Published. A change request can only be at one stage at a time.

*(Stage) Transition* \- name given to a transition of a change request from one stage to another, made by a transition manager.

*Draft* \- State in which a content request is being created and saved ‘locally’ in a creator’s workspace.

*Review\** \- State in which a content request is under review, which can be approved by a reviewer.

*Legal\** \- State in which a content request is under review by legal analysts, which check for compliance to legal standards.

*Approved\** \- State in which a content request is approved and ready to be published, only needing the approval of a publisher.

*Published\** \- State in which a content batch is published and accessible for end users.

\* *\- terms that are not generalized and are associated with the use case indicated in the scenario given by the professor*

### **2.4.2 Primary Functionalities**

#### UC-1 : Create Change Request

A manager should be able to create a change request with their drafted changes and submit it to the next stage, if it exists. An interface should be in place to facilitate the submission of new change requests.

#### UC-2 : Transition Forwards

A transition manager should be able to transition a change request to one of the next available stages. This transition should notify the involved roles/users.

#### UC-3 : Transition Backwards

A transition manager should be able to transition a change request to one of the previous available stages. This transition should notify the involved roles/users.

#### UC-4 : Workflow Creation

A system administrator should be able to create a new (blank or with standard stages) editorial workflow.

#### UC-5 : Stage Creation

A system administrator should be able to create a new stage within an editorial workflow, with rules regarding the user/roles that can perform stage transitions and other rules.

#### UC-6 : Stage Edition

A system administrator should be able to edit an existing stage within an editorial workflow, including rules regarding the user/roles that can perform stage transitions and other rules.

#### UC-7 : Stage Deletion

A system administrator should be able to delete an existing stage within an editorial workflow, raising warnings on stages that are associated with the now deleted stage.

#### UC-8 : Workflow Dashboard

A manager should be able to access a dashboard that documents the changes to content overtime.

#### UC-9 : Change Request Interface

A transition manager should be able to view information regarding a particular change request, including the changes, roles involved, creator, comments, stage and others.

#### UC-10 : Conflict Between Content Changes

A transition manager should be able to solve conflicts between new changes that were made for older (overwritten) content.

### **2.3.3 Quality Attributes**

#### QA-1 : Stage Transitions

*Performance*

Stage transitions complete within 3 seconds, even when triggering complex validations and notifications.

*Associated Use Case: UC-2 and UC-3*

#### QA-2 : Read Access

*Performance, Usability*

The system can handle 1000 concurrent read operations while maintaining sub-second (\<1000ms) response times.

*Associated Use Case: All*

#### QA-3 : Workflow Dashboard Load

*Performance*

Dashboard views displaying workflow status load within 3 seconds, even when showing 500+ content items.

*Associated Use Case: UC-10*

#### QA-4 : Number of Stages

*Extensibility, Scalability, Maintainability*

The system should be able to handle a large number of stages (30+) for a single workflow.

*Associated Use Case: UC-5*

#### QA-5 : Notification Time

*Performance, Usability*

When a stage transition is performed, a notification should appear to the appropriate managers within 5 seconds.

*Associated Use Case: UC-2 and UC-3*

#### QA-6 : Permission for Transition

*Security*

The system can handle 100 concurrent read operations while maintaining sub-second (\<1000ms) response times.

*Associated Use Case: UC-2 and UC-3*

#### QA-7 : Transition Load

*Performance, Security*

There should be 0 cases where unauthorized transitions are performed by underprivileged managers/users.

*Associated Use Case: UC-2 and UC-3*

### **2.3.4 Constraints**

#### CON-1 : Deadline

Implementation must be completed within the project timeline (by June 3rd-4th deadline).

#### CON-2 : Compatibility

Must integrate with existing Piranha CMS content model without breaking core functionality.

#### CON-3 : Stakeholder

Must align with Content'r'us commercial strategy for CMS offerings.

#### CON-4 : Large Teams

Must handle large teams with at least 100 members, all working concurrently, while maintaining high performance.

### **2.3.3 Architectural Concerns**

#### AC-1 : Study Existing Architecture

Explore the current PiranhaCMS implementation and pinpoint the systems where integration will be required.

#### AC-2 : Decide Between Fork and Module

Discuss about how we will integrate the new feature with the current framework, one option being the forking of the repository and performing alterations to the source code, and the other being the development of a separate module that can be optionally added by the users.

#### AC-3 : Allocate Work

Allocate work to the members of the development team, ensuring that each member stays informed of all components of the project.

## **2.4 Entity Structure**

*\* \- Optional, depends on implementation decisions*

**Workflow**

- **Guid** *Id*  
- **String** *Title*  
- \***Guid** *ContentTypeId* (from Piranha)  
- \***String** *ContentType* (from Piranha)

**Stage**

- **Guid** *Id*  
- **Guid** *WorkflowId*  
- **String** *Title*  
- **String** *Color*  
- **List**\<**Guid**\> *NextStageIds*  
- **List**\<**Guid**\> *PreviousStageIds*  
- **List**\<**Guid**\> *TransitionRules*

**Transition**

- **Guid** *Id*  
- **Guid** *ChangeRequestId*  
- **Guid** *FromStageId*  
- **Guid** *ToStageId*  
- **Guid** *PerformedById*  
- **DateTime** *PerformedAt*  
- **Guid** *ChangeRequestCommentId*

**Change Request**

- **Guid** *Id*  
- **Guid** *WorkflowId*  
- **Guid** *CreatedById*  
- **DateTime** *CreatedAt*  
- **Guid** *StageId*

**Change Request Comment**

- **Guid** *Id*  
- **Guid** *ChangeRequestId*  
- **Guid** *CreatedById*  
- **DateTime** *CreatedAt*  
- **String** *Comment*  
- **String** *Type (Enum: Comment, Transition)*

**Content Batch**

- **Guid** *Id*  
- **Guid** *ChangeRequestId*

**Content Change**

- **Guid** *Id*  
- **Guid** *ContentBatchId*  
- **Guid** *OldContentId*  
- **Guid** *NewContentId*  
- 

**Content Base (from Piranha)**

- **Guid** *Id*  
- **String** *ContentType*  
- **String** *Title*  
- **List**\<**String**\> *Permissions*  
- **DateTime** *Created*  
- **DateTime** *LastModified*

### 

# **3\. Identify and Model Core Domains**

## **3.1 Domain Classification:**

The domain classification helps us prioritize the development effort and identify areas of competitive advantage. For our chosen scenario, we classify the domains as follows:

- **Core Domains** (competitive advantage):  
  - **Content Management:** This domain represents our primary value proposition and directly impacts the user satisfaction. It encompasses how the content is structured, stored, and presented, differentiating our CMS through flexibility and structured content modeling.  
  - **Editorial Workflow:** This domain addresses the central strategic goals for our scenario, enabling multi-stage approval processes. It serves as a key differentiator by providing configurable workflows.  
- **Supporting Domains** (necessary but not differentiating):  
  - **User & Permission Management:** While essential for controlling the access and actions within the system, permission systems are common across platforms. This domain supports our core functionality.  
  - **Publishing:** The mechanisms for publishing approved content to various channels are critical, but largely standardized across CMS platforms. This domain enables content delivery, but does not constitute our main competitive edge.  
- **Generic Domains** (commodity functions):  
  - **Notification:** This domain handles the communication about system events to the appropriate users. While essential, the notification systems follow established patterns that can be implemented using standard approaches, or even third-party solutions.  
  - **Audit & Versioning:** These functions track changes and maintain historical records for compliance and recovery. While important for governance, they follow established patterns common across enterprise systems.

##  **3.2 Bounded Contexts (DDD):**

Bounded contexts establish clear boundaries between different parts of the system, each with its own ubiquitous language and domain model. Each context is detailed below:

### **3.2.1 Content Management Context:**

- **Domain Definition:** Responsible for defining, storing, and retrieving content structures, while maintaining the content integrity (independent of its approval state), throughout the workflow process.  
- **Aggregates, Entities & Value Objects:**  
  - **Aggregate Root: `ContentBatch`**  
    - Properties:  
      - `Id` (Guid)  
      - `ChangeRequestId` (Guid)  
    - Invariants:  
      - Cannot be published without the proper workflow approval.  
      - Must maintain the structural integrity.  
    - Responsibility: Groups related content changes for coordinated processing.  
  - **Entities:**  
    - `ContentChange`: Records specific changes to content items.  
      - Properties: `Id` (Guid), `ContentBatchId` (Guid), `OldContentId` (Guid), `NewContentId` (Guid).ms.  
      - Tracks specific modifications for versioning and audit.  
    - `ContentBase`: (from Piranha CMS)  
      - Properties: `Id` (Guid), `ContentType` (String), `Title` (String), `Permissions` (List\<String\>), `Created` (DateTime), `LastModified` (DateTime).  
      - Foundation content model extended with workflow attributes.  
  - **Value Objects:**  
    - `ContentStatus`: Current state representation for quick reference.  
      - Properties: `ContentId` (Guid), `Status` (String).  
    - `VersionReference`: Pointer to specific content version.  
      - Properties: `ContentId` (Guid), `Version` (Int).  
  - **Domain Services:**  
    - `ContentRepository`: Storage and retrieval mechanisms.  
    - `ContentVersionManager`: Handles version tracking and retrieval.  
    - `ContentValidator`: Validates the content against type-specific rules.  
- **Domain Events:**   
  - `ContentCreated`: Signals that new content exists, triggering the workflow initialization.  
  - `ContentUpdated`: Indicates content changes that might require review.  
  - `ContentArchived`: Signals content removal from the active status, without deletion.  
  - `ContentRestored`: Indicates that previously archived content returned to active status.

### **3.2.2 Editorial Workflow Context:**

- **Domain Definition:** Manages the lifecycle states of the content and the rules governing transactions between states. This context ensures that the content meets the organizational standards, through configurable review and approval processes.  
- **Aggregates, Entities & Value Objects:**  
  - **Aggregate Root: `Workflow`**  
    - Properties:  
      - `Id` (Guid)  
      - `Title` (string)  
      - `ContentTypeId` (Guid, optional)  
      - `ContentType` (String, optional)  
    - Invariants:  
      - Must have at least two stages (e.g.: Draft and Published).  
      - Must maintain consistency between stages and transitions.  
    - Responsibility: Defines the structure and rules for content progression.  
  - **Entities:**  
    - `Stage`: A discrete step in the workflow (e.g.: Draft, Review, Legal, Approved, Published).  
      - Properties: `Id` (Guid), `WorkflowId` (Guid), `Title` (String), `Color` (String).  
      - Maintains the structural position in the workflow.  
    - `ChangeRequest`: Represents a request to move the content through the workflow.  
      - Properties: `Id` (Guid), `WorkflowId` (Guid), `CreatedById` (Guid), `CreatedAt` (DateTime), `StageId` (Guid).  
      - Tracks a specific content item’s progress through its workflow lifecycle.  
    - `Transition`: Record of movement between stages.  
      - Properties: `Id` (Guid), `ChangeRequestId` (Guid), `FromStageId` (Guid), `ToStageId` (Guid), `PerformedById` (Guid), `PerformedAt` (DateTime).  
      - Tracks the accountability for workflow transitions.  
    - `ChangeRequestComment`: Contains the approval/rejection decisions, with comments.  
      - Properties: `Id` (Guid), `ChangeRequestId` (Guid), `CreatedById` (Guid), `CreatedAt` (DateTime), `Comment` (String), `Type` (Enum: Comment, Transition).  
      - Maintains the historical record of review decisions, for audit purposes.  
  - **Value Objects:**  
    - `TransitionRule`: Conditions that must be met to move between stages.  
      - Properties: `FromStageId` (Guid), `ToStageId` (Guid), `RequiredRoles` (String).  
    - `ApprovalStatus`: `Enum` (`Pending`, `Approved`, `Rejected`).  
      - Simple value object representing a decision outcome.  
  - **Domain Services:**  
    - `WorkflowEngine`: Central service evaluating and orchestrating transitions between stages.  
    - `TransitionValidator`: Ensures transitions are valid according to workflow rules and user permissions.  
    - `ReviewCoordinator`: Manages collecting required approvals and determining when the thresholds are met.  
- **Domain Events:**    
  - `StageEntered(stage="Review", contentId=123)`: Signals that the content has moved to a new stage.  
  - `StageExited(from="Draft", to="Review")`: Records the completion of one stage, and transition to another.  
  - `ContentApproved(Stage="Legal", approver="John")`: Indicates a specific approval from a required reviewer.  
  - `ContentRejected(stage="Review", reason="Needs clarification")`: Signals that the content requires revision.  
  - `WorkflowCompleted`: Indicates all required approvals received and that the content is ready for publication.

### **3.2.3 User & Permission Context:**

- **Domain Definition:** Manages the user identity, role assignments, and permission rules that control the access across the system, extending Piranha’s existing capabilities.  
- **Aggregates, Entities & Value Objects:**  
  - **Aggregate Root: `UserAccount`** (extends Piranha’s user model)  
    - Properties:  
      - `UserId` (Guid)  
      - `Email` (String)  
      - `Name` (String)  
      - `Status` (Enum)  
    - Invariants:  
      - `Email` must be unique.  
      - Must have at least one role assignment.  
    - Responsibility: Centralizes the user identity and access control.  
  - **Entities:**  
    - `Role`: Named set of permissions that can be assigned to users.  
      - Examples: `Creator`, `Reviewer`, `LegalReviewer`, `Publisher`.  
      - Aggregates permissions for a simplified administration.  
    - `Permission`: Discrete capability within the system.  
      - Format: Operation (e.g.: `Create`, `Approve`).  
      - Provides granular control over the system actions.  
  - **Value Objects:**  
    - `RoleAssignment`: Links users to roles with optional expiration.  
      - Properties: `UserId` (Guid), `RoleId` (Guid), `ExpiresAt` (DateTime).  
    - `PermissionScope`: Defines the context where a permission applies.  
      - Properties: `WorkflowId` (Guid), `StageId` (Guid).  
  - **Domain Services:**  
    - `AuthenticationService`: Validates user credentials and manages sessions.  
    - `PermissionEvaluator`: Determines if a user can perform specific actions.  
    - `RoleManager`: Handles role assignments and hierarchy management.  
- **Domain Events:**  
  - `UserRegistered`: Signals a new user creation, triggering an initial role assignment.  
  - `RoleAssigned(user=”alice”, role=”Editor”)`: Records the role changes, for audit purposes.  
  - `PermissionGranted`: Tracks specific permission assignments.

([https://piranhacms.org/docs/master/architecture/authentication](https://piranhacms.org/docs/master/architecture/authentication), [https://piranhacms.org/docs/master/extensions/authentication](https://piranhacms.org/docs/master/extensions/authentication))

### **3.2.4 Publishing Context:**

- **Domain Definition:** Handles the final stage of the editorial process, making the content visible to end users across delivery channels.  
- **Aggregates, Entities & Value Objects:**  
  - **Aggregate Root: `PublicationJob`**  
    - Properties:  
      - `JobID` (Guid)  
      - `ContentID`  (Guid)  
      - `RequestedBy`  (Guid, reference to `UserAccount`)  
      - `Status`  (Enum)  
      - `ScheduleTime`  (DateTime)  
    - Invariants:   
      - Can only publish content in “`Approved`” state.  
      - Scheduled time must be in the future for scheduled jobs.  
    - Responsibility: Manages the publication process from request to completion.  
  - **Entities:**  
    - `PublishRecord`: Historical record of a publication.  
      - Properties: `ContentId` (Guid), `Version` (Int), `PublishedDate` (DateTime), `PublishedBy` (Guid).  
      - Maintains the audit trail of publishing activities.  
  - **Value Objects:**  
    - `PublishMode`: Immediate or Scheduled with DateTime.   
      - Determines when the content should be published.  
    - `PublishResult`: Success/failure with status code and message.   
      - Records the outcome of the publish attempt.  
  - **Domain Services:**  
    - `PublicationService`: Orchestrates end-to-end publishing process.  
    - `CacheInvalidator`: Ensures that the delivery channels refresh the content after updates.  
- **Domain Events:**    
  - `PublicationRequested`: Indicates the intent to publish, triggering the validation.  
  - `ContentPublished`: Signals a successful publication, updating the status displays.  
  - `PublishFailes(reason=”Unreachable”)`: Records failed attempts for retry.

### **3.2.5 Notification Context:**

- **Domain Definition:** Handles the communication to users about system events and required actions, respecting the user preferences, while ensuring that critical information is delivered reliably.  
- **Aggregates, Entities & Value Objects:**  
  - **Aggregate Root: `NotificationRule`**  
    - Properties:  
      - `RuleID` (Guid)  
      - `EventType` (String)  
      - `RecipientType` (Enum: `User`/`Role`)  
      - `RecipientID` (Guid)  
    - Invariants: Must specify valid event types and recipients.  
    - Responsibility: Defines when and how the notifications are triggered.  
  - **Entities:**  
    - `NotificationDelivery`: The record of a sent notification  
      - Properties: `DeliveryId` (Guid), `RecipientId` (Guid), `SentDate` (DateTime), `Status` (Enum).  
      - Tracks whether the notifications were successfully delivered.  
  - **Value Objects:**  
    - `NotificationTemplate`: Content structure with variables.  
      - Properties: `Subject` (Guid), `Body` (String), `Format` (String).  
    - `DeliveryStatus`: `Enum` (Sent, Failed, Received, Read)  
      - Tracks the current state of the notification delivery.  
  - **Domain Services:**  
    - `NotificationDispatcher`: Sends notifications through the appropriate channels.  
    - `TemplateEngine`: Renders the templates with context data.  
- **Domain Events:**    
  - `NotificationRequested`: Signals the intent to notify, initiating the template rendering.  
  - `NotificationDelivered`: Confirms the successful delivery to the recipient.  
  - `NotificationFailed`: Indicates a delivery problem.

### **3.2.6 Audit & Versioning Context:**

- **Domain Definition:** Records the change history and preserves past states for compliance and recovery., providing non-repudiation and historical traceability.  
- **Aggregates, Entities & Value Objects:**  
  - **Aggregate Root: `AuditLogEntry`**  
    - Properties:  
      - `EntryId` (Guid)  
      - `Timestamp` (DateTime)  
      - `UserId` (Guid)  
      - `EntityType` (String)  
      - `EntityId` (Guid)  
      - `Action` (Enum: `Created`, `Updated`, `StateChanged`)  
    - Invariants:  
      - Entries are immutable once created.  
      - The timestamps must be in chronological order.  
    - Responsibility: Provides tamper-evident records of system changes.  
  - **Entities:**  
    - `VersionSnapshot`: Complete serialized entity state at a point in time.  
      - Properties: `EntityId` (Guid), `Version` (Int), `SerializedData` (String), `Timestamp` (DateTime).  
      - Enables reconstruction of previous states.  
    - `ChangeRecord`: Detailed record of which properties changed.  
      - Properties: `PropertyName`, `OldValue`, `NewValue`.  
      - Provides fine-grained information about specific changes.  
  - **Value Objects:**  
    - `ChangeDelta`: Structured representation of modifications.  
      - `PropertyName` (String), `OldValue` (String), `NewValue` (String).  
    - `AuditAction`: `Enum` (Created, Updated, Deleted, StateChanged).   
      - Classifies change types for filtering and reporting.  
  - **Domain Services:**  
    - `AuditTrailService`: Records meaningful changes across the system.  
    - `VersioningService`: Creates and retrieves entity snapshots.  
    - `ComplianceReporter`: Generates audit reports for specific periods or entities.  
- **Domain Events:**    
  - `EntityChanged`: Generic event capturing changes to tracked entities.  
  - `SnapshotCreated`: Indicates point-in-time version preservation.  
  - `AuditReportGenerated`: Records when and by whom the reports were created.

## **3.3 Context Mapping:**

The context mapping defines how bounded contexts interact with each other.

### **3.3.1 Integration Patterns**

([https://medium.com/n11-tech/domain-driven-design-part-i-strategic-design-and-integration-patterns-1ed169543c8a](https://medium.com/n11-tech/domain-driven-design-part-i-strategic-design-and-integration-patterns-1ed169543c8a))

1. **Partnership:**  
- Between Content Management and Editorial Workflow.  
- Close operation where both “teams” coordinate the API changes.  
2. **Customer-Supplier:**  
- The Publishing (customer) relies on the Editorial Workflow (supplier).  
- The Workflow provides a clearly defined “approved” state that the Publishing consumes.  
3. **Conformist:**  
- The Notification conforms to events from various contexts.  
- The Notification must accept event structures as defined by other contexts.  
4. **Anti-Corruption Layer:**  
- Between Piranha CMS core and new bounded contexts.  
- Translates between the old content model and a new DDD model.  
5. **Open Host Service:**  
- Audit & Versioning provides a generic service for all contexts.  
- Well-documented, stable API for recording the changes.

### **3.3.2 Context Map Diagram** 

### 

## **3.4 Implementation Strategies for Loose Coupling & High Cohesion:**

### **3.4.1 Loose Coupling Strategies:**

1. **Event-Driven Architecture:**  
- **Benefit:** Enables loose coupling through asynchronous communication.  
- **Implementation:** Define clear, immutable event contracts between contexts.  
- **Example.:** When the content moves to the “`Legal`” stage, emit the `StageEntered` event with a payload containing only the necessary data.  
2. **API Gateways for Context Boundaries:**  
- **Benefit:** Provides clear synchronous interfaces between contexts, when needed.  
- **Implementation:** Each context exposes a (e.g.: REST) API for synchronous operations.  
- **Example:** The workflow context API includes the `POST /api/workflow/transition` endpoint to request a state change.  
3. **Anti-Corruption Layers (ACL):**  
- **Benefit:** Protects the domain models from external concepts and legacy systems.  
- **Implementation:** Translation layers between contexts with different models.

### **3.4.2 High Cohesion Strategies:**

1. **Single Responsibility Principle:**  
- **Benefit:** Creates focused components that are easier to maintain and test.  
- **Implementation:** Each domain service handles one core workflow or concern.  
- **Example:** Split the review logic into specialized services: `ReviewRequestCoordinator`, `ApprovalCollector`, `FeedbackManager`  
2. **Aggregate Design Guidelines:**  
- **Benefit:** Creates natural transaction boundaries and improves performance.  
- **Implementation:** Keep aggregates small and focused on core business invariants.  
- **Example:** `WorkflowInstance` manages the state transitions, but does not contain content data, only a reference to the content ID.  
3. **Domain Services Specialization:**  
- **Benefit:** Creates a clear separation of the domain logic from the infrastructure.  
- I**mplementation:** The services focus on orchestrating complex domain operations.

**Notes:**  
DDD: [https://www.geeksforgeeks.org/domain-driven-design-ddd/](https://www.geeksforgeeks.org/domain-driven-design-ddd/)  
[https://martinfowler.com/bliki/DDD\_Aggregate.html](https://martinfowler.com/bliki/DDD_Aggregate.html)  
[https://www.semrush.com/blog/what-is-a-url-slug/](https://www.semrush.com/blog/what-is-a-url-slug/)  
[https://miromind.com/blog/what-is-metadata-and-why-is-it-important](https://miromind.com/blog/what-is-metadata-and-why-is-it-important)  
[https://searchengineland.com/meta-tags-for-seo-what-you-need-to-know-454411](https://searchengineland.com/meta-tags-for-seo-what-you-need-to-know-454411)  
[https://github.com/ddd-crew/context-mapping](https://github.com/ddd-crew/context-mapping)

# **4\. Address Cross-Cutting Concerns**

## **4.1 Security & Authentication** 

### **4.1.1 Concerns**

1. **Access Control:** With complex editorial workflows, ensuring users only perform authorized actions on content is critical.  
2. **Data Protection:** Sensitive content in various workflow stages requires consistent protection.  
3. **Role-Based Security:** Different editorial roles (authors, reviewers, legal) need precisely defined permissions.

### **4.1.2 Implementation Strategy**

1. **Integration with User & Permission Context:** Using the *UserAccount* aggregate and *Permission* entities already defined in section 3.2.3.  
2. **Security Façade:** Implement a unified security service that intercepts requests across context (e.g.: A permission evaluator to check if users can make a transition on the document’s state or to check if they can edit it).  
3. **Claims-Based Authentication:** Extend Piranha’s existing authentication system to include workflow-specific claims, such as *ContentItem.Create*, *ContentItem.Review*, *ContentItem.Approve*, and *Workflow.Configure*.

### **4.1.3 Cross-Context Security Patterns**

1. **Decorator Pattern:** Wrap domain services with security checks.  
2. **Aspect-Oriented Programming:** Integrate security checks via middleware or attributes.

## **4.2 Logging & Monitoring**

### **4.2.1 Concerns**

1. **Audit Trail:** Editorial workflows require comprehensive tracking of who did what and when.  
2. **Performance Monitoring:** Workflow processing must maintain performance under load.  
3. **Error Tracking:** Failed transitions or approvals need traceable error records.

### **4.2.2 Implementation Strategy**

1. **Integration with Audit & Versioning Context:** Connect to the *AuditLogEntry* aggregate defined in section 3.2.6.  
2. **Structured Logging:** Implement consistent logging across all contexts (e.g. “Workflow \<workflow\_id\> transitioned from \<from\_stage\> to \<to\_stage\> by \<user\_id\>”).  
3. **Correlation IDs:** Track operations across context boundaries through correlation identifiers (they allow the visualization of the whole flow with the same ID).

### **4.2.3 Cross-Context Logging Patterns**

1. **Event Sourcing:** Use domain events from section 3 (e.g. *StageEntered*, *ContentApproved*) as the basis for logging.  
2. **Log Aggregation:** Implement a central log collection service that ingests logs from all contexts to provide a unified view.

## **4.3 Caching & Performance**

### **4.3.1 Concerns**

1. **Read Performance:** Editorial teams need fast access to content in all workflow stages.  
2. **Cache Invalidation:** When content transitions between workflow stages, caches must be updated.  
3. **Scalability:** System must handle large volumes of content and concurrent users.

### **4.3.2 Implementation Strategy**

1. **Distributed Caching:** Implement a caching layer accessible to all contexts (try to get items from cache, repository as fallback, and remove from cache when content changes state).  
2. **Read/Write Segregation:** Implement CQRS pattern for high-traffic domains (read model optimized for queries, distinct from the write model).

### **4.3.3 Cross-Context Performance Patterns**

1. **Event-Based Cache Invalidation:** Subscribe to domain events to invalidate caches (await a stage change notification to invalidate the cache).  
2. **Eager Loading Strategies:** For editorial dashboards, preload related content and workflow data in a single query.

## **4.4 Error Handling & Resilience**

### **4.4.1 Concerns**

1. **Workflow Consistency:** Editorial workflows must maintain consistency even when errors occur.  
2. **Transactional Boundaries:** Operations spanning multiple contexts require coordinated error handling.  
3. **User Feedback:** Editorial teams need clear error messages relevant to their domain context.

### **4.4.2 Implementation Strategy**

1. **Domain Exceptions:** Define specialized exceptions for each bounded context (using contentID, attempted transition, and the message itself).  
2. **Circuit Breaker Pattern:** Protect external systems and resources with circuit breakers.

### **4.4.3 Cross-Context Error Handling Patterns**

1. **Compensating Transactions:** When operations span contexts, implement compensating actions (e.g.: try to approve in workflow context, then try to publish in publishing context, if the last fails revert to previous state).  
2. **Consistent Error Responses:** Standardize error responses across context (containing error code, user friendly message, technical details and context name that triggered the error).

References (from section 4):  
[https://piranhacms.org/docs/master/architecture/introduction](https://piranhacms.org/docs/master/architecture/introduction)  
[https://www.milanjovanovic.tech/blog/balancing-cross-cutting-concerns-in-clean-architecture](https://www.milanjovanovic.tech/blog/balancing-cross-cutting-concerns-in-clean-architecture)  
[https://medium.com/%40avdunusinghe/mastering-cross-cutting-concerns-with-the-mediator-r-in-net-43c33ceba582](https://medium.com/%40avdunusinghe/mastering-cross-cutting-concerns-with-the-mediator-r-in-net-43c33ceba582)  
[https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)  
[https://jessemcdowell.ca/2024/05/Cross-Cutting-Concerns/](https://jessemcdowell.ca/2024/05/Cross-Cutting-Concerns/)

# 

# **5\. Proposed Architecture & Roadmap**

## **5.1 Overview & Diagrams**

### **5.1.1 System Architecture**

Below is an overview of our envisioned system architectural structure of the PiranhaCMS application extension, illustrating some user role interactions, interface components, and functional modules:

![][image1]

At the top level, five distinct user roles (Admin, Creator, Reviewer, LegalAnalyst, and Publisher) interact with the system. While Admin maintains system-wide configuration access and is mandatory, the content-focused roles (Creator, Reviewer, LegalAnalyst, and Publisher) primarily engage with content management functionalities, and are organization-dependent (we chose them to coincide with the remainder of our report).

The Manager UI serves as the central interface component to facilitate three primary interaction paths:

1. Administrative access for system maintenance  
2. Content management operations for editorial staff  
3. User notification delivery

Content operations flow through the Content API, which interfaces with the Workflow Module (implemented as a State Machine) to enforce the defined editorial process. The Workflow Module performs three critical functions:

1. Permission validation through the Authentication Module  
2. State transition recording via the Versioning & Audit Module  
3. Status change notification through the Notification Module

The persistence layer is constituted by the Content Database, which stores both current content and historical version records to ensure comprehensive tracking of all content modifications.

### **5.1.2 Content Workflow State Machine**

Organization administrators should be able to customize their workflows and interactions, and below there is a state-machine diagram to demonstrate an idealized set of transitions:

![][image2]

The state-machine diagram illustrates a structured content approval workflow with five states. Content originates in the Draft state and progresses through subsequent states based on specific transition criteria (organization dependent).

Upon achieving sufficient quality (as per organization standards), content transitions from Draft to Review, represented by the *submitForReview()* function. In the Review state, three possible transitions exist: 

- *requestChanges()* returns content to Draft for further modification;  
- *requiresLegal()* forwards content for juridical assessment;  
- *approve()* advances content directly to Approved (skipping juridical assessment, this is merely illustrative and may vary across workflow definitions).

When legal review is required, the content enters the Legal state where it can be rejected, symbolized by *legalReject()*, going back to the Review state when compliance issues are identified. It can also be approved, shown by *legalApprove()* to advance to the Approved state.

From the Approved state, the final transition into the Published state is seen with *publish()*, representing a successful workflow completion.

**Note:** This representation is simplified and omits potential version control mechanisms that would enable reversion from the Published state to previous ones (more described below).

## **5.2. Proposed Features**

Since, we have a constrained 1 month period for the development of our PiranhaCMS functionality extension, we have decided on some core features to implement:

### **Feature 1: Editorial Workflow Engine**

* **Description**  
  A fully configurable workflow engine that orchestrates multi-stage content approval processes (e.g. Draft \-\> Review \-\> Legal \-\> Approved \-\> Published). Organization admins can define custom stages and assign roles or groups to each.  
    
* **Benefit/Alignment with Goals**  
- Quality & Compliance: Enforces consistent review cycles and audit trails.  
- Governance: Provides clear accountability by recording who moved content between stages and when.

* **Success Criteria**  
- Support creation of at least 3 custom stages.  
- Demonstrate a full cycle: Draft \-\> Review \-\> Publish in an end-to-end test.

* **Priority & Timeline**  
- Phase 1 (Weeks 1-2): Core state-machine \+ UI for stage configuration.  
- Phase 2 (Weeks 2-3): Integration with notification module for stage-entry alerts.

### **Feature 2: Workflow Role-Based Access Control (RBAC)** 

* **Description**  
  Granular, stage-aware permissions layered on top of Piranha’s existing user/role system. Defines which roles (e.g. Creator, Reviewer, LegalAnalyst, Publisher) may perform actions or advance content to the next state.  
    
* **Benefit/Alignment with Goals**  
- Security & Governance: Ensures only authorized users can perform sensitive transitions.  
- Scalability for Teams: Easily onboard large editorial teams with clear separation of duties.

* **Success Criteria**  
- Enforce zero unauthorized transitions in a scripted test harness.  
- Role assignment UI lets admins bind users to workflow-stage permissions.

* **Priority & Timeline**  
- Phase 1 (Weeks 1-2): Define roles and wire up policy checks in the workflow engine.  
- Phase 2 (Weeks 2-3): Admin UI and self-service role assignment

### **Feature 3: Versioning & Audit Trail**

* **Description**  
  Automatic snapshotting of every content save and state transition. Maintains a complete version history with metadata (who, when, from-state, to-state). Provides UI to compare versions and roll back to a previous one.  
    
* **Benefit/Alignment with Goals**  
- Compliance & Accountability: Full traceability of content evolution and approvals.  
- Resilience: Ability to revert to any prior version in case of error or dispute.

* **Success Criteria**  
- Persist at least 3 historical versions per set of published changes.  
- Execute and verify rollback under simulated concurrent load.

* **Priority & Timeline**  
- Phase 1 (Weeks 2-3): Hook into Piranha’s save pipeline and store versions.  
- Phase 2 (Weeks 3-4): Version comparison and rollback UI.

# 

## **5.3. Implementation Strategy**

# Our implementation strategy will focus on minimal changes by extending current Piranha CMS components (outlined in orange in the architecture diagram) and adding new modules only where necessary (blue fill in the architecture diagram). Below we will list our intended approach for each component.

| Component | Exists in Core? | Implementation Approach |
| :---- | :---: | :---- |
| Manager UI | ✓ Yes | **Extend** existing UI with workflow stage dashboards, version comparison screens, and role assignment panels. |
| Content API (Piranha.WebApi) | ✓ Yes | **Hook into existing API** via middleware to inject audit/version logging, emit state transition events, and validate user permissions. |
| Authentication/RBAC Module | ✓ Yes | **Enhance** existing authentication (with ASP.NET Identity) with specific workflow “stage-action” claims to enforce permissions. |
| Content Database | ✓ Yes | **Migrate with minimal schema changes** (add WorkflowStates and ContentVersions tables, indexes and cache). |
| Notification Module | ✗ No (Minimal) | **Build** a new component to improve the UI alerts to a workflow event subscriber service. |
| Workflow Engine/State Machine | ✗ No | **Implement** a new core module to manage workflow states, drive UI changes, emit transition events and enforce permission checks. |
| Versioning & Audit Module | ✗ No | **Create** a new module to snapshot content changes, provide rollback/comparison, maintain audit trails and track metadata for state transitions. |

# 

# 

# **6\. Presentation \- Notes**

Presentation: [https://www.canva.com/design/DAGmHu0k9sI/fMUqSD\_LbAoke\_sqd-zVaw/edit?utm\_content=DAGmHu0k9sI\&utm\_campaign=designshare\&utm\_medium=link2\&utm\_source=sharebutton](https://www.canva.com/design/DAGmHu0k9sI/fMUqSD_LbAoke_sqd-zVaw/edit?utm_content=DAGmHu0k9sI&utm_campaign=designshare&utm_medium=link2&utm_source=sharebutton)  


[image1]: <data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAloAAAIoCAYAAACxsUUgAACAAElEQVR4XuydCdxM1f/Hp4VC+VF20oIskSVEZI1EQkkSFVmjkiwle9bsu4SQJeT/U+mXFlIpkixFVJRs2Xchy/nfz7lzxp0zyzPzzJ2ZO8983q/X9zV3zj333HvPOXO/nznn3HNcLkIIIYQQQgghhBBCCCGEEEJ0rqKlWSOEEEJIjIEDvtpi19DSrFnLOWkpnDlz5lX333//4cmTJ4sffvhBAHxWr179xE033bQacfSDSMTIfK9SpcrhKVOmMN9Th1ceqvzDNvMwNALVQTMPq6o8JIHx+ztmHfTLVZfOftBPnP9Q0JLLzhx/f6BR/tcbdq3LFFxJ1dJV17DLo0aNOiefDn4YM2bMJSOOcMcl9oC8FMz3iGAeRk7dYPkH3HnI/PMP62B4XH3hzJL+uhOmpX07fvC9IUb5ZzbsOtcVsZUUXJslS5aDS5cu1Z8NPiAO4uIYPRESPsz3iGHdjRyZh3p++YP55xfWwfC5hkIrOe3Q3jlvGuWfzbCMhqVHXXAlQ6tWoUKF/q9Vq1aH9YdCIBAXx+jpkPBhvkcG627khJOHzD9fwsk/wDyUXHv+1OIB4vwHgpZctufPGaOM8s/jMlu1rF2IaZurrrrqkoH+PAgI4l599dWX9XRI+DDfI4N1N3LCyUPmny/h5B9gHkrSnT2x6A3dCdPSvv3529tjjfK/1bCsLrNVK50rGYRWgQIFjukPg5QoWLDgMePQMnpaJCzK6PmaEsx3L8pEUHeJSdh5yDroRdj5B5iHrvQUWslp27e+Nd4o/wKG3WzYDagLrmQQWk899VTYD4rmzZvjQdFGT4uERVs9X1OC+e5F2wjqLjEJOw9ZB70IO/9AkuchxuNcd+b4woG6E6YZdsFPWBqyXzdPnmiU/52GZTfsRtQFlzlOK22DH73+IEgJPFyMQ9vqaZGwaKPna0ow371oE0HdJSZh5yHroBdh5x9I8jyk0EpicwutIq5kE1poxtYfBCmB5nJXcjd920HYXYfMdy/KRFB3iUnYecg66EXY+QeSPA+vCK1zhvOlJZVt+3nSJFcyCi2+NRM/mO+RwbobOeHkIfPPl3DyDzAPKbSS2dxCq6hhOVzJJLRcnAcmbjDfI4Z1N3I4j1ZksA6Gh0VoLRG05LJkFlqg7lVXXSVnL9YfDoqxY8cKxMmYMWN9/WCSapjvkcM8jJy6wfIPIA+ZfwFhHQwdCq0ktmQXWqBwvnz5NlSuXPmQvtYhwox9GxFHP4hEDPJ9I/M9IrzyUOUftpmHoRGoDmp5SALj93fMOuhDGhRaH/oJo/kzCq0rPJc3b94FOXLk+MPYFjlz5vwzXbp0yfoqcixhvkeOzEOXO//y58+/iHkYFj51kHkYNl55yPzzIQ0KLVqoRqHlHyyCSmIP8z0ymH+RwzyMHOahLxRaSWwUWv7hgyI+MN8jg/kXOczDyGEe+kKhlcRGoeUfPijiA/M9Mph/kcM8jBzmoS8UWklsaV1o9XOZP/pYWD8XUfRz+eZPtKyfK23Sz+V7r9Gyfq60iX6f0bJ+rrRJP5fvvUbL+rnSNm6h9Z4htP4raMll236ekKaFVmrBD5/EHuZ7ZDD/Iod5GDnMQ18otJLYKLT8wwdFfGC+RwbzL3KYh5HDPPSFQiuJjULLP3xQxAfme2Qw/yKHeRg5zENfKLSS2Ci0/MMHRXxgvkcG8y9ymIeRwzz0hUIriY1Cyz98UMQH5ntkMP8ih3kYOcxDXyi0HG//5yfMHqPQ8g8fFPGB+R4ZzL/IYR5GDvPQFwqtJDYKLf/wQREfmO+RwfyLHOZh5DAPfbEILbSc0JLJKLT8wwdFfGC+RwbzL3KYh5HDPPTFFFpHDaF11nC+tKSybZsotPyBB0WiWVpAv6dYW6Kj3w8tdZbo6PcTDyPeUGglsVFo+YcPivgQz3yP57ntIhHu4SvDVuiBDiIR8jAl4n0P8T6/E6HQSmKj0PIPHxTxIZ75Hs9z20Ui3MPXhi3XAx1EIuRhSsT7HuJ9ficSktC67CeMlvhGoeUfPijiQzzzPZ7ntotEuIdvDPtCD3QQiZCHKRHve4j3+Z1ISELLajWr3y2uvfYaacax4pprrpbbe7a/LbJlyywO75nlc0w0bfOPY8ToN1v6hIdrr3Z9VPz0w2if8HgZrgefE0a3EZ061PXZb4dRaPmHD4r4EM98j+e57SIR7mGVYZ/rgQ4iEfIwJeJ9D/E+vxMJW2hZ7bZbc4hVKwZ5vsdaaJ07vkDcXeJWUad2aZ994doDNe4WKz97wyfcn106875PmN2G68EnhVbs4YMiPsQz3+N5brtIhHv41rDP9EAHkQh5mBLxvod4n9+JuIXWPENoLRbh2hWhZX6H0Jo4po24q9gtosAducT4Ua09+0refZvIlSuLaPLYfeLI3pkybN8f00StmiVF5swZxeL53Txxx418Ttx+Ww5xx+05RfcuDcXlfyBsfM//8gv1xX0VCruFlve+s8fmizatHhA5c2YR//lPRvFYowrin6PzxMbvR4p7yxcSWbPeIB6sVUrs+v0tMaBPU5Ep0/XynLifXq82Fnnz3iStxyuNxMXTC2WaWbJkkte5f+d0MXlcW5Ev780y7Qb1y4szR+Z5nf/SmUUB00E+4Vjk0YTRV/JImboeXAv2t2hWVTR8pLzIk+cmUb9eWXnNiIdPpFWzegmx9pthPumkZNs2jaPQ8gMfFPEhnvkez3PbRSLcw3eGfaoHOohEyMOUiPc9xPv8TsR2ofVE40pSUKxY1l92K547/p44vGem+Gb5QBnetXMDKRYQv2P7OqJd69oyDoQHwj79qLcUWLt/nypOHpwjqlQuJkYOe8bn3J9/3FeKtHmzOvsVWjPf7iRKlbxdHNr9jji6b5YoU/oOMX/2y+LW/NmlWILowvkfrmtei9miNUBuQxRCTOHYsvcUEFMntpfhEFpPP1VNHPhrhsiY8TqxdeNYeY0QeyqO9fyB0kE+XTi1UHz1+Ruy2xX3r1+/2aK1WAotxNmwZoQ4f+I9ec+v93hM7qt4b2Ep6JYs7CHz7/j+2T7pBDMKLf/wQREf4pnv8Ty3XSTCPaw2bJke6CASIQ9TIt73EO/zOxHbhdZnS/t4vqPVCCJr+pTnPWF7tk8VV199tRQofV9vIluVfv1pvGc/Wm8G9W/m+Q4hdU+ZAl7nRZpoNYMYe+/dLn6FFtJXwmPnr1NEpYpFxJTx7aTgUnHQsqZa0qxC690ZL3riLJjTRdR+oKTchtCC4Dl9eK5syZo7s7NsOdPPDYOAC5QO8kmFZ8+O7lazhc9qVqEFIafCx45oJTp1eEjs+GWSbAlU4XcWyiM+WvyaTzrBjELLP3xQxId45ns8z20XiXAPawz7WA90EImQhykR73uI9/mdiO1Cyyoa1Hd0s6kwtOR8+6V5zJb1Y6RoQasMRAO6EtEiBgGj4uP4zT9ikPqV86JV6oYbrpd23XXpZMsZhI81TtvnaklTrUWtnqkhhRa6Da1pDx/ytNy2Cq0577zkifP+vK6yFQnbEFo45tjfs8VvP5viEK1VzZ+sIp5vV8fr/BBagdKxCi10p6YktCCsVLj6jta0QgVze8LXrx4uxaWeTjCj0PLlHpf5oMAniR3xzve04BzimX+h8r1hS/VAhxDvOmgX8azLKg+JNzERWhhLtHrlENn1hXFL1asWl/vrPXSPHLsEsQXR9Oe2ybLlB11uEF3o8kMrUP/eT/icW1mgFi2MyerT83G5bQgKcfPNN8qxX7lzZxUf/7enFGCdX3jY040JYbPsw95yu3Sp2+V14/zlyxUSk8aaQlEJrR9WDZPpbFo7UqYDkfVsi+pe55/xVseA6dghtCBOi9+VX3bHLv2/nvLaTh2i0IqUdYa96/4ksSPe+Z4WnEM88y9U1hr2kR7oEOJdB+0innVZ5SHxJiZCC9tFi+STYgACAl15CPt53Wg5dunGGzN4Wr0w8H1gvyfFLfmyyeOfe7amFGj6uZUFElpo4cE589+STYquUcOelYPJ13w1RHYf4log+NS1vDm4hRQ9GEvWs/tjnkHs3V5uIFvhEEcJLWy/1u1ReX0IQ/fn3h1ve50fQihQOqEILVwPriWQ0MI2BCS6ZyG4ln/SzyeNlIxCy5u33Ka2SWxwQr7H0znZgTX/4pWHofCDYR/qgQ5Ar4NOzsOUiFdd1vOQXCEioUVLbKPQukJbl+8/WYSR6OKUfI+Xc7IDPQ+xHY88DAVc2wd6YJzR8w84OQ9TIh51Wc/DRM6/aGARWphCIRUm55SiJaJRaJkEGpvhL4zYh5PyPR7OyQ785aG/MKfwo2FL9MA4EiivAoUnArGuy/7yyl9YMhO50KIlrFFomQT696X/SyP24qR8j7Vzsgsn5WEorDfsv3pgHAmUf8CpeZgSsa7LgfIwUfMvGlBoJbFRaKU8HiOl/SR1pJSvKe23m1g7JztIKY9S2h8PNhj2f3pgHEkpf1La70RiWZdTqmMp7U8WKLSS2JJdaIX6jyvQPzaSOsLI93SxyvdYOic7CCMPHVV3Nxq2WA+ME6HmS6jxnEKs6nKi1sF4cEVoYZkbWlJZMgutcH/8oT5USMo4Md9j5ZzsIJHr7ibDFuqBcSCcPHRS/oVCLOpyOPkHEi0P7SbVQuvcsfnixP7Z4vuvh/jss5qc4mH3Oz7hWFZGD0vJLpxcIC5hvUA/+5xmmPbh+N+zfMLtMjvST2ahlZrmbDaDR05q8i8W+R4L52QHqc2L1BwTDX4ybIEeGGNSk4epOSZeRLsupzYvUntcWsAttOYYQmuRSMk2rhkuqlW5S7ze41FxT+k7xLpVQ0XZMgUM8bNA9HiloVzAGeHtnqsler36mDwGQmvimNbuRaZbybD6dcuiLsj5n84fn+dJ/+zRud4LQTesIP45Mkfug7BA+P4/3xa7fpss565C2uaCyhB73tcaKA6uH5+4ZsyntXTxq/L72BEt5bxgmNNr28YxMmzC6OdE40YVRJHCeeV8Vc+2qCZmT+skCt+Zx3Nc65Y1xYvP15VrDd500w3iqab3i1MHZ7uF0EwZZ+Wn/bwX1d4zw+d6F83tIpfRwfU+2uBecWjXdHn+tkZ+lLunoMiR4z/ikXplxe7fp3jyA+mPfvNZTxooly4vPuyTdiDbtmlsUgqtcP6J6YT7T45cIZJ/tdHO92g7JzuIJP9ANPMvVH427D09MIZEkofRroN2Ee26nNr8A4mSh3YTttDKlOl68eag5uLfE/M9QgviA2sJnjwwW/y0doQUK1ah9UTj+8SKT/qai0wfM4WV2aLlnf7MqR1FqZK3SZFxdO875kLQszrLfRAWSB/bFe+9Uwx94ykplpYs7G4uqOwWNcoCxal8XxExb+ZLUvxByCHuzm2TxPXXpxc//zBSXh+EIsIhdDBjPUTbvh1T5ULSEIkQjFXvLybjQGhhvcJf1o+WAqtunTLilZfqe4TQ4d0z5PY3XwwQF0+9J7p2fkSmYb3W7ZvHSyEHMQjhCXHV7InK8vzIsx+/HSZF6EMPlvbkq0q/Vs27PelAuK5a/oZX2sEsGYWWeu04tfC15dQRab5FenxKRFInYoEd9x/p8Xaw2bD5emCMiDQPIz0+VkSzLuPeI7n/RMlDuwlbaEEQQLzguxJa9R4qI1tkVDyIBKvQ+uyjXnJbLjK922zN8Se0IKSUYIL4MReCbiu/Q1jgc8eWCXJdRNXSBZMLKr9vtjClFAdiCrPFY73EPdvN1iGImwM7p8lttDY1fbyS3IbQgUhUaUC8/XdBN5k+Wr8QBqHVvUsDTxyIIqSthND0yR28xBDOiUW11XfYiCFPi+eereH5jmvBGo44P86pwiEOO7WvI7dV+hC1pw+9K8UaWsxU2YRiySi07PhHFcm/4mTF6fkeTedkB07Pv1DZYtg8PTBGpJU8TIlo1mU77j0R8tBuwhZaxYrm83xXQgutUN99OdAT3u3lR7yE1sG/TBFjjtcKLLQgMBo8XE62PqGLDy00utBCN1yGDOnlPqt9sKi7J52U4qAr0SqgLpx8T7ZCYdkfdNNZhVb71rU98dAahhYjiECr0IIAUnGO7Zsp0qW71iOE+r3eRN63fi3W+0YrV/9eTbzC0MKG86vWNRjyQhdaEHG4L4g167WGYskmtOwcI2BXOsmA3fluV1pWoumc7MCue45W/oUKhNYcPTAG2HnfdqYVDaJVl+28b7vSSRTCFlpWkaCEFhZ+fn/eK57w5k9W8RJaSlylJLTQZQZT3YutnqnhI7S2bhgjChXM7XXc+u/e9HQrphTnq8/6ixLF84u8eW4Sm74fIfehKxHdlOrawhVaGJ+m4iCPIBSVEJo8ro0cd6b2Q9R9u+KKKIVBJCEd9R3XkT79tT7n9ye0MLYMYgzX9vnS3l7ppmTJJLSi8S8q0n/HyUA08t3u9EC0nJMd2F3P7GjZSS2/uGK/6HC06mC88jAlolGXo5GHTs2/aGCL0HrLEABVKhcTZw7PkWOVIAJCEVpKUCnDmKk+PRvLbQxIv/nmG8W4keYAeiW00DWmWrow5gmD0rEP46NUOsHioEUO46Xeeet5KU4QH2OualQrLsedQbxgMDrCdaETSGhhkP5vP42T94/xVy90eMgjhDC+CwPZV68cJLsokS9YzNp63zgWA+k3rH5TCrEObWrLQfP6+f0JLXQZYowY8hbHWtNNyZJFaEVrXEA00kxLRCvfIx0n4o9oOCc7UHloJ9Eql1DYathsPTCKROteo5WuHSRKfYlGmk7FFqEFMQMBAEHxQI27Rcunq4shA5rJOIGEVsP65WXLj/WtQ7Q6FS2ST46hgugaNewZkSf3TeKvXyd5hBYMIgyD0THmC9ez/H99fK41UJxnmleTn5fPLBTlyxYUs97uJAULrhuCCWIKY53+772uPkInkNBCnNKlbvcM/D+xf5ZHCCEOxobhvhCG8+B4/XoXvPuyHEcG0dTokfKyu1U/vz+hhW28FanuKxxLBqFlZ3O3P6KdfqIS7XyxO327nZMd2H2POtFO3x+/GjZTD4wSsbi/aKefGuysy9HOw2in7xQsQgvzU6XO9v85VU5rALEBGzeypU+ctGoQWtMmtfcJj5Wt/26YFF1nj2Lgv+/+YJbWhVY0mrv94eRuhHiQiPlup3Oyg0TMw1CA0JqhB0aJWOQfiGX+hYJddTmt1sF4YIvQSmZb+81gsWPLeJ/wWFnuXFnF2xPb+YSHYmldaPVzxaZpGufopwcmMf1ciZfvdjknu+jnSrw8DIXfDJuuB0aJWOQf6KcHxBm76nI/V2zyMNZ1MB5QaCWxpXWhRUio2OWcSHB+N2yaHkhshXXZeVBoJbFRaBFiQucUG7Yb9rYeSGyFddl5BBVazZ+8X2xeN9I9IedCuV38rlt84sXT+r7+uLgl383i8O7pPvvssFe7NpSz3WN7wuhW7sHovvHstrp1SsvP669PJ04fwhuVvnEiNQotQkzonGIDhFYyDH6OJ6zLziOg0Frz1SDxUse6YtXyAe71AZ0ltM4dmys/sSaiuXagbxzd1DHh2AM1SsgJULEdS6FlzjO2UKz4pI98q1Pfb4dRaBFiQucUG3YYNkUPJLbCuuw8AgotzDiOQd6335ZDZMp0nVxWB0IL81ChlQfLzFQoX8g9bcJC+aYh4t5xe07ZAnb5zAIx6+2OcqZ37P98aS9RpvTtPs7eXJT5Ia8FmRG+67dJ4sFaJUW2bDe6F4UeLDauedNrUWtMpXD11VeJokXyyhat+yoUlm/g3VuukHwbD+nox+CtSKSH8yE+lgfCfeTJnVUM7v+k17UN6N1E3jvuC4ITQqtFsyoybv2698hrDHSt+n0in0refatcO7HAHTk94fNnvSQKFsglcub8j5x4FG8PIm2jXOTUF4ivWrTMhadzy/OYC09jCohW7oWnC1gWnp4s408e11oKUZTBmcPv+lwThRYhJnROseEPwybrgcRWWJedh1+hhRYiiAls6y1aRnw5wSeEFLrtKlUsLD798HUpsODgTx6YJapULipGDn1azgWVOXMGKRjQEjSwb1MfZ68WZTYXZC4tl8JBuLkodDNx6fR7YsnCbnLOLYgmCB9zUWvMv7XQI0QunJwvpk4wJyiFwIMQRBz9GFz/fxd0lesg4hxIF/eL7kEsGq0LEr1FCy1NuB8IUYi3QNeq3+dtt2YXM6Z0kNf57vROMuyX9aOkwNq6YbQ4sPNtKfhGDGkh96kWLXV/2zePE1mzZpIiznvh6VbuhaeHSpGmFp5GergflAcEJfJGvyYKLUJM6Jxiw5+G4aFDogfrsvMwhdbh2QOFIZyUff5RLymWsL3qCwitYnJ78w8jRLp014hThvPG902GiEFXYosnq4hB/Zp6jp/3zouy9QjbECFID+OofvlxpCeOstbP1hDdX35Ebv/47RApkHYYogIC7R+IHnc8tORsXDNMio1Lp+Z7wqUQOThLrFs1xCtdtHJ9t+INn2Ny5cwiLhuCCNuvdW0oXur4kOcYXOPOrRO80pFCa1lfuT3BEJj3VbhTbo8d/qwhHh8MeK3WNGB7DBGK8yLeu9M6ybB+hlDt8mI9T5wthpD91hC22JZC68yV+xsxuIV47pkanrgH/pxqLjxtXJNceNodPnH0c/K6cAxas84euXJdum3bMJpCixAXnVOs2GnYeD2Q2ArrsvPwK7Q+er+7qFOrlNzWhZYco+WOp76jC2/ujBc84Yd3TZP7sA0xVvjOPOL17o18HD0MQqsHBtsb2xBFaA3aun6UuVahJd7674bK/dbzwwIJLcSDaNGPwSzqartXj0fFwL5PeL6j1SkloQURY90OdK3W7/t2TJGLXCuhuevXifITeaLuHYZ7WDTnZbntT2ghr1Rc5LFcD9G4jvata3nCp4xrI6/r2N4Z4rdNY8Shv96WLzU837a21zXBKLQIMaFzig1/GTZODyS2wrrsPPwKre0/jzXHEZ0xhRa6tLAdSGgtmN1ZlCxxqxQUR/dMF7Vr3i3693rcEw8tRWit0h09DOIhZ47/iDOHZsuxSS90qCNbn5AuRMPFk/PF0sU95NgrXTTBlBC5cGKemDGlvWw1Qosaznnu6ByfY1IjtJZ90FNu+xNaga7VmsbPa4fLsCO7p8s4qhUNeYIuWuQ3hBOW+BmJrsMzvkILoslcD3GYvNcObWqJJo9VDCi0fvhmsJzMFHkAkfUsluixXBMsGYRWc5f54Hle32FhtGEd9cAgnDUskx5IXPkM22fYMcP+5zIrlh08adhTeqDNxNo5PWfYZcMuuA1dam28YoROD8OG64EOBUJrjB7ohx8Ma6AH2shLLvN3rwjlOZESepqhgvptJ6HWZTzzrHXwpGEfe8XwJa8e4MZ6759Yd/jhkGE364FpHL9CC4aut5P7Z0rhhMHajzYoH1BoQdxAsEDcYKA2urjO4w0/dzyrENANQgv7cZxcJ/Dvd5QIcK9VmEmeY/nHvX1EE0wJEWyjCw1iBGsYqhYu/Zhwhdabg56S3Y3ffN7fr9AKdK3WNGAY8I9FsiFIp09uL1556WEZjm0Mtsf9t2xRzZNvDeuXk9vW+4OgRbekZz3EnW8HFFrYRtcoBN6DD5QUe7dP9rmmZBBaSw37xrCv9R0WwhVa1V1pLJNsAJVnl2HZDctqWG+X2U2UwRIHXKt9D4XXDeurBwYhvR4QAqE6J7uA0FIO6WrDmhh2ybDbVIQwuNWwQnqgzaQmT/2BOhKKGIm10ArlOZESepqhgvptJ6HWZTzzPrJ8L2DYCst3f4QitEpZd/ghtULLrjoYDwIKrZlTnxdj3nzGxzmnxiBo9DBlEFrTJrXzCadF39K60LrJZf5LgyNCKxRaXBRfGHbEsE8Nm++6IrTwEMA/bnw+bNgSl9lKs8BlOkRwymW2aGE+oLWGHTDsA5d3+snGQMPmaGHIk6ru7WOGPWrYfsNuMWyZy8xjlEM5d5zrDZtq2HHD3neZIq2Oy8xfWGeXWQZvGLbHsKGuK5X1vy6zRegXlylawiVU52QXVqEF8CBGXa3k/q7nEfIG9e52936A/Vgj7hWX2aKFOHij72/DNhjWwh0PDhB5VdZl3qc6R3/XlTmtXnCZbwQOc5nXAiLNU3/sNmyUHuiHYELL37VmcZm/1cPuz8/d4TUM2+gy8w5zeKk0rcIg2HNikcs8F54BeFiq+mZNV08zv8t8tqi4N7jMc1Q27CeXeQzuDw9eVb9Rt+0i1LqsCy3wtPsT+VfTEo66iu8QWgNcZn5glv/H3Put+anSbO8yyxu/Z5RJRnc46jRaDlEendxhQJUrpgBB2YJo1MF4EFBooUvs4YfK+DjncAwtXeguQ0uRvk8ZhVb8LK0LLfxA33Nv4wf7smXfCJf5DwkPOrQkWIUWHhj4YeCBVd6w/7jMH381dxwltC4aVsZlOjh0lUEAJCurDGukB1o4Ztgsl1nJvnOZ3V0QTXBSeBgjj59xmQIBLWI/GtZUHundooU4cHDZXKazUt1tKF8cYxUi4RCqc7ILq9DCjw4ODuIRdQnoeYT8gdNXjgn5eNpltgwooTXYZTo0CNSCLtMZ3usy/zCgfHDsCcO6uUwg4OC8arvM+g2B8ZVhXdz7I81Tf6Cs8dtLiUBCK9C1Yv1ECDi0mKKOqPL807CW7nB0D25zh1uFQbDnBPIcdQ0PSPxJUOLDmq6/NCGo7nNvKzGy2WWWM547iKf+mDilRQtCUwnUYEJrpGHpXGZZnHGZZWG9d6SJ/MK+Ii6zrn7ruvJbxTMW+V3dZT5DUf+t5Yr4KFsQjToYDwIKLdivG0f7hIVj+/94S3aJBes6XPv1YPnmnh5Oi76ldaGFZvCH3NtPGPa9exvOCc5IgYeKVWjh4QDg0BWzXeYDHCihBWeowD+08ZbvycZewyrogRaOua50KcDZW/P/V5cpBlABISjwwIcwaOfebxVaeIjDYQKIBLRIAjyQI2kVCNU52QWEFgQ+WlDgbDBWBi0e4A6Xbx4hf5q5zFYsgHtX42mU0EK3nHLuAK1VEDVKlMGxIx7yCj9ynAMiAnW7p/sYjBda596ONE/9ATGJa0iJQEIr0LXiN4l7UeA8AMIATg55iXqD1j5gFQaBnhMAQkux0HVFNFnT9ZcmRC9aecG77s+Vhr3p8r5OEE+hpeog7LzrStdhMKGFFjrFh4a1dvkKLTwf0ZKFOqv+PCjwjK3l3j7qMp/H1nIFatxaNOpgPLAILUx5QEsmS8tCK7fLfIigCR8/bDh6PIDwz+guSzyA7iqr0Mrp3lb/7sA7Ll+hZZ3hGqIgmYXWapdvi1ZVwyq6t5H/GL8F/jHsZ80ecZmVEC0yX7rD/AktOOAq7m201iAewAP5cfd2agjVOdmF3qKFlhm0mKAFC/mm5xHyByJU1b25ritdg6+4TPGC+r7FcgxMtbLiTwEELbokcZ7SLrP1ECC/d7quHKPEXKR56g8IcoiNlAgktPxdK/LlX0scgC59gFYn3Ddakxa7fEVRsOcEsAqt+a4rQsuarp4mgOBd7zJbvJA2KOEyx4KhbCHmUM4gnkLrIz3QjS60kM9KaFkZ4zKvXxdaAEMFUA4Qcfhdo+4B5LN6FmD7ZpdvucJANOpgPKDQSmJLy0ILjl/9K1I867riXDAGADeKhx1aFFIjtCZf2Z30Qgvdfeiqwr912BDXlS5BAAeGByrAQxT5hfyv596Hf8kYl9XfsMIuc6zNC+74eJAjPQAHBweG88FZdXCH44GsnGBqCNU52YVVaCkwTqiAyxRbeh6pVgTcLxw88krxissUWvgxz3SZrSxIB+WhnHk1936Ali7UYXSbA7SOofUWogMthH3c4ZHmqT9wTaosgxFIaAW61nku8zeNvEMrCloIi7vMuoXxUchHiAIlepQwSOk54U9o6enqaSrQSgMRBhGN60ILIv6MQHw95TK7H4EThRZE6ViXed11XWaaSmipZye6/tAihd+79d6RJsYDoqzvdplOZaLLfIYCJa6s29ZyxW8bZQuiUQfjQUhCa+fW8eK3TehG9N1np507isk1fcOVoRtSD7Nag4fLisXzuviEh2J4O7HU3bfJbXPeMN84kRrur3Onuu6XDK6Eb1g9VJQonl9uW68j2pZWhdZtLvMfbhYtHP+i8O8VXYNoIoczX+4ynRSFVuTAuSP/4Hj+5/Ke3sEqtCCkVrrMhzQERQ13OFpZfnGZIgLjZND6gYHF1VzmQxuCAg/+QS6zawgtI3BaINIHcqjOyS78Ca0fDevq3tbzSJHHZYoI670iX1CHM7tM537AZebPi5Y4+GGr+ouWMPw+UIcBnACcPboep7muvN0VaZ76A8ID3WopAaGFa1TdWrAZrsDXigcYWl1w7xNcphgHEAv4ncOJt3KZeYzuVCUMUnpO+BNawJqunqbiCZd36xhaZjDYG4IaQu5+d3g1l1mGdhFqXQ4mtPCCykqX+Vve6jLH8ymhhS5otIoivLY7vi60AMoZzwP89lE2qLvAn9CylivCULYgGnUwHoQktEYMbi66yukITLEzamgLObEo5tqaMKqlT3zMG4UZ1M11BwuK9d8NEau+6C/niVJx1qx8w/MdwqNendKiYztMqumd1uhhT8tzYUC9VWiNG/GswLQId9yeQ84sf/n0fHHjjRlE/luyiU+WvCrOHpkt2rQ05+jCDOmPNbxX/HN4tpyf6uLJeZ50rr8+nfyuBM7hXW+713as5HUdEEg9umAG+/dQj+X9YLvPa4+Jtq1qyu1Fc8zpF3Cdh/6a6rk3HKfuTwmtd6d1lBO57vp1gsxHXAfmEbMKLbytmTfPTdKQBq7Teh0/fDPI73WEamlVaBESLqE6JxIZEFpq7JKd9HVdGZ+GAdVoTY03ECZK8MUS1mXnkSqh1bjRveLCibniq0/7uifWvBIX4Zh3a+oETOA5T8x6+3m5rE5KQmvlsj5SLFnT+vKTPnIOqy3rRohTB2Z6hNanH/aUAmv3bxPFyf3vmGsrDmkh35Jcuri7jDNzagcpWCB4ju6ZJsqUul3Mn/liikILYf5atDBbPtZ1xDaW3MH8WtiuWb24WDD7JTnpqFyL8OtB4vyxOR6hhntDuLo/JbQgCP/8ZbyMs27VYFG2DJYs8m7Rwpxb+/+YIu8B+5Gn1usYP7Klz3Xo1x3MKLQIMaFzig1oCXlDD7QBjAXE+Ce0MKE7EN1a8QTdtxg8bx3gHStYl51HqoQWhI7ah8kzrXEhGsxZ5a+EYfLTlISWfk4YJt7s27Ox57sSWi2evF8M6oeJRs3wee+8INdWtAotCLDj+2bIbXR9QpxMGdc61UIL6aGlCy1lyAtMKIrjILoghJBHzz1T3RMf6xBCWOHe0KWpwiG0nm5WRQpBFRZIaKHVS8WBiMKM+9breKppZZ/r0K87mFFoEWJC5xQbILQwDi+tg/GDGJ6A8UaxhnXZeaRKaP39xxTPPrQ4WeNC6FSvepdXWJ1aJX2E1uovUxZaaDmzChIltCCE0EqGWdiVYfZzq9A68OdbUuCg2xHxzWVyfIUWBFEoQguGmee/+byf7O5DlyS6RNUxyB9z2SEzLgQcuiFxb+Y6g2Y4hBa6OCEMVQteIKGFFkN1HISpOcP9letAN6V+HeEYhRYhJnROsQFjqPrpgcRWWJedh1tozTSEFpy+fxsx+Cm30Jovxc7+P7Cci7nPFFpX4q5bNUh2HVrDIBBWfdFPjttSYe+81d4QWugCQ6vP0z7nhL3Q4UHR73W0aJnfTaE1XzzRuKKYO6OTJ/zwrqli8w9vWoTWfDleCXbu6Gz5vdXT1Qyh9ZwUWv8ex6D7+eLPX8bJ8VYXT841BM5QQ6zcKsNNoeV7Pb16NBIvv1BXbhcrms8QTQ958gV51PpZtGiZcbHg86VTc+W9mUvimOE4ZsiAplIkzX77eRmGPDOF1nyv65gzHS1a5nHvz+0satUo4XUdpUve5nMd4di2DSMptAhx0TnFioOu8JZTIuHDuuw8bBdaF07MkV2HM6a0E5dPzxPz3ukk10D86fthIkOG9LI16OyRWXJcVUpCa+Wy3iJ3rizi142j5DFmN+V8sWD2i+5FrCeJo3vedi9i3dhLaD3WsLzo8xqW/pGCQq4zOG7EM3ItRAxaR3j3l+v7FVrmItq+1/PlJ73l2oPYhojDNgbe4/tvm0a5F30eIvOgyWMVZLg/oYWwJQteEfny3iT+OTwroNCCkIKIxD2WL1tATBrTyus6sLC0fh2wb5f3l8fo168bhRYhJnROsQFvlPXWA4mtsC47D9uFFmyTIRYq3lvIvcBzASkkILq6dX5Yigt0nUEkpCS0YCOHNJfH4K1Dcymf+TKtgX2bWBaxri7OH3vXS2ihOw1jwzDoHKILb0rmyZ1VzHq7g7irWD75RmTD+mVlmC601CLa+rWgdQxiEdtojUqX7hpx+iAWwTb3QwCqRZ8P7nxLhgUSWthGyxnuI5DQ6tmtoeetQ+QdBJz1OtCt6u860G2p8iGYUWgRYkLnFBsgtHrpgcRWWJedxxWhhfFCtDRhHy7sKn74epBPuG4UWoSY0DnFBsw99ZoeSGyFddl5UGglsVFoEWJC5xQbMAFmDz2Q2ArrsvOg0Epio9AixITOKTZAaHXXA4mtsC47D4vQmidoyWUUWoSY0DnFBiwppJYZItGBddl5UGglsVFoEWJC5xQbILTsXNeP+MK67DxSFFpzpj8vmjet7BMezF595RHx0/dDfcLtNsxbNW1iG5/wWFndB0v5hIVra74cYL5x6GdfOIY3FTGX2KJ3XxKzMMmrnzi6UWgRYkLnFBuwwDAWDCfRg3XZeURFaD1QvbhY+Ulvn3C7Ld5CS67z6Cc8HDuxb5r4fuUAn/BwTQmtiyfmiGr3FxPnMVGrn3hWo9AixITOKTZAaHXWA4mtsC47j5CEFuabwlxUmBerQvmC0qFj38A+TeQs5dgeOqCp3B7Qq7Fci+/223KIVZ/39Upr/sxOomCBnHLZmHbP1RRnD8+U4WOHPy3n1rq3XEGxbf0IGTZh1LOibasaotw9BUSO7JnFI/Xu8bk2CC3ML4U4uLZjmKTTCN+1bbx48IG75RxbNasVF2u/esPn2I2rh8jzYcFnxMUxCLeKJ7kOoiFcYLhepCdnpzf21a9bBvVZFC92ixQ1P68dJme+x/xcSBdxkE+YdBTXgOvD/s8+fE3m4WCs1WjEWffNQE+L1gvtH5RzhsGQRxBhCMdEq8hPLKSNSVYvn5orw1f873WZN8+3qSXuKprPUy7DBzUTk0a39Lln3Si0CDGhc4oNxw17UQ8ktsK67DxCElpGHLF4bmfx77HZov/rjaVTh7P3J7Sw7a9F65d1w6V42PrjCHHgz8lSbGAi1J2/jJWC5tyRWaJ96wekoEF8CK1rrrla/LhqkBRkD9Uu6XNtEFoQGkjz1P4ZUnAgHJOl4nounZwjlrzXRQqX43tN0QKDMLo1fzZ5T/8cmik6tq0tHn6otNznT2ihOw6C6ejuqeLPLWPFrxtGesW9cPxdKSCnjm8t46PrDnkF4YO8++/8LvI8uC5cy5FdU0XGjNeJMwff8RJaVnvp+Try89MPXpUCa/ev48XJv6eLKpWLyElcISohEnHu3q82kudRQuu75f1FDaw3qaWpG4UWISZ0TrHhhGEd9UBiK6zLziMkofV4o3u9wrD8zarP+3oJrSH9gwstiCK0XOnpQyi88mI92SpVvUox0bRxRRkOoaVEF2zK2Od8jkWaE0ddabnBDOk4L2ZNR0uT1T5Y8IonHu6p0SPlfNKDWYWWXHDaEE5KVELwQNyobjkVd+n73eS1W9N5f05nKXyKFM7jCUP+IB1sY5Z7iExdaP29Y5KcrR5jt/Ad3YAQhdZ7QQsc8ufZ5lU8xxUqmMsjtGBoIYS4s16TbhRahJjQOcUGCK3n9UBiK6zLzsMttGYYQgvdUb4GUWJ2hZnfL554V3afQSQM7PO46NmtgQx/5qn7DSHRUG6bQquXVzqvd28genSp7/kOgbHo3RfFvHc6ijKlbpNhs9/u4BZac6WQaN8aQsuMP2VsK6/0YBBa1jQhXrb+OFyKDmu89d8OEif/RouW+f3zj17zuqfDf02R3W3YNsXTXPHnljGylQj3u2vbOEMATZThSH/h7Be94uJe0KJlPee3X/Q1hM8wt9Ayw5A/yDNs+wqtuYagmyNq1ywh3uhtxoE98VgFMXfG817XinTRGlft/qIy7NieqbL1D+H4/s+hd6RIRHrWa9Jt24bhFFqEuOicYsVJw9rrgcRWWJedR0hCy4gjPlz4irh08l05tkiJlHEjnpZrGUKI3HzzDV5Ca9mSHl7p/LhqoGyp2f7TKCkWsM7hyCFPifEjn5HdXMf3vi3DHm2AlqbQhRa6I3/fNFKcOThDvPpKfXmNaPVBfIikpe93FVmyZBSn9k/3HIe4WKz648XdxLkjM0XnjnXkmCvsw1gqfHZ/+WGP0IIIq1WjhNj7+wTZfTjzrXYyDoQWjr9wfLa5kPbktlLcQDwiPFyhNWJwM3F/pcLynOqYBbNfECVL5Bf7tpvnhhDr//pjMr/Quob7RZpXXXWVR2j99P0QUeKuW3zySzcKLUJM6JxiA4RWOz2Q2ArrsvMISWjVq1NKjg2CmKp6f1Gx4+fRct+RXW+Jxo3MQfItnqzsEVpvDnxSLjb9zWd9vNKaPqmNuP227LJFrGWLquL80VlSMECYQWSh5QbH/d+8ziELLQyYL1Ykr8ieLbMUUAjftn64vE4IEYiu5R/39Dl2zZf9ZUsaRBi6/SB6ED5ransZ5llw2hA9aA17pF4ZkTlzBjkmDMIKcREHY65wH5vWDPFaSBv7wxVaapFstYD2hu8GS+GGY64soF1Nng/xv1j6mhSayAN0JyqhhTKDUNTvWTcKLUJM6JxiwynD2uiBxFZYl51HikKLlnhWp1ZJT1dnMKPQIsSEzik2QGi11gOJrbAuOw8KrTRm6KJFF6oe7s8otAgxoXOKDacNa6UHElthXXYeFFppzDBe7cQ+zCfmu083Ci1CTOicYgOEVks9kNgK67LzoNBKYqPQIsSEzik2QGg9owcSW2Fddh6m0DpoCC3MNk5LKtu2nkKLEEDnFBvOGNZcDyS2wrrsPCxCa46gJZdRaBFiQucUG/4xrJkeSGyFddl5UGglsVFoEWJC5xQbILSe1AOJrbAuOw8KrSQ2Ci1CTOicYgOE1hN6ILEV1mXnQaGVxEahRYgJnVNsOGvY43ogsRXWZedBoZXERqFFiAmdU2yA0GqsBxJbYV12HhRaSWwUWoSY0DnFBgitR/VAYiusy84k/dG/3mp77vA7fxkmIrELx2b5OHOac41CixATOqfYcM6whnogsRXWZWeS/oslrxXb+9vYYcf2vLXr5L6pIrUGsSVOvUtLENu2/k0KLUJcdE6xAkKrvh5IbIV12Zlca1gGw/4zZVSrsr9vGj76yK7Je3QRFYqdOzzDx5nTnGsUWoSY0DnFhvOG1dMDia2wLjsTOFY4WDjam8cMbnHvbxveHJMasUWhlVhGoUWISYrOSZCISZ8+vfj444/1YJIK9PppIdg+Ej+uNiydYRkN+49hOYYPeLLitvXDxoYrtii0EssotAgxSdE56Y6OhA+E1ieffKIHk1Sg108LwfaR+IE3D1WrVibDshqWa2i/pvdtXTd0nBJRp/wIK90otBLLKLQIMUnROemOjoRPunTpxKeffqoHk1Sg108LwfaR+IJWLYzVgqO9wbCbDMs1uHeTSuG0alFoJZZRaBFikqJzUg5u+vTpiCuWLVtm9XsJRa9evcSECRPkPTRv3lzfHZRBgwaJkydPinPnzol8+fLpu4MCofXZZ595vh84cEBUqVJF3HPPPTLdZGLDhg3ivffe04NTROW/Xj8tBNtH4gtatZTYut51RWzlRquWKbbeEimZKbRm0xLEKLQIMUnROSlHV716dVG5cmXx9NNPW/1fQqGE1unTp8W+ffv03UEpXry42L9/v7h8+bLYvn27vjsoEForVqzwfB84cKAYMmSITKtQoULi+PHjlthpm/fff1+0b99eD04Rlf96/bQQbB+JP0psYbyWR2wN7v1opa3rBhtia1KKYuvc4ek+zpzmXKPQIsQkRecEJ7d7926RJUsWsWXLFnHjjTd6nB+cZtGiRcVtt90munbtKoXDsWPHxOOPPy5ba9B6AT7//HPpKG+//Xbx5JNPilOnTol+/fqJUqVKSRs/fryMt2nTJlG3bl1x5513igYNGnjOo5g5c6YoWLCgyJs3r6hatao4ePCgFCotWrQQ+fPnl+f45ptvZFwVbg1TQuuLL77wtGht3rxZ3HrrrfJ8R44ckffw0ksvyXvKlSuXaNOmjRg+fLjIlCmTuO+++8TZs2c9LVonTpyQ93PHHXeIDz/8UIZBRHXo0EGULFlSFChQQAwdOlQKrZUrV8r94KOPPhJNmjQRP/74oyhTpown3AquqU6dOuKWW26RnxMnTpT38tNPP8n9a9eulXmM+0YZfPvttzK8W7duokSJEqJYsWLif//7nwxDPj388MMyTXXtaB167LHHZH7iWvCJ1jowcuRIUbhwYdGnTx/5XQflbi1zaz7gvCov9Hw4dOiQuOuuu0SePHnE1KlT5XlQJ6znWrVqlahfv75s8cM1Xbp0ySv/Xaaj9keKdZnEHX9iK3eoYotCK7GMQosQkxSdE5zfsGHDpGgBEEZg7969ImfOnLJlCOKjYsWKUtC88MIL0tkvWbJE1KxZUxw9elQKo61bt4qLFy9KhwqRA6d84cIF6Xzr1asnHSocProowZgxY+SnAg4dwmLPnj3ye8uWLcWUKVOkGHrttddk2PLly6WQ+Pfffz3h1jB/Quvuu++WaXfp0kW88cYbYuPGjVLs4dpwDEQfUC0quAclVrp37y6FGMiaNau8FwgtiA3kCURntmzZxLXXXiu++uorGQ9AmED0FClSROajP3AM3lREvkCM4XpxblwnaN26tfjggw/k9qxZs0TTpk3ldqtWreQn7rFSpUpyG6IG9w527twpP1999VV5/WDhwoWyWxhCCy1v5cqVk2L4wQcfFNOmTZNxFKrcrWVuzQe09qm80PMBaVpbtHAe1A/ruSC0rrnmGvHHH3/Ie4dIB2zRSjPoYkt2IUqx9YMhtv4yxNZeQ1j5MSm0Ts6mJYhRaBFikqJzQisT3ppD6wIMrRNwjnPmzJEtVwq0msC5Ig5avsAvv/wiVq9eLVsoFBAwOB4OGC07nTp1kq0jEB/gt99+E+PGjRNly5b1HANwDFqpBg8eLJ0xhALEGATVr7/+6omHOBBL1nAVpgstiLYcOXLIOGfOnJEi5Ouvv5atLhAn69evF9WqVZP7/QktCAXVkoSuVQgjCC2rSIQwhXBQ8RYtWiTatWsntyFaIDhxrXr3IYSJAteqxsY9++yz8vOhhx6S6WCQ/eLFi0XDhg1lOO4TID0IOQChXKNGDXnu33//XYbhutCiBiB00OoGoYUuTbQ0AeQJulmtWMtdlbk1H4DKCz0fkH9WoaXOA9S5ULbqXoAaz0WhleZQgstrzFYwwXXuEIVWIhmFFiEmKTontE6p1goFWh7+/PNP2bIBZ3v+/Hlx//33yxYWxIWgQbdV6dKlpQOFkIJgQ8sR0oODRavRP//8I4UXhM2aNWtkC5jq7po3b57XOSGAypcvL7cPHz0sx4u9+eabUqSobifEwXcIBhVuDdOFFq5HtVj16NFDvPzyy2LAgAHyGgFa4dBlCuDoIYysQgvxlGhAKw7yIpDQguAEEJfoToMQgsB54IEHZMuP6rZTBBNaaOnJmDGjFKc4rnPnzjI/gT+hBZEF4YP7ReuVahnr2bOn3A/x5HK3aP3www/yXiGEUab4bkWVu7XMrfmwY8cOT17o+aCElqpPOA+u03quYEIL+a/XTwvB9hFnArHlNWbLFURsUWglllFoEWKSknO6Gt1+GA9kBS0WYO7cuXJ8EMYRoWUKjvzw4cPSUWKckDruyy+/lN8xngitW3CsGNOD1iS0hkAQ4Vi8madazqytYADdeOguRFcfRBZaaTDWByIKAgnXAcGGcwEVbg3ThRbAODJ049WuXVteO4QEurEghho3biy7QkHHjh2lIII4tI7ReuKJJ2SLmerGCyS0ICQB7hNjl3Lnzi2F4+TJk+V16WI2mNACI0aMkHmBbkWMcUN6iONPaCG8QoUK8p4gKCGwcO2PPPKIzDeMtUJ+KcaOHSvLtHfv3p4wKyh3a5lb8wH5rfJCzwcIrW3btslxWZMmTZLnQVlbzxVIaKn8N+pker2SukmpLhNnoo/ZCii2KLQSyyi0CDFJ0Tl5PJ5DgUBAC41OoPB4AKGli9V4g3FdasoJtNxB8CYCev20EGwfcS76mC0fsXVy7xQBM4XWLFqC2Lb1Qym0CHGF4Jx0R+c0AgmqQOHxAEJr3bp1enBcwfi1Ro0aybf7MA4N49ESAb1+Wgi2jziboGLryF8T95hCa5qPM6c519xCq4hh2V0UWiSJSQvOqaAe4CZQeDy4aFhpPZDYSlqoy8lMQLFldiFO3EOhlVj264ZhE10UWoTQOcWIS4aV0gOJrbAuJz5+xZbqQjyzf+oe3ZnTnGtuoXWni0KLJDnybTMaLY0YSXz8ia1cQ/s1vu/Hrwd03/7T8PFw4L+sGzJ589pBb8F+WjNwKs0ZhvLY8sPgKSifBbM6PmOUXQHDbnaXI15kQdkSQojtQAQU0wMJIQGxCi446awuc1B1XsNuc5lDA9Bagq4pZRh4TYufqXJAuUBg3WpYHsMyu0zRjLUuKbQIIVEBQgsPIEJI6CixhS6nTIb9x2W2juR0mQ78FsPyu0yHTnOGoTxQLhDEuQzLZlhGl9mahW5DlCkhhNiOcJn/8ggh4QHHjJYQiC04bIz1geDCQHmILoz/gaG1ixZfU2UBQ9mgFRKtWSg7tmYRQqIKhFYhPZAQEhJw0GgNQTcinHYGlym60KUIg/iiOctQLigja5chW7MIIVEDQgtjFgghqUN1I0JwwXFDdKE7CgbxlWBW0E9YmjKUC8qIIosQEhMgtG7XAwkhYaMElzIIL5rzzFpGhBASdSC0MFCUEGI/EF80ZxkhhMQUCC28iUMIIUkPHog0Go2WpHaVnzAaLSpGkhR9DVNCCIkIPFf279+vBxOStOA3oTtfkjzo9YEQ4nQu6wHOAs+VAwcO6MGEJC34TejOlyQPen0ghJCIwHPl0KFDejAhSQt+E7rzJcmDXh8IISQi8Fw5cuSIHkxI0oLfhO58SfKg1wdCSALjhF5FPFeOHTumBxOnckkPIHaD34TufEnyoNcHQggxSaUDxnPlxIkTejAhSQt+E7rzJcmDXh8IISQi8Fw5deqUHkxI0oLfhO58SfKg1wdCCIkIPFfOnDmjBxOStOA3oTtfkjzo9YEQQsJi9erVYt26dZ7veK6cP39ehmEfIckOfhO68yXJg14fCCEkLLJkySIdSfbs2aW4wvaaNWvkJ/YRkuzgt+DtekkyodcHQggJCyWu/Jm1pYuQZMX9eyBJil4fCCEkbG644QYfkZUpUyY9GiFJifs3QZIUvT4QQkjY+GvVYmsWISbu3wRJUvT6QAgh/klhXq0MGTJ4CS1CiIn7N0GSFL0+EEJIqtBbtQghJu7fBElS9PpACCGpJnPmzNKpYMwWIcQEvwnd+ZLkQa8PhBCSalSrFsdnEXIF/CZ050uSB70+EELixbIeQmxeLMTBrUKcO67vJdEGeY68RxmgLHZ/r8cgJFXA1+rOlyQPen0ghMSa86eEWD9LiOO79D0kXqAsvh1jlgvKh5AIgK/VnS9JHvT6QAiJNXDmv32ih6aOFN4MJGGCckH5EBIB8LW68yXJg14fCCGxBN1TaDkhzoXlQyIEvlZ3viR50OsDISRWYEwQxgKxy9DZoHw4Zo5EAHyt7nxJ8qDXB0JIrNix3Bx4TZzP9i/0EEJCBr5Wd74kedDrAyEkVqweZ77lRpwPyoqQVAJfqztfkjzo9SGhCTYOePr06bKyL1u2TN8VdTp27CguXLigB4dMhQoVxPbt2/VgyenTp8Ubb7yhB3s4d+6cyJcvnx4cFb744gvRuHFjuR3oegOxYcMG8d5778ntV199VUybNk2LkQZZ3pddUokCyoqQVALfoztfkjzo9SFNsnv3bpElSxaxZcsWceONN4p//vnHs69gwYLS5syZI79v3rxZVKxYUdx6662iQYMGMmzkyJHi9ttvF4ULFxZ9+vSRYSdOnBDPPPOMKFq0qFi+fLkMW7hwoahUqZIoUqSI6NChg0dcLV26VH4izTp16ohbbrlFfk6cOFEUL15clC5dWu6fOXOmvJa8efOKqlWrioMHD8rwypUrS+GCNEuWLCkKFCgghg4dKvc1adJEZMyYUTz//PPipZdeErfddpvIlSuXaNOmjbh8+bK4ePGiR2g98MADokSJEqJYsWLif//7nwyzgvPhGhF/ypQpMgyfOMcdd9wh8ufPL9q1a+c37N9///UrtF5//XWfPF67dq08Dnn37bffikOHDom77rpL5MmTR0ydOlX06tVLCq0PPvhAXivO06xZM3Hy5El5PNJC3t9///3izz//lGEJyced9RDiVFhWJALga3XnS5IHvT6kSYYNGyZatGght0uVKiUFEVi8eLE4f/682LNnjxQnZ8+eFXfffbf4+OOPpUjp0qWLWLFihShXrpw4evSoOHXqlHjwwQelCJg0aZJ4+umnZTrdunWTnzly5BA7duwQly5dEs8995z4+eefZThalcC1114r08b+MmXKiObNm0shNGTIEHm+e+65R14LaNmypUfsKKEF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EkNZKUpiga+VzlfxDrI++IO5m5ToQMsqqsD9Jx26mjvNUVupKEFA/RG+V/EQsj74o847etCyiipw/0mHruZOc9RWKkoQUH+E71U8hKwP/qjzjh60rKIK3H/Soau50xy1lYoSBNQf4XsVDyHrgz9J5LxLlixJefPmpb/++kvu8mP37t00evRoaxsr0oeDtm3b8mu6dOno/PnzYm/8mPP6/PPPqVatWnJ35EmislJA+J+Ewv0nHbqaO81RW6koQUD9Eb5X8RCyPviTBM57+/btVKhQISpWrBht2bJF7vZj69at1LRpU94Op9BKlSoVv37wwQeOBJ/EnBdWu//iCxcshZMEZaUkHbj/pENXc6c5aisVJQioP8L3Kh5C1gd/ksB5jxgxgsaOHUtjxoyhwYMHW+kQLlgxHiBKhO1Tp05R8eLFKWPGjLyMA4RWhQoVqGjRolSvXj0foTZ79mzOW6JECSsClitXLpo1axYLO0TR5syZw+kdOnTgyn/lyhXKlCmTFdF64403qFSpUhxtu/feeznt+eefZ1GYOXNmqlu3Lu3fv9/nvL766isronXt2jUaP348FSxYkA3XGNd5hJUkKCunhD/ec/OD+icdupo7zVFbqShBQP3xdb2Kl5D1wZ8kcN516tSxtsuVK0cbNmzg7UBCy6TbI1qpU6fm7W+//daKbvXp04emTZvG2+D111/nVwic999/30rPnTs3iyRgIlpGaE2ePJlFoGHPnj0sxH799Vd+f/r0aXrwwQd57TNgzssutNq3b09LliyxjvHmm2/ya1znETaSoKyUpAP3n3Toau40R22logQB9Uf4XsVDyPrgTxI4b4yJgvCApUmThvr168fpdqG1bdu2oEILES2zbYQW9iPKhffGAL7jl19+4W2QL1++oEJr0KBBHL2y8+eff9Kjjz5KhQsXptq1a1OzZs3iFFp4/eijj6zPQzCCuM4jbCRBWSlJB+4/6dDV3GmO2kpFCQLqj/C9ioeQ9cGfMDtvCBd0vRm+++47ypIlC12+fJmFy6233srpWIA0mNAyIsq+3b17d1q2bBlvAyNiIHCOHz9upccltMaNG2d19QEIKETGatSoYX3mtddei1NoIaK1dOlS6xirVq3i17jOI2yEUFa4/uHDh1OePHmoefPmHM0DuMYuXbqI3M7p378/rVu3TiYnOahP6KZ1CrqyFy5c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>

[image2]: <data:image/png;base64,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>