using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    public class BlaBlogRepository<T> : IStagingRepository<T> where T : class
    {
        private readonly DbContext _context;
        private DbSet<T> _enteties;
        public BlaBlogRepository(DataContext context)
        {
            _context = context;
        }


        public object GetContext()=> _context;


        public IQueryable<T> Table { get { return Enteties; } }
        public IQueryable<T> TableNoTracking { get { return Enteties.AsNoTracking(); } }
        protected virtual DbSet<T> Enteties
        {
            get
            {

                if (_enteties == null)
                    _enteties = _context.Set<T>();
                return _enteties;
            }
        }

        public void Delete(T obj)
        {
            _enteties.Remove(obj);
            _context.SaveChanges();
        }
        public T GetById(object id) => Enteties.Find(id);


        public void Insert(T obj)
        {
            Enteties.Add(obj);
            _context.SaveChanges();
        }

        public void Update(T obj)
        {
            _context.SaveChanges();
        }
    }

}
