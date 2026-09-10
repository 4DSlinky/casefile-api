using Casefile.Domain;
using FluentAssertions;

namespace Casefile.Domain.Tests;

public class CaseRulesTests
{
    [Theory]
    [InlineData("Fix login bug")]
    [InlineData("  Pad the title  ")]
    public void NormalizeTitle_accepts_valid_input(string title)
    {
        var normalized = CaseRules.NormalizeTitle(title);
        normalized.Should().Be(title.Trim());
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("ab")]
    public void NormalizeTitle_rejects_short_or_blank(string title)
    {
        var act = () => CaseRules.NormalizeTitle(title);
        act.Should().Throw<CaseValidationException>();
    }

    [Fact]
    public void NormalizeTitle_rejects_too_long()
    {
        var act = () => CaseRules.NormalizeTitle(new string('x', CaseRules.TitleMaxLength + 1));
        act.Should().Throw<CaseValidationException>();
    }

    [Fact]
    public void NormalizeDescription_rejects_too_long()
    {
        var act = () => CaseRules.NormalizeDescription(new string('x', CaseRules.DescriptionMaxLength + 1));
        act.Should().Throw<CaseValidationException>();
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("")]
    [InlineData("plain.com")]
    public void NormalizeEmail_rejects_invalid(string email)
    {
        var act = () => CaseRules.NormalizeEmail(email);
        act.Should().Throw<CaseValidationException>();
    }

    [Fact]
    public void NormalizeEmail_lowercases_valid_address()
    {
        CaseRules.NormalizeEmail("Alex@Example.COM").Should().Be("alex@example.com");
    }
}
