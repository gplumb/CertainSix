using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using ShippingContainer.Interfaces;

namespace ShippingContainer
{
    /// <summary>
    /// Controls WebAPI configuration and startup
    /// </summary>
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public bool IsDevEnvironment { get; private set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

#if DEBUG
            IsDevEnvironment = true;
#else
            IsDevEnvironment = env.IsDevelopment();
#endif
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            if (IsDevEnvironment)
            {
                // Use in-memory EF provider
                services.AddDbContext<ShippingRepository>(options => options.UseInMemoryDatabase("ShippingContainer"));
            }
            else
            {
                // Use Sql Server provider
                services.AddDbContext<ShippingRepository>(options => options.UseSqlServer(Configuration["SqlConnectionString"]));
            }

            // Allow DI via constructor
            services.AddTransient<IShippingRepository, ShippingRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (IsDevEnvironment)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Static file serving supports ~400 mimetypes, but yaml isn't one of them!
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".yaml"] = "text/yaml";

            // By adding our new provider, we can safely serve the API spec, which is
            // handy for Swagger UI
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
                ContentTypeProvider = provider,
                RequestPath = ""
            });

            app.UseMvc();

            if (IsDevEnvironment)
            {
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/apispec.yaml", "Shipping Container Spoilage v1.0.0");
                });
            }
        }
    }
}
