using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Repositories;
using Miscellaneous;

namespace Data.Repositories
{
    public abstract partial class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            this.Context = context;
        }
        public async Task AddAsync(TEntity entity)
        {
            await Context.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await Context.Set<TEntity>().AddRangeAsync(entities);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetPagedAsync(int page, int pageSize, string order, string sort)
        {
            IEnumerable<TEntity> entities;

            int skipRows = (Math.Max(0, page - 1)) * pageSize;

            if (order.ToUpper() == "DESC")
            {
                entities = await Context.Set<TEntity>().OrderByDescending(sort).Skip(skipRows).Take(pageSize).ToListAsync();
            }
            else 
            {
                entities = await Context.Set<TEntity>().OrderBy(sort).Skip(skipRows).Take(pageSize).ToListAsync();  
            }
            
            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return  await Context.Set<TEntity>().ToListAsync();
        }

        public async ValueTask<TEntity> GetByIdAsync(int id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public void Update(TEntity entity)
        {
            Context.Set<TEntity>().Update(entity);
        }

        public abstract ValueTask<TEntity> GetFullByIdAsync(int id);
    }
}