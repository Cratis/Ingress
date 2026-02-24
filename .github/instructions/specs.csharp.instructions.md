---
applyTo: "**/for_*/**/*.cs, **/when_*/**/*.cs"
---

# 🧪 How to Write C# Specs

Use the base instructions for writing specs can be found in [Specs Instructions](./specs.instructions.md) and
then adapt with the C# specific conventions below.

## Test Frameworks & Conventions

- **Frameworks:**
  - Uses [Xunit](https://xunit.net/) as test framework and execution.
  - Uses [NSubstitute](https://nsubstitute.github.io/) for mocking.
  - Uses [Cratis Specifications](https://github.com/Cratis/Specifications/blob/main/README.md) for BDD style specification by example testing.
  - Use separate projects for specs, e.g. `Ingress.Specs`.

## Base class

- At the root of inheritance, `Specification` must be the base class.

## Test Class Pattern

- Use BDD-style methods:
  - `void Establish()` for setup.
  - `void Because()` for the action under test.
  - `[Fact] void should_<expected_behavior>()` for assertions.
  - Keep them focused on a single behavior or aspect.

**Example:**

```csharp
public class when_something_happens : Specification
{
    string _result;

    void Establish() => _result = string.Empty;

    void Because() => _result = "something";

    [Fact] void should_have_the_expected_result() => _result.ShouldEqual("something");
}
```
