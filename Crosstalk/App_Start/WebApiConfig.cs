using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Crosstalk.Core.Handlers;
using Crosstalk.Core.Repositories;

namespace Crosstalk.Core.App_Start
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

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new {id = RouteParameter.Optional}
            );

            // Dependency Injection

            var builder = new ContainerBuilder();

            builder.RegisterInstance<IMessageRepository>(new MessageRepository(MongoConfig.GetDb()));
            builder.RegisterInstance<IEdgeRepository>(new EdgeRepository(Neo4JConfig.GetClient()));
            builder.RegisterInstance<IIdentityRepository>(new IdentityRepository(MongoConfig.GetDb()));

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .Where(t => !t.IsAbstract && typeof(ApiController).IsAssignableFrom(t))
                   .InstancePerMatchingLifetimeScope(AutofacWebApiDependencyResolver.ApiRequestTag);
            
            var container = builder.Build();
            
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            // Filters

            config.Filters.Add(new ObjectNotFoundExceptionAttribute());


        }
    }
}
