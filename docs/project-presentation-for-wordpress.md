

## Architecture our project

DialAutoCADPlugin is the reusable CAD/plugin integration layer. It consumes the shared dial logic from DialMock.Core and exposes the CAD-oriented behavior that a host can call.

AutoCadMock is a demo/test host that exercises the same plugin layer outside the real AutoCAD environment.

This allows us to validate plugin behavior without needing AutoCAD for every test, while keeping the actual integration logic reusable for a real AutoCAD host.



> I am already working with C#/.NET assemblies, separated core logic, application hosting, build and deployment. For an AutoCAD plugin, the hosting model differs because the DLL is loaded by AutoCAD rather than started as a web app, but the .NET/C# architecture skills transfer.

> For an AutoCAD integration, I understand that the architecture would be different: the DLL would typically be a plugin class library loaded by AutoCAD rather than a standalone web application. The reusable business logic can still be separated into a core library, while the AutoCAD-specific project would handle commands, integration, and interaction with the AutoCAD API.

> For our current project, `DialMock.dll` is not a generic Windows DLL but the compiled ASP.NET Core application assembly of a Blazor web app. It is started with `dotnet DialMock.dll`, which launches the web host and serves both the server-side logic and the UI.
>
> In an AutoCAD scenario, the DLL would usually be a class library plugin, not a standalone web application. AutoCAD would load that assembly and call into it through its plugin API. The code would still be C#/.NET, but the hosting model, references, project type, and runtime integration would be different.


