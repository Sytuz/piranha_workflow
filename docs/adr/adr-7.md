# Decision Record #7 - Role-Based Access Control for Workflow Management (23-05-2025)

## Status: Accepted

## Summary
> *In the context of **role-based access control for workflow management**, we decided for **the integration of the existing Role system and stage-based authorization** to achieve **a compatible and easy-to-use access control**, accepting **the added complexity**.*

## Context
In our project, we needed to implement access control mechanisms for workflow management to ensure proper content management and security. In order to achieve this, we first considered the existing role-based access control system of PiranhaCMS, which allows for defining user roles and permissions. After analyzing the code, we determined that the existing role system could be extended to include permissions for workflow stage transitions. By using the Permission system already in place, we could manage access to actions related to workflow creation, editing, and deletion.

The same Permission system, however, is not efficient for managing access to individual workflow stages, especially when there are multiple stages and workflows involved. Therefore, we needed to decide how to handle access control for workflow stages specifically. This led us to further analyze the options available for managing stage-based access control, more particularly whether to associate a role with each stage or to use a more granular approach by defining permissions for each stage transition. We opted to go with the first option, associating roles with workflow stages, for more simplicity.

## Decision
We have decided to **integrate with the existing Role system and implement stage-based authorization** for workflow management access control. This decision is based on the following reasons:
- **Consistency**: Maintains consistency with the existing authentication and authorization infrastructure of PiranhaCMS.
- **Hybrid Approach**: Leverages existing permissions for workflow operations while implementing role-to-stage associations for granular stage access control.
- **Simplicity**: Associating roles directly with workflow stages provides a simpler and more intuitive approach than permission-based stage transitions.
- **User Familiarity**: Users are already familiar with the role-based approach from other system components.
- **Maintainability**: Centralized role management with stage associations reduces complexity in system administration.

## Consequences
- Role definitions will be extended to include associations with specific workflow stages.
- The existing Permission system will continue to handle workflow creation, editing, and deletion operations.
- A new stage-based authorization layer will be implemented to control access to individual workflow stages.
- Administrators can control stage access through role-to-stage mappings in the existing role management interface.
- The system will validate both user permissions for workflow operations and role-stage associations for stage transitions.
- Additional complexity is introduced in role configuration with stage mappings, but this is offset by the simplicity and consistency benefits.
- Future workflow customizations will need to consider both permission-based and role-stage-based restrictions.

## Associated Drivers
*CON-2*