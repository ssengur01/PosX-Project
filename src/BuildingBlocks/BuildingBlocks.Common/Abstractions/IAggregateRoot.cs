namespace BuildingBlocks.Common.Abstractions;

/// <summary>
/// Marker interface for aggregate roots in DDD
/// </summary>
public interface IAggregateRoot : IEntity
{
}

/// <summary>
/// Marker interface for aggregate roots with specific ID type
/// </summary>
/// <typeparam name="TId">Type of the identifier</typeparam>
public interface IAggregateRoot<TId> : IEntity<TId> where TId : notnull
{
}
