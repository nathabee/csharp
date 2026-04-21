

---

```text
to be done :    

- Phase 13 : Backend Foundation (API + Persistence)
- Phase 14 : Job Pipeline and Worker Service
- Phase 15 : Real-Time Monitoring (SignalR + UI Job Console)
- Phase 16 : Print/Plot Framing Strategy (your original idea — now placed correctly)
- Phase 17 : Industrial Workflow Simulation (PLM-style logic)
- Phase 18 : Observability and Diagnostics
- Phase 19 : CAD Integration Hardening
- Phase 20 : External Integration Simulation

```


---
## 1.2.x change AutocadMock into an interactive Desktop App
---

1.2.2 Phase 12 : CI/CD - Windows desktop packaging
1.2.1 Phase 11 : CI/CD- GitHub Release automation
1.2.1 Phase 10 : CI/CD and Docker/Jenkins adaptation
1.2.0 Phase 9 : conversion AutocadMock from console host to interactive desktop app using Avalonia
---
## 1.1.x Add AutoCadMock (console host), DialAutoCADPlugin (DLL producing the DXF) using the same logic and data defined in the Core and modelCAD
---
1.1.8 Phase 8 : extended CAD-path test coverage
1.1.7 Phase 7 : external CAD validation
1.1.6 Phase 6 : introduce DXF export service
1.1.5 Phase 5 : CAD mapping refinement
1.1.4 Phase 4 : tighten plugin boundary for external-host usage
1.1.3 Phase 3 : `AutoCadMock` console host calling the plugin and producing a CAD summary
1.1.2 phase 2 DialAutoCADPlugin creation
1.1.1 Phase 1 : add DialMock.CadModel
1.1.0 Release before starting DialAutoCADPlugin and AutoCadMock developpement
---
## 1.0.x   Blazor UI DialMock , web app creating Dials and display them using SVG
---
1.0.6 Fix Blazor static asset resolution causing missing blazor.web.js in production
1.0.5 Add Jenkins and scripts/ci_* to handle Ci/CD and Docker build
1.0.4 Phase 4B — Add tests and a non tested Jenkins file
1.0.3 Phase 4A — Clean SVG adapter - Move SVG-specific conversion out of Razor
1.0.2 Core is the source of dial description, independent of rendering technology (ie Autodcad, SVG)
1.0.1 Refactor to put the logic display object position in the Core DialGeometryCalculator 
1.0.0  initialise the core and added Dockerfile for the Dialmock UI   