using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Crosstalk.Core.Models.Messages;

namespace Crosstalk.Core.Repositories
{
    public interface ICommentRepository : IReportableRepository, IRevokableRepository
    {
        void Save(Comment comment);
        IEnumerable<Comment> Search(NameValueCollection parameters);
        Comment GetById(string id);
    }
}