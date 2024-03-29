﻿namespace SampleProject
{
    using Dataflow.Pipeline.Builder;
    using System;
    using System.Threading.Tasks;

    public class Application
    {

        public async Task<string> StartAsync()
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

            var result = await pipeline.ExecuteAsync($"Disturbed - The Sound Of Silence{Environment.NewLine}");

            return result;
        }


    }
}
