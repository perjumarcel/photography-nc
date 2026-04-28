namespace Photography.SharedKernel;

/// <summary>
/// Base entity with a strongly-typed identifier and creation/update audit timestamps in UTC.
/// </summary>
public abstract class EntityBase<TId>
    where TId : struct
{
    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAtUtc { get; protected set; }
    public DateTime? UpdatedAtUtc { get; protected set; }

    protected void Touch() => UpdatedAtUtc = DateTime.UtcNow;
}

public abstract class AggregateRoot<TId> : EntityBase<TId>
    where TId : struct
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent evt) => _domainEvents.Add(evt);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

public interface IDomainEvent
{
    DateTime OccurredAtUtc { get; }
}
