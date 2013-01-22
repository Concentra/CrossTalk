using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crosstalk.Models;
using MongoDB.Bson;

namespace Crosstalk.Repositories
{
    public interface IIdentityRepository
    {
        IIdentityRepository Save(Identity identity);
        Identity GetById(ObjectId id);
    }
}
