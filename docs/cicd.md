# CI/CD

## Overview

This project uses a **Docker-based Jenkins build model**.

Jenkins does **not** need the .NET SDK installed on the host machine.  
All restore, build, test, and publish work runs inside Docker using the **`.NET SDK 10.0` image**.

The repository now supports **two runtime families**:

- **DialMock**  
  Blazor web host, deployed as a Docker container

- **AutoCadMock**  
  Avalonia desktop host, packaged as a downloadable desktop artifact

This means the project does **not** use one single deployment model for everything.

---

## Delivery model

### DialMock

- built in Docker
- published in CI
- packaged into a runtime Docker image
- deployed by Jenkins as a running web container

### AutoCadMock

- built in Docker
- published in CI
- packaged as a Linux desktop archive
- archived in Jenkins
- optionally uploaded to a GitHub Release
- downloaded and run locally outside Docker

---

## Pipeline structure

This repository uses **four Jenkins pipelines**.

### 1. `dialmock-ci`

Purpose:

- checkout source
- verify Docker access
- build the whole solution in Docker
- run tests in Docker
- publish both hosts
- archive generated artifacts

This is the main verification pipeline.

### 2. `dialmock-deploy`

Purpose:

- checkout source
- verify Docker access
- build the DialMock runtime image
- deploy the local web container

This pipeline is **web-only**.  
It does **not** deploy the desktop application.

### 3. `dialmock-desktop-build`

Purpose:

- build the AutoCadMock publish output in Docker
- extract the desktop files
- package them as `.tar.gz`
- archive the package in Jenkins

This is an on-demand packaging pipeline.

### 4. `dialmock-desktop-release`

Purpose:

- build the AutoCadMock publish output in Docker
- package the Linux desktop archive
- archive it in Jenkins
- create or reuse a GitHub Release
- upload the archive as a release asset

This is the release/distribution pipeline.

---

## Architecture flow

```mermaid
flowchart TD
    A[Developer PC] -->|git push| B[GitHub repository]

    B --> C[dialmock-ci]
    C --> D[Docker build environment]
    D --> E[Build whole solution]
    E --> F[Run tests]
    F --> G[Publish DialMock web]
    F --> H[Publish AutoCadMock desktop]

    G --> I[Jenkins archived web artifact]
    H --> J[Jenkins archived desktop artifact]

    B --> K[dialmock-deploy]
    K --> L[Build DialMock runtime image]
    L --> M[Run / replace DialMock web container]

    B --> N[dialmock-desktop-build]
    N --> O[Package AutoCadMock desktop tar.gz]
    O --> P[Jenkins archived desktop package]

    B --> Q[dialmock-desktop-release]
    Q --> R[Package AutoCadMock desktop tar.gz]
    R --> S[GitHub Release asset]
````

---

## Docker build model

The `Dockerfile` is a **multi-stage build**.

### Stages

1. **build**
   Restores the whole solution and builds all projects

2. **test**
   Runs the test suite and writes test results

3. **publish-dialmock**
   Publishes the Blazor web host into `/artifacts/DialMock-web`

4. **publish-autocadmock**
   Publishes the Avalonia desktop host into `/artifacts/AutoCadMock-desktop`

5. **runtime**
   Produces the final ASP.NET runtime image for `DialMock` only

---

## Repository files

### Main CI/CD files

```text
Dockerfile

Jenkinsfile.ci
Jenkinsfile.deploy
Jenkinsfile.desktop-build
Jenkinsfile.desktop-release

scripts/ci-build.sh
scripts/ci-test.sh
scripts/ci-publish-dialmock.sh
scripts/ci-publish-autocadmock.sh
scripts/ci-docker-build.sh
scripts/ci-deploy.sh
scripts/ci-docker-push.sh
```

### Script responsibilities

#### `scripts/ci-build.sh`

Builds the whole solution using the Docker `build` stage.

#### `scripts/ci-test.sh`

Builds the Docker `test` stage, runs tests, and extracts test results.

#### `scripts/ci-publish-dialmock.sh`

Builds the Docker `publish-dialmock` stage and extracts the web publish output.

#### `scripts/ci-publish-autocadmock.sh`

Builds the Docker `publish-autocadmock` stage and extracts the desktop publish output.

#### `scripts/ci-docker-build.sh`

Builds the final runtime image for `DialMock` only.

#### `scripts/ci-deploy.sh`

Stops the old DialMock container and starts the new one.

#### `scripts/ci-docker-push.sh`

Optional registry push helper for future use.

---

## Jenkins job creation

All Jenkins jobs should be created as **Pipeline** jobs using **Pipeline script from SCM**.

Repository URL:

```text
https://github.com/nathabee/csharp.git
```

Branch:

```text
*/main
```

### Job 1 — `dialmock-ci`

Recommended name:

```text
dialmock-ci
```

Script path:

```text
Jenkinsfile.ci
```

Purpose:

* build
* test
* publish web artifact
* publish desktop artifact
* archive artifacts

### Job 2 — `dialmock-deploy`

Recommended name:

```text
dialmock-deploy
```

Script path:

```text
Jenkinsfile.deploy
```

Purpose:

* build DialMock runtime image
* deploy the web container

### Job 3 — `dialmock-desktop-build`

Recommended name:

```text
dialmock-desktop-build
```

Script path:

```text
Jenkinsfile.desktop-build
```

Purpose:

* package Linux desktop build
* archive `.tar.gz` in Jenkins

### Job 4 — `dialmock-desktop-release`

Recommended name:

```text
dialmock-desktop-release
```

Script path:

```text
Jenkinsfile.desktop-release
```

Purpose:

* package Linux desktop build
* archive `.tar.gz`
* publish artifact to GitHub Release

---

## Jenkins job configuration notes

### Recommended general options

For all pipeline jobs:

* use **Pipeline script from SCM**
* point to the `main` branch
* keep workspace cleanup enabled through the Jenkinsfile
* no local .NET SDK is required on the Jenkins host

### Trigger model

#### `dialmock-ci`

Can be triggered by:

* GitHub webhook
* manual run

#### `dialmock-deploy`

Usually triggered manually, or from a controlled deploy flow.

#### `dialmock-desktop-build`

Usually manual / on demand.

#### `dialmock-desktop-release`

Should be manual, because publishing a release is a deliberate action.

---

## Jenkins credentials

### GitHub release token

The release pipeline requires a GitHub token so Jenkins can create releases and upload assets.

In Jenkins:

1. Open **Manage Jenkins**
2. Open **Credentials**
3. Choose the appropriate store, usually **(global)**
4. Click **Add Credentials**
5. Kind: **Secret text**
6. Secret: paste the GitHub token
7. ID: use

```text
github-token
```

8. Description: for example

```text
GitHub token for desktop release upload
```

This ID must match the default expected by `Jenkinsfile.desktop-release`.

---

## Creating the GitHub token

Create the token in GitHub from the account that owns or can manage the repository.

### Fine-grained token approach

Recommended if available.

Repository access:

* select repository: `nathabee/csharp`

Repository permissions:

* **Contents** → Read and write
  needed for release creation and release asset upload

### Classic token approach

If you use a classic personal access token, it typically needs:

* `repo`

That is broader than ideal, but it works for private repository release publishing.

---

## Using the release pipeline

### Required Jenkins parameter

`dialmock-desktop-release` requires at least:

```text
RELEASE_TAG
```

Example:

```text
v1.2.1
```

### Other useful parameters

#### `RELEASE_NAME`

Optional human-readable title.

Example:

```text
AutoCadMock Linux v1.2.1
```

#### `TARGET_COMMITISH`

Usually:

```text
main
```

#### `GITHUB_REPOSITORY`

Default:

```text
nathabee/csharp
```

#### `GITHUB_TOKEN_CREDENTIALS_ID`

Default:

```text
github-token
```

#### `DRAFT`

Set to `true` if you want to inspect the release before publishing it.

#### `PRERELEASE`

Set to `true` for preview/test builds.

---

## Produced artifacts

### CI pipeline artifacts

`dialmock-ci` archives:

```text
artifacts/testresults/**
artifacts/DialMock-web/**
artifacts/AutoCadMock-desktop-linux-x64/**
```

### Desktop build pipeline artifacts

`dialmock-desktop-build` archives:

```text
artifacts/AutoCadMock-desktop-linux-x64.tar.gz
```

### Desktop release pipeline artifacts

`dialmock-desktop-release` archives:

```text
artifacts/AutoCadMock-desktop-linux-x64.tar.gz
```

and uploads the same package to the configured GitHub Release.

---

## Accessing Jenkins artifacts

After a successful build:

1. Open the Jenkins job
2. Open the build number
3. Open **Artifacts** / **Archived Artifacts**
4. Download the desired file

For example, from `dialmock-desktop-build`:

```text
artifacts/AutoCadMock-desktop-linux-x64.tar.gz
```

---

## Release model

At the moment:

* Jenkins build artifacts are available immediately from Jenkins
* GitHub Releases are created only by the dedicated release pipeline

This means:

* **build pipeline** = packaging and archive
* **release pipeline** = packaging plus GitHub publication

---

## Platform scope

### Current desktop release target

Current packaged target:

```text
linux-x64
```

Framework-dependent publish is used at this stage.

### Planned later

Later the release pipeline can be extended to also publish:

* `win-x64`
* self-contained desktop builds
* multiple assets in one GitHub Release

---

## Runtime separation

### DialMock runtime

* Docker container
* served over HTTP
* deployed by `dialmock-deploy`

### AutoCadMock runtime

* desktop executable package
* downloaded from Jenkins artifact or GitHub Release
* run locally by the user

This separation is intentional and is part of the architecture.

---

## Prerequisites on the Jenkins host

The Jenkins machine must have:

* Jenkins
* Git
* Docker

The Jenkins user must be able to access Docker.

Example:

```bash
sudo usermod -aG docker jenkins
sudo systemctl restart jenkins
```

Then verify:

```bash
id jenkins
getent group docker
ls -l /var/run/docker.sock
```

You want to see:

* `jenkins` belongs to the `docker` group
* `/var/run/docker.sock` is group-owned by `docker`

Typical good result:

```text
srw-rw---- 1 root docker ... /var/run/docker.sock
```

---

## Notes

### `.gitignore`

Generated output should stay ignored, for example:

```text
artifacts/
TestResults/
```

These files are archived by Jenkins, not committed to Git.

### Build cache behavior

Docker build caching improves repeated pipeline speed, especially for:

* restore
* unchanged projects
* repeated desktop packaging

### Current status

The project now supports:

* whole-solution CI in Docker
* web deployment in Docker
* desktop artifact packaging
* GitHub desktop release publishing
 