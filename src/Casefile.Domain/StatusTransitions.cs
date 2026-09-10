namespace Casefile.Domain;

/// <summary>
/// Allowed status moves. Closed is terminal. Open cannot jump straight to Resolved.
/// </summary>
public static class StatusTransitions
{
    private static readonly Dictionary<CaseStatus, CaseStatus[]> Allowed = new()
    {
        [CaseStatus.Open] = [CaseStatus.InProgress, CaseStatus.Closed],
        [CaseStatus.InProgress] = [CaseStatus.Open, CaseStatus.Resolved, CaseStatus.Closed],
        [CaseStatus.Resolved] = [CaseStatus.InProgress, CaseStatus.Closed],
        [CaseStatus.Closed] = []
    };

    public static bool CanTransition(CaseStatus from, CaseStatus to) =>
        from != to && Allowed[from].Contains(to);

    public static void EnsureCanTransition(CaseStatus from, CaseStatus to)
    {
        if (from == to)
        {
            throw new CaseConflictException($"Case is already {from}.");
        }

        if (!CanTransition(from, to))
        {
            var options = Allowed[from].Length == 0
                ? "none (terminal state)"
                : string.Join(", ", Allowed[from]);
            throw new CaseConflictException(
                $"Cannot move a case from {from} to {to}. Allowed next statuses: {options}.");
        }
    }

    public static IReadOnlyCollection<CaseStatus> NextStatuses(CaseStatus from) => Allowed[from];
}
