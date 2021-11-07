using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentScheduler;
using System.Diagnostics;

namespace PixelIT.web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Conifg fill with the passed Enviroments.
            Globe.Config = ConfigMapping.DictionaryToObject<Config>(configuration.AsEnumerable().ToDictionary(x => x.Key, x => x.Value));

            // Init Application
            App.InitApplication(new ApplicationStartupParameter()
            {
                ApplicationName = "PixelIT.web",
                SerilogConfiguration = new SerilogConfiguration() { SeqServer = Globe.Config.SEQ_SERVER, SeqAPIKey = Globe.Config.SEQ_APIKEY }
            });

            JobManager.AddJob(() => new PixelTools().CheckAndCreateThumb(), (s) => s.NonReentrant().ToRunEvery(1).Minutes());
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddRazorPages();
            // For HttpContext in the enricher
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddCors(opt =>
            {
                opt.AddPolicy(name: "CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Necessary that the client IP address is also tangible.
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All,
                ForwardLimit = null, // null = disable check
                RequireHeaderSymmetry = false,
                KnownProxies = { IPAddress.Parse("172.18.0.254"), IPAddress.Parse("127.0.0.1") },
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });                      
        }
    }
}
