using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.Caching;
using System.IO;

namespace adrius.ricardo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Console.WriteLine("Enumerating available options:");
            foreach (var item in Configuration.AsEnumerable())
                Console.WriteLine($"📌 {item.Key} : {item.Value}");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var cachePath = Configuration.GetValue<string>("CACHE_PATH", 
                Path.Combine(Directory.GetCurrentDirectory(), "FileCache")
            );

            services.AddControllers();
            Console.WriteLine( $"Cache at: {cachePath}" );
            var fileCache = new FileCache(cachePath);
            services.AddSingleton(fileCache.GetType(), fileCache);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
