using System;
using System.Collections.Generic;
using System.Web.Http;
using Autofac;
using Crosstalk.Core.Models;
using Crosstalk.Core.Repositories;
using MongoDB.Bson;
using Neo4jClient;

namespace Crosstalk.Core.Controllers
{
    public class IdentityController : ApiController
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IGraphClient _graphClient;

        public IdentityController(IIdentityRepository identityRepository, IGraphClient graphClient)
        {
            this._identityRepository = identityRepository;
            this._graphClient = graphClient;
        }

        public Identity GetById(string id)
        {
            var identity = this._identityRepository.GetById(id);
            return identity;
        }

        [HttpGet]
        public IEnumerable<Identity> Search(string field, string value)
        {
            return this._identityRepository.Filter(i =>
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
                });
        }

        public Identity Post([FromBody] Identity model)
        {
            model.OId = ObjectId.GenerateNewId();
            model.GraphId = this._graphClient.Create(model.ToGraphIdentity()).Id;
            this._identityRepository.Save(model);
            return model;
        }

        public Identity Post(string id, [FromBody] Dictionary<string, object> model)
        {
            var identity = this._identityRepository.GetById(id);
            var type = identity.GetType();
            foreach (var kv in model)
            {
                var field = type.GetField(kv.Key);
                if (null != field)
                {
                    field.SetValue(identity, kv.Value);
                }
            }
            this._identityRepository.Save(identity);
            return identity;
        }

    }
}
