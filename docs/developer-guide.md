# Developer Guide

`DialMock` is a small Blazor app with:

- one interactive page
- one `DialSpec` model
- one `DialRuleEngine`
- one SVG preview
- one validation panel

This is enough to be credible and still finishable.

---

## Ubuntu install

For development, install the **.NET SDK**, not only the runtime.

Try:

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

---

## Create a new project

Use this only if you want to create a new Blazor project from scratch.

```bash
mkdir -p ~/coding/github/csharp
cd ~/coding/github/csharp

dotnet new blazor -n DialMock
cd DialMock

dotnet run
```

---

## Clone the repository

SSH:

```bash
cd ~/coding/github
git clone git@github.com:nathabee/csharp.git
cd csharp
```

HTTPS:

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
├── DialMock.Core/
├── DialMock.Tests/
├── docs/
├── scripts/
├── Dockerfile
├── Jenkinsfile
└── DialMock.slnx
```

---

## Build and run in development

Go to the Blazor project:

```bash
cd ~/coding/github/csharp/DialMock
```

Build:

```bash
dotnet restore
dotnet build
```

Run with hot reload:

```bash
dotnet watch
```

Open the URL shown in the terminal, for example:

```text
http://localhost:5185
```

### Notes

* `dotnet watch` is the normal development workflow.
* It rebuilds automatically when files change.
* This is intended for local development only.

---

## Publish for release

Publish outside the project folder:

```bash
cd ~/coding/github/csharp/DialMock
rm -rf ../publish
dotnet publish -c Release -o ../publish
```

This creates:

```text
~/coding/github/csharp/publish
```

---

## Run the published application

```bash
cd ~/coding/github/csharp/publish
dotnet DialMock.dll
```

The application usually starts on a local HTTP port, for example:

```text
http://localhost:5000
```

Stop with:

```text
Ctrl+C
```

---

## Update from GitHub

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

### Publish fails because of old output

Fix:

```bash
rm -rf ../publish
dotnet publish -c Release -o ../publish
```

Do not publish to `./publish` inside the web project folder.

### HTTPS redirection warning in development

Example:

```text
Failed to determine the https port for redirect
```

For local mock/prototype work, this is usually not blocking.

### UI changes are not visible

Try:

```bash
dotnet clean
dotnet build
dotnet watch
```

If needed, hard-refresh the browser.

---

## Git hygiene

Do not commit generated folders such as:

* `bin/`
* `obj/`
* `publish/`

These should stay ignored by `.gitignore`.

---

## Docker validation

### Build test stage and extract test results

```bash
cd ~/coding/github/csharp

docker build --target test -t dialmock:ci-test-local .
docker create --name dialmock-test-copy dialmock:ci-test-local
docker cp dialmock-test-copy:/artifacts/testresults/. ./TestResults/
docker rm -f dialmock-test-copy
```

### Build runtime image

```bash
docker build -t dialmock:local .
```

### Run locally

```bash
docker run --rm -p 8080:8080 --name dialmock-local dialmock:local
```

Open:

```text
http://localhost:8080
```

If the app loads, then you have validated:

* build inside Docker
* test inside Docker
* publish inside Docker
* runtime image works
* container starts correctly

---

## Functional tests

See:

[docs/test.md](docs/test.md)

---

## CI/CD

See:

[docs/ci-cd.md](docs/ci-cd.md)
 