namespace Casefile.Domain;

public abstract class CaseException : Exception
{
    protected CaseException(string message) : base(message) { }
}

public sealed class CaseValidationException : CaseException
{
    public CaseValidationException(string message) : base(message) { }
}

public sealed class CaseNotFoundException : CaseException
{
    public CaseNotFoundException(Guid id) : base($"Case '{id}' was not found.") { }
}

public sealed class CaseConflictException : CaseException
{
    public CaseConflictException(string message) : base(message) { }
}
