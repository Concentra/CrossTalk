using Crosstalk.Core.Models.Messages.Channels;
using Neo4jClient;

namespace Crosstalk.Core.Models.Messages.Relationships
{
    public class PrivateChannel : BaseChannel
    {
        public PrivateChannel(NodeReference targetNode) : base(targetNode) {}

        public override string TypeKey { get { return ChannelType.Labels.Private; } }
    }
}