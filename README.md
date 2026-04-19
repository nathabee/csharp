# DialMock / AutoCadMock

A C# prototype workspace for dial-generation workflows across **web**, **desktop**, and **future CAD integration** paths.

The repository demonstrates how the same dial logic can be reused through different runtime hosts:

- **DialMock** — Blazor web host for SVG preview
- **AutoCadMock** — Avalonia desktop host for CAD-side simulation and DXF generation
- **DialAutoCADPlugin** — reusable CAD-oriented integration layer shared by the CAD path

---

## What this project shows

This workspace is built to demonstrate:

- clean separation between domain logic, rendering, and CAD integration
- reuse of the same core logic across multiple hosts
- neutral CAD modeling before vendor-specific integration
- DXF export as a reusable output path
- Docker-based CI/CD with host-specific packaging
- Jenkins-based build, deploy, packaging, and release automation

---

## Current runtime model

The repository contains **two real runtime hosts**.

### DialMock

Blazor web host for:

- dial input
- validation feedback
- SVG rendering
- browser-based preview

Runtime model:

- packaged and deployed in Docker
- exposed as a web application

### AutoCadMock

Avalonia desktop host for:

- CAD-side workflow simulation
- request construction
- plugin invocation
- DXF generation
- desktop-side validation

Runtime model:

- built in Docker during CI/release packaging
- published as a downloadable desktop artifact
- run locally outside Docker

---

## Architecture

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
````

---

## Main projects

### `DialMock.Core`

Shared domain logic.

Responsibilities:

* dial validation
* geometry generation
* neutral dial drawing output

### `DialMock`

Blazor web host.

Responsibilities:

* user interaction
* SVG preview
* validation display

### `DialMock.CadModel`

Neutral CAD contract.

Responsibilities:

* CAD-shaped drawing model
* lines, arcs, circles, text, layers
* vendor-independent representation

### `DialAutoCADPlugin`

Reusable CAD-oriented integration layer.

Responsibilities:

* accept `DialCadRequest`
* convert request to core input
* validate and generate geometry
* map to CAD-neutral entities
* export DXF

### `AutoCadMock`

Avalonia desktop host.

Responsibilities:

* collect user input
* create `DialCadRequest`
* call plugin services
* generate DXF output
* simulate external CAD host behavior

---

## Current capabilities

Implemented today:

* dial rule validation
* dial geometry generation
* SVG preview
* CAD-neutral entity generation
* normalized CAD-style arc representation
* DXF export
* interactive desktop host
* LibreCAD validation
* automated tests
* Docker-based CI
* Jenkins web deploy pipeline
* Jenkins desktop packaging pipeline
* Jenkins desktop GitHub Release pipeline

Planned later:

* real AutoCAD host integration
* AutoCAD API adapter
* AutoCAD DB object creation
* print/plot framing strategy
* optional headless execution mode
* Windows desktop release asset

---

## CI/CD

Docker-based Jenkins pipelines build and package both hosts without requiring a local .NET SDK on the Jenkins machine.

- **DialMock** → Docker runtime
- **AutoCadMock** → desktop artifact / GitHub Release asset

```mermaid
flowchart TD
    A[GitHub push] --> B[Jenkins CI]
    A --> C[Jenkins Deploy]
    A --> D[Jenkins Desktop Build]
    A --> E[Jenkins Desktop Release]

    B --> F[Build + test in Docker]
    F --> G[Publish DialMock artifact]
    F --> H[Publish AutoCadMock artifact]

    C --> I[Build DialMock runtime image]
    I --> J[Deploy web container]

    D --> K[Package Linux desktop archive]
    K --> L[Archive in Jenkins]

    E --> M[Package Linux desktop archive]
    M --> N[Upload to GitHub Release]
````

### Pipelines

* `Jenkinsfile.ci` — build, test, publish, archive
* `Jenkinsfile.deploy` — deploy `DialMock` web container
* `Jenkinsfile.desktop-build` — package `AutoCadMock` for Jenkins download
* `Jenkinsfile.desktop-release` — publish desktop package to GitHub Releases

```mermaid
flowchart LR
    A[Shared solution] --> B[DialMock]
    A --> C[AutoCadMock]

    B --> D[Docker image]
    D --> E[Running web app]

    C --> F[tar.gz package]
    F --> G[Jenkins artifact]
    F --> H[GitHub Release asset]
```

See [CI/CD user manual](docs/cicd.md).

## Development

From repository root:

```bash
dotnet restore
dotnet build
```

Run the web host:

```bash
dotnet run --project DialMock/DialMock.csproj
```

Run the desktop host:

```bash
dotnet run --project AutoCadMock/AutoCadMock.csproj
```

Run tests:

```bash
dotnet test DialMock.slnx
```

---

## Documentation

Detailed documentation is available under `docs/`.

Important files:

```text
docs/architecture.md
docs/cicd.md
docs/developer-guide.md
docs/install.md
docs/test.md
docs/history.md
docs/version.md
```

---

## Status

Phase 10 — **CI/CD, packaging, and runtime adaptation** — is complete.

Current next steps:

* Phase 11 — print/plot framing strategy
* Phase 12 — optional headless execution mode
* Phase 13 — optional workflow / PLM-style request injection
* Phase 14 — future real AutoCAD host integration

---

## License

MIT License

See `LICENSE`.
