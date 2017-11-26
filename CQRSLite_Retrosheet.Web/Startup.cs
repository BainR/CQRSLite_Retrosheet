using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CQRSlite.Commands;
using CQRSlite.Events;
using CQRSlite.Routing;
using CQRSlite.Domain;
using CQRSlite.Caching;
using AutoMapper;
using System.Reflection;
using CQRSlite.Messages;
using CQRSLite_Retrosheet.Domain.EventStore;
using CQRSLite_Retrosheet.Domain.CommandHandlers;
using System;
using CQRSLite_Retrosheet.Domain.ReadModel.Repositories;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
using CQRSLite_Retrosheet.Web.Filters;
using FluentValidation.AspNetCore;
using FluentValidation;
using CQRSLite_Retrosheet.Domain.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace CQRSLite_Retrosheet.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            LoggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }
        public ILoggerFactory LoggerFactory { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            #region Repository

            // https://radu-matei.github.io/blog/aspnet-core-json-dependency-injection/
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration

            // The following is a hybrid of the two patterns described above.  
            // A configuration section from applicationsettings.json both selects the instance type of the repository and sets its connections string.

            var repositoryConfig = Configuration.GetSection("Repository");

            string assemblyName = repositoryConfig.GetValue<string>("AssemblyName");
            string repositoryTypeName = repositoryConfig.GetValue<string>("RepositoryType");
            string connectionString = repositoryConfig.GetValue<string>("ConnectionString");
            string repoNS = repositoryConfig.GetValue<string>("RepositoryNameSpace");
            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            Type repositoryType = assembly.GetTypes().First(t => t.Name.StartsWith(repositoryTypeName));

            Type baseballPlayRepositoryType = repositoryType.MakeGenericType(typeof(BaseballPlayRM));
            services.AddSingleton<BaseballPlayRepository>(y => new BaseballPlayRepository(Activator.CreateInstance(baseballPlayRepositoryType, connectionString, repoNS, LoggerFactory) as IRepository<BaseballPlayRM>));

            Type lineupRepositoryType = repositoryType.MakeGenericType(typeof(LineupChangeRM));
            services.AddSingleton<LineupChangeRepository>(y => new LineupChangeRepository(Activator.CreateInstance(lineupRepositoryType, connectionString, repoNS, LoggerFactory) as IRepository<LineupChangeRM>));

            Type battingOrderRepositoryType = repositoryType.MakeGenericType(typeof(LineupRM));
            services.AddSingleton<LineupRepository>(y => new LineupRepository(Activator.CreateInstance(battingOrderRepositoryType, connectionString, repoNS, LoggerFactory) as IRepository<LineupRM>));

            Type teamRepositoryType = repositoryType.MakeGenericType(typeof(TeamRM));
            services.AddSingleton<TeamRepository>(y => new TeamRepository(Activator.CreateInstance(teamRepositoryType, connectionString, repoNS, LoggerFactory) as IRepository<TeamRM>));

            Type rosterMemberRepositoryType = repositoryType.MakeGenericType(typeof(RosterMemberRM));
            services.AddSingleton<RosterMemberRepository>(y => new RosterMemberRepository(Activator.CreateInstance(rosterMemberRepositoryType, connectionString, repoNS, LoggerFactory) as IRepository<RosterMemberRM>));

            Type playerRepositoryType = repositoryType.MakeGenericType(typeof(PlayerRM));
            services.AddSingleton<PlayerRepository>(y => new PlayerRepository(Activator.CreateInstance(playerRepositoryType, connectionString, repoNS, LoggerFactory) as IRepository<PlayerRM>));

            Type gameSummaryRepositoryType = repositoryType.MakeGenericType(typeof(GameSummaryRM));
            services.AddSingleton<GameSummaryRepository>(y => new GameSummaryRepository(Activator.CreateInstance(gameSummaryRepositoryType, connectionString, repoNS, LoggerFactory) as IRepository<GameSummaryRM>));

            #endregion

            services.AddAutoMapper(typeof(Startup));

            services.AddMemoryCache();

            //Add Cqrs services
            services.AddSingleton<Router>(new Router());
            services.AddSingleton<ICommandSender>(y => y.GetService<Router>());
            services.AddSingleton<IEventPublisher>(y => y.GetService<Router>());
            services.AddSingleton<IHandlerRegistrar>(y => y.GetService<Router>());
            services.AddScoped<CQRSlite.Domain.ISession, Session>();
            services.AddSingleton<IEventStore, InMemoryEventStore>(); 
            services.AddScoped<ICache, MemoryCache>();
            services.AddScoped<IRepository>(y => new CacheRepository(new Repository(y.GetService<IEventStore>()), y.GetService<IEventStore>(), y.GetService<ICache>()));

            // Don't want this.  Will use repository instead.
            // services.AddTransient<IReadModelFacade, ReadModelFacade>(); // https://github.com/gautema/CQRSlite/tree/master/Sample/CQRSCode/ReadModel

            //Scan for commandhandlers and eventhandlers
            services.Scan(scan => scan
                .FromAssemblies(typeof(RetrosheetCommandHandlers).GetTypeInfo().Assembly) // https://github.com/gautema/CQRSlite/blob/master/Sample/CQRSCode/WriteModel/Handlers/InventoryCommandHandlers.cs
                    .AddClasses(classes => classes.Where(x => {
                        var allInterfaces = x.GetInterfaces();
                        return
                            allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(IHandler<>)) ||
                            allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICancellableHandler<>));
                    }))
                    .AsSelf()
                    .WithTransientLifetime()
            );

            // Add framework services.
            // https://github.com/JeremySkinner/FluentValidation/issues/412

            services
                .AddMvc(options =>
                {
                    options.Filters.Add(typeof(ValidationActionFilter));
                })
                .AddFluentValidation(fv => { });
            // do not add validator for CreateBaseballPlayRequest
            services.AddTransient<IValidator<CreateTeamRequest>, CreateTeamRequestValidator>();
            services.AddTransient<IValidator<CreateRosterMemberRequest>, CreateRosterMemberRequestValidator>();
            services.AddTransient<IValidator<CreateLineupChangeRequest>, CreateLineupChangeRequestValidator>();
            services.AddTransient<IValidator<CreateGameSummaryRequest>, CreateGameSummaryRequestValidator>();

            //Register router
            var serviceProvider = services.BuildServiceProvider();
            //var registrar = new RouteRegistrar(new DependencyResolver(serviceProvider));
            var registrar = new RouteRegistrar(new Provider(serviceProvider));
            registrar.Register(typeof(RetrosheetCommandHandlers));

            return serviceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }

    //This makes scoped services work inside router.
    public class Provider : IServiceProvider
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _contextAccessor;

        public Provider(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _contextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
        }

        public object GetService(Type serviceType)
        {
            return _contextAccessor?.HttpContext?.RequestServices.GetService(serviceType) ??
                   _serviceProvider.GetService(serviceType);
        }
    }
}
