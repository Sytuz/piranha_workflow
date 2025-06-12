# Decision Record #6 - Default Workflow (21-05-2025)

## Summary
> *In the context of **the default editorial workflow**, we decided for **the workflow described in the scenario** to achieve **an easy-to-use universal standard**, accepting **no major downside**.*

## Context
In our project, we needed to establish the default workflow and stages that would be available at initialization. As mentioned in ADR#5, the default stages that must be present in the workflow are 'Draft' and 'Published'. Because of that, we considered two main options:
- **Simple Workflow**: A basic workflow with the 'Draft' and 'Published' stages, suitable for simple projects.
- **Full Workflow**: A comprehensive workflow with multiple stages, including 'Draft', 'Review', 'Legal', 'Approved' and 'Published', providing a complete editorial process.

## Decision
We have decided to implement the **Full Workflow** as the default workflow for our project. This decision is based on the following reasons:
- **Comprehensive Coverage**: The full workflow covers all necessary stages of the editorial process, ensuring that all aspects of content creation and approval are addressed.
- **Flexibility**: The full workflow allows for more complex editorial processes, making it suitable for a wide range of projects.
- **Ease of Use**: By providing a complete workflow, we ensure that users have a clear understanding of the process and can easily navigate through the stages.
- **No Major Downsides**: There are no significant downsides to implementing the full workflow, as it does not impose any additional complexity on users who may only need the basic stages (due to the fact that any user can create their own workflow with the stages they need).

## Consequences
- The default workflow will include the 'Draft', 'Review', 'Legal', 'Approved', and 'Published' stages.
- Users will have a clear and comprehensive editorial process to follow, which can be customized as needed.
- The implementation will ensure that the workflow is easy to use and understand, providing a universal standard for content creation and approval.
- The full workflow will be implemented in the system, allowing users to easily create and manage their content through the defined stages.

## Associated Drivers
*UC-4, AC-6*

## References
- [Link to ADR#5](adr-5.md)