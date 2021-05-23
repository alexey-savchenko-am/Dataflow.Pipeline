namespace Dataflow.Pipeline.Core
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Dataflow.Pipeline.DI;
    using Microsoft.Extensions.DependencyInjection;

    public static class PipepelineServiceCollectionExtensions
    {
        public static void RegisterPipeline<T>(this IServiceCollection serviceCollection,
            bool stepAutoregistration,
            Func<StepBuilder<T>, StepBuilder<T>> stepFactory)
        {
            if(stepAutoregistration)
                RegisterSteps(serviceCollection);
            
            serviceCollection.AddSingleton<IPipeline<T, T>>(provider =>
            {
                var serviceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

                var stepBuilder 
                    = stepFactory(new StepBuilder<T>());

                return stepBuilder
                    .Builder
                    .Build(new ScopedServicesDependencyResolver(serviceScopeFactory));
            });
        }
        
        public static void RegisterSteps(this IServiceCollection serviceCollection)
        {
            var assembly =  Assembly.GetEntryAssembly();
            
            var types = assembly.GetTypes()
                .Where(x =>
                    (x.IsInterface || x.IsClass)
                    &&  x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineStep<>)))
                .ToList();
            
            types.ForEach(t =>serviceCollection.AddScoped(t));
            
        }
        
        
    }
}