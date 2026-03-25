# PlantService

## Overview

The **PlantService** is a RESTful API microservice within the Forget Me Not (FMN) application ecosystem that manages the collectible plant gacha system. This service provides gamification features through virtual plants that users can collect, grow, and display in their greenhouse.

### Key Features

- Plant catalog with 14 unique plants across 5 rarity tiers
- Gacha system with weighted random drops
- Plant growth through 5 stages (Seedling to Overgrown)
- Care system (water, sunlight, nutrients)
- Plant selling with value based on stage, rarity, and care
- Greenhouse collection management
- User-specific plant data with authentication
- RESTful API with OAuth2 authentication via Keycloak
- PostgreSQL database for reliable data persistence
- API versioning support
- Swagger/OpenAPI documentation

## Prerequisites

Before running the PlantService, ensure you have the following installed:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [Docker](https://www.docker.com/get-started) and [Docker Compose](https://docs.docker.com/compose/install/)
- [PostgreSQL](https://www.postgresql.org/download/) (if running locally without Docker)
- Access to Keycloak authentication server
- `.env` file with required environment variables (obtain from @wtvamp or @lunarjuice)

## Architecture & Technology Stack

### Technology Stack

- **Framework**: ASP.NET Core 10.0
- **Language**: C# 12
- **Database**: PostgreSQL 15+
- **ORM**: Entity Framework Core with Npgsql provider
- **Authentication**: JWT Bearer tokens via Keycloak OAuth2
- **API Documentation**: Swagger/OpenAPI 3.0
- **Containerization**: Docker & Docker Compose
- **API Versioning**: Query string-based versioning

### Architecture

The PlantService follows a layered architecture pattern:

```
┌─────────────────────────────────────┐
│    Controllers (API Endpoints)      │
│   - PlantCatalogController          │
│   - UserPlantController             │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│       Business Logic Layer          │
│   - Gacha System                    │
│   - Care Management                 │
│   - Growth Progression              │
│   - Sell Value Calculation          │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Data Access Layer (EF Core)    │
│   - PlantContext                    │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      PostgreSQL Database            │
│   - PlantCatalogs Table             │
│   - UserPlants Table                │
└─────────────────────────────────────┘
```

## Getting Started

### Environment Variables

| Variable | Description | Example Value |
|----------|-------------|---------------|
| `Keycloak__Authority` | Keycloak server authority URL for JWT validation | `https://keycloak.example.com/realms/fmn` |
| `Keycloak__Audience` | Expected audience claim in JWT tokens | `fmn-api` |
| `Keycloak__OAuthClientId` | OAuth2 client ID for Swagger UI authentication | `fmn-swagger-client` |
| `Keycloak__OAuthClientSecret` | OAuth2 client secret for Swagger UI | `your-client-secret` |
| `ConnectionString__DefaultConnection` | PostgreSQL connection string | `Host=plantdb;Database=plant_db;Username=postgres;Password=yourpassword` |
| `POSTGRES_PASSWORD` | PostgreSQL database password (Docker only) | `yourpassword` |
| `POSTGRES_DB` | PostgreSQL database name (Docker only) | `plant_db` |

**IMPORTANT**: Never commit `.env` files to source control. Obtain the appropriate `.env` file from your team lead.

### Setup Instructions

#### Option 1: Docker Compose Setup (Recommended)

1. **Navigate to the service directory:**
   ```bash
   cd PlantService
   ```

2. **Start the service:**
   ```bash
   docker compose up -d
   docker compose logs -f plantservice
   ```

3. **Verify the service:**
   - API: http://localhost:90
   - Swagger UI: http://localhost:90/swagger
   - Database: localhost:5441

#### Option 2: dotnet run Setup

```bash
# Start database only
docker compose up -d plantdb

# Run service
dotnet run
```

**Access:**
- HTTPS: https://localhost:7090
- HTTP: http://localhost:5090
- Swagger UI: https://localhost:7090/swagger

## API Endpoints

### Plant Catalog

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/PlantCatalog` | Get all available plants |
| GET | `/api/PlantCatalog/{id}` | Get plant by ID |
| GET | `/api/PlantCatalog/rarity/{rarity}` | Get plants by rarity |

### User Plants

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/UserPlant` | Get user's plant collection |
| GET | `/api/UserPlant/{id}` | Get specific owned plant |
| GET | `/api/UserPlant/greenhouse` | Get greenhouse view |
| POST | `/api/UserPlant/gacha` | Roll for new plant |
| POST | `/api/UserPlant/{id}/care` | Care for plant |
| POST | `/api/UserPlant/{id}/sell` | Sell plant |
| GET | `/api/UserPlant/{id}/sell-preview` | Preview sell value |
| PATCH | `/api/UserPlant/{id}/greenhouse` | Toggle greenhouse placement |

## Plant System

### Rarity Tiers

| Rarity | Gacha Weight | Examples |
|--------|--------------|----------|
| Common | 100 | Pothos, Spider Plant, Snake Plant |
| Uncommon | 50 | Fern, Peace Lily, Red Mushroom |
| Rare | 25 | Monstera, Fiddle Leaf Fig |
| Epic | 10 | Cherry Blossom, Bird of Paradise |
| Legendary | 2-3 | Rainbow Eucalyptus, Corpse Flower |

### Growth Stages

| Stage | Name | Sell Value | Location |
|-------|------|------------|----------|
| 1 | Seedling | 50 coins | Shelf |
| 2 | Sprout | 100 coins | Shelf/Table |
| 3 | Young Plant | 200 coins | Table/Floor |
| 4 | Mature | 350 coins | Floor |
| 5 | Overgrown | 500 coins | Floor (with vines) |

### Care Actions

| Action | Cost | Effect |
|--------|------|--------|
| Water | 10 coins | +30 water level |
| Sunlight | 15 coins | +30 sunlight level |
| Nutrients | 20 coins | +30 nutrient level |
| Boost | 50 coins | Max all + bonus growth |

## Development

### Database Migrations

```bash
dotnet ef migrations add MigrationName
dotnet ef migrations remove
dotnet ef database update
```

### Running Tests

```bash
dotnet test
```

## Support

For questions or issues:
- Contact: @wtvamp or @lunarjuice
- Check service logs for error messages
- Verify Keycloak configuration
