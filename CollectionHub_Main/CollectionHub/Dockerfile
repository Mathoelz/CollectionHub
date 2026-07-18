# Build-Phase: enthält das vollständige .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Zuerst nur die Projektdateien kopieren.
# Dadurch kann Docker den NuGet-Restore später cachen.
COPY CollectionHub_Main/CollectionHub/CollectionHub.csproj \
     CollectionHub_Main/CollectionHub/

COPY CollectionHub_Shared/CollectionHub.Shared/CollectionHub.Shared/CollectionHub.Shared.csproj \
     CollectionHub_Shared/CollectionHub.Shared/CollectionHub.Shared/

RUN dotnet restore CollectionHub_Main/CollectionHub/CollectionHub.csproj

# Danach den restlichen Quellcode kopieren
COPY CollectionHub_Main/CollectionHub/ \
     CollectionHub_Main/CollectionHub/

COPY CollectionHub_Shared/CollectionHub.Shared/CollectionHub.Shared/ \
     CollectionHub_Shared/CollectionHub.Shared/CollectionHub.Shared/

RUN dotnet publish CollectionHub_Main/CollectionHub/CollectionHub.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore


# Runtime-Phase: enthält nur die ASP.NET-Core-Laufzeit
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CollectionHub.dll"]

