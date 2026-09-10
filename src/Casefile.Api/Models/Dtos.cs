using Casefile.Domain;

namespace Casefile.Api.Models;

public sealed record CreateCaseRequest(
    string Title,
    string? Description,
    string RequesterEmail,
    CaseSeverity Severity);

public sealed record UpdateCaseRequest(
    string Title,
    string? Description,
    CaseSeverity Severity);

public sealed record ChangeStatusRequest(CaseStatus Status);

public sealed record CaseResponse(
    Guid Id,
    string Title,
    string Description,
    string RequesterEmail,
    CaseSeverity Severity,
    CaseStatus Status,
    IReadOnlyCollection<CaseStatus> AllowedNextStatuses,
    DateTimeOffset CreatedUtc,
    DateTimeOffset UpdatedUtc)
{
    public static CaseResponse From(SupportCase c) => new(
        c.Id,
        c.Title,
        c.Description,
        c.RequesterEmail,
        c.Severity,
        c.Status,
        StatusTransitions.NextStatuses(c.Status),
        c.CreatedUtc,
        c.UpdatedUtc);
}

public sealed record PagedCasesResponse(
    IReadOnlyList<CaseResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
