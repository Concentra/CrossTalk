using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Crosstalk.Common.Models;
using Crosstalk.Core.Models.Messages;
using MongoDB.Bson;

namespace Crosstalk.Core.Repositories
{
    public interface IIdentityRepository
    {
        IIdentityRepository Save(Identity identity);
        Identity GetById(string id);
        Identity GetById(ObjectId id);
        Identity GetPublicSpace();
        IEnumerable<TItem> BindPartials<TItem>(IEnumerable<TItem> items, IEnumerable<string> fields);
        TItem BindPartial<TItem>(TItem item, IEnumerable<string> fields);
        IEnumerable<Identity> Filter(Func<Identity, bool> selector);
        IEnumerable<Identity> Search(string field, string value);
        IEnumerable<Identity> Search(NameValueCollection parameters);
    }
}
