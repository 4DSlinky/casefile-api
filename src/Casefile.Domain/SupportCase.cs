namespace Casefile.Domain;

/// <summary>
/// Domain model. The API maps to/from this; it does not leak EF entities to callers.
/// </summary>
public sealed class SupportCase
{
    public Guid Id { get; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string RequesterEmail { get; }
    public CaseSeverity Severity { get; private set; }
    public CaseStatus Status { get; private set; }
    public DateTimeOffset CreatedUtc { get; }
    public DateTimeOffset UpdatedUtc { get; private set; }

    private SupportCase(
        Guid id,
        string title,
        string description,
        string requesterEmail,
        CaseSeverity severity,
        CaseStatus status,
        DateTimeOffset createdUtc,
        DateTimeOffset updatedUtc)
    {
        Id = id;
        Title = title;
        Description = description;
        RequesterEmail = requesterEmail;
        Severity = severity;
        Status = status;
        CreatedUtc = createdUtc;
        UpdatedUtc = updatedUtc;
    }

    public static SupportCase Open(
        string title,
        string? description,
        string requesterEmail,
        CaseSeverity severity,
        DateTimeOffset? now = null)
    {
        var stamp = now ?? DateTimeOffset.UtcNow;
        return new SupportCase(
            id: Guid.NewGuid(),
            title: CaseRules.NormalizeTitle(title),
            description: CaseRules.NormalizeDescription(description),
            requesterEmail: CaseRules.NormalizeEmail(requesterEmail),
            severity,
            CaseStatus.Open,
            stamp,
            stamp);
    }

    public static SupportCase Rehydrate(
        Guid id,
        string title,
        string description,
        string requesterEmail,
        CaseSeverity severity,
        CaseStatus status,
        DateTimeOffset createdUtc,
        DateTimeOffset updatedUtc)
    {
        return new SupportCase(id, title, description, requesterEmail, severity, status, createdUtc, updatedUtc);
    }

    public void UpdateDetails(string title, string? description, CaseSeverity severity, DateTimeOffset? now = null)
    {
        EnsureNotClosed();
        Title = CaseRules.NormalizeTitle(title);
        Description = CaseRules.NormalizeDescription(description);
        Severity = severity;
        Touch(now);
    }

    public void TransitionTo(CaseStatus next, DateTimeOffset? now = null)
    {
        StatusTransitions.EnsureCanTransition(Status, next);
        Status = next;
        Touch(now);
    }

    public void EnsureDeletable()
    {
        if (Status != CaseStatus.Open)
        {
            throw new CaseConflictException("Only Open cases can be deleted. Transition or close instead.");
        }
    }

    private void EnsureNotClosed()
    {
        if (Status == CaseStatus.Closed)
        {
            throw new CaseConflictException("Closed cases cannot be edited.");
        }
    }

    private void Touch(DateTimeOffset? now) => UpdatedUtc = now ?? DateTimeOffset.UtcNow;
}
