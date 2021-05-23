using System;
using System.Threading.Tasks;
using Dataflow.Pipeline;

namespace SampleWebProject
{
    public class Step2
        : IPipelineStep<string>
    {
        private readonly ISomeService _someService;

        public Step2(ISomeService someService)
        {
            _someService = someService;
        }
        public async Task<string> InvokeAsync(string input, Func<string, Task<string>> next)
        {
            input += "STEP2 :" + _someService.GetSomeString(100);
            return await next(input);
        }
    }
}