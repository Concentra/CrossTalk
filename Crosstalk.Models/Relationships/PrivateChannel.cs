using Crosstalk.Core.Models.Channels;
using Neo4jClient;

namespace Crosstalk.Core.Models.Relationships
{
    public class PrivateChannel : BaseChannel
    {
        public PrivateChannel(NodeReference targetNode) : base(targetNode) {}

        public override string TypeKey { get { return ChannelType.Labels.Public; } }
    }
}