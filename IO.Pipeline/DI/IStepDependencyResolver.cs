using System;

namespace IO.Pipeline.DI
{
    public interface IStepDependencyResolver
    {
        TStep Resolve<TStep>();
        
        object Resolve(Type type);
    }
}