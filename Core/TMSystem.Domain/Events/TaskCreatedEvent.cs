using TMSystem.Domain.Common;

namespace TMSystem.Domain.Events;

public sealed record TaskCreatedEvent(Guid TaskId, Guid ProjectId) : IDomainEvent;
