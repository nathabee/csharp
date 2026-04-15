# TODO list



Actuell architecture is in the docs/architecture.md, this file just list what we still need to do.
 

#  roadmap of the Next Phases

## Phase 4 — Extract SVG Renderer

Move SVG conversion logic from Razor into: SvgDialRenderer.cs

Result:
* cleaner UI
* reusable SVG output
* consistent rendering model



## Phase 4A — Clean SVG adapter
done

## Phase 4B — Add tests

Create `DialMock.Tests` and test:

* tick count
* needle line count
* arc existence
* known value to expected geometry positions
* validation behavior

This is mandatory before industrial reuse.

## Phase 4C — Improve Core contract

Decide whether:

* `DialRenderData` remains
* or title/unit move into `DialDrawing`
* or a new `DialDocument` wraps both metadata and geometry

I would likely move toward:

```csharp
DialDocument
  - Title
  - Unit
  - Drawing
```

But not yet unless needed.

## Phase 4D — Create AutoCAD skeleton

Only after SVG adapter + tests are in place:

* create `DialMock.AutoCAD`
* target proper framework for AutoCAD compatibility
* create command method
* call `DialEngine.BuildDrawing(spec)`
* map primitives to AutoCAD entities

---

 

---

## Phase 5 — Add Unit Tests

Create:

```text
DialMock.Tests
```

Test:

* tick count
* needle position
* arc correctness
* value mapping
* validation rules

Required before CAD integration.

---

## Phase 6 — AutoCAD Adapter

Create:

```bash
dotnet new classlib -n DialMock.AutoCAD
```

Implement:

```csharp
[CommandMethod("DIALMOCK")]
```

Use:

```csharp
DialEngine.BuildDrawing()
```

Convert to:

```text
AutoCAD entities
```

---

# Summary

You now have:

```text
Reusable Dial Engine ✔
Renderer-neutral Geometry ✔
Blazor Preview using Core ✔
```

Next:

```text
SVG Renderer extraction
Then testing
Then AutoCAD integration
```

That is the correct industrial-grade path.
