using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Neo4jClient;

namespace Crosstalk.Repositories
{
    public abstract class BaseNeo4JRepository
    {
        private readonly GraphClient _client;

        protected BaseNeo4JRepository(GraphClient client)
        {
            this._client = client;
        }

        protected GraphClient GetClient()
        {
            this._client.Connect();
            return this._client;
        }
    }
}