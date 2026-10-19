using TMSystem.Domain.Exceptions;

namespace TMSystem.Domain.ValueObjects;

public sealed record Email
{
    public const int MaxLength = 256;

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email boş ola bilməz.");

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > MaxLength)
            throw new DomainException($"Email {MaxLength} simvoldan uzun ola bilməz.");

        var atIndex = normalized.IndexOf('@');
        if (atIndex <= 0 || atIndex != normalized.LastIndexOf('@') || !normalized[(atIndex + 1)..].Contains('.'))
            throw new DomainException("Email formatı yanlışdır.");

        return new Email(normalized);
    }

    public override string ToString() => Value;
}
