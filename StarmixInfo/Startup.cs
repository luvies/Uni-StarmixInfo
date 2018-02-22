using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StarmixInfo
{
    public class Startup
    {
        const string _LogImportantFormat = "==== {0:l} ====";
        readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<Models.DataContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // needed services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // custom services
            services.AddScoped<Services.IAdminLogon, Services.AdminLogon>();
            services.AddScoped<Services.IDbSettings, Services.DbSettings>();
            services.AddScoped<Services.IConfigHelper, Services.ConfigHelper>();
            services.AddSingleton<Services.IUnityApiHelper, Services.UnityApiHelper>(impFactory =>
                                                                                     new Services.UnityApiHelper(
                                                                                         Configuration.GetValue<string>("UnityAuthToken"),
                                                                                         impFactory.GetService<ILogger<Services.UnityApiHelper>>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // set up logging
            appLifetime.ApplicationStarted.Register(OnStartup); // on startup
            app.Use((context, next) => // on hit
            {
                _logger.LogInformation("Request {0:l} originated from {1:l}:{2}", context.Connection.Id,
                                      context.Connection.RemoteIpAddress.ToString(), context.Connection.RemotePort);
                return next();
            });
            appLifetime.ApplicationStopping.Register(OnStopping); // on stopping
            appLifetime.ApplicationStopped.Register(OnStopped); // on stopped

            // setup app
            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
            app.UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true });
            app.UseMvcWithDefaultRoute();
        }

        void OnStartup() => _logger.LogInformation(_LogImportantFormat, "Server Fully Started");

        void OnStopping() => _logger.LogInformation(_LogImportantFormat, "Server Stopping");

        void OnStopped() => _logger.LogInformation(_LogImportantFormat, "Server Stopped, Exit Imminent");
    }
}
