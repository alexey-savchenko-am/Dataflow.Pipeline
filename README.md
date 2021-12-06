# Dataflow.Pipeline
Represents an implementation of Pipeline pattern for .NET.\
It allows to break down a large bunch of code into small pieces for better readability, extensibility and testability.

[![NuGet version (Dataflow.Pipeline)](https://img.shields.io/nuget/v/Dataflow.Pipeline.svg?style=flat-square&color=blue)](https://www.nuget.org/packages/Dataflow.Pipeline/)
[![Downloads](https://img.shields.io/nuget/dt/IO.Pipeline?style=flat-square&color=blue)]()

# Usage

First of all you should create a type which will be passed between the pipeline steps:

```csharp
public class InsurancePremiumModel
{
  public decimal TotalPremium { get; set; }
}
```
Use PipelineBuilder to create pipeline and fill one with steps.
As an example lets assume that we want to calculate common insurance premium for set of customers:

```csharp
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
    var dentalPremium = _dentalService.Calculate(basePrice);
    model.TotalPremium += customersCount * dentalPremium;
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

```csharp
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

```csharp
  builder.AddStep<RoundOffStep>();
```

To obtain pipeline object use method Build of PipelineBuilder.
After that you can call method ExecuteAsync of pipeline to perform all added steps:

```csharp
  var pipeline = builder.Build();
  var result = await pipeline.ExecuteAsync(new InsurancePremiumModel());
```
