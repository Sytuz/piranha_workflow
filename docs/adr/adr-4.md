# Decision Record #4 - Feature Branching (17-05-2025)

## Summary
> *In the context of **the development of the project**, we decided for **the use of feature branching** to achieve **isolation, collaboration, and code quality**, accepting **the need for discipline in managing branches**.*

## Context
As a team with multiple developers, we needed a way to manage our code changes and collaborate effectively. We considered some options for branching strategies, including:
- **Trunk-based development**, where all developers work on a single branch and merge changes frequently.
- **Feature branching**, where each new feature or bug fix is developed in its own branch and merged back into the main branch when complete.

In the end, we decided to go with feature branching. This decision was influenced by both the need for isolation of changes and the ability to work on multiple features simultaneously without interfering with each other, combined with the fact that we, as a team, are more familiar with this approach and have used it in previous projects.

## Decision

We have decided to use feature branching for our development process. This decision is based on the following reasons:
- **Isolation**: Feature branches allow developers to work on new features or bug fixes in isolation, reducing the risk of introducing bugs into the main branch.
- **Collaboration**: Feature branches enable multiple developers to work on different features simultaneously without interfering with each other's work.
- **Code Quality**: By using pull requests to merge feature branches back into the main branch, we can ensure that code is reviewed and tested before being integrated, improving overall code quality.
- **Trade-off**: While feature branching requires discipline in managing branches and ensuring that they are kept up to date with the main branch, we believe that the benefits of isolation and collaboration outweigh the downsides. We will need to establish guidelines for managing branches and merging changes, but we expect this to improve our development process.

## Consequences
- We will implement feature branching for our development process, creating a new branch for each new feature or bug fix.
- We will have a 'main' branch for production-ready code and a 'develop' branch for ongoing development.
- We will use pull requests to merge feature branches back into the main branch.