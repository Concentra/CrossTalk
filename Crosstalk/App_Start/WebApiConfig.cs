using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Crosstalk.Core.Handlers;
using Crosstalk.Core.Repositories;
using MongoDB.Driver;
using Neo4jClient;

namespace Crosstalk.Core.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new
                    {
                        controller = "messages|edge"
                    }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Dependency Injection

            var builder = new ContainerBuilder();

            builder.RegisterInstance(((Func<MongoDatabase>)(() =>
            {
                var parts = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString.Split('/');
                if (parts.Length < 2)
                {
                    throw new ConfigurationErrorsException("The connection string for MongoDB is incorrectly formatted, it should be: \"host:port/database\"");
                }
                var server = (new MongoClient("mongodb://" + parts[0])).GetServer();
                return server.GetDatabase(parts[1]);
            }))());

            builder.RegisterInstance(((Func<IGraphClient>) (() =>
                {
                    var client =
                        new GraphClient(new Uri(ConfigurationManager.ConnectionStrings["Neo4J"].ConnectionString), false,
                                        false);
                    client.Connect();
                    return client;
                }))());

            builder.Register<IMessageRepository>(c => new MessageRepository(c.Resolve<MongoDatabase>()));
            builder.Register<IEdgeRepository>(c => new EdgeRepository(c.Resolve<IGraphClient>()));
            builder.Register<IIdentityRepository>(c => new IdentityRepository(c.Resolve<MongoDatabase>()));

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
