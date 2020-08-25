using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IO.Pipeline.DI;

namespace IO.Pipeline.Builder
{
      public abstract class BaseBuilder<TIn, TOut, TBuilder>
        where TBuilder: BaseBuilder<TIn, TOut, TBuilder>, new()
    {
        private IStepDependencyResolver _dependencyResolver 
            = new DefaultStepDependencyResolver();

        private readonly Func<TIn, Task<TOut>> DummyStep
            = (async input =>
            {
                var dataTmp = input;

                if (typeof(TIn) == typeof(TOut))
                    return (TOut) Convert.ChangeType(dataTmp, typeof(TOut));

                return default(TOut);
            });
        
        
        private readonly LinkedList<StepHandler<TIn, TOut>> _stepHandlers 
            = new LinkedList<StepHandler<TIn, TOut>>();
        
        private static TBuilder _builderInstance;

        public static TBuilder WithSteps (IEnumerable<IPipelineStep<TIn, TOut>> steps)
        {
            _builderInstance = new TBuilder();
            
            foreach (var step in steps)
            {
                _builderInstance.AddStep(step);
            }

            return _builderInstance;

        }
        
        
        public static TBuilder StartWith(StepHandler<TIn, TOut> step)
        {
            _builderInstance = new TBuilder();
            return _builderInstance.AddStep(step);
        }
        
        public static TBuilder StartWith<TPipelineStep>()
            where TPipelineStep : IPipelineStep<TIn, TOut>
        {
            _builderInstance = new TBuilder();
            return _builderInstance.AddStep<TPipelineStep>();
        }
        
        public static TBuilder StartWith<TPipelineStep>(TPipelineStep step)
            where TPipelineStep : IPipelineStep<TIn, TOut>
        {
            _builderInstance = new TBuilder();
            return _builderInstance.AddStep<TPipelineStep>(step);
        }
        
        public static TBuilder StartWith<TPipelineStep>(Type type)
            where TPipelineStep : IPipelineStep<TIn, TOut>
        {
            _builderInstance = new TBuilder();
            return _builderInstance.AddStep<TPipelineStep>(type);
        }
        
        public TBuilder AddStep(StepHandler<TIn, TOut> step)
        {
            _stepHandlers.AddLast(step);
            return _builderInstance;
        }
        
        public TBuilder AddStep<TPipelineStep>()
            where TPipelineStep : IPipelineStep<TIn, TOut>
        {
            StepHandler<TIn, TOut> handler = async (TIn data, Func<TIn, Task<TOut>> next) =>
            {
                var step =  _dependencyResolver.Resolve<TPipelineStep>();
                return await step.InvokeAsync(data, next);

            };

            return AddStep(handler);
        }
        
        public TBuilder AddStep<TPipelineStep>(Type type)
            where TPipelineStep : IPipelineStep<TIn, TOut>
        {
            StepHandler<TIn, TOut> handler = async (TIn data, Func<TIn, Task<TOut>> next) =>
            {
                var step =  (TPipelineStep)_dependencyResolver.Resolve(type);
                return await step.InvokeAsync(data, next);

            };

            return AddStep(handler);
        }
        
        
        public TBuilder AddStep<TPipelineStep>(params Object[] args)
            where TPipelineStep : IPipelineStep<TIn, TOut>
        {
            StepHandler<TIn, TOut> handler = async (TIn data, Func<TIn, Task<TOut>> next) =>
            {
                var step =  _dependencyResolver.Resolve<TPipelineStep>();
                return await step.InvokeAsync(data, next);

            };

            return AddStep(handler);
        }
        
        public TBuilder AddStep<TPipelineStep>(TPipelineStep step)
            where TPipelineStep : IPipelineStep<TIn, TOut>
        {
            StepHandler<TIn, TOut> handler 
                = async (TIn data, Func<TIn, Task<TOut>> next) 
                    => await step.InvokeAsync(data, next);

            return AddStep(handler);
        }
        
        
        public IPipeline<TIn, TOut> Build()
        {
            return BuildInternal();
        }
        
        public IPipeline<TIn, TOut> Build(IStepDependencyResolver stepDependencyResolver)
        {
            if(stepDependencyResolver != null)
                _dependencyResolver = stepDependencyResolver;
            
            return Build();
        }

        protected IPipeline<TIn, TOut> BuildInternal()
            => new Pipeline<TIn, TOut>(async (TIn data) =>
            {
                var chain = BuildChain(_stepHandlers.First);
                return await chain.Invoke(data);
            });
        
        
        private Func<TIn, Task<TOut>> BuildChain(LinkedListNode<StepHandler<TIn, TOut>> node)
        {
            if (node == null) return DummyStep;
            return input => node.Value.Invoke(input, BuildChain(node.Next));
        }
    }
}