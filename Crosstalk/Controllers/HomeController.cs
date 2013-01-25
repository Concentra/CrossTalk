using System.Web.Mvc;
using Crosstalk.Core.App_Start;
using Crosstalk.Core.Models;
using Crosstalk.Core.Repositories;
using Crosstalk.Core.Services;
using MongoDB.Bson;

namespace Crosstalk.Core.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Bootstrap()
        {
            var mClient = MongoConfig.GetDb();
            mClient.GetCollection("indentity").RemoveAll();
            mClient.GetCollection("messages").RemoveAll();

            var gClient = Neo4JConfig.GetClient();
            gClient.Connect();
            gClient.ExecuteScalarGremlin("g.V.each{g.removeVertex(it)}", null);

            var idSrv = new IdentityService(
                new IdentityRepository(MongoConfig.GetDb()),
                new EdgeRepository(gClient),
                gClient
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
            var gClient = Neo4JConfig.GetClient();
            gClient.Connect();
            return new ContentResult {Content = gClient.ExecuteScalarGremlin("g.V.Id", null).ToJson()};
        }
    }
}
