

## Architecture our project

DialAutoCADPlugin is the reusable CAD/plugin integration layer. It consumes the shared dial logic from DialMock.Core and exposes the CAD-oriented behavior that a host can call.

AutoCadMock is a demo/test host that exercises the same plugin layer outside the real AutoCAD environment.

This allows us to validate plugin behavior without needing AutoCAD for every test, while keeping the actual integration logic reusable for a real AutoCAD host.



> I am already working with C#/.NET assemblies, separated core logic, application hosting, build and deployment. For an AutoCAD plugin, the hosting model differs because the DLL is loaded by AutoCAD rather than started as a web app, but the .NET/C# architecture skills transfer.

> For an AutoCAD integration, I understand that the architecture would be different: the DLL would typically be a plugin class library loaded by AutoCAD rather than a standalone web application. The reusable business logic can still be separated into a core library, while the AutoCAD-specific project would handle commands, integration, and interaction with the AutoCAD API.

> For our current project, `DialMock.dll` is not a generic Windows DLL but the compiled ASP.NET Core application assembly of a Blazor web app. It is started with `dotnet DialMock.dll`, which launches the web host and serves both the server-side logic and the UI.
>
> In an AutoCAD scenario, the DLL would usually be a class library plugin, not a standalone web application. AutoCAD would load that assembly and call into it through its plugin API. The code would still be C#/.NET, but the hosting model, references, project type, and runtime integration would be different.


##### 
## Phase 9 — desktop interactive `AutoCadMock`

A small executable host with form input, roughly mirroring the operator experience of a CAD-side tool.

### Responsibilities

* user edits dial values in the host UI
* host builds `DialCadRequest`
* host calls `DialAutoCADPlugin`
* plugin returns CAD output / writes DXF through exporter
* host shows status and output path
 

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

That order is cleaner than doing CLI-first now.

## One important practical note

Because you said this runs in Docker/Jenkins too:

* the **desktop interactive host** is for local/manual testing
* the **non-interactive mode** can still exist later for CI

So the final host may support both:

* **interactive desktop mode** for human testing
* **headless file/request mode** for Jenkins/container runs
 
**For version 1.1.x, `AutoCadMock` will be a lightweight desktop host that collects user input, builds a `DialCadRequest`, calls the DLL, and writes DXF. CI/headless execution will be added later as a separate mode.**

That keeps:

* architecture clean
* host/DLL split realistic
* future PLM integration possible
* current development understandable

The next step should be to redesign Phase 9 around a **small desktop UI host**, not around CLI parsing.

[1]: https://www.autodesk.com/support/technical/article/caas/tsarticles/ts/EnnoS3yadnVRrxfh2zWXQ.html?utm_source=chatgpt.com "Lesson 1 : The Basic AutoCAD Plug-in"
[2]: https://forums.autodesk.com/t5/net-forum/connecting-to-an-autocad-instance-from-an-external-exe-including/td-p/6546992?utm_source=chatgpt.com "Connecting to an AutoCAD instance from an external EXE ..."
