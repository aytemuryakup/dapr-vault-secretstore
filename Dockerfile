FROM mcr.io/dotnet/aspnet:6.0.9-alpine3.16 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.io/dotnet/sdk:6.0.401-bullseye-slim AS build
WORKDIR /src
COPY . .
RUN dotnet restore --no-cache "./DaprSecretLab.csproj"

RUN dotnet build "./DaprSecretLab.csproj" -c Release --no-restore -o /app/build

FROM build AS publish
RUN dotnet publish "./DaprSecretLab.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DaprSecretLab.dll"]
