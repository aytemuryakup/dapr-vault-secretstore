using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DaprSecretsExample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public IConfiguration Configuration { get; }
        private string firstSecret = null;
        private string secondSecret = null;
        private string thirdSecret = null;
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public async void ConfigureServices(IServiceCollection services)
        {
            // Configuration Provider
            firstSecret = Configuration["WhoAmI"];

            // DaprClient
            DaprClient daprClient = new DaprClientBuilder().Build();
            Dictionary<string, string> secrets = await daprClient.GetSecretAsync("secretstore-lab", "WhoAmI");
            secondSecret = secrets.FirstOrDefault().Value;

            // Dapr through HTTP
            var client = new HttpClient();
            var response = await client.GetAsync("http://localhost:3500/v1.0/secrets/secretstore-lab/WhoAmI");
            thirdSecret = await response.Content.ReadAsStringAsync();
        }

        // HTTP
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync($"Using Configuration Provider: {firstSecret}\n");
                    await context.Response.WriteAsync($"Using DaprClient: {secondSecret}\n");
                    await context.Response.WriteAsync($"Using HTTP: {thirdSecret}");
                });
            });
        }
    }
}
