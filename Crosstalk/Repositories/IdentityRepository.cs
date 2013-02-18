using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Crosstalk.Common.Models;
using Crosstalk.Common.Repositories;
using Crosstalk.Core.Exceptions;
using Crosstalk.Core.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Crosstalk.Core.Repositories
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

        public Identity GetPublicSpace()
        {
            return this.GetCollection().AsQueryable().First(i => Identity.Public == i.Type);
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

        public IEnumerable<Identity> Filter(Func<Identity, bool> selector)
        {
            return this.GetCollection().AsQueryable<Identity>().Where(selector);
        }

        public IEnumerable<Identity> Search(string field, string value)
        {
            var type = typeof (Identity);
            if (null != type.GetProperty(field))
            {
                return from i in this.GetCollection().AsQueryable()
                       where Query.EQ(field, BsonValue.Create(value)).Inject()
                       select i;
            }
            return this.GetCollection().AsQueryable().Where((Func<Identity, bool>) (i =>
                {
                    if (null != i.Others && i.Others.Contains(field))
                    {
                        if (i.Others[field].IsBsonArray)
                        {
                            return -1 < i.Others[field].AsBsonArray.IndexOf(BsonDocument.Parse(value));
                        }
                        return i.Others[field].AsBsonDocument.ContainsValue(BsonDocument.Parse(value));
                    }
                    return false;
                }));
        }

        public IEnumerable<Identity> Search(NameValueCollection parameters)
        {
            var queries = new List<IMongoQuery>();
            
            foreach (var key in parameters.AllKeys)
            {
                var vals = parameters.GetValues(key);
                if (null == vals) continue;
                if (vals.Count() > 1)
                {
                    queries.Add(Query.In(key, vals.Select(BsonValue.Create)));
                } else if (vals.Count() == 1)
                {
                    queries.Add(Query.EQ(key, BsonValue.Create(vals.First())));
                }
            }

            return this.GetCollection().Find(Query.And(queries));
        }
    }
}