using Dataflow.Pipeline.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SampleWebProject
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISomeService, SomeService>();
            
            services.RegisterPipeline<string>(
                stepAutoregistration: true,
                builder =>
                    builder
                        .RegisterStep<Step1>()
                        .RegisterStep<Step2>()
            );
            
            services.AddMvc();

        }

        public void Configure(IApplicationBuilder app)
        {

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Pipeline}/{action=ExecuteRegisteredPipeline}");
            });

        }
    }
}