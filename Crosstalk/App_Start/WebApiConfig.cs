using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Crosstalk.Common;
using Crosstalk.Common.Models;
using Crosstalk.Core.Handlers;
using Crosstalk.Core.Repositories;
using Crosstalk.Core.Services;
using MongoDB.Driver;
using Neo4jClient;
using Crosstalk.Common.Repositories;
using Crosstalk.Core.Models;
using Crosstalk.Core.Models.Messages;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;

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

            builder.Register<IRepositoryStore>(c => RepositoryStore.GetInstance()).SingleInstance();

            builder.Register<IMessageRepository>(c => new MessageRepository(c.Resolve<MongoDatabase>()));
            builder.Register<ICommentRepository>(c => new CommentRepository(c.Resolve<MongoDatabase>(), c.Resolve<IMessageRepository>()));
            builder.Register<IEdgeRepository>(c => new EdgeRepository(c.Resolve<IGraphClient>()));
            builder.Register<IIdentityRepository>(c => new IdentityRepository(c.Resolve<MongoDatabase>()));
            builder.Register<IReportRepository>(c => new ReportRepository(c.Resolve<MongoDatabase>()));

            builder.Register<IMessageService>(
                c => new MessageService(c.Resolve<IMessageRepository>(), c.Resolve<IEdgeService>()));
            builder.Register<IEdgeService>(
                c => new EdgeService(c.Resolve<IEdgeRepository>(), c.Resolve<IIdentityRepository>()));
            builder.RegisterType<ReportService>().As<IReportService>();

            //builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            //       .Where(t => !t.IsAbstract && (t.Name.EndsWith("Repository") || t.Name.EndsWith("Service")))
            //       .InstancePerMatchingLifetimeScope(AutofacWebApiDependencyResolver.ApiRequestTag);

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .Where(t => !t.IsAbstract && typeof(ApiController).IsAssignableFrom(t))
                   .InstancePerMatchingLifetimeScope(AutofacWebApiDependencyResolver.ApiRequestTag);
            
            var container = builder.Build();
            
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            // Filters

            config.Filters.Add(new ObjectNotFoundExceptionAttribute());

            // Model to Repository Mapping

            RepositoryStore.GetInstance()
                .Add<Edge>(() => container.Resolve<IEdgeRepository>())
                .Add<Message>(() => container.Resolve<IMessageRepository>())
                .Add<Identity>(() => container.Resolve<IIdentityRepository>());

            BsonClassMap.RegisterClassMap<Partial>(cm =>
                {
                    cm.MapIdProperty(c => c.Id);
                });

        }
    }
}
