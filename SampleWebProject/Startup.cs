using IO.Pipeline.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SampleWebProject
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISomeService, SomeService>();
            
            services.RegisterPipeline<string>(
                builder =>
                    builder
                        .RegisterStep<Step1>()
                        .RegisterStep<Step2>()
            );
            
            services.AddMvc();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Pipeline}/{action=Index}/{id?}");
            });
        }
    }
}