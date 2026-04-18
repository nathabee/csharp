# C# Technical Prototypes

This repository contains C# technical prototype projects, simulations, and interface experiments.

The purpose of this workspace is to build reusable technical components with clear architectural separation between:

- domain logic
- UI rendering
- CAD integration
- CAD export pipelines
- external host simulation

## Layered Architecture

The current architectural layers are:

- **DialMock.Core**: business rules and neutral dial geometry
- **DialMock**: Blazor UI for interactive SVG preview
- **DialMock.CadModel**: neutral CAD contract
- **DialAutoCADPlugin**: reusable CAD/plugin integration layer
- **AutoCadMock**: interactive desktop host that calls the plugin
- **DXF export service**: plugin-side reusable service
- **future real AutoCAD adapter**: maps the same CAD model into AutoCAD DB objects inside transactions

The intent is:

- the **Blazor UI** uses Core to preview the dial as SVG
- the **CAD/plugin path** uses Core to build dial geometry, converts it to a CAD-neutral model, and makes it reusable for:
  - DXF export
  - desktop host simulation
  - future real AutoCAD integration

---

## Current Status

**Work in progress**

Implemented so far:

- `DialMock.Core`
- `DialMock`
- `DialMock.CadModel`
- `DialAutoCADPlugin`
- `AutoCadMock`
- plugin request boundary via `DialCadRequest`
- CAD summary output
- DXF export
- normalized CAD-style arc model (`StartAngleDeg`, `EndAngleDeg`)
- interactive desktop host UI for `AutoCadMock`
- CAD viewer validation in LibreCAD
- extended CAD-path test coverage

Not yet implemented:

- real AutoCAD adapter
- native AutoCAD DB object creation
- print/plot framing strategy
- CI/CD packaging split between web host and desktop host
- optional headless execution mode for CI/workflow use

---

## Repository Structure

<details>
<summary>Click to expand directory structure</summary>

```text
.
├── AutoCadMock/               Interactive desktop CAD host
│   ├── Assets/
│   ├── Diagnostics/
│   ├── Models/
│   ├── ViewModels/
│   ├── Views/
│   ├── App.axaml
│   ├── App.axaml.cs
│   ├── Program.cs
│   ├── ViewLocator.cs
│   └── AutoCadMock.csproj
│
├── DialAutoCADPlugin/         CAD integration layer
│   ├── Abstractions/
│   ├── Export/
│   ├── Mapping/
│   ├── Models/
│   ├── Services/
│   └── DialAutoCADPlugin.csproj
│
├── DialMock/                  Blazor UI preview
│   ├── Components/
│   ├── Rendering/
│   ├── Services/
│   └── DialMock.csproj
│
├── DialMock.CadModel/         CAD-neutral entity model
│   ├── Geometry/
│   ├── Model/
│   └── DialMock.CadModel.csproj
│
├── DialMock.Core/             Domain logic and geometry generation
│   ├── Engine/
│   ├── Geometry/
│   ├── Models/
│   ├── Samples/
│   ├── Services/
│   └── DialMock.Core.csproj
│
├── DialMock.Tests/            Unit tests
│
├── docs/                      Project documentation
│   ├── architecture.md
│   ├── cicd.md
│   ├── developer-guide.md
│   ├── install.md
│   ├── apache.md
│   ├── test.md
│   ├── history.md
│   ├── version.md
│   └── project-presentation-for-wordpress.md
│
├── scripts/                   CI helper scripts
├── Dockerfile
├── Jenkinsfile.ci
├── Jenkinsfile.deploy
├── DialMock.slnx
├── TODO.md
├── VERSION
├── LICENSE
└── README.md
```

</details>

---

## Architecture Overview

```mermaid

graph TD
    A[DialSpec input]

    A --> B[DialMock.Core<br/>shared dial logic]

    B --> C[DialMock<br/>web host]
    C --> D[SVG renderer]
    D --> E[Browser demo]

    B --> F[DialAutoCADPlugin<br/>reusable CAD integration layer]
    F --> G[Neutral CAD model]

    G --> H[AutoCadMock<br/>desktop mock host]
    H --> I[DXF exporter]
    I --> J[DXF viewer]

    G --> K[Future AutoCAD host<br/>planned]
    K --> L[AutoCAD API adapter<br/>planned]
    L --> M[AutoCAD DB entities<br/>planned]

    subgraph Implemented today
        B
        C
        D
        E
        F
        G
        H
        I
        J
    end

    subgraph Planned integration
        K
        L
        M
    end

```
 

### Architectural Notes

* `DialMock.Core` remains renderer-neutral.
* `DialMock.CadModel` is not a second business layer; it is a CAD-shaped neutral contract.
* `DialAutoCADPlugin` owns the plugin-facing request contract and the CAD mapping/export logic.
* `AutoCadMock` simulates how an external CAD host would call the plugin.
* DXF export belongs to the reusable CAD/plugin side, not to Core business logic.
* The current CAD path uses a normalized CAD-style arc convention.
* Future real AutoCAD integration should adapt the same CAD model into native AutoCAD database objects.

---

## Project Roles

### DialMock.Core

Domain logic and geometry generation.

Responsibilities:

* validation rules
* dial geometry generation
* neutral drawing output
* sample dial definitions

Key outputs:

```text
DialDrawing
Line2
Arc2
Text2
Point2
```

`Arc2` uses a normalized CAD-style representation with:

```text
StartAngleDeg
EndAngleDeg
```

Core contains **no UI logic** and **no CAD export logic**.

---

### DialMock

Blazor-based dial preview application.

Responsibilities:

* user input
* validation display
* SVG rendering
* visual debugging

Used as:

* visualization tool
* geometry validation UI
* development sandbox

---

### DialMock.CadModel

Neutral CAD entity model.

Responsibilities:

* represent CAD geometry
* maintain layer structure
* remain independent from CAD vendors

Key entities:

```text
CadDrawing
CadEntity
CadLine
CadArc
CadCircle
CadText
CadLayer
```

Contains **no export logic** and **no business rules**.

---

### DialAutoCADPlugin

Reusable CAD integration layer.

Responsibilities:

* accept plugin-facing request input
* convert request to Core input
* validate input
* generate geometry via Core
* map geometry to CAD entities
* export DXF output

Public API:

```csharp
CadDrawing Build(DialCadRequest request);
```

Important:

The plugin owns the external request contract:

```text
DialCadRequest
```

This isolates Core from external consumers and makes the plugin callable by a simulated or future real CAD host.

---

### AutoCadMock

Interactive desktop CAD host simulator.

Responsibilities:

* collect dial values from the user
* build `DialCadRequest`
* call plugin services
* generate DXF output
* display CAD summary and output path

Used for:

* integration testing
* manual CAD workflow simulation
* validating the host-driven interaction model
* local DXF generation and inspection

This project **does not reference Core**.

---

## Current Functional Capabilities

The system currently supports:

* dial rule validation
* dial geometry generation
* CAD entity generation
* normalized CAD-style arc storage
* layered drawing output
* DXF export
* interactive desktop host input
* SVG preview rendering
* CAD viewer validation in LibreCAD
* unit and integration-style test coverage for the CAD path

Typical output example:

```text
CAD DRAWING SUMMARY
===================
Layers   : 6
Entities : 27

Layers:
- DIAL_ARC
- DIAL_TICKS
- DIAL_LABELS
- DIAL_NEEDLE
- DIAL_CENTER
- DIAL_META
```

---

## Development

From repository root:

```bash
dotnet restore
dotnet build
```

Run the Blazor UI:

```bash
dotnet run --project DialMock/DialMock.csproj
```

Run the interactive desktop CAD host:

```bash
dotnet run --project AutoCadMock/AutoCadMock.csproj
```

Run tests:

```bash
dotnet test DialMock.slnx
```

---

## Runtime Model

The repository currently contains two different host types:

### `DialMock`

A web host for SVG preview and business-rule-oriented interaction.

### `AutoCadMock`

A desktop host for CAD-side interaction.

For version 1.2.x, `AutoCadMock` is implemented as a lightweight interactive desktop application.
A future headless mode may be added later for CI, container, or workflow-driven execution.

---

## CI/CD

Automated pipelines are configured using:

```text
Jenkinsfile.ci
Jenkinsfile.deploy
Jenkinsfile.desktop-build
Dockerfile
scripts/
```

graph TD

    A[Git commit]

    A --> B[Jenkinsfile.ci]

    B --> C[Build solution]
    C --> D[Test solution]

    A --> E[Jenkinsfile.deploy]

    E --> F[Build DialMock Docker image]
    F --> G[Deploy web container]

    A --> H[Jenkinsfile.desktop-build]

    H --> I[Publish AutoCadMock]
    I --> J[Create Linux package]
    J --> K[Archive desktop artifact]
At present, CI/CD still needs to be updated so that:

* `DialMock` is handled as a web host
* `AutoCadMock` is handled as a desktop artifact
* both outputs are built and published appropriately

See:

```text
docs/cicd.md
```

---

## Documentation

Detailed documentation is available under:

```text
docs/
```

Important files:

```text
architecture.md        system architecture
developer-guide.md     development notes
install.md             development installation
cicd.md                CI/CD pipeline details
apache.md              Apache and DNS configuration
test.md                functional test reference
history.md             version history
version.md             versioning rules
```

---

## Roadmap (Short-Term)

Upcoming phases:

```text
Phase 10 — CI/CD, packaging, and runtime adaptation
Phase 11 — print/plot framing strategy
Phase 12 — optional headless execution mode
Phase 13 — optional workflow/PLM-style request injection
```

---

## License

MIT License

See `LICENSE`.
