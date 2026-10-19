using TMSystem.Domain.Entities;

namespace TMSystem.Domain.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<IReadOnlyList<Project>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
