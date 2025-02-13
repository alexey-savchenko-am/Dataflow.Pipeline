﻿namespace Dataflow.Pipeline.DI;

using System;

public class DefaultStepDependencyResolver
    : IStepDependencyResolver
{
    public TStep Resolve<TStep>()
    {
        return Activator.CreateInstance<TStep>();
    }

    public object Resolve(Type type)
    {
        return Activator.CreateInstance(type);
    }
}