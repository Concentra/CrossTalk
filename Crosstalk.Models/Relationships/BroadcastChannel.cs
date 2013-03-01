using Crosstalk.Core.Models.Channels;
using Neo4jClient;

namespace Crosstalk.Core.Models.Relationships
{
    public class BroadcastChannel : BaseChannel
    {
        public BroadcastChannel(NodeReference targetNode) : base(targetNode) {}

        public override string TypeKey
        {
            get { return ChannelType.Labels.Public; }
        }
    }
}