using System.Web;
using Crosstalk.Core.Models.Messages;
using Crosstalk.Core.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Crosstalk.Common.Models;

namespace Crosstalk.Core.Controllers
{
    public class CommentController : ApiController
    {

        private readonly ICommentRepository _repository;

        public CommentController(ICommentRepository repository)
        {
            this._repository = repository;
        }

        [HttpPost]
        public void Save(JObject obj)
        {
            var comment = obj.ToObject<Comment>();
            this._repository.Save(comment);
        }

        public dynamic Get()
        {
            var nvc = HttpUtility.ParseQueryString(this.Request.RequestUri.Query);
            if (nvc.AllKeys.Contains("id"))
            {
                return this._repository.GetById(nvc["id"]);
            }
            return this._repository.Search(nvc);
        }

        public void Delete(string id, string action)
        {
            if (ReportRevokeActionType.Revoke == action)
            {
                this._repository.Revoke(id);
            }
            else if (ReportRevokeActionType.Moderate == action)
            {
                this._repository.Moderate(id);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Action is not recognised", "action");
            }
        }

    }
}
