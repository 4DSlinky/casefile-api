using System.Net.Mail;

namespace Casefile.Domain;

/// <summary>
/// Input rules live here so unit tests can cover them without spinning up HTTP.
/// </summary>
public static class CaseRules
{
    public const int TitleMinLength = 3;
    public const int TitleMaxLength = 120;
    public const int DescriptionMaxLength = 2000;

    public static string NormalizeTitle(string? title)
    {
        var value = title?.Trim() ?? string.Empty;
        if (value.Length < TitleMinLength || value.Length > TitleMaxLength)
        {
            throw new CaseValidationException(
                $"Title must be {TitleMinLength}-{TitleMaxLength} characters after trimming.");
        }

        return value;
    }

    public static string NormalizeDescription(string? description)
    {
        var value = description?.Trim() ?? string.Empty;
        if (value.Length > DescriptionMaxLength)
        {
            throw new CaseValidationException(
                $"Description must be at most {DescriptionMaxLength} characters.");
        }

        return value;
    }

    public static string NormalizeEmail(string? email)
    {
        var value = email?.Trim() ?? string.Empty;
        if (value.Length == 0)
        {
            throw new CaseValidationException("Requester email is required.");
        }

        try
        {
            var parsed = new MailAddress(value);
            if (!string.Equals(parsed.Address, value, StringComparison.OrdinalIgnoreCase))
            {
                throw new CaseValidationException("Requester email is not valid.");
            }
        }
        catch (FormatException)
        {
            throw new CaseValidationException("Requester email is not valid.");
        }

        return value.ToLowerInvariant();
    }
}
