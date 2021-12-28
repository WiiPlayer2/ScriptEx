using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScriptEx.Core.Internals;
using ScriptEx.Shared;

namespace ScriptEx.Core
{
    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddSingleton<IScriptEngineRegistry, ScriptEngineRegistry>();
            services.AddSingleton<ScriptRunner>();
            services.AddOptions<AppOptions>()
                .Bind(Configuration.GetSection(AppOptions.SECTION));

            // API
            services.AddGraphQLServer()
                .AddInMemorySubscriptions()

                // Misc.
                .ModifyRequestOptions(options => { options.IncludeExceptionDetails = true; });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseWebSockets()
                .UseRouting()
                .UseEndpoints(endpoints => { endpoints.MapGraphQL(); });
        }
    }
}
