namespace AutoServiceAW.API.Shared.Domain.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> ListAsync();
    Task<TEntity?> FindByIdAsync(int id);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}