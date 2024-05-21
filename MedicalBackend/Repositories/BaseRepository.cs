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
        return _table.Where(t=>!t.IsDeleted).ToListAsync();
    }

    public async Task<TEntity> GetById(Guid id)
    {
        var entry = await _table.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
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

    public async Task<TEntity> Edit(TEntity newObject)
    { 
        var response = _table.Attach(newObject);
        _table.Entry(newObject).State = EntityState.Modified;
        await Save();
        return response.Entity;
    }

    public async Task<string?> Delete(Guid id)
    {
        var entry = await _table.FirstOrDefaultAsync(t => t.Id == id);
        if (entry is null)
        {
            return null;
        }

        entry.IsDeleted = true;
        await Save();
        return "Deleted";
    }

    public async Task Save()
    {
        await _dbContext.SaveChangesAsync();
    }
    
}