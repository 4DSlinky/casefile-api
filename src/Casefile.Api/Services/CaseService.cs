using Casefile.Api.Data;
using Casefile.Api.Models;
using Casefile.Domain;
using Microsoft.EntityFrameworkCore;

namespace Casefile.Api.Services;

public sealed class CaseService(AppDbContext db) : ICaseService
{
    public async Task<CaseResponse> CreateAsync(CreateCaseRequest request, CancellationToken ct)
    {
        var domain = SupportCase.Open(
            request.Title,
            request.Description,
            request.RequesterEmail,
            request.Severity);

        db.Cases.Add(SupportCaseRecord.FromDomain(domain));
        await db.SaveChangesAsync(ct);
        return CaseResponse.From(domain);
    }

    public async Task<CaseResponse> GetAsync(Guid id, CancellationToken ct)
    {
        var record = await db.Cases.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (record is null)
        {
            throw new CaseNotFoundException(id);
        }

        return CaseResponse.From(record.ToDomain());
    }

    public async Task<PagedCasesResponse> ListAsync(CaseStatus? status, int page, int pageSize, CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 50 ? 20 : pageSize;

        var query = db.Cases.AsNoTracking().AsQueryable();
        if (status is not null)
        {
            query = query.Where(x => x.Status == status);
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.CreatedUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedCasesResponse(
            items.Select(x => CaseResponse.From(x.ToDomain())).ToList(),
            page,
            pageSize,
            total);
    }

    public async Task<CaseResponse> UpdateAsync(Guid id, UpdateCaseRequest request, CancellationToken ct)
    {
        var record = await db.Cases.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (record is null)
        {
            throw new CaseNotFoundException(id);
        }

        var domain = record.ToDomain();
        domain.UpdateDetails(request.Title, request.Description, request.Severity);
        record.CopyFrom(domain);
        await db.SaveChangesAsync(ct);
        return CaseResponse.From(domain);
    }

    public async Task<CaseResponse> ChangeStatusAsync(Guid id, CaseStatus next, CancellationToken ct)
    {
        var record = await db.Cases.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (record is null)
        {
            throw new CaseNotFoundException(id);
        }

        var domain = record.ToDomain();
        domain.TransitionTo(next);
        record.CopyFrom(domain);
        await db.SaveChangesAsync(ct);
        return CaseResponse.From(domain);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var record = await db.Cases.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (record is null)
        {
            throw new CaseNotFoundException(id);
        }

        var domain = record.ToDomain();
        domain.EnsureDeletable();
        db.Cases.Remove(record);
        await db.SaveChangesAsync(ct);
    }
}
