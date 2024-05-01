FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./

# Restore as distinct layers
RUN dotnet restore

# Build and publish a release
RUN dotnet publish "./src/RinhaBackend.2024.Q1.Api/RinhaBackend.2024.Q1.Api.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runtime
WORKDIR /App
COPY --from=build-env /App/out .

ENTRYPOINT ["dotnet", "RinhaBackend.2024.Q1.Api.dll"]