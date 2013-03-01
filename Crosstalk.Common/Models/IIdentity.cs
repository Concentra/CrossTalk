using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Crosstalk.Common.Models
{
    public interface IIdentity : ISupportsPartial
    {
        string Name { get; set; }
        string AvatarUrl { get; set; }
        string Type { get; set; }
    }

    public interface IIdentity<TCollection> : IIdentity
    {
        TCollection Data { get; set; }
    }

    public static class IdentityExtensions
    {
        public static bool HasId(this IIdentity identity)
        {
            return ObjectId.Parse(identity.Id) != ObjectId.Empty;
        }
    }
}
