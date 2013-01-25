using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using Crosstalk.Exceptions;
using Crosstalk.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Crosstalk.Repositories
{
    public class IdentityRepository : BaseMongoRepository<Identity>, IIdentityRepository
    {
        public IdentityRepository(MongoDatabase database) : base(database)
        {
        }

        protected override string Collection
        {
            get { return "identity"; }
        }

        public IIdentityRepository Save(Identity identity)
        {
            if (!identity.HasId())
            {
                identity.OId = ObjectId.GenerateNewId();
            }
            if (!this.GetCollection().Insert(identity).Ok)
            {
                throw new IOException("Could not save identity");
            }
            return this;
        }

        public Identity GetById(string id)
        {
            ObjectId oid;
            return ObjectId.TryParse(id, out oid) ? this.GetById(oid) : null;
        }

        public Identity GetById(ObjectId id)
        {
            var item = this.GetCollection().AsQueryable().SingleOrDefault(i => i.Id == id.ToString());
            if (null == item)
            {
                throw new ObjectNotFoundException<Identity>(id);
            }
            return item;
        }

        public IEnumerable<TItem> BindPartials<TItem>(IEnumerable<TItem> items, IEnumerable<string> fields)
        {
            var enumeratedFields = fields as IList<string> ?? fields.ToList();
            var result = new List<TItem>();
            foreach (var item in items)
            {
                this.BindPartial(item, enumeratedFields);
                result.Add(item);
            }
            return result;
        }

        public TItem BindPartial<TItem>(TItem item, IEnumerable<string> fields)
        {
            foreach (var property in fields
                    .Select(field => item.GetType().GetField(field))
                    .Where(property => null != property && (null != property.GetValue(item) as Identity)))
            {
                property.SetValue(item, this.GetById(((Identity)property.GetValue(item)).Id));
            }
            return item;
        }
    }
}