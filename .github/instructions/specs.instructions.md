---
applyTo: "**/for_*/**/*.*, **/when_*/**/*.*"
---

# 🧪 How to Write Specs

We call automated tests for specs or specifications based on Specification by Example related to BDD (Behavior Driven Development).
Keep tests focused, isolated, and descriptive!

## Structure

- **File/Folder Structure:**
  - Organize tests by feature/domain, e.g. `Events/for_EventHandler/when_handling`.
  - Use descriptive folder and file names:
    - `for_<TypeUnderTest>/` for the unit under test
    - `when_<behavior>/` for behaviors with multiple outcomes
    - Single file `when_<behavior>` for simple behaviors with single outcomes

## What to specify

- Write specs that verify what is promised from the signature of methods, not based on its implementation.
- **Focus on behaviors (methods that perform actions)**, not simple state.
- **Ignore simple properties** - properties that just return constructor parameters or field values should not have specs.

## Naming

- Use clear, descriptive names for test classes and methods.
- Folder: `for_<Unit>`.
- Class: Single condition - `when_<action>[_and_<aspect>]`.
- Method: `should_<expected_result>`

## Formatting

- Use blank lines to separate the three sections of a spec (`Establish`, `Because`, `should_*`).
- Keep spec classes focused on a single behavior.
