using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Crosstalk.Repositories
{
    public abstract class MongoRepository<TDocument>
    {
        private MongoDatabase _database;

        public MongoRepository(MongoDatabase database)
        {
            this._database = database;
        }

        protected MongoCollection<TDocument> GetCollection()
        {
            return this._database.GetCollection<TDocument>(this.GetCollectionName());
        }

        protected abstract string GetCollectionName();
    }
}