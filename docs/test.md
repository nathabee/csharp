# Test

## Objective

Protect the reusable dial engine before AutoCAD integration.

Current focus:
- engine geometry
- validation rules
- stable drawing contract

---

## Create test project

```bash
cd ~/coding/github/csharp

dotnet new xunit -n DialMock.Tests
dotnet sln add DialMock.Tests/DialMock.Tests.csproj
dotnet add DialMock.Tests/DialMock.Tests.csproj reference DialMock.Core/DialMock.Core.csproj

# check
dotnet sln list

# remove default example
rm DialMock.Tests/UnitTest1.cs

# create test files
touch DialMock.Tests/DialEngineTests.cs
touch DialMock.Tests/DialRuleEngineTests.cs

# run
dotnet test
```

---

## Test project structure

```text
DialMock.Tests
├── DialEngineTests.cs
└── DialRuleEngineTests.cs
```

### `DialEngineTests.cs`

Tests geometry output from `DialEngine`.

Checks:

* arc creation
* tick count
* needle count
* label count
* known value to expected position

### `DialRuleEngineTests.cs`

Tests validation and rule behavior.

Checks:

* valid input accepted
* invalid ranges rejected
* preview out of range rejected
* invalid tick count rejected

---

## Current test status

### Geometry / engine

* [x] one arc is created
* [x] expected number of lines is created
* [x] expected number of labels is created
* [x] mid value places needle at top
* [x] min value places needle on upper-left side
* [x] max value places needle on upper-right side

### Validation / rules

* [x] valid spec is accepted
* [x] max must be greater than min
* [x] preview value must stay in range
* [x] major tick count must be valid

### Result

* [x] 10 tests pass

---

## Still missing

### Geometry robustness

* [ ] overlapping labels for high tick counts
* [ ] label placement for long values
* [ ] exact tick spacing for several ranges
* [ ] exact arc start/end coordinates
* [ ] zero or invalid range handling at engine level
* [ ] decimal values
* [ ] negative min/max values

### Contract stability

* [ ] needle is always the last line
* [ ] title/unit moved fully into drawing contract
* [ ] drawing stays valid when spec evolves

### Renderer tests

* [ ] SVG smoke test
* [ ] SVG arc path correctness
* [ ] SVG contains expected classes/elements
* [ ] visual regression screenshot test

### Future AutoCAD tests

* [ ] mapping `Line2` to AutoCAD line entities
* [ ] mapping `Arc2` to AutoCAD arc entities
* [ ] mapping `Text2` to AutoCAD text entities
* [ ] plugin command smoke test inside AutoCAD

---

## Notes

These tests currently protect the Core engine, not the UI layout.

This means:

* Blazor can change
* SVG renderer can change
* AutoCAD adapter can be added later

As long as Core tests stay green, the dial logic remains stable.

For Docker / Jenkins / deployment validation, see:

* `developer-guide.md`
* `ci-cd.md`
 