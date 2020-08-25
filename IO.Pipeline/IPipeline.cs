using System.Threading.Tasks;

namespace IO.Pipeline
{
    public interface IPipeline<TIn, TOut>
    {
        Task<TOut> ExecuteAsync(TIn data);
    }
    
    
    public interface IPipeline<T>
        where T : IPipeline<T, T>
    { }
}