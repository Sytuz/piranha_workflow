# Decision Record #1 - Use of Markdown for Decision Records (03-05-2025)

## Status: Accepted

## Summary
> *In the context of **documenting our architectural decisions**, we decided for **the use of Markdown as the format for our ADRs** to achieve **simplicity, readability, and ease of version control**, accepting **no notable downsides**.

## Context

We needed to decide on a format for documenting our architectural decisions. The options we considered included:
- Markdown
- Google Docs
- Text files

Google Docs was ruled out due to the need for internet access and adding extra steps to including the decision records in our version control system (Git).
Text files were considered, but they lack the readability and formatting capabilities of Markdown.
The template we are using for our decision records is written in Markdown, which influenced our decision.

## Decision
We have decided to use Markdown as the format for our decision records. This decision is based on the following reasons:
- **Simplicity**: Markdown is a lightweight markup language that is easy to read and write. It allows us to focus on the content of our decisions without getting bogged down in complex formatting.
- **Readability**: Markdown is designed to be readable in plain text, making it easy to understand the content of our decisions without needing to render it in a specific format.
- **Ease of Version Control**: Markdown files can be easily tracked and managed in version control systems like Git, allowing us to maintain a history of our decisions and changes over time.
- **No Notable Downsides**: We did not identify any significant downsides to using Markdown for our decision records. It is widely supported and can be easily converted to other formats if needed in the future.

## Consequences
- We will use Markdown for all future architectural decision records.
- We will follow the template provided for consistency in formatting and structure.
- We will ensure that all team members are familiar with Markdown and its usage for documenting decisions.

## References
- [Markdown Template](https://github.com/joelparkerhenderson/architecture-decision-record/tree/main/locales/en/templates/decision-record-template-for-alexandrian-pattern)