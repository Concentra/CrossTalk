using System.Configuration;
using Neo4jClient;

namespace Crosstalk.Core.App_Start
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