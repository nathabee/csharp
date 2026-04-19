# ==========================================
# Base SDK image (restore + build)
# ==========================================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution and project files first for better restore caching
COPY DialMock.slnx ./
COPY DialMock/DialMock.csproj DialMock/
COPY DialMock.Core/DialMock.Core.csproj DialMock.Core/
COPY DialMock.CadModel/DialMock.CadModel.csproj DialMock.CadModel/
COPY DialAutoCADPlugin/DialAutoCADPlugin.csproj DialAutoCADPlugin/
COPY AutoCadMock/AutoCadMock.csproj AutoCadMock/
COPY DialMock.Tests/DialMock.Tests.csproj DialMock.Tests/

RUN dotnet restore DialMock.slnx

# Copy full source after restore
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
# Publish AutoCadMock (desktop, Linux x64)
# Artifact-only export target for buildx local output
# ==========================================

FROM build AS publish-autocadmock-build

WORKDIR /src

RUN dotnet publish AutoCadMock/AutoCadMock.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    -o /artifacts/AutoCadMock-desktop

FROM scratch AS publish-autocadmock

COPY --from=publish-autocadmock-build /artifacts/ /artifacts/

# ==========================================
# Publish AutoCadMock (desktop, Windows x64)
# Artifact-only export target for buildx local output
# ==========================================

FROM build AS publish-autocadmock-win-build

WORKDIR /src

RUN dotnet publish AutoCadMock/AutoCadMock.csproj \
    -c Release \
    -r win-x64 \
    --self-contained true \
    -o /artifacts/AutoCadMock-desktop-win-x64

FROM scratch AS publish-autocadmock-win

COPY --from=publish-autocadmock-win-build /artifacts/ /artifacts/

# ==========================================
# Runtime image (DialMock only)
# ==========================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=publish-dialmock /artifacts/DialMock-web/ ./

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "DialMock.dll"]