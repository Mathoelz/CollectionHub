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

## Logging and Monitoring

CollectionHub uses Azure Application Insights and a connected
Log Analytics workspace to monitor the Azure Functions backend.

The monitoring implementation includes:

- automatic HTTP request and dependency tracking
- automatic exception telemetry
- structured application logs using `ILogger`
- request, trace, dependency and exception correlation through
  Application Insights operation IDs
- telemetry sampling to control ingestion volume
- filtered framework logs to reduce unnecessary telemetry
- structured CRUD logs containing media type and media ID
- Blob Storage cover-cache hit and miss telemetry
- reusable KQL queries for:
  - endpoint health and success rates
  - request volume and response times
  - failed requests
  - exceptions and failed dependencies
  - cover-cache hit rates

Sensitive values such as access tokens, secrets, connection
strings and complete request payloads are not logged.

External APIs:
- IGDB
- AniList
