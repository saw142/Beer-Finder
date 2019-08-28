using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication.Web.DAL;
using WebApplication.Web.Providers.Auth;

namespace WebApplication.Web
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
            // Session must be configured for our authentication
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This determines whether user consent for non-essential cookies
                //is needed.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Configures Swagger to look at the XmlComments above our methods
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // Enables Cors allowing client to make cross-origin requests
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins("http://127.0.0.1:5500", "http://127.0.0.1:8080", "http://localhost:5500", "http://localhost:8080")
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader());
            });


            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // Sets session expiration to 20 minuates
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });

            // Dependency Injection
            // For Authentication
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthProvider, SessionAuthProvider>();
            services.AddTransient<IUserDAL>(m => new UserSqlDAL(@"Data Source=.\SQLEXPRESS;Initial Catalog=CleBrews;Integrated Security=True"));
            services.AddTransient<IBeerDAL>(m => new BeerSqlDAL(@"Data Source=.\SQLEXPRESS;Initial Catalog=CleBrews;Integrated Security=True"));
            services.AddTransient<IBreweryDAL>(m => new BrewerySqlDAL(@"Data Source=.\SQLEXPRESS;Initial Catalog=CleBrews;Integrated Security=True"));
            services.AddTransient<IBeerReviewDAL>(m => new BeerReviewSqlDAL(@"Data Source=.\SQLEXPRESS;Initial Catalog=CleBrews;Integrated Security=True"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });


            // Enables the server to allow Cross Origin Request Sharing (CORS)
            app.UseCors("CorsPolicy");


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
