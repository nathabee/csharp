


## Ubuntu install

On supported Ubuntu versions, Microsoft’s install page shows installing the SDK with `apt`, and on Ubuntu 22.04+ the Ubuntu feeds and the Ubuntu .NET backports repository are the supported package sources. If `dotnet-sdk-10.0` is not found directly, add the backports PPA and retry. ([Microsoft Learn][3])

Run this first:

```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0
dotnet --version
```

If the package is not found:

```bash
sudo add-apt-repository ppa:dotnet/backports
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0
dotnet --version
```

For development, install the **SDK**, not only the runtime. Microsoft explicitly distinguishes SDK for development and ASP.NET Core Runtime for running apps. ([Microsoft Learn][3])

## Create a new project  (to create your own project, not using existing code)

Microsoft’s ASP.NET Core getting-started flow uses the .NET CLI to create and run a Blazor Web App. ([Microsoft Learn][2])

```bash
mkdir -p ~/coding/github/csharp
cd ~/coding/github/csharp

# first install and start code from null
dotnet new blazor -n DialMock
 
cd csharp/DialMock

dotnet run


```

## add some files


after this step we get a UI mock displayig 


## extract core to separate AutoCad extraction and SVG mok UI

Refactor DialMock for AutoCAD reuse
Inventory current repository files and identify reusable core components.
Extract models and validation into a new DialMock.Core class library.
Refactor geometry and angle math into a neutral DialGeometryCalculator.
Update Blazor UI to reference DialMock.Core and use neutral geometry outputs.
Create an AutoCAD adapter project that consumes DialMock.Core for rendering.


```bash


cd ~/coding/github/csharp/DialMock

dotnet new sln -n DialMock
dotnet sln add DialMock.csproj


dotnet new classlib -n DialMock.Core
dotnet sln add DialMock.Core/DialMock.Core.csproj

dotnet add DialMock.csproj reference DialMock.Core/DialMock.Core.csproj

dotnet remove DialMock.csproj reference DialMock.Core/DialMock.Core.csproj

``` 
