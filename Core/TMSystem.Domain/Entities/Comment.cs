using TMSystem.Domain.Common;
using TMSystem.Domain.Exceptions;

namespace TMSystem.Domain.Entities;

public sealed class Comment : AuditableEntity
{
    public const int ContentMaxLength = 2000;

    public string Content { get; private set; } = string.Empty;
    public Guid TaskItemId { get; private set; }
    public Guid AuthorId { get; private set; }

    private Comment() { } // EF Core üçün

    // Yalnız TaskItem.AddComment() vasitəsilə yaradılır
    internal static Comment Create(Guid taskItemId, Guid authorId, string content)
    {
        if (authorId == Guid.Empty)
            throw new DomainException("Şərhin müəllifi göstərilməlidir.");

        var comment = new Comment { TaskItemId = taskItemId, AuthorId = authorId };
        comment.Edit(content);
        return comment;
    }

    public void Edit(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Şərh boş ola bilməz.");
        if (content.Length > ContentMaxLength)
            throw new DomainException($"Şərh {ContentMaxLength} simvoldan uzun ola bilməz.");

        Content = content.Trim();
    }
}
