using System;
using System.Collections.Generic;

namespace Crosstalk.Common.Repositories
{
    public interface IMongoRepository<TDocument>
    {
        IEnumerable<object> Select(Func<TDocument, object> projection);
        IEnumerable<TDocument> Where(Func<TDocument, bool> predicate); 
        bool Insert(TDocument model);
        bool Delete(TDocument model);
    }
}
