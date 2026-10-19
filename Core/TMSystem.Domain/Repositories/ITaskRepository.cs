using TMSystem.Domain.Entities;

namespace TMSystem.Domain.Repositories;

public interface ITaskRepository : IRepository<TaskItem>
{
    // Comments, Attachments və Labels ilə birlikdə yükləyir
    Task<TaskItem?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetByAssigneeIdAsync(Guid assigneeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetOverdueAsync(DateTime now, CancellationToken cancellationToken = default);
}
