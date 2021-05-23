using System;
namespace IO.Pipeline.DI
{
    using Microsoft.Extensions.DependencyInjection;

    public class ScopedServicesDependencyResolver
        : IStepDependencyResolver
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScopedServicesDependencyResolver(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        public TStep Resolve<TStep>()
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<TStep>();
        }

        public object Resolve(Type type)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService(type);
        }
    }
}