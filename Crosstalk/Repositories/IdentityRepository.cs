using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Crosstalk.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Crosstalk.Repositories
{
    public class IdentityRepository : BaseMongoRepository<Identity>, IIdentityRepository
    {
        public IdentityRepository(MongoDatabase database) : base(database) {}

        protected override string Collection {
            get { return "identity"; }
        }

        public IIdentityRepository Save(Identity identity)
        {
            if (ObjectId.Empty == identity.Id)
            {
                identity.Id = ObjectId.GenerateNewId();
            }
            if (!this.GetCollection().Insert(identity).Ok)
            {
                throw new IOException("Could not save identity");
            }
            return this;
        }

        public Identity GetById(ObjectId id)
        {
            return this.GetCollection().AsQueryable().Single(i => i.Id == id);
        }
    }
}