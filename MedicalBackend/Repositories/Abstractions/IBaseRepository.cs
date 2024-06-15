using MedicalBackend.Entities;

namespace MedicalBackend.Repositories.Abstractions;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAll();
    Task<TEntity> GetById(Guid id);
    Task<TEntity> Create(TEntity newObject);
    Task<TEntity> Edit(TEntity newObject);
    Task<string?> Delete(Guid id);
}