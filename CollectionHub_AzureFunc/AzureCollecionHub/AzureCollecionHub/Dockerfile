# -------------------------
# Build stage
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Projektdateien zuerst kopieren, damit dotnet restore gecacht werden kann
COPY CollectionHub_AzureFunc/AzureCollecionHub/AzureCollecionHub/CollectionHub.Functions.csproj \
     CollectionHub_AzureFunc/AzureCollecionHub/AzureCollecionHub/

COPY CollectionHub_Shared/CollectionHub.Shared/CollectionHub.Shared/CollectionHub.Shared.csproj \
     CollectionHub_Shared/CollectionHub.Shared/CollectionHub.Shared/

RUN dotnet restore \
    CollectionHub_AzureFunc/AzureCollecionHub/AzureCollecionHub/CollectionHub.Functions.csproj

# Danach erst den eigentlichen Quellcode kopieren
COPY CollectionHub_AzureFunc/AzureCollecionHub/AzureCollecionHub/ \
     CollectionHub_AzureFunc/AzureCollecionHub/AzureCollecionHub/

COPY CollectionHub_Shared/CollectionHub.Shared/CollectionHub.Shared/ \
     CollectionHub_Shared/CollectionHub.Shared/CollectionHub.Shared/

RUN dotnet publish \
    CollectionHub_AzureFunc/AzureCollecionHub/AzureCollecionHub/CollectionHub.Functions.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore


# -------------------------
# Runtime stage
# -------------------------
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS runtime

COPY --from=build /app/publish /home/site/wwwroot
