using System;
using System.Threading.Tasks;

namespace IO.Pipeline
{
    public delegate Task<TOut> StepHandler<TIn, TOut>(TIn data, Func<TIn, Task<TOut>> next);
}