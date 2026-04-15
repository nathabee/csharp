# Dvelopper guide


`DialMock` = a small Blazor app with:

* one interactive page
* one `DialSpec` model
* one `DialRuleEngine`
* one SVG preview
* one validation panel

That is enough to be credible and still finishable.


 


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

## Clone the repository

Choose a working directory and clone the repository:

```bash
cd ~/coding/github
git clone git@github.com:nathabee/csharp.git
cd csharp
```

If HTTPS is preferred:

```bash
git clone https://github.com/nathabee/csharp.git
cd csharp
```

---

## Repository layout

Example structure:

```text
csharp/
├── DialMock/
├── TestMock/
├── docs/
└── README.md
```

---

## Build and run DialMock in development

Go to the project folder:

```bash
cd ~/coding/github/csharp/DialMock
```

Restore and build:

```bash
dotnet restore
dotnet build
```

Run in development mode with hot reload:

```bash
dotnet watch
```

Open the URL shown in the terminal, for example:

```text
http://localhost:5185
```

### Development notes

* `dotnet watch` is the normal development workflow.
* It recompiles automatically when files change.
* This mode is intended for local testing and implementation work.

---

## Publish DialMock for release

Publish outside the project directory to avoid recursive publish issues:

```bash
cd ~/coding/github/csharp/DialMock
rm -rf ../publish
dotnet publish -c Release -o ../publish
```

This creates a release output directory here:

```text
~/coding/github/csharp/publish
```

---

## Run the published application

Start the published build:

```bash
cd ~/coding/github/csharp/publish
dotnet DialMock.dll
```

The application usually starts on:

```text
http://localhost:5000
```

---

## Stop the application

In the terminal where the app is running:

```text
Ctrl+C
```

---

## Updating from GitHub

To update an existing clone:

```bash
cd ~/coding/github/csharp
git pull
```

Then rebuild or republish as needed.

### Rebuild for development

```bash
cd DialMock
dotnet build
dotnet watch
```

### Republish release output

```bash
cd DialMock
rm -rf ../publish
dotnet publish -c Release -o ../publish
```

---

## Common issues

### Publish fails because old publish output is detected

Cause:
the publish directory was created inside the project tree or old generated assets remain.

Fix:

```bash
rm -rf ../publish
dotnet publish -c Release -o ../publish
```

Do not publish to `./publish` inside the project folder for this web project.

### HTTPS redirection warning in development

Example warning:

```text
Failed to determine the https port for redirect
```

This usually means the app is running on HTTP only in the current local setup.

For local mock or prototype work, this is not necessarily blocking.
If needed later, configure HTTPS properly in the launch profile or hosting setup.

### UI changes are not visible

Try:

```bash
dotnet clean
dotnet build
dotnet watch
```

If necessary, refresh the browser fully.

---

## Git hygiene

Do not commit generated folders such as:

* `bin/`
* `obj/`
* `publish/`

These must remain ignored by `.gitignore`.

---
