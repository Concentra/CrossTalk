using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Wrappers;

namespace Crosstalk.Common.Repositories
{
    public abstract class BaseMongoRepository<TDocument>: IMongoRepository<TDocument>
    {
        private readonly MongoDatabase _database;

        protected BaseMongoRepository(MongoDatabase database)
        {
            this._database = database;
        }

        protected MongoCollection<TDocument> GetCollection()
        {
            return this._database.GetCollection<TDocument>(this.Collection);
        }

        public bool Insert(TDocument document)
        {
            return this.GetCollection().Insert(document).Ok;
        }

        public IEnumerable<TDocument> Where(Func<TDocument, bool> predicate)
        {
            return this.GetCollection().AsQueryable().Where(predicate);
        }

        public IEnumerable<object> Select(Func<TDocument, object> projection)
        {
            return this.GetCollection().AsQueryable().Select(projection);
        }

        public bool Delete(TDocument document)
        {
            return this.GetCollection().Remove(new QueryWrapper(document)).Ok;
        }

        protected abstract string Collection { get; }
    }
}