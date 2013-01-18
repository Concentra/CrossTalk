using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Crosstalk.App_Start;
using Crosstalk.Binders;
using Crosstalk.Repositories;

namespace Crosstalk
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Dependency Injection

            var builder = new ContainerBuilder();

            builder.RegisterInstance<IMessageRepository>(new MessageRepository(MongoConfig.GetDb()));

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .Where(t => !t.IsAbstract && typeof(ApiController).IsAssignableFrom(t))
                   .InstancePerMatchingLifetimeScope(AutofacWebApiDependencyResolver.ApiRequestTag);;
            
            var container = builder.Build();
            
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            // Binders

            ModelBinders.Binders.Add(typeof(MongoDB.Bson.ObjectId), new ObjectIdModelBinder());

        }
    }
}
