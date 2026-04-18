# ==========================================
# Base SDK image (build + test + publish)
# ==========================================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution and restore early for cache efficiency

COPY DialMock.slnx ./

COPY DialMock/DialMock.csproj DialMock/
COPY DialMock.Core/DialMock.Core.csproj DialMock.Core/
COPY DialMock.CadModel/DialMock.CadModel.csproj DialMock.CadModel/
COPY DialAutoCADPlugin/DialAutoCADPlugin.csproj DialAutoCADPlugin/
COPY AutoCadMock/AutoCadMock.csproj AutoCadMock/
COPY DialMock.Tests/DialMock.Tests.csproj DialMock.Tests/

RUN dotnet restore DialMock.slnx

# Copy full source

COPY . .

RUN dotnet build DialMock.slnx \
    -c Release \
    --no-restore

# ==========================================
# Test stage
# ==========================================

FROM build AS test

WORKDIR /src

RUN mkdir -p /artifacts/testresults

RUN dotnet test DialMock.slnx \
    -c Release \
    --no-build \
    --logger "trx;LogFileName=DialMock.Tests.trx" \
    --results-directory /artifacts/testresults

# ==========================================
# Publish DialMock (web)
# ==========================================

FROM build AS publish-dialmock

WORKDIR /src

RUN dotnet publish DialMock/DialMock.csproj \
    -c Release \
    --no-build \
    -o /artifacts/DialMock-web

# ==========================================
# Publish AutoCadMock (desktop)
# Framework-dependent for now
# ==========================================

FROM build AS publish-autocadmock

WORKDIR /src

RUN dotnet publish AutoCadMock/AutoCadMock.csproj \
    -c Release \
    --no-build \
    -o /artifacts/AutoCadMock-desktop

# ==========================================
# Runtime image (DialMock only)
# ==========================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=publish-dialmock \
    /artifacts/DialMock-web \
    .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "DialMock.dll"]