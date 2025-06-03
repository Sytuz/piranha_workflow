# Decision Record #5 - Base Stages for Editorial Workflow (20-05-2025)

## Status: Accepted

## Summary
> *In the context of **the editorial workflow**, we decided for **the implementation of 'Draft' and 'Published' stages in every workflow** to achieve **an easier integration with the current PiranhaCMS**, accepting **no major downside**.*

## Context
In our project, there was a need to integrate the current features of PiranhaCMS with the editorial workflow. More specifically, PiranhaCMS has a built-in 'Save' and 'Publish' functionality, which aligns with the basic stages of an editorial workflow. There were no doubts that these two stages would be beneficial and necessary for the editorial process. However, we had to decide how they would be implemented in the context of our project, in regards to their flexibility. The options we considered were:
- **Fixed Stages**: Implementing 'Draft' and 'Published' stages as fixed stages in every workflow, ensuring that they are always present and cannot be removed.
- **Optional Stages**: Allowing 'Draft' and 'Published' stages to be part of the workflow but not enforcing them, meaning they could be removed or modified by users.
- **Restricted Stages**: Implementing 'Draft' and 'Published' stages as restricted stages, meaning that they are always present but can be customized in terms of their permissions and behavior.

## Decision

We have decided to implement the **'Draft' and 'Published' stages as restricted stages** in every workflow. This decision is based on the following reasons:
- **Integration with PiranhaCMS**: The 'Draft' and 'Published' stages align with the existing functionality of PiranhaCMS, making it easier to integrate and use.
- **Flexibility**: By making these stages restricted, we allow users to customize their workflows while ensuring that the essential stages are always present.
- **Ease of Use**: Users will have a clear understanding of the basic stages of the editorial process, which simplifies content creation and approval.
- **No Major Downsides**: There are no significant downsides to implementing these stages as restricted, as they do not impose additional complexity on users who may only need the basic stages.

The implementation will require two additional attributes in the workflow stage model:
- `isImmutable`: A boolean attribute indicating whether the stage can be removed or not.
- `isPublished`: A boolean attribute indicating whether the stage is a 'Published' stage or not.

This is to ensure that the behavior of the 'Draft' and 'Published' stages differs from other stages in the workflow, allowing for a clear distinction between them and other custom stages.

## Consequences
- The 'Draft' and 'Published' stages will be implemented as restricted stages in every workflow, ensuring they are always present
- Users will have the flexibility to customize their workflows while still having the essential stages available.
- The integration with PiranhaCMS will be seamless, allowing users to leverage the existing functionality for content creation and approval.
- The implementation will ensure that the editorial workflow is easy to use and understand, providing a universal standard for content creation and approval.

## Associated Drivers
*UC-4, AC-6, CON-2*