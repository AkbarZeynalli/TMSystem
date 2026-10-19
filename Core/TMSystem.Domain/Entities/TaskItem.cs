using TMSystem.Domain.Common;
using TMSystem.Domain.Enums;
using TMSystem.Domain.Events;
using TMSystem.Domain.Exceptions;

namespace TMSystem.Domain.Entities;

public sealed class TaskItem : AuditableEntity
{
    public const int TitleMaxLength = 200;

    private readonly List<Comment> _comments = [];
    private readonly List<Attachment> _attachments = [];
    private readonly List<Label> _labels = [];

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskItemStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid? AssigneeId { get; private set; }

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();
    public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyCollection<Label> Labels => _labels.AsReadOnly();

    private TaskItem() { } // EF Core üçün

    public static TaskItem Create(
        string title, string? description, Guid projectId, TaskPriority priority, DateTime? dueDate = null)
    {
        if (projectId == Guid.Empty)
            throw new DomainException("Tapşırıq bir layihəyə aid olmalıdır.");

        var task = new TaskItem
        {
            ProjectId = projectId,
            Status = TaskItemStatus.Todo
        };
        task.Update(title, description, priority, dueDate);
        task.AddDomainEvent(new TaskCreatedEvent(task.Id, projectId));
        return task;
    }

    public void Update(string title, string? description, TaskPriority priority, DateTime? dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Tapşırığın başlığı boş ola bilməz.");
        if (title.Length > TitleMaxLength)
            throw new DomainException($"Tapşırığın başlığı {TitleMaxLength} simvoldan uzun ola bilməz.");

        Title = title.Trim();
        Description = description?.Trim();
        Priority = priority;
        DueDate = dueDate;
    }

    // Vaxt parametr kimi gəlir ki, Domain DateTime.UtcNow-dan asılı olmasın və test edilə bilsin
    public void ChangeStatus(TaskItemStatus newStatus, DateTime changedAt)
    {
        if (Status == newStatus)
            return;

        // Bitmiş tapşırıq yalnız InProgress-ə qaytarıla bilər (yenidən açılma)
        if (Status == TaskItemStatus.Done && newStatus != TaskItemStatus.InProgress)
            throw new DomainException("Bitmiş tapşırıq yalnız 'InProgress' statusuna qaytarıla bilər.");

        Status = newStatus;
        CompletedAt = newStatus == TaskItemStatus.Done ? changedAt : null;
    }

    public void AssignTo(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("İstifadəçi göstərilməlidir.");
        if (AssigneeId == userId)
            return;

        AssigneeId = userId;
        AddDomainEvent(new TaskAssignedEvent(Id, userId));
    }

    public void Unassign() => AssigneeId = null;

    public bool IsOverdue(DateTime now) =>
        DueDate.HasValue && Status != TaskItemStatus.Done && DueDate.Value < now;

    public Comment AddComment(Guid authorId, string content)
    {
        var comment = Comment.Create(Id, authorId, content);
        _comments.Add(comment);
        return comment;
    }

    public void RemoveComment(Guid commentId)
    {
        var comment = _comments.FirstOrDefault(c => c.Id == commentId)
            ?? throw new DomainException("Şərh tapılmadı.");
        _comments.Remove(comment);
    }

    public Attachment AddAttachment(
        Guid uploadedById, string fileName, string storagePath, string contentType, long sizeInBytes)
    {
        var attachment = Attachment.Create(Id, uploadedById, fileName, storagePath, contentType, sizeInBytes);
        _attachments.Add(attachment);
        return attachment;
    }

    public void RemoveAttachment(Guid attachmentId)
    {
        var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId)
            ?? throw new DomainException("Fayl tapılmadı.");
        _attachments.Remove(attachment);
    }

    public void AddLabel(Label label)
    {
        if (label.ProjectId != ProjectId)
            throw new DomainException("Label tapşırıqla eyni layihəyə aid olmalıdır.");
        if (_labels.Any(l => l.Id == label.Id))
            return;

        _labels.Add(label);
    }

    public void RemoveLabel(Guid labelId) => _labels.RemoveAll(l => l.Id == labelId);
}
