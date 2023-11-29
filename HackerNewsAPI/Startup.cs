// Startup.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace HackerNewsAPI
{
    public class Startup
    {
        // Constructor for Startup class
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Gets the configuration from appsettings.json and other sources
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Other configurations...
            services.AddHttpClient<HackerNewsService>();
            services.AddScoped<HackerNewsService>();

            // Add services required for MVC and other dependencies
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HackerNewsAPI", Version = "v1" });
            });
        }


        // Configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Show detailed exception information in the development environment
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackerNewsAPI v1"));
            }
            else
            {
                // Use a more generic error page in production
                app.UseExceptionHandler("/Home/Error");

                // Redirect to /Home/Error for status codes 404, 500, etc.
                app.UseStatusCodePagesWithRedirects("/Home/Error");
            }

            app.UseHttpsRedirection();

            // Enable routing and endpoints for controllers
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // Configure logging services
        public void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                // Add console logging for development purposes 
                builder.AddConsole(); 
            });
        }
    }
}
