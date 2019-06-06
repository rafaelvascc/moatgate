using MoatGate.Core.DomainModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoatGate.Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : IBaseDomainEntity
    {
        T GetById(Guid id);
        IList<T> GetAll();
        IList<T> GetByPredicate(Func<T, bool> predicate);
        T Add(T obj);
        IList<T> AddMany(IList<T> objs);
        T Update(T obj);
        void Delete(Guid id);
        void DeleteMany(IList<Guid> ids);
        void Delete(T obj);
        void DeleteMany(IList<T> objs);

        Task<T> GetByIdAsync(Guid id);
        Task<IList<T>> GetAllAsync();
        Task<IList<T>> GetByPredicateAsync(Func<T, bool> predicate);
        Task<T> AddAsync(T obj);
        Task<IList<T>> AddManyAsync(IList<T> objs);
        Task<T> UpdateAsync(T obj);
        Task DeleteAsync(Guid id);
        Task DeleteManyAsync(IList<Guid> ids);
        Task DeleteAsync(T obj);
        Task DeleteManyAsync(IList<T> objs);
    }
}
