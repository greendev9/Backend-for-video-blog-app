using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
    public partial interface  IStagingRepository<T> where T: class 
    { 
        T GetById(object id); 
        void Insert(T obj); 
        void Update(T obj); 
        void Delete(T obj); 
        IQueryable<T> Table { get; }  
        IQueryable<T> TableNoTracking  { get; } 

        object GetContext(); 
    }
}
