using System.Collections.Generic;
using CoreWeb1.Controllers;
using CoreWeb1.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreWeb1
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Things such as..
            // Debug level
            // database connection strings
            // admin email address
            // and other quasi dynamic configuration data
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            //our routing framework
            services.AddMvc().AddJsonOptions(options =>
            {
                // PascaleCaseJson
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //enable web sockets
            app.UseWebSockets();

            //wire our chat service
            app.UseMiddleware<ChatService>();

            // this enables routing / controller framework
            app.UseMvc();

            //ensure DB is configured
            using (var context = new ScoreContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
