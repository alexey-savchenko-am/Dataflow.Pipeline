namespace IO.Pipeline
{
    using System;
    using System.Threading.Tasks;

    public interface IPipelineStep<TIn, TOut>
    {
        Task<TOut> InvokeAsync(TIn input, Func<TIn, Task<TOut>> next);
    }
    
    public interface IPipelineStep<T> 
        : IPipelineStep<T, T>
    {
    }
}