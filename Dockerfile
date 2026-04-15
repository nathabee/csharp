# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first (better layer caching)
COPY DialMock.slnx .
COPY DialMock/DialMock.csproj DialMock/
COPY DialMock.Core/DialMock.Core.csproj DialMock.Core/

RUN dotnet restore DialMock/DialMock.csproj

# Copy everything else
COPY . .

RUN dotnet publish DialMock/DialMock.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "DialMock.dll"]