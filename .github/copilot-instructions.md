# GitHub Copilot Instructions

## General

- Make only high confidence suggestions when reviewing code changes.
- Always use the latest version C#, currently C# 13 features.
- Never change global.json unless explicitly asked to.
- Never leave unused using statements in the code.
- Always ensure that the code passes all tests.
- Always ensure that the code adheres to the project's coding standards.
- Always ensure that the code is maintainable.
- Always reuse the active terminal for commands.
- Do not create new terminals unless current one is busy or fails.

## Formatting

- Honor the existing code style and conventions in the project.
- Apply code-formatting style defined in .editorconfig.
- Prefer file-scoped namespace declarations and single-line using directives.
- Insert a new line before the opening curly brace of any code block (e.g., after `if`, `for`, `while`, `foreach`, `using`, `try`, etc.).
- Ensure that the final return statement of a method is on its own line.
- Use pattern matching and switch expressions wherever possible.
- Use `nameof` instead of string literals when referring to member names.
- Place private class declarations at the bottom of the file.

## Instructions

- Write clear and concise comments for each function.
- Prefer `var` over explicit types when declaring variables.
- Do not add unnecessary comments or documentation.
- Use `using` directives for namespaces at the top of the file.
- Sort the `using` directives alphabetically.
- Use namespaces that match the folder structure.
- Remove unused `using` directives.
- Use file-scoped namespace declarations.
- Use single-line using directives.
- Prefer using `record` types for immutable data structures.
- Use expression-bodied members for simple methods and properties.
- Use `async` and `await` for asynchronous programming.
- Use `Task` and `Task<T>` for asynchronous methods.
- Use `IEnumerable<T>` for collections that are not modified.
- Never return mutable collections from public APIs.
- Don't use regions in the code.
- Never add postfixes like Async, Impl, etc. to class or method names.
- Favor collection initializers and object initializers.
- Use string interpolation instead of string.Format or concatenation.
- Favor primary constructors for all types.

## Naming Conventions

- Follow PascalCase for component names, method names, and public members.
- Use camelCase for private fields and local variables.
- Prefix private fields with an underscore (e.g., `_privateField`).
- Prefix interface names with "I" (e.g., IUserService).

## Testing

- Follow the following guides:
   - [C# specifics](./instructions/csharp.instructions.md)
   - [How to Write Specs](./instructions/specs.instructions.md)
   - [How to Write C# Specs](./instructions/specs.csharp.instructions.md)

## Documentation

- Follow the [documentation instructions](./instructions/documentation.instructions.md).

## Pull Requests

- Follow the [pull request instructions](./instructions/pull-requests.instructions.md).

## Header

All files should start with the following header:

```csharp
// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
```
