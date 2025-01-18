namespace Dataflow.Pipeline;

using System.Threading.Tasks;

public interface IPipeline<TIn, TOut>
{
    int StepCount { get; }
    Task<TOut> ExecuteAsync(TIn data);
}

public interface IPipeline<T>
    where T : IPipeline<T, T>
{ }