using MedicalBackend.Database;
using MedicalBackend.Entities;
using MedicalBackend.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MedicalBackend.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : Entity
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<TEntity> _table;
    
    public BaseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _table = _dbContext.Set<TEntity>();
    }

    public Task<List<TEntity>> GetAll()
    {
        return _table.ToListAsync();
    }

    public async Task<TEntity> GetById(Guid id)
    {
        var entry = await _table.FirstOrDefaultAsync(t => t.Id == id);
        if (entry is null)
        {
            return null;
        }
        return entry;
    }

    public async Task<TEntity> Create(TEntity newObject)
    {
        newObject.Id = new Guid();
        var response = await _table.AddAsync(newObject);
        await Save();
        return response.Entity;
    }

    public Task<TEntity> Edit(TEntity newObject)
    {
        throw new NotImplementedException();
    }

    public async Task<string?> Delete(Guid id)
    {
        var entry = await _table.FirstOrDefaultAsync(t => t.Id == id);
        if (entry is null)
        {
            return null;
        }
        _table.Remove(entry);
        await Save();
        return "Deleted";
    }

    public async Task Save()
    {
        await _dbContext.SaveChangesAsync();
    }
    
}