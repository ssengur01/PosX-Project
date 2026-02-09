namespace BuildingBlocks.Common.Abstractions;

/// <summary>
/// Base interface for all entities
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    Guid Id { get; }
}

/// <summary>
/// Base interface for entities with specific ID type
/// </summary>
/// <typeparam name="TId">Type of the identifier</typeparam>
public interface IEntity<TId> where TId : notnull
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    TId Id { get; }
}
