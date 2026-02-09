namespace BuildingBlocks.Common.Abstractions;

/// <summary>
/// Base repository interface for aggregate roots
/// </summary>
/// <typeparam name="TEntity">Type of aggregate root</typeparam>
public interface IRepository<TEntity> where TEntity : IAggregateRoot
{
    /// <summary>
    /// Gets an entity by its identifier
    /// </summary>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// Deletes an entity
    /// </summary>
    void Delete(TEntity entity);
}

/// <summary>
/// Base repository interface with specific ID type
/// </summary>
/// <typeparam name="TEntity">Type of aggregate root</typeparam>
/// <typeparam name="TId">Type of the identifier</typeparam>
public interface IRepository<TEntity, TId> where TEntity : IAggregateRoot<TId> where TId : notnull
{
    /// <summary>
    /// Gets an entity by its identifier
    /// </summary>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// Deletes an entity
    /// </summary>
    void Delete(TEntity entity);
}
