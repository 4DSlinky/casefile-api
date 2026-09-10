# Casefile API

Small ASP.NET Core API for opening, updating, and closing IT support cases.

This is a **practice repo**. I can explain every file. It is not a production system and it is not connected to any government application.

Built to keep C#, REST, validation, tests, and Docker warm for entry-level software interviews.

## What it does

A case starts `Open` and can only move through a small status map:

```
Open --> InProgress --> Resolved --> Closed
 |              |              |
 +--> Closed    +--> Open      +--> InProgress
                +--> Closed
```

`Open -> Resolved` is rejected on purpose so the API has a business rule you can see in a test.

Other rules:

- Title 3-120 characters
- Description max 2,000 characters
- Requester email must be a real address
- Closed cases cannot be edited
- Only `Open` cases can be deleted

## API

| Method | Path | Notes |
|---|---|---|
| GET | `/health` | Public |
| GET | `/swagger` | Public |
| POST | `/api/cases` | Create |
| GET | `/api/cases` | List (`status`, `page`, `pageSize`) |
| GET | `/api/cases/{id}` | Get one |
| PUT | `/api/cases/{id}` | Edit title/description/severity |
| POST | `/api/cases/{id}/status` | Legal transition only |
| DELETE | `/api/cases/{id}` | Open cases only |

Protected routes expect header `X-Api-Key`. Local default: `local-dev-key-change-me`.

## Run locally

Needs the .NET 8 SDK.

```bash
dotnet test Casefile.sln
dotnet run --project src/Casefile.Api/Casefile.Api.csproj
```

API: http://localhost:5080
Swagger: http://localhost:5080/swagger

Sample requests are in `http/cases.http`.

## Run in Docker

Needs Docker Desktop (or another engine). This is one container, not Kubernetes.

```bash
docker compose up --build
```

API: http://localhost:8080

## Layout

```
src/Casefile.Domain     rules and the SupportCase model (no HTTP)
src/Casefile.Api        controllers, EF Core/SQLite, middleware
tests/Casefile.Domain.Tests     unit tests
tests/Casefile.Api.Tests        HTTP integration tests
CONCEPTS.md             refresher map: idea -> file
```

## Interview version of this repo

A small case-intake API in C#. Domain rules sit in their own project so they can be unit tested without HTTP. The API layer maps those cases to REST, stores them in SQLite with EF Core, returns 400/404/409 correctly, and has integration tests through WebApplicationFactory. It is also containerized with a multi-stage Dockerfile. The API key middleware is demo auth only.

Container depth: one Docker image and compose file. No cluster.

## Later, if this grows

1. Swap SQLite for Postgres
2. Replace the API key with real auth
3. Add a tiny frontend
4. Only after those are boring: a second service, then maybe Kubernetes

See `CONCEPTS.md` for the study map.
