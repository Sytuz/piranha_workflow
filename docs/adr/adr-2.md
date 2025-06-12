# Decision Record #2 - Multi-Stage Workflow for Content Management (04-05-2025)

## Summary
> *In the context of **the editorial workflow for content management**, we decided for **a multi-stage workflow** to achieve **flexibility, control, and collaboration**, accepting **additional implementation complexity as a trade-off**.

## Context
The scenario mentions the need to support the handoff of content *from creators to reviewers, legal teams, and final approvers, in custom tailored flows*, so our first plan was to implement a static workflow with a fixed number of stages, similar to what is shown in the image below:

![image](../assets/StaticWorkflow.png)

In this version, the workflow is defined in advance and does not allow for dynamic changes or custom flows. This approach is simple to implement but lacks flexibility and may not meet the needs of all teams.

However, as we discussed the requirements further, we realized that a more flexible approach was needed. We wanted to allow for custom flows and dynamic changes to the workflow based on the specific needs of each team or project. This led us to consider a multi-stage workflow with the following characteristics:

- **Dynamic Stages**: The ability to add, remove, or modify stages in the workflow as needed. This allows teams to tailor the workflow to their specific needs and adapt it over time.
- **Bi-directional Transitions**: The ability to move content back and forth between stages, allowing for iterative review and feedback. This is important for teams that need to collaborate closely and make changes based on feedback.
- **Customizable Transition Rules**: The ability to define custom rules for transitioning between stages, allowing teams to enforce specific requirements or conditions for moving content through the workflow.

Also, we took into consideration the previous workflow we had in mind, and decided to keep it as the default workflow for the system, while allowing for the creation of custom workflows. This way, we can provide a simple and straightforward option for teams that do not need the complexity of a multi-stage workflow, while still allowing for customization when needed.

## Decision
We have decided to implement a multi-stage workflow for content management, with a default workflow for easier initiation. This decision is based on the following reasons:
- **Flexibility**: The multi-stage workflow allows teams to customize their workflows to meet their specific needs, making it easier to adapt to changing requirements.
- **Control**: The ability to define custom transition rules and bi-directional transitions gives teams more control over the content review process, ensuring that all necessary steps are followed.
- **Collaboration**: The multi-stage workflow supports collaboration between different teams and stakeholders, allowing for iterative feedback and improvements to the content.
- **Trade-off**: While this approach adds some complexity to the implementation by generalizing the workflow, we believe that the benefits of flexibility and control outweigh the downsides. We will need to invest time in designing and implementing the workflow, but we expect this to pay off in the long run.

## Consequences
- We will implement a multi-stage workflow for content management, allowing for dynamic stages, bi-directional transitions, and customizable transition rules.
- We will need to invest time in designing and implementing the workflow, including defining the stages, transitions, and rules.
- We will provide a default workflow for easier initiation, while allowing for the creation of custom workflows.

## References
- [Storyblok Workflow](https://www.storyblok.com/docs/editor-guides/workflows-basic-custom)