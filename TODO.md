
## Proposed next phases


## Recommended phase order now

Use this order:

**Phase 9 — interactive desktop AutoCadMock**
Build a simple UI host for request entry and DXF generation.

**Phase 10 — CI/CD, Docker, and Jenkins adaptation**
Support both:

* Blazor/SVG host
* AutoCadMock desktop/mock host artifacts

**Phase 11 — print/plot framing strategy**
Define printable extents or an explicit print frame.

**Phase 12 — optional workflow-style input source**
Add JSON/file-driven or later PLM-style request injection.



 ## Phase 9 — desktop interactive `AutoCadMock`

A small executable host with form input, roughly mirroring the operator experience of a CAD-side tool.

### Responsibilities

* user edits dial values in the host UI
* host builds `DialCadRequest`
* host calls `DialAutoCADPlugin`
* plugin returns CAD output / writes DXF through exporter
* host shows status and output path
 

 

### Phase 10 — print/plot framing strategy

Decide how printable area should be controlled for CAD output.

Scope:

* extents-only workflow first
* optional explicit print frame/page box later
* document whether plotting should use:

  * `Extents`
  * `Window`
  * or a future layout/frame concept

This matters because AutoCAD-style plotting is based on plot area settings like `Extents`, `Window`, and `Layout`, not just raw geometry. ([Autodesk Help][2])

### Phase 11 — CI/CD and packaging update

Adapt Jenkins, scripts, and Docker so both hosts are buildable and testable.

Scope:

* build `DialMock`
* build `AutoCadMock`
* run tests for all projects
* publish Blazor app
* publish AutoCadMock artifacts
* archive generated DXF output as build artifact in CI if useful

This should come after Phases 8–10 so automation reflects a stable workflow, not a moving target.

### Phase 12 — documentation and architecture hardening

Update README, docs, and dev guide for the new CAD path.

Scope:

* interaction model
* DXF export responsibility
* print strategy
* host vs plugin responsibilities
* how to run SVG path and CAD path locally

---

## How the interactivity should be managed

For **this version**, do **not** try to simulate full PLM-driven enterprise behavior yet. That would add too much complexity too early.

The right staged approach is:

### Version now

`AutoCadMock` acts like a simplified CAD host that:

* receives or asks for dial parameters
* calls the plugin DLL
* writes DXF
* optionally prints a summary

That is enough to test the integration path realistically.

### Later version

You can add an “industry-like” orchestration model where:

* a PLM or workflow system provides context
* a selected item/version drives the dial parameters
* the CAD host opens the right object and invokes the plugin

That matches real PLM/CAD integration better. Siemens materials on Teamcenter’s CAD integrations describe CAD users accessing managed design information through the integrated CAD environment, while Teamcenter also acts as a backbone for workflows and shared product data. ([plm.automation.siemens.com][3])

---

## Industry-like interaction: who provides the data?

In real industry setups, there are usually two broad patterns.

### Pattern A — CAD user selects something inside the CAD-integrated environment

Typical flow:

* user opens/selects a managed item/drawing/configuration
* plugin/adapter reads the relevant attributes
* CAD entities are generated in the CAD session

This is common in CAD integrations, where the user works inside the CAD application but against PLM-managed data. Siemens’ Teamcenter integration material for AutoCAD emphasizes searching, accessing, controlling, and managing design information from within the AutoCAD environment. ([media.plm.automation.siemens.com][4])

### Pattern B — PLM/workflow starts a controlled process

Typical flow:

* PLM workflow identifies the target item/revision/configuration
* data is prepared centrally
* CAD or an export service is invoked
* output is generated and stored back into the managed process

This is also realistic, especially for controlled release/manufacturing flows. Teamcenter positions itself as a backbone for shared information and workflows, which fits this orchestration model. ([Siemens Blog Network][5])

---

## What is the right approximation for your project now?

For this version, the best approximation is:

### AutoCadMock should behave like a CAD host with local user-driven input

Meaning:

* it prompts the user or accepts CLI args
* it builds a `DialCadRequest`
* it calls the plugin
* it produces DXF

That is simpler than PLM orchestration, but still structurally faithful:

* host supplies request context
* plugin does generation
* output is produced in a reusable form

This is the right learning step before introducing PLM-style workflow complexity.

---
 