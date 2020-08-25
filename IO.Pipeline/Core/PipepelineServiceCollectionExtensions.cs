using System;
using System.Linq;
using System.Reflection;
using IO.Pipeline.DI;
using Microsoft.Extensions.DependencyInjection;

namespace IO.Pipeline.Core
{
    public static class PipepelineServiceCollectionExtensions
    {
        public static void RegisterPipeline<T>(this IServiceCollection serviceCollection,
           Func<StepBuilder<T>, StepBuilder<T>> stepFactory, bool registerSteps = true)
        {
            if(registerSteps)
                RegisterSteps(serviceCollection);
            
            serviceCollection.AddSingleton<IPipeline<T, T>>(provider =>
            {
                var stepBuilder 
                    = stepFactory(new StepBuilder<T>());

                return stepBuilder
                    .Builder
                    .Build(new MsStepDependencyResolver(provider));
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
            
            types.ForEach(t =>serviceCollection.AddSingleton(t));
            
        }
        
        
    }
}