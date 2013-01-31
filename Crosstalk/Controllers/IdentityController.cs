using System;
using System.Collections.Generic;
using System.Web.Http;
using Crosstalk.Core.Models;
using Crosstalk.Core.Repositories;
using MongoDB.Bson;

namespace Crosstalk.Core.Controllers
{
    public class IdentityController : ApiController
    {
        private readonly IIdentityRepository _repository;

        public IdentityController(IIdentityRepository repository)
        {
            this._repository = repository;
        }

        public Identity GetById(string id)
        {
            return this._repository.GetById(id);
        }

        [HttpGet]
        public IEnumerable<Identity> Search(string field, string value)
        {
            return this._repository.Filter(s => true);
            //return this._repository.Filter(i => null != i.Data && i.Data.Contains(field) && value == i.Data.GetValue(field));
        }

        public Identity Post([FromBody] Identity model)
        {
            model.OId = ObjectId.GenerateNewId();
            this._repository.Save(model);
            return model;
        }

        public Identity Post(string id, [FromBody] Dictionary<string, object> model)
        {
            var identity = this._repository.GetById(id);
            var type = identity.GetType();
            foreach (var kv in model)
            {
                var field = type.GetField(kv.Key);
                if (null != field)
                {
                    field.SetValue(identity, kv.Value);
                }
            }
            this._repository.Save(identity);
            return identity;
        }

    }
}
