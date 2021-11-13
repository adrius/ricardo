using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace adrius.ricardo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"💰 Ricardo Ricón is starting up!");
            Console.WriteLine($"🖥️ Host platform: {System.Environment.OSVersion} ({(System.Environment.Is64BitOperatingSystem ? "64 bits" : "32 bits")})" );
            Console.WriteLine($"⚙️ .net {System.Environment.Version}");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
