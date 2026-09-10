using Casefile.Domain;

namespace Casefile.Api.Data;

public sealed class SupportCaseRecord
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
    public CaseSeverity Severity { get; set; }
    public CaseStatus Status { get; set; }
    public DateTimeOffset CreatedUtc { get; set; }
    public DateTimeOffset UpdatedUtc { get; set; }

    public SupportCase ToDomain() =>
        SupportCase.Rehydrate(Id, Title, Description, RequesterEmail, Severity, Status, CreatedUtc, UpdatedUtc);

    public static SupportCaseRecord FromDomain(SupportCase c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        RequesterEmail = c.RequesterEmail,
        Severity = c.Severity,
        Status = c.Status,
        CreatedUtc = c.CreatedUtc,
        UpdatedUtc = c.UpdatedUtc
    };

    public void CopyFrom(SupportCase c)
    {
        Title = c.Title;
        Description = c.Description;
        RequesterEmail = c.RequesterEmail;
        Severity = c.Severity;
        Status = c.Status;
        CreatedUtc = c.CreatedUtc;
        UpdatedUtc = c.UpdatedUtc;
    }
}
