namespace Datataflow.Pipepeline.UnitTests
{
    using IO.Pipeline.Builder;
    using System.Threading.Tasks;
    using AutoFixture;
    using IO.Pipeline;
    using Xunit;
    using System;

    public class PipelineBuilderTests
    {
        private readonly Fixture _fixture;
        public PipelineBuilderTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task PipelineSuccessfullyCreatedWithSpecifiedSteps()
        {
            var stepCount = _fixture.Create<byte>();

            var builder = PipelineBuilder<PipelineModel>
               .StartWith(_fixture.Create<StepHandler<PipelineModel, PipelineModel>>());

            for (byte i = 0; i < stepCount - 1; i++)
                builder.AddStep(_fixture.Create<StepHandler<PipelineModel, PipelineModel>>());

            var pipeline = builder.Build();

            var result = await pipeline.ExecuteAsync(new PipelineModel());

            Assert.Equal(stepCount, pipeline.StepCount);
            Assert.NotNull(result.AnyString);
            Assert.True(result.Counter > 0);
        }


        [Fact]
        public async Task PipelineSuccessfullyPassModelObjectAlongSteps()
        {
            var stepCount = _fixture.Create<byte>();

            var builder = PipelineBuilder<PipelineModel>
               .StartWith(IncrementAsync);

            for (byte i = 0; i < stepCount - 1; i++)
                builder.AddStep(IncrementAsync);

            var pipeline = builder.Build();

            var result = await pipeline.ExecuteAsync(new PipelineModel());

            Assert.Equal(stepCount, result.Counter);
        }

        private async Task<PipelineModel> IncrementAsync(PipelineModel model, Func<PipelineModel, Task<PipelineModel>> next)
        {
            model.Counter++;
            return await next(model);
        }


    }


}
