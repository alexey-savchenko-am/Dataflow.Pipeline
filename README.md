# Dataflow.Pipeline
A simple pipeline representation for .NET.
It represents a sequence of steps that are processed consistently.
This approach enables breaking down large blocks of code into smaller, more manageable pieces for improved readability, extensibility, and testability.

[![NuGet version (Dataflow.Pipeline)](https://img.shields.io/nuget/v/Dataflow.Pipeline.svg?style=flat-square&color=blue)](https://www.nuget.org/packages/Dataflow.Pipeline/)
[![Downloads](https://img.shields.io/nuget/dt/IO.Pipeline?style=flat-square&color=blue)]()

# Usage

First, define a type that will be passed between the pipeline steps:

```csharp
public class InsurancePremiumModel
{
  public decimal TotalPremium { get; set; }
}
```
Use the PipelineBuilder to create a pipeline and populate it with steps.
For example, let's assume we want to calculate the total insurance premium for a set of customers:

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

You can also define a pipeline step as a type by implementing the IPipelineStep<T> interface.
In the example below, we create a RoundOffStep to round off the total insurance premium:

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
You can add the newly created step to the builder like this:

```csharp
  builder.AddStep<RoundOffStep>();
```

To obtain the pipeline object, use the Build method of PipelineBuilder.
Once built, you can execute all added steps by calling the ExecuteAsync method:

```csharp
  var pipeline = builder.Build();
  var result = await pipeline.ExecuteAsync(new InsurancePremiumModel());
```
