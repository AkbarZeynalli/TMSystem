using TMSystem.Domain.Common;
using TMSystem.Domain.Exceptions;
using TMSystem.Domain.ValueObjects;

namespace TMSystem.Domain.Entities;

public sealed class User : AuditableEntity
{
    public const int NameMaxLength = 100;

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    private User() { } // EF Core üçün

    public static User Create(string firstName, string lastName, Email email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Şifrə hash-i boş ola bilməz.");

        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            IsActive = true
        };
        user.SetName(firstName, lastName);
        return user;
    }

    public void UpdateProfile(string firstName, string lastName) => SetName(firstName, lastName);

    public void ChangeEmail(Email email) => Email = email;

    public void ChangePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Şifrə hash-i boş ola bilməz.");
        PasswordHash = passwordHash;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    private void SetName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Ad və soyad boş ola bilməz.");
        if (firstName.Length > NameMaxLength || lastName.Length > NameMaxLength)
            throw new DomainException($"Ad və soyad {NameMaxLength} simvoldan uzun ola bilməz.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }
}
