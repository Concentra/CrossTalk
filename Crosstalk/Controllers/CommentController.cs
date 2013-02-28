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

namespace Crosstalk.Core.Controllers
{
    public class CommentController : ApiController
    {

        private readonly IMessageRepository _repository;

        public CommentController(IMessageRepository repository)
        {
            this._repository = repository;
        }

        public void Save(JObject obj)
        {
            var comment = obj.ToObject<Comment>();
            if (null == comment.ParentMessage)
            {
                throw new Exception("ParentMessage is required to save comment");
            }
            var message = comment.ParentMessage.Value;
            message.Comments = null == message.Comments
                ? new List<Comment>()
                : (message.Comments as List<Comment>
                    ?? message.Comments.ToList<Comment>());
            (message.Comments as List<Comment>).Add(comment);
            this._repository.Save(message);
        }
    }
}
