using System.Web.Http;
using Crosstalk.Core.Models;
using Crosstalk.Core.Repositories;

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
    }
}
