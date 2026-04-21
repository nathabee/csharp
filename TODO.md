
# TODO

Revised Future Phases  
 

# Phase 13 — Backend Foundation (API + Persistence)

**Goal:**
Introduce a real backend layer so the UI stops working purely in-memory.

**This is the true start of “full stack DialMock.”**

## New projects

```text
DialMock.Api
DialMock.Persistence
DialMock.Application
```

## Technologies introduced

* ASP.NET Core Minimal API
* EF Core
* SQLite (first database)
* Dependency Injection
* DTO mapping

## What gets implemented

### 1. Minimal API

Endpoints such as:

```text
POST   /api/dials/validate
POST   /api/dials/render
POST   /api/dials/export
GET    /api/templates
POST   /api/templates
```

The UI stops calling Core directly and instead calls API routes.

---

### 2. EF Core persistence

Create database model:

```text
DialTemplate
DialTemplateRevision
PrintProfile
ExportJob
ArtifactRecord
```

Add:

```text
DialMockDbContext
Initial migration
SQLite database
```

---

### 3. Application services layer

This becomes your orchestration layer.

Example:

```text
DialValidationService
DialLayoutService
CadExportService
TemplateService
```

Important:

These call **DialMock.Core**, not the reverse.

---

## Why Phase 13 matters

Without this step:

You don't have:

* stored templates
* job history
* reusable configurations
* real system state

With this:

DialMock becomes a **real application**, not only a demo tool.

---

# Phase 14 — Job Pipeline and Worker Service

**Goal:**
Introduce background processing — the heart of industrial workflows.

This phase turns DialMock into a **production-style processing engine**.

---

## New project

```text
DialMock.Worker
```

## Technologies introduced

* Worker Service
* BackgroundService
* Job queue model
* Hosted services

---

## What the Worker does

### Core concept

Jobs are stored in DB.

Worker executes them.

Not the UI.

---

## Implement job model

```text
ExportJob
ExportJobItem
JobStatus
ValidationReport
```

Typical lifecycle:

```text
Queued
Validating
Rendering
Exporting
Completed
Failed
```

---

## Worker pipeline

Typical steps:

```text
1 Validate DialSpec
2 Generate layout
3 Build CAD model
4 Export DXF
5 Store artifact
6 Update status
```

This simulates:

**real inline dial production workflows**

---

## Optional sub-features

* batch generation
* folder ingestion
* scheduled jobs
* template reprocessing

All realistic industrial scenarios.

---

# Phase 15 — Real-Time Monitoring (SignalR + UI Job Console)

**Goal:**
Make background activity visible to users.

This creates a **live engineering console feel.**

---

## Technologies introduced

* SignalR
* WebSockets
* Live UI updates

---

## UI additions

New pages:

```text
/jobs
/jobs/{id}
```

Live display:

```text
Queued
Processing
Exporting
Completed
Failed
```

Without refresh.

---

## Why this matters

Industrial systems always show:

* queues
* job state
* warnings
* failures

Otherwise it's not believable as production tooling.

---

# Phase 16 — Print/Plot Framing Strategy (your original idea — now placed correctly)

**Goal:**
Formalize physical print-space logic.

This is where your earlier Phase 13 truly belongs.

---

## Introduce

```text
PrintFrame
PrintArea
MarginRules
AlignmentRules
BleedZone
SafeZone
```

These simulate:

Real dial printing constraints.

---

## New logic

```text
Compute printable region
Check text overflow
Detect clipping
Align dial center
Apply margins
```

---

## Export additions

Support:

```text
DXF framing
SVG framing
PDF layout export
```

---

# Phase 17 — Industrial Workflow Simulation (PLM-style logic)

**Goal:**
Move from tool to workflow system.

This is where your original **PLM-style workflow** becomes meaningful.

---

## Introduce workflow states

```text
Draft
Validated
Approved
Released
Archived
```

---

## Add entities

```text
ApprovalRecord
User
Role
Project
ChangeRequest
```

---

## Example flow

```text
Engineer creates template
↓
Validation step
↓
Supervisor approval
↓
Release for production
↓
Export package generated
```

Now you're simulating:

**real industrial lifecycle behavior**

---

# Phase 18 — Observability and Diagnostics

**Goal:**
Make system measurable and traceable.

Essential for production credibility.

---

## Technologies introduced

* OpenTelemetry
* Structured logging
* Metrics

---

## Track things like

```text
Render duration
Export duration
Validation failures
Queue latency
Artifact size
```

---

## Why this matters

Real engineering systems measure performance.

Always.

---

# Phase 19 — CAD Integration Hardening

**Goal:**
Strengthen the CAD abstraction.

Still inside your mock world.

---

## Expand CAD-neutral model

Add:

```text
Layer
LineWeight
TextStyle
ColorProfile
Block
Viewport
```

---

## Add validation rules

Examples:

```text
Layer usage rules
Font availability checks
Geometry overlap checks
Print safety validation
```

---

This phase prepares:

Future real AutoCAD integration.

---

# Phase 20 — External Integration Simulation

**Goal:**
Simulate enterprise integration.

Very strong portfolio impact.

---

## Add import/export connectors

Example:

```text
JSON spec ingestion
CSV customer import
ERP export simulation
PLM export package
```

---

## Example scenario

```text
External system sends DialSpec
↓
API receives
↓
Worker processes
↓
Artifacts generated
↓
Status returned
```

That mirrors real industrial integration patterns.

---
  