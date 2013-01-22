using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Neo4jClient;

namespace Crosstalk.App_Start
{
    public class Neo4JConfig
    {
        private static GraphClient _client;

        public static GraphClient GetClient()
        {
            return Neo4JConfig._client ??
                   (Neo4JConfig._client =
                    new GraphClient(new System.Uri(ConfigurationManager.ConnectionStrings["Neo4J"].ConnectionString), false, false));
        }
    }
}