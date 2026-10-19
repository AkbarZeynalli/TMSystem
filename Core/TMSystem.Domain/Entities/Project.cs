using TMSystem.Domain.Common;
using TMSystem.Domain.Exceptions;

namespace TMSystem.Domain.Entities;

public sealed class Project : AuditableEntity
{
    public const int NameMaxLength = 150;

    private readonly List<TaskItem> _tasks = [];

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public bool IsArchived { get; private set; }

    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    private Project() { } // EF Core üçün

    public static Project Create(string name, string? description, Guid ownerId)
    {
        if (ownerId == Guid.Empty)
            throw new DomainException("Layihənin sahibi göstərilməlidir.");

        var project = new Project { OwnerId = ownerId };
        project.Update(name, description);
        return project;
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Layihənin adı boş ola bilməz.");
        if (name.Length > NameMaxLength)
            throw new DomainException($"Layihənin adı {NameMaxLength} simvoldan uzun ola bilməz.");

        Name = name.Trim();
        Description = description?.Trim();
    }

    public void Archive()
    {
        if (IsArchived)
            throw new DomainException("Layihə artıq arxivdədir.");
        IsArchived = true;
    }

    public void Restore()
    {
        if (!IsArchived)
            throw new DomainException("Layihə arxivdə deyil.");
        IsArchived = false;
    }
}
