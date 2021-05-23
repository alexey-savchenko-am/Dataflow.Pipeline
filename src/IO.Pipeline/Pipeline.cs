namespace DataflowIO.Pipeline
{
    using Dataflow.Pipeline;
    using System;
    using System.Threading.Tasks;

    public class Pipeline<TIn, TOut>
        : IPipeline<TIn, TOut>
    {
        public int StepCount { get => _stepCount; }

        private readonly int _stepCount;
        private readonly Func<TIn, Task<TOut>> _handler;

        public Pipeline(int stepCount, Func<TIn, Task<TOut>> handler)
        {
            _stepCount = stepCount;
            _handler = handler;
        }

        public async Task<TOut> ExecuteAsync(TIn data)
        {
            return await _handler.Invoke(data).ConfigureAwait(false);
        }
    }
}