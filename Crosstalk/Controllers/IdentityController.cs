using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Crosstalk.Models;
using Crosstalk.Repositories;
using MongoDB.Bson;

namespace Crosstalk.Controllers
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
            return this._repository.GetById(ObjectId.Parse(id));
        }
    }
}
