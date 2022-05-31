using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Snips
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) =>
                {
                    string elasticURL = context.Configuration["ElasticURL"];
                    string password = context.Configuration["ElasticPassword"];
                    configuration.Enrich.FromLogContext()
                        .MinimumLevel.Information()
                        .WriteTo.Console()
                        .WriteTo.Elasticsearch(
                            new(new Uri(elasticURL))
                            {
                                IndexFormat =
                                    $"blog-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                                AutoRegisterTemplate = true,
                                NumberOfShards = 2,
                                NumberOfReplicas = 1,
                                ModifyConnectionSettings = x => x.BasicAuthentication("elastic", password),
                            }).Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName ?? string.Empty);
                       
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
