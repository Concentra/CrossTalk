using Neo4jClient;
using Crosstalk.Core.Models.Channels;

namespace Crosstalk.Core.Models.Relationships
{
    public class RequestChannel : BaseChannel
    {
        public RequestChannel(NodeReference targetNode) : base(targetNode) {}

        public override string TypeKey
        {
            get { return ChannelType.Labels.Request; }
        }
    }
}
