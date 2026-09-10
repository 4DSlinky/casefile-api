# Concept map

Read this file when you want a refresher. Every idea below exists in this repo. If you cannot point at a file, you do not need the idea yet.

## C# language

| Concept | Where it lives |
|---|---|
| Classes vs records | `SupportCase` is a class with behavior. DTOs in `Models/Dtos.cs` are records. |
| Enums | `CaseStatus`, `CaseSeverity` |
| Static helpers | `CaseRules`, `StatusTransitions` |
| Exceptions | `CaseExceptions.cs` — validation vs not-found vs conflict |
| Nullable reference types | Enabled in every `.csproj` |
| Primary constructors | `AppDbContext`, `CaseService`, middleware |
| Collection expressions | `[CaseStatus.InProgress, CaseStatus.Closed]` |
| LINQ | Filtering and paging in `CaseService.ListAsync` |
| Async / await / CancellationToken | Every service and controller method |
| GUID and DateTimeOffset | Case identity and timestamps |

## Design

| Concept | Where it lives |
|---|---|
| Domain model separate from HTTP | `src/Casefile.Domain` has no ASP.NET reference |
| Entity vs DTO | `SupportCase` / `SupportCaseRecord` / `CaseResponse` |
| Dependency injection | `Program.cs` registers `AppDbContext` and `ICaseService` |
| Interface + implementation | `ICaseService` / `CaseService` |
| State machine | `StatusTransitions` + `SupportCase.TransitionTo` |
| Input validation | `CaseRules` before anything is saved |
| Persistence | EF Core + SQLite (`AppDbContext`) |
| Mapping | `SupportCaseRecord.FromDomain` / `ToDomain` |

## HTTP API

| Concept | Where it lives |
|---|---|
| REST resources | `/api/cases` |
| GET / POST / PUT / DELETE | `CasesController` |
| 201 Created + Location | `CreatedAtAction` |
| Query filters and paging | `GET /api/cases?status=&page=&pageSize=` |
| Problem-style errors | `ExceptionMiddleware` → 400 / 404 / 409 / 500 |
| Health check | `GET /health` (no API key) |
| OpenAPI / Swagger | `/swagger` |
| Demo auth | `X-Api-Key` middleware. Not SSO. Not JWT. |

## Tests

| Concept | Where it lives |
|---|---|
| Unit tests | `tests/Casefile.Domain.Tests` — no HTTP, no database |
| Integration tests | `tests/Casefile.Api.Tests` — real HTTP pipeline via `WebApplicationFactory` |
| Theory vs fact | `[Theory]` + `[InlineData]` for rule tables |
| Arrange / Act / Assert | Every test method |
| Test isolation | Each factory gets its own SQLite file |

## Git / CI / containers

| Concept | Where it lives |
|---|---|
| Solution layout | `src/` and `tests/` |
| GitHub Actions | `.github/workflows/ci.yml` runs `dotnet test` |
| Dockerfile (multi-stage) | SDK image builds, aspnet image runs |
| docker compose | One service, named volume for the SQLite file |
| Secrets stay out of git | `.gitignore`, `.env.example`, API key from config/env |

## What this is not

Kubernetes, JWT, Postgres, a UI, a message bus, or a federal system. Those are later add-ons, not this repo.
