# C# Technical Prototypes

This repository contains my C# technical prototype projects, simulations, and interface experiments.

The goal of this workspace is to group reusable C# mockups and proof-of-concept applications in one place, with clean project separation and lightweight deployment notes.

At the moment, the main project is:

- **DialMock**: a Blazor-based dial and gauge simulation used to test rendering, scale logic, and preview behavior for technical instrument layouts.

Further C# projects can be added later in the same repository as separate folders.

## Repository structure

```text
.
├── DialMock/              # Blazor dial simulation project
├── TestMock/              # temporary or experimental test project
├── docs/                  # shared documentation
└── README.md
```

## Current main project: DialMock

DialMock is a small Blazor web application that renders a configurable gauge preview.

It is intended as a mock environment for:

* dial layout experiments
* rule validation
* scale and value rendering checks
* future simulation and UI prototyping around instrument visualization

## Development

From the repository root or project folder:

```bash
cd DialMock
dotnet restore
dotnet build
dotnet watch
```

Then open the local URL shown in the terminal.

## Release publish

Publish the application outside the project tree:

```bash
cd DialMock
rm -rf ../publish
dotnet publish -c Release -o ../publish
```

Run the published application with:

```bash
cd ../publish
dotnet DialMock.dll
```

## Notes

* Do not commit `bin/`, `obj/`, or `publish/`.
* For deployment instructions, see the documentation in `docs/`.
* If local machine-specific configuration is needed later, keep it outside the repository or in ignored override files.

```

---
 