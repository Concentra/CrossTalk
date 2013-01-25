using System.Collections;
using System.Collections.Generic;
using Crosstalk.Models;
using MongoDB.Bson;

namespace Crosstalk.Repositories
{
    public interface IIdentityRepository
    {
        IIdentityRepository Save(Identity identity);
        Identity GetById(string id);
        Identity GetById(ObjectId id);
        IEnumerable<TItem> BindPartials<TItem>(IEnumerable<TItem> items, IEnumerable<string> fields);
        TItem BindPartial<TItem>(TItem item, IEnumerable<string> fields);
    }
}
