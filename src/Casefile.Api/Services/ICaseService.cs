using Casefile.Domain;
using Casefile.Api.Models;

namespace Casefile.Api.Services;

public interface ICaseService
{
    Task<CaseResponse> CreateAsync(CreateCaseRequest request, CancellationToken ct);
    Task<CaseResponse> GetAsync(Guid id, CancellationToken ct);
    Task<PagedCasesResponse> ListAsync(CaseStatus? status, int page, int pageSize, CancellationToken ct);
    Task<CaseResponse> UpdateAsync(Guid id, UpdateCaseRequest request, CancellationToken ct);
    Task<CaseResponse> ChangeStatusAsync(Guid id, CaseStatus next, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
