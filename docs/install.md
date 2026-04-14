# install

 



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

## Create the project  (if you want to create a new project)

Microsoft’s ASP.NET Core getting-started flow uses the .NET CLI to create and run a Blazor Web App. ([Microsoft Learn][2])

```bash
mkdir -p ~/coding/github
cd ~

# first install and start code from null
# dotnet new blazor -n DialMock

# install from github
git clone https://github.com/nathabee/csharp.git
cd csharp/DialMock

# dotnet run

```

## Clone the repository

Choose a working directory and clone the repository:

```bash
cd ~/coding/github
git clone git@github.com:nathabee/csharp.git
cd csharp
```