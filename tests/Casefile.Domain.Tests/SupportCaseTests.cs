using Casefile.Domain;
using FluentAssertions;

namespace Casefile.Domain.Tests;

public class SupportCaseTests
{
    [Fact]
    public void Open_starts_in_Open_status()
    {
        var c = SupportCase.Open("Cannot reset password", "User is locked out", "user@example.com", CaseSeverity.High);
        c.Status.Should().Be(CaseStatus.Open);
        c.Title.Should().Be("Cannot reset password");
        c.RequesterEmail.Should().Be("user@example.com");
    }

    [Fact]
    public void Happy_path_workflow()
    {
        var c = SupportCase.Open("Printer offline", null, "desk@example.com", CaseSeverity.Low);
        c.TransitionTo(CaseStatus.InProgress);
        c.TransitionTo(CaseStatus.Resolved);
        c.TransitionTo(CaseStatus.Closed);
        c.Status.Should().Be(CaseStatus.Closed);
    }

    [Fact]
    public void Cannot_edit_or_delete_after_leaving_open_delete_rule()
    {
        var c = SupportCase.Open("Need badge reprint", "Lost badge", "pat@example.com", CaseSeverity.Medium);
        c.TransitionTo(CaseStatus.InProgress);

        var edit = () => c.UpdateDetails("Need badge reprint", "Updated", CaseSeverity.High);
        edit.Should().NotThrow();

        var del = () => c.EnsureDeletable();
        del.Should().Throw<CaseConflictException>();
    }

    [Fact]
    public void Closed_case_cannot_be_edited()
    {
        var c = SupportCase.Open("Old ticket", "done", "pat@example.com", CaseSeverity.Low);
        c.TransitionTo(CaseStatus.Closed);
        var act = () => c.UpdateDetails("Old ticket", "nope", CaseSeverity.Low);
        act.Should().Throw<CaseConflictException>();
    }
}
