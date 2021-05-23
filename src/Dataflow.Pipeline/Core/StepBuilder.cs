namespace Dataflow.Pipeline.Core
{
    using Dataflow.Pipeline.Builder;

    public class StepBuilder<T>
    {

        public PipelineBuilder<T> Builder
        {
            get => _builder;
        }
        
        private PipelineBuilder<T> _builder 
            = new PipelineBuilder<T>();
        
        public StepBuilder<T> RegisterStep<TStep>()
            where TStep : IPipelineStep<T, T>
        {
            _builder.AddStep<TStep>();
            return this;
        }
        
        
    }
}