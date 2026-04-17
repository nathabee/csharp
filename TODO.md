
## Proposed next phases

### Phase 8 — extended CAD-path test coverage

Add tests for the CAD/plugin branch before it grows further.

Scope:

* `DialAutoCADPlugin` builder tests
* mapper tests
* DXF exporter tests
* regression tests for normalized arc storage
* tests for layer assignment and entity counts
* tests for invalid request handling

Why first:

* you already changed the arc convention once
* DXF export now exists
* this is the point where regressions start becoming expensive

### Phase 9 — interactive AutoCadMock

Turn `AutoCadMock` from a fixed console run into an interactive mock host.

Scope:

* command-line options first
* optional prompt-based interactive mode second
* editable inputs for:

  * title
  * unit
  * min/max
  * preview value
  * major tick count
* selectable output path
* selectable sample preset

Why next:

* this gives you a practical test harness
* it stays close to a host-driven CAD workflow
* it avoids premature UI complexity

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

## My recommended Phase 8+ order

Use this order:

```text
Phase 8 — extended CAD-path test coverage
Phase 9 — interactive AutoCadMock
Phase 10 — print/plot framing strategy
Phase 11 — CI/CD and Docker/Jenkins adaptation
Phase 12 — documentation and architecture hardening
Phase 13 — optional PLM-style workflow simulation
```

That order is coherent because:

* correctness first
* usability second
* printing behavior third
* automation fourth
* enterprise-style orchestration later

---

## What Phase 9 interactivity should look like concretely

Start small:

### Step 1

Add CLI flags such as:

```text
--title
--unit
--min
--max
--preview
--ticks
--out
```

### Step 2

If no flags are provided, fall back to interactive prompts.

### Step 3

Add presets, for example:

```text
--sample default
--sample pressure100
```

### Step 4

Later, add simple JSON input:

```text
--input dial-request.json
```

That is the cleanest path because it mimics machine-driven invocation without requiring a real PLM system yet.

---
 