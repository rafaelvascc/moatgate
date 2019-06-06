using MoatGate.Core.Interfaces.Repositories;
using MoatGate.Core.DomainModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.Repositories.MongoDb
{
    public class GenericMongoDbRepository<T> : IGenericRepository<T> where T : IBaseDomainEntity
    {
        protected MongoClient _mongoClient;
        protected IMongoDatabase _mongoDatabase;
        protected IMongoCollection<T> _mongoCollection;

        public GenericMongoDbRepository(MongoDbConnectionOptions connectionOptions)
        {
            if (connectionOptions == null)
                throw new ArgumentNullException(nameof(connectionOptions));

            if (string.IsNullOrWhiteSpace(connectionOptions.ConnectionString))
                throw new ArgumentNullException(nameof(connectionOptions.ConnectionString));

            if (string.IsNullOrWhiteSpace(connectionOptions.DatabaseName))
                throw new ArgumentNullException(nameof(connectionOptions.DatabaseName));

            _mongoClient = new MongoClient(connectionOptions.ConnectionString);
            _mongoDatabase = _mongoClient.GetDatabase(connectionOptions.DatabaseName);
            _mongoCollection = _mongoDatabase.GetCollection<T>($"{typeof(T).Name.ToLower()}_collection");
        }

        public T Add(T obj)
        {
            _mongoCollection.InsertOne(obj);
            return obj;
        }

        public async Task<T> AddAsync(T obj)
        {
            await _mongoCollection.InsertOneAsync(obj);
            return obj;
        }

        public IList<T> AddMany(IList<T> objs)
        {
            _mongoCollection.InsertMany(objs);
            return objs;
        }

        public async Task<IList<T>> AddManyAsync(IList<T> objs)
        {
            await _mongoCollection.InsertManyAsync(objs);
            return objs;
        }

        public void Delete(Guid id)
        {
            _mongoCollection.FindOneAndDelete((e) => e.Id == id);
        }

        public void Delete(T obj)
        {
            _mongoCollection.FindOneAndDelete((e) => e.Id == obj.Id);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _mongoCollection.FindOneAndDeleteAsync((e) => e.Id == id);
        }

        public async Task DeleteAsync(T obj)
        {
            await _mongoCollection.FindOneAndDeleteAsync((e) => e.Id == obj.Id);
        }

        public void DeleteMany(IList<Guid> ids)
        {
            _mongoCollection.DeleteMany((e) => ids.Contains(e.Id));
        }

        public void DeleteMany(IList<T> objs)
        {
            var ids = objs.Select(o => o.Id).ToList();
            _mongoCollection.DeleteMany((e) => ids.Contains(e.Id));
        }

        public async Task DeleteManyAsync(IList<Guid> ids)
        {
            await _mongoCollection.DeleteManyAsync((e) => ids.Contains(e.Id));
        }

        public async Task DeleteManyAsync(IList<T> objs)
        {
            var ids = objs.Select(o => o.Id).ToList();
            await _mongoCollection.DeleteManyAsync((e) => ids.Contains(e.Id));
        }

        public IList<T> GetAll()
        {
            return _mongoCollection.Find(Builders<T>.Filter.Empty).ToList();
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _mongoCollection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        public T GetById(Guid id)
        {
            return _mongoCollection.Find(e => e.Id == id).SingleOrDefault();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _mongoCollection.Find(e => e.Id == id).SingleOrDefaultAsync();
        }

        public IList<T> GetByPredicate(Func<T, bool> predicate)
        {
            return _mongoCollection.Find(e => predicate.Invoke(e)).ToList();
        }

        public async Task<IList<T>> GetByPredicateAsync(Func<T, bool> predicate)
        {
            return await _mongoCollection.Find(e => predicate.Invoke(e)).ToListAsync();
        }

        public T Update(T obj)
        {
            return _mongoCollection.FindOneAndReplace(e => e.Id == obj.Id, obj);
        }

        public async Task<T> UpdateAsync(T obj)
        {
            return await _mongoCollection.FindOneAndReplaceAsync(e => e.Id == obj.Id, obj);
        }
    }
}
