using System;
using IO.Pipeline.Builder;

namespace SampleProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var pipeline = PipelineBuilder<string>
                .StartWith((data, next) =>
                {
                    data += $"Hello darkness, my old friend{Environment.NewLine}";
                    return next.Invoke(data);
                })
                .AddStep((data, next) =>
                {
                    data += $"I've come to talk with you again{Environment.NewLine}";
                    return next.Invoke(data);
                })
                .AddStep((data, next) =>
                {
                    data += $"Because a vision softly creeping{Environment.NewLine}";
                    return next.Invoke(data);
                })
                .AddStep<FinalStep>()
                .Build();

            var resultTask = pipeline.ExecuteAsync($"Disturbed - The Sound Of Silence{Environment.NewLine}");
            
            Console.WriteLine(resultTask.Result);
            Console.ReadKey();
        }
    }
}