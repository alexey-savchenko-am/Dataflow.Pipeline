namespace SampleProject
{
    using System;
    using System.Threading.Tasks;
    using IO.Pipeline;

    public class FinalStep
        : IPipelineStep<string>
    {
        public Task<string> InvokeAsync(string input, Func<string, Task<string>> next)
        {
            input += $"Left its seeds while I was sleeping{Environment.NewLine}";
            return next.Invoke(input);
        }
    }

}