using Casefile.Domain;
using FluentAssertions;

namespace Casefile.Domain.Tests;

public class StatusTransitionsTests
{
    [Theory]
    [InlineData(CaseStatus.Open, CaseStatus.InProgress)]
    [InlineData(CaseStatus.Open, CaseStatus.Closed)]
    [InlineData(CaseStatus.InProgress, CaseStatus.Resolved)]
    [InlineData(CaseStatus.Resolved, CaseStatus.Closed)]
    public void Allows_expected_moves(CaseStatus from, CaseStatus to)
    {
        StatusTransitions.CanTransition(from, to).Should().BeTrue();
    }

    [Theory]
    [InlineData(CaseStatus.Open, CaseStatus.Resolved)]
    [InlineData(CaseStatus.Closed, CaseStatus.Open)]
    [InlineData(CaseStatus.Closed, CaseStatus.InProgress)]
    public void Blocks_illegal_moves(CaseStatus from, CaseStatus to)
    {
        var act = () => StatusTransitions.EnsureCanTransition(from, to);
        act.Should().Throw<CaseConflictException>();
    }

    [Fact]
    public void Closed_is_terminal()
    {
        StatusTransitions.NextStatuses(CaseStatus.Closed).Should().BeEmpty();
    }
}
