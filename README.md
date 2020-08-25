# Pipeline
Represents an implementation of Pipeline pattern for .NET.
It allows to break down a large bunch of code into small pieces for better readability, extensibility and testability.

# Usage

First of all you should create a type which will be passed between the pipeline steps:

```
pubclic class InsurancePremiumModel
{
  public decimal TotalPremium { get; set; }
}
```
Use PipelineBuilder to create pipeline and fill one with steps.
As an example lets assume that we want to calculate common insurance premium for set of customers:

```
var customers = GetCustomers();
var customersCount = customers.Count();

var builder = PipelineBuilder<InsurancePremiumModel>
  .StartWith((model, next) => {
    var basePrice = GetBasePrice(Options.Ambulance, customers);
    var ambulancePremium = _ambulanceService.Calculate(basePrice);
    model.TotalPremium += customersCount * ambulancePremium;
    return next.Invoke(model);
  })
  .AddStep((model, next) => {
    var basePrice = GetBasePrice(Options.Dental, customers);
    var ambulancePremium = _dentalService.Calculate(basePrice);
    model.TotalPremium += customersCount * ambulancePremium;
    return next.Invoke(model);
  })
  .AddStep((model, next) => {
    var basePrice = GetBasePrice(Options.HomeCare, customers);
    var homeCarePremium = _homeCareService.Calculate(basePrice);
    model.TotalPremium += customersCount * homeCarePremium;
    return next.Invoke(model);
  });

```

It is possible to describe a pipeline step as a type by implementing an interface IPipelineStep<T>.
In an example below we create RoundOffStep to round off total insurance premium we got:

```
public class RoundOffStep
  : IPipelineStep<PriceModel>
  {
        public Task<PriceModel> InvokeAsync(PriceModel input, Func<PriceModel, Task<PriceModel>> next)
        {
            input.TotalPremium = RoundOff(input.TotalPremium);
            return next.Invoke(input);
        }
        
        private decimal RoundOff(decimal price)
        {
            var rem = price % 10;
            
            if (rem == 0) return price;
            
            var result = Math.Round(price / 10, MidpointRounding.AwayFromZero) * 10;

            return rem < 5 ? result + 10.00m : result;
        }

    }
```
So, you can add newly created step to builder like this:

```
  builder.AddStep<RoundOffStep>();
```

To obtain pipeline object use method Build of PipelineBuilder.
After that you can call method ExecuteAsync of pipeline to perform all added steps:

```
  var pipeline = builder.Build();
  var result = await pipeline.ExecuteAsync(new InsurancePremiumModel());
```

# Usage with .NET Core

You can register all classes which implement IPipelineStep interface within ConfigureServices method of Startup class:

```
  services.RegisterSteps();
```

After that it is possible to inject objects of steps into controllers, services and build pipeline with them:

```
    public class Step1
        : IPipelineStep<string>
    {
        private readonly ISomeService _someService;

        public Step1(ISomeService someService)
        {
            _someService = someService;
        }
        public async Task<string> InvokeAsync(string input, Func<string, Task<string>> next)
        {
            input += "STEP1 :" + _someService.GetSomeString(100);
            return await next(input);
        }
    }
    
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

   public class PipelineController
        : Controller
    {
        private readonly Step1 _step1;
        private readonly Step2 _step2;


        public PipelineController(
            Step1 step1,
            Step2 step2)
        {
            _step1 = step1;
            _step2 = step2;
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
            
            var result = pipeline.ExecuteAsync("hello, darkness, my old friend \r\n");
                
            return Json(result);
        }
    }

```

There is also an extension method RegisterPipeline<T> which lets you simply create and register as singletone an object of Pipeline:

```
  services.RegisterPipeline<string>(
                builder =>
                    builder
                        .RegisterStep<Step1>()
                        .RegisterStep<Step2>()
            );
```

After that you can inject object of pipeline into service:


```
  private readonly IPipeline<string, string> _pipeline;
  
  public PipelineController(
            IPipeline<string, string> pipeline)
            {
              _pipeline = pipeline;
            }
```


