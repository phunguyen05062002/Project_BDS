using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.DataContexts;
using Project_BDS.Domain.Entities;
using Project_BDS.Domain.InterfaceRepostories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.ImplementRepostories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected IDbContext _IDbContext;
        protected DbSet<TEntity> _dbSet;
        protected DbContext _dbContext;
        protected DbSet<TEntity> _entities
        {
            get
            {
                if (_dbSet == null)
                {
                    _dbSet = _dbContext.Set<TEntity>() as DbSet<TEntity>;
                }
                return _dbSet;
            }
        }
        public BaseRepository(IDbContext dbContext)
        {
            _IDbContext = dbContext;
            _dbContext = (DbContext)dbContext;
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression = null)
        {
            IQueryable<TEntity> query = expression != null ? _entities.Where(expression) : _entities;
            return await query.CountAsync();
        }

        public async Task<int> CountAsync(string include, Expression<Func<TEntity, bool>> expression = null)
        {
            IQueryable<TEntity> query;
            if (!string.IsNullOrEmpty(include))
            {
                query = BuildQueryable(new List<string> { include }, expression);
                return await query.CountAsync();
            }
            return await CountAsync(expression);
        }
        protected IQueryable<TEntity> BuildQueryable(List<string> includes, Expression<Func<TEntity, bool>> expression = null)
        {
            IQueryable<TEntity> query = _entities.AsQueryable();
            if (expression != null)
            {
                query = query.Where(expression);
            }
            if (includes != null && includes.Count > 0)
            {
                foreach (string include in includes)
                {
                    query = query.Include(include.Trim());
                }
            }
            return query;
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            _entities.Add(entity);
            await _IDbContext.ComitChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<TEntity>> CreateAsync(IEnumerable<TEntity> entities)
        {
            _entities.AddRange(entities);
            await _IDbContext.ComitChangesAsync();
            return entities;
        }

        public async Task DeleteAsync(int id)
        {
            var dataEntity = await _entities.FindAsync(id);
            if (dataEntity != null)
            {
                _entities.Remove(dataEntity);
                await _IDbContext.ComitChangesAsync();
            }
        }
       

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            IQueryable<TEntity> query = expression != null ? _entities.Where(expression) : _entities;
            var dataQuery = query;
            if (dataQuery != null)
            {
                _entities.RemoveRange(dataQuery);
                await _IDbContext.ComitChangesAsync();
            }
        }

        public async Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression = null)
        {
            IQueryable<TEntity> query = expression != null ? _entities.Where(expression) : _entities;
            return query;
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _entities.FirstOrDefaultAsync(expression);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<TEntity> GetByIdAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _entities.FirstOrDefaultAsync(expression);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _IDbContext.ComitChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity?> entities)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Modified | EntityState.Deleted;
            }
            await _IDbContext.ComitChangesAsync();
            return entities;
        }
        public async Task<bool> ValidateIdAsync(int id)
        {
            return await _entities.AnyAsync(e => EF.Property<int>(e, "Id") == id);
        }
        public async Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();
        }
    }
}
