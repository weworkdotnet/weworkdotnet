using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WeWorkDotnet.Web.Data;
using WeWorkDotnet.Web.Models;
using WeWorkDotnet.Web.Models.ConfigurationModels;
using WeWorkDotnet.Web.Services;

namespace WeWorkDotnet.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbConn = Configuration.GetConnectionString("DefaultConnection");

            // For more Hangfire info, check https://github.com/HangfireIO/Hangfire
            services.AddHangfire(x => x.UseSqlServerStorage(dbConn));

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(dbConn));

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            services.Configure<SendGridConfig>(sendGridConfig =>
            {
                sendGridConfig.ApiKey = Configuration.GetValue<string>("SendGrid:ApiKey");
                sendGridConfig.FromEmail = Configuration.GetValue<string>("SendGrid:FromEmail");
                sendGridConfig.FromName = Configuration.GetValue<string>("SendGrid:FromName");
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IAutoEmailService, AutoEmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            app.UseTwitterAuthentication(new TwitterOptions
            {
                ConsumerKey = Configuration.GetValue<string>("Twitter-ConsumerKey"),
                ConsumerSecret = Configuration.GetValue<string>("Twitter-ConsumerSecret"),
                RetrieveUserDetails = true
            });

            if (env.IsProduction())
            {
                app.UseHangfireServer();
                app.UseHangfireDashboard();

                RecurringJob.AddOrUpdate<AutoEmailService>("WeeklyUpdate", a => a.WeeklyUpdate(), "0 13 * * 5");
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
