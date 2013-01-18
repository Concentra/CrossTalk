using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;

namespace Crosstalk.Binders
{
    public class ObjectIdModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            return result == null ? ObjectId.Empty : ObjectId.Parse((string) result.ConvertTo(typeof(string)));
        }
    }
}