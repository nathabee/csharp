# syntax=docker/dockerfile:1

# ---- restore ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

COPY DialMock.slnx .
COPY DialMock/DialMock.csproj DialMock/
COPY DialMock.Core/DialMock.Core.csproj DialMock.Core/
COPY DialMock.Tests/DialMock.Tests.csproj DialMock.Tests/

RUN dotnet restore DialMock.slnx

# ---- test ----
FROM restore AS test
WORKDIR /src
COPY . .

RUN dotnet build DialMock.slnx -c Release --no-restore
RUN mkdir -p /artifacts/testresults

CMD ["dotnet", "test", "DialMock.Tests/DialMock.Tests.csproj", "-c", "Release", "--no-build", "--logger", "trx;LogFileName=DialMock.Tests.trx", "--results-directory", "/artifacts/testresults"]

# ---- publish ----
FROM restore AS publish
WORKDIR /src
COPY . .

RUN dotnet publish DialMock/DialMock.csproj \
    -c Release \
    --no-restore \
    -o /app/publish

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
ENTRYPOINT ["dotnet", "DialMock.dll"]