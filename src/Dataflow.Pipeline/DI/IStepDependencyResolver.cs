namespace Dataflow.Pipeline.DI;

using System;

public interface IStepDependencyResolver
{
    TStep Resolve<TStep>();
    
    object Resolve(Type type);
}