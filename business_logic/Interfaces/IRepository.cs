using Ardalis.Specification;

namespace business_logic.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(object id);
        Task<TEntity?> GetItemBySpecAsync(ISpecification<TEntity> specification);
        Task<IEnumerable<TEntity>> GetListBySpecAsync(ISpecification<TEntity> specification);
        Task InsertAsync(TEntity entity);
        void Update(TEntity entityToUpdate);
        Task DeleteByIdAsync(object id);
        Task SaveAsync();
        Task<int> CountAsync(ISpecification<TEntity> spec);
    }
}