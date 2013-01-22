using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Crosstalk.App_Start;
using Crosstalk.Models;
using Crosstalk.Repositories;
using Crosstalk.Services;
using MongoDB.Bson;
using Neo4jClient;

namespace Crosstalk.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Bootstrap()
        {
            var gClient = Neo4JConfig.GetClient();
            gClient.Connect();
            gClient.ExecuteScalarGremlin("g.V.each{g.removeVertex(it)}", null);

            var idSrv = new IdentityService(
                new IdentityRepository(MongoConfig.GetDb()),
                new EdgeRepository(gClient),
                gClient
                );
            // Create some identities
            var root = idSrv.CreateIdentity("root");

            var trustA = idSrv.CreateIdentityWithAssociation("trustA", root);
            var trustB = idSrv.CreateIdentityWithAssociation("trustB", root);
            var trustC = idSrv.CreateIdentityWithAssociation("trustC", root);

            var groupA = idSrv.CreateIdentityWithAssociation("groupA", trustA);
            var groupB = idSrv.CreateIdentityWithAssociation("groupB", trustA);
            var groupC = idSrv.CreateIdentityWithAssociation("groupC", trustB);
            var groupD = idSrv.CreateIdentityWithAssociation("groupD", trustB);
            var groupE = idSrv.CreateIdentityWithAssociation("groupE", trustC);

            var idA = idSrv.CreateIdentityWithAssociation("idA", groupA);
            var idB = idSrv.CreateIdentityWithAssociation("idB", groupB);
            var idC = idSrv.CreateIdentityWithAssociation("idC", groupB);
            var idD = idSrv.CreateIdentityWithAssociation("idD", groupB);
            var idE = idSrv.CreateIdentityWithAssociation("idE", groupB);
            var idF = idSrv.CreateIdentityWithAssociation("idF", groupC);
            var idG = idSrv.CreateIdentityWithAssociation("idG", groupD);
            var idH = idSrv.CreateIdentityWithAssociation("idH", groupE);

            return new ContentResult {Content = "ok"};
        }
    }
}
