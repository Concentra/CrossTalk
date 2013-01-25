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
        private bool _isConnected = false; 

        protected BaseNeo4JRepository(GraphClient client)
        {
            this._client = client;
        }

        protected GraphClient GetClient()
        {
            if (!this._isConnected)
            {
                this._client.Connect();
                this._isConnected = true;
            }
            return this._client;
        }

        protected GraphClient Client
        {
            get { return this.GetClient(); }
        }
    }
}