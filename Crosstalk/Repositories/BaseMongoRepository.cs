using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Crosstalk.Repositories
{
    public abstract class BaseMongoRepository<TDocument>
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

        protected abstract string Collection { get; }
    }
}