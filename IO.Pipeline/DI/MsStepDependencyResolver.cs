using System;
using Microsoft.Extensions.DependencyInjection;

namespace IO.Pipeline.DI
{
    public class MsStepDependencyResolver
        : IStepDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public MsStepDependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public TStep Resolve<TStep>()
        {
            return _serviceProvider.GetRequiredService<TStep>();
        }

        public object Resolve(Type type)
        {
            return _serviceProvider.GetRequiredService(type);
        }
    }
}