using System.Threading.Tasks;
using IO.Pipeline;
using IO.Pipeline.Builder;
using Microsoft.AspNetCore.Mvc;

namespace SampleWebProject
{
    public class PipelineController
        : Controller
    {
        private readonly IPipeline<string, string> _pipeline;
        private readonly Step1 _step1;
        private readonly Step2 _step2;


        public PipelineController(
            IPipeline<string, string> pipeline,
            Step1 step1,
            Step2 step2)
        {
            _pipeline = pipeline;
            _step1 = step1;
            _step2 = step2;
        }

        [HttpGet]
        public async Task<ActionResult> ExecuteRegisteredPipeline()
        {
            var result = await _pipeline.ExecuteAsync("hello, darkness, my old friend \r\n");
            return Json(result);
        }
        
        [HttpGet]
        public async Task<ActionResult> ExecutePipelineFromRegisteredSteps()
        {
            var pipeline = PipelineBuilder<string>
                .StartWith(_step1)
                .AddStep(_step2)
                .AddStep(async (data, next) =>
                {
                    data += "Final Step!!!";
                    return await next.Invoke(data);
                })
                .Build();
            
            var result = await pipeline.ExecuteAsync("hello, darkness, my old friend \r\n");
                
            return Json(result);
        }
    }
}