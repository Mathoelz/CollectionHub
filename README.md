# CollectionHub
A cloud-native ASP.NET Core application for managing gaming backlogs and anime collections, built with Azure.

[![CollectionHub CI](https://github.com/Mathoelz/CollectionHub/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Mathoelz/CollectionHub/actions/workflows/ci.yml)
[![CollectionHub CD](https://github.com/Mathoelz/CollectionHub/actions/workflows/deploy.yml/badge.svg?branch=main)](https://github.com/Mathoelz/CollectionHub/actions/workflows/deploy.yml)

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

## CI/CD

CollectionHub uses GitHub Actions for continuous integration and
continuous deployment.

### Continuous Integration

The CI workflow runs automatically for pushes to `main` and for
pull requests targeting `main`.

It performs the following checks:

- restores and builds the Blazor web application in Release mode
- restores and builds the .NET isolated Azure Functions
- builds the Blazor Docker image
- builds the Azure Functions Docker image
- prevents deployment when a build or container validation fails

The Docker images are built to verify that the application remains
reproducibly containerized. They are not pushed to a container
registry because the Azure production environment currently uses
code-based deployments.

### Continuous Deployment

After a successful CI workflow on the `main` branch, the CD workflow:

1. checks out the exact commit previously validated by CI
2. authenticates to Azure using GitHub OpenID Connect
3. publishes and deploys the Azure Functions backend
4. publishes and deploys the Blazor frontend to Azure App Service

Pull requests are validated by CI but are never deployed.

The CD workflow can also be started manually through GitHub Actions.

### Deployment Security

GitHub does not store an Azure password, client secret or publish
profile.

A user-assigned managed identity and federated GitHub credential are
used to obtain short-lived Azure tokens through OpenID Connect.

The deployment identity follows the principle of least privilege and
has `Website Contributor` access only to the CollectionHub App Service
and Azure Function App.