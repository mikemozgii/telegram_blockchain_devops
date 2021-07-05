using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.WebApp.Hubs;
using TONBRAINS.TONOPS.WebApp.Middleware;
using TONBRAINS.TONOPS.WebApp.Services;
using TONBRAINS.TONOPS.WebApp.Services.Implementations;
using Quartz;
using TONBRAINS.TONOPS.WebApp.Quartz;
using Quartz.Impl.Matchers;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.QuartzJobs;

namespace TONBRAINS.TONOPS.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
          .Enrich.FromLogContext()
          .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
          .CreateLogger();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            var connectionString = GlobalAppConfHandler.TonOpsDbConnectionString;

            services.Configure<QuartzOptions>(Configuration.GetSection("Quartz"));

            services.AddQuartz(q =>
            {
                
                // handy when part of cluster or you want to otherwise identify multiple schedulers
                q.SchedulerId = "Scheduler-Core";

                // we take this from appsettings.json, just show it's possible
                // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

                // we could leave DI configuration intact and then jobs need to have public no-arg constructor
                // the MS DI is expected to produce transient job instances

                // this is default configuration if you don't alter it
                q.UseMicrosoftDependencyInjectionJobFactory(options =>
                {
                    // if we don't have the job in DI, allow fallback to configure via default constructor
                    options.AllowDefaultConstructor = true;

                    // set to true if you want to inject scoped services like Entity Framework's DbContext
                    options.CreateScope = false;
                });

                // these are the defaults
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

       

                // convert time zones using converter that can handle Windows/Linux differences
                q.UseTimeZoneConverter();

                // add some listeners
                q.AddSchedulerListener<QuartzSchedulerListener>();
                q.AddJobListener<QuartzJobListener>(GroupMatcher<JobKey>.GroupEquals(GlobalQuartzConfHdlr.DefaultJobGroup));
                q.AddTriggerListener<QuartzTriggerListener>();

            });

            // ASP.NET Core hosting
            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                //Password = dbPassword
            };
            services.AddSignalR();
            services.AddTransient<BasicQJ>();
            services.AddTransient<IThemeService, ThemeService>();
            services.AddTransient<IPasswordHashService, PasswordHashService>();
            services.AddTransient<IEnvironmentService, EnvironmentService>();
            services.AddTransient<INodeSvc, NodeSvc>();
            services.AddSingleton<ISshStream, SshStream>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddDbContext<TonOpsDbContext>(options => options.UseNpgsql(builder.ConnectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }
            //app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseMiddleware<UserSessionMiddleware>();
            app.UseRouting();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<NodeHub>("/nodehub");
                endpoints.MapFallbackToController("Index", "Home");
                endpoints.MapControllers();
            });
        }
    }
}
