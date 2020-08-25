using System;
using System.Threading.Tasks;

namespace IO.Pipeline
{
    public class Pipeline<TIn, TOut>
        : IPipeline<TIn, TOut>
    {
        private readonly Func<TIn, Task<TOut>> _handler;

        public Pipeline(Func<TIn, Task<TOut>> handler)
        {
            _handler = handler;
        }

        public async Task<TOut> ExecuteAsync(TIn data)
        {
            return await _handler.Invoke(data);
        }
    }
}