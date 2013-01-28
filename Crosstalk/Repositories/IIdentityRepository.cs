using System;
using System.Collections.Generic;
using Crosstalk.Core.Models;
using MongoDB.Bson;

namespace Crosstalk.Core.Repositories
{
    public interface IIdentityRepository
    {
        IIdentityRepository Save(Identity identity);
        Identity GetById(string id);
        Identity GetById(ObjectId id);
        IEnumerable<TItem> BindPartials<TItem>(IEnumerable<TItem> items, IEnumerable<string> fields);
        TItem BindPartial<TItem>(TItem item, IEnumerable<string> fields);
        IEnumerable<Identity> Filter(Func<Identity, bool> selector);
    }
}
