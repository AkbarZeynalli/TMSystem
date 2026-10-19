using TMSystem.Domain.Common;

namespace TMSystem.Domain.Events;

public sealed record TaskAssignedEvent(Guid TaskId, Guid AssigneeId) : IDomainEvent;
