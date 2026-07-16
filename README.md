# CollectionHub
A cloud-native ASP.NET Core application for managing gaming backlogs and anime collections, built with Azure.

## Goals

- Manage games
- Manage anime
- Learn Azure
- Learn Blazor
- Build a production-like application

## Planned Azure Services

- Azure App Service
- Azure SQL
- Azure Blob Storage
- Azure Key Vault
- Application Insights

- ## Azure Architecture

Frontend:
- Azure App Service (Linux)
- Blazor Server

Backend:
- Azure Functions (.NET Isolated)
- HTTP Triggers

Storage:
- Cosmos DB
- Blob Storage

Security:
- Azure Key Vault
- Managed Identity
- RBAC

External APIs:
- IGDB
- AniList
