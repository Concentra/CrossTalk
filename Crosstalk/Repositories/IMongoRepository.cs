using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Core.Repositories
{
    public interface IMongoRepository<TDocument>
    {
        IEnumerable<object> Select(Func<TDocument, object> projection);
        IEnumerable<TDocument> Where(Func<TDocument, bool> predicate); 
        bool Insert(TDocument model);
        bool Delete(TDocument model);
    }
}
