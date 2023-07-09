using DemoBed.Base.Data;
using DemoBed.Base.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoBed.Services.Payment.Data
{
    public class Repository<T>: IRepository<T> where T: AuditableEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _table;

        public Repository(DbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public T? GetById(int id)
        {
            var entity = _table
                .Where(e => e.IsDeleted == false && e.Id == id)
                .Single();
            return entity;
        }

        public async Task<T?> GetByIdAsync(int id, 
            CancellationToken cancellationToken = default)
        { 
            return await _table
                .Where(e => e.IsDeleted == false && e.Id == id)
                .SingleAsync(cancellationToken);
        }

        public IEnumerable<T?> GetAll()
        {
            return _table.Where(e => e.IsDeleted == false).ToList();
        }

        public async Task<IEnumerable<T?>> 
            GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _table
                .Where(e => e.IsDeleted == false)
                .ToListAsync(cancellationToken);
        }

        public IQueryable<T> GetAllQueryable()
        {
            return _table.Where(e => e.IsDeleted == false).AsQueryable();
        }

        public bool Add(T entity)
        {
            _table.Attach(entity);
            _context.SaveChanges();

            return true;
        }

        public async Task<bool> AddAsync(T entity, 
            CancellationToken cancellationToken = default)
        {
            _table.Attach(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _table.AttachRange(entities);
            _context.SaveChanges();

            return entities;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, 
            CancellationToken cancellationToken = default)
        {
            _table.AttachRange(entities);
            await _context.SaveChangesAsync(cancellationToken);

            return entities;
        }

        public bool Update(T entity)
        {
            _table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            _context.SaveChanges();

            return true;
        }

        public async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public bool Remove(T entity)
        {
            _table.Remove(entity);
            _context.SaveChanges();

            return true;
        }

        public async Task<bool> RemoveAsync(T entity, CancellationToken cancellationToken = default)
        {
            _table.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public bool RemoveRange(IEnumerable<T> entities)
        {
            _table.RemoveRange(entities);
            _context.SaveChanges();

            return true;
        }

        public async Task<bool> RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            _table.RemoveRange(entities);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
