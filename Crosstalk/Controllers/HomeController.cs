using System.Web.Mvc;
using Crosstalk.Core.App_Start;
using Crosstalk.Core.Models.Messages;
using Crosstalk.Core.Repositories;
using Crosstalk.Core.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using Neo4jClient;

namespace Crosstalk.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly MongoDatabase _database;
        private readonly IGraphClient _graphClient;

        public HomeController()
        {
            this._database = DependencyResolver.Current.GetService<MongoDatabase>();
            this._graphClient = DependencyResolver.Current.GetService<IGraphClient>();
        }

        public ActionResult Index()
        {
            return new HttpNotFoundResult();
        }

        public ActionResult Bootstrap()
        {
            _database.GetCollection("indentity").RemoveAll();
            _database.GetCollection("messages").RemoveAll();

            _graphClient.ExecuteScalarGremlin("g.V.each{g.removeVertex(it)}", null);

            var idSrv = new BootstrapService(
                new IdentityRepository(_database),
                new EdgeRepository(_graphClient),
                (GraphClient) _graphClient
                );
            // Create some identities
            var root = idSrv.CreatePublic();

            var trustA = idSrv.CreateGroup("NHS Trust", null, new Identity[] {root});
            var trustB = idSrv.CreateGroup("NHS Trust", null, new Identity[] { root });
            var trustC = idSrv.CreateGroup("NHS Trust", null, new Identity[] { root });

            var groupA = idSrv.CreateGroup("Expectant Mothers", null, new Identity[] {root, trustA});
            var groupB = idSrv.CreateGroup("Breastfeeding", null, new Identity[] { root, trustA });
            var groupC = idSrv.CreateGroup("Baby Café - Islington", null, new Identity[] { root, trustB });
            var groupD = idSrv.CreateGroup("Baby Café - Chelsea", null, new Identity[] { root, trustB });
            var groupE = idSrv.CreateGroup("Coffee Group", null, new Identity[] { root, trustC });

            var idA = idSrv.CreatePerson("Alison", "/images/avatars/1.jpg", new Identity[] { root, groupA, groupB }, new Identity[] { groupA, groupB });
            var idB = idSrv.CreatePerson("Amy", "/images/avatars/1.jpg", new Identity[] { root, groupA, groupB }, new Identity[] { groupA, groupB });
            var idC = idSrv.CreatePerson("Eve", "/images/avatars/1.jpg", new Identity[] { root, groupA, groupC }, new Identity[] { groupA, groupC });
            var idD = idSrv.CreatePerson("Steve", "/images/avatars/1.jpg", new Identity[] { root, groupC, idC }, new Identity[] { groupC, idC });
            var idE = idSrv.CreatePerson("Jon", "/images/avatars/1.jpg", new Identity[] { root, groupE, groupD }, new Identity[] { groupE, groupD });
            var idF = idSrv.CreatePerson("Theo", "/images/avatars/1.jpg", new Identity[] { root, groupD, groupE, idE }, new Identity[] { groupD, groupE, idE });
            var idG = idSrv.CreatePerson("Lydia", "/images/avatars/1.jpg", new Identity[] { root, groupA, groupB, idA, groupC, idC }, new Identity[] { groupA, groupB, idA, groupC, idC });
            var idH = idSrv.CreatePerson("Connor", "/images/avatars/1.jpg", new Identity[] { root, groupD, groupE, idE, idF }, new Identity[] { groupD, groupE, idE, idF });

            return new ContentResult {Content = "ok"};
        }

        public ActionResult Dump()
        {
            return new ContentResult {Content = _graphClient.ExecuteScalarGremlin("g.V.Id", null).ToJson()};
        }
    }
}
