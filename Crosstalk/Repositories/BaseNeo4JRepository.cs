using Neo4jClient;

namespace Crosstalk.Core.Repositories
{
    public abstract class BaseNeo4JRepository
    {
        private readonly IGraphClient _client;
        private bool _isConnected = false; 

        protected BaseNeo4JRepository(IGraphClient client)
        {
            this._client = client;
        }

        private IGraphClient GetClient()
        {
            if (!this._isConnected)
            {
                ((GraphClient) this._client).Connect();
                this._isConnected = true;
            }
            return this._client;
        }

        protected GraphClient Client
        {
            get { return (GraphClient) this.GetClient(); }
        }
    }
}