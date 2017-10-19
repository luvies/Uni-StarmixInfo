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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // log hits
            app.Use((context, next) =>
            {
                logger.LogInformation("hit from {0}", context.Connection.RemoteIpAddress);
                return next();
            });

            // setup app
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
