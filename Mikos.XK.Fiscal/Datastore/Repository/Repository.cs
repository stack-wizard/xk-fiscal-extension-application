using Mikos.XK.Fiscal.Migrations;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace Mikos.XK.Fiscal.Datastore.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly FiscalContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(FiscalContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public T GetById(long id)
        {
            return _dbSet.Find(id);
        }

        public void Insert(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(long id)
        {
            T entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
