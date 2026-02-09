using BuildingBlocks.Common.Abstractions;

namespace BuildingBlocks.Common.BaseClasses;

/// <summary>
/// Base class for aggregate roots with Guid identifier
/// </summary>
public abstract class AggregateRoot : Entity, IAggregateRoot
{
    protected AggregateRoot() : base()
    {
    }

    protected AggregateRoot(Guid id) : base(id)
    {
    }
}

/// <summary>
/// Base class for aggregate roots with specific ID type
/// </summary>
/// <typeparam name="TId">Type of the identifier</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId> where TId : notnull
{
    protected AggregateRoot() : base()
    {
    }

    protected AggregateRoot(TId id) : base(id)
    {
    }
}
