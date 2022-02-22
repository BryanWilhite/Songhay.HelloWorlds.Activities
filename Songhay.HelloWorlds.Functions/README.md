# Azure Functions (Q1 2022)

## recommendations

The recommendations for Q1 2022 are:

- consider .NET 6.0 and Azure Functions 4.x the minimum entry into the product
- celebrate that .NET 6.0 projects _can_ reference .NET Framework 4.6.1 projects without warning(s)
- minimize the use of the `Microsoft.AspNetCore.Mvc` namespace
- consider that effective usage of [Azure Durable Functions](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp) requires some understanding of [the event-sourcing design pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing) and how it applies to how Azure Functions _replays_ functions for scalability
- verify the assertion that using `Task.WhenAll` in an Orchestration is an anti-pattern
- verify the assertion that Durable Entities can be the replacement for some temporary-table-based designs in SQL server
- consider the possibility that ‘advanced usage’ of Durable Entities (beyond temporary-table designs) requires an understanding of the [Actor Model](https://en.wikipedia.org/wiki/Actor_model) in general and Microsoft’s [Orleans](https://dotnet.github.io/orleans/) product in particular

## consider .NET 6.0 and Azure Functions 4.x the minimum entry into the product

Microsoft is asserting that, for mature, C#-based designs, .NET 6.0 is [a prerequisite](https://docs.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-csharp?tabs=in-process#configure-your-environment). Requiring Azure Durable Functions, means Azure Functions must run In-Process (instead of an [Isolated Process](https://docs.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#differences-with-net-class-library-functions)). BTW: the only notable advantage that Out-of-process has is related to [the middleware pattern](https://www.iamdivakarkumar.com/azurefunctions-middleware/).

## celebrate that .NET 6.0 projects _can_ reference .NET Framework 4.6.1 projects without warning(s)

A few basic experiments show that .NET 6.0 projects _can_ reference .NET Framework 4.6.1 projects without warning(s). This implies that there is no driving need to upgrade ‘legacy code’ (in the 4.6.1 .NET Framework time-frame) not aware of the existence of Azure Functions.

## minimize the use of the `Microsoft.AspNetCore.Mvc` namespace

The `Microsoft.AspNetCore.*` namespaces contain loads of convenience methods that appear to be _transitional_ and _not permanent_ in the world of Azure Functions. For example, capturing query strings with `Microsoft.AspNetCore.Mvc` helpers is baked in (using `HttpRequest`):

```csharp
string name = req.Query["name"];
```

Doing the same without `Microsoft.AspNetCore.*` namespaces (using `HttpRequestMessage`):

```csharp
if (!req.Options.TryGetValue(new HttpRequestOptionsKey<HttpContext>(nameof(HttpContext)), out var aspContext))
{
    return new BadRequestObjectResult("The expected HTTP context is not here.");
}

if (!aspContext.Request.Query.TryGetValue("instanceId", out var name))
{
    return new BadRequestObjectResult("The expected `name` is not here.");
}
```

Also, the use of `BadRequestObjectResult` (or `OkObjectResult`) can be replaced by this pattern:

```csharp
return new HttpResponseMessage(HttpStatusCode.BadRequest)
{
    ReasonPhrase = "The expected Request is not here."
};
```

Minimizing the use of code dedicated to the ASP.NET MVC product reduces our responsibility to track compatibility changes with said product.

## consider that effective usage of Azure Durable Functions requires some understanding of the event-sourcing design pattern and how it applies to how Azure Functions replays functions for scalability

Whenever the Azure Functions runtime encounters an `async` call, it puts the running code to “sleep” and “wakes” it up when the `async` call completes, _replaying_ the code up to the last `async` call. The statement in the previous sentence can open up a world of hurt that can only be mitigated by designing for simplicity and studying [the event-sourcing design pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing).

## verify the assertion that using `Task.WhenAll` in an Orchestration is an anti-pattern

Empirical evidence informs us that the following Durable Orchestration may not complete:

```csharp
[FunctionName(BadOrchestrationFunctionName)]
public static async Task<List<string>> RunOrchestrator2(
    [OrchestrationTrigger] IDurableOrchestrationContext context)
{
    var cities = AzureHelloData.GetHelloCitiesAsync().GetAwaiter().GetResult(); // ⚠ this is not great 😒

    var tasks = cities.Select(city => context.CallActivityAsync<string>(ActivityFunction2Name, city));
    await Task.WhenAll(tasks);

    /*
        Awaiting multiple tasks per function call in Azure Functions (AF) looks like an anti-pattern.

        AF might prefer to await singly per function call (AF only supports one task per sleep step of replay behavior?).

        Trying to yield multiple tasks from an async lamba function, looks like an approach that should be avoided.

        ⚠ Reminder: close Visual Studio and delete under `%LocalAppData%\Temp\Azurite` to remove orchestrations that do not complete ♻
    */

    List<string> outputs = tasks.Select(t => t.Result).ToList();

    return outputs;
}
```

The following (based on sample code from Microsoft) is the more reliable equivalent of the above:

```csharp
[FunctionName(OrchestrationFunctionName)]
public static async Task<List<string>> CallActivity(
    [OrchestrationTrigger] IDurableOrchestrationContext context)
{
    var cities = await context.CallActivityAsync<string[]>(ActivityFunction1Name, null);

    /*
        ⚠ we cannot do this:

        var cities = await AzureHelloData.GetHelloCitiesAsync();

        (because Azure Functions cannot replay it in Durable context)

    */

    var outputs = new List<string>();

    foreach (var city in cities)
    {
        // Replace "hello" with the name of your Durable Activity Function.
        outputs.Add(await context.CallActivityAsync<string>(ActivityFunction2Name, city));
    }

    return outputs;
}
```

## verify the assertion that Durable Entities can be the replacement for some temporary-table-based designs in SQL server

One way to introduce Durable Entities is as a bag of state in the cloud that can prevent making multiple calls to a DBMS during an Orchestration. This bag in the cloud plays the same role that temporary tables do in many SQL stored procedures.

## consider the possibility that ‘advanced usage’ of Durable Entities (beyond temporary-table designs) requires an understanding of the Actor Model in general and Microsoft’s Orleans product in particular

The fact that [Microsoft associates Durable Entities with the Actor Model](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-entities?tabs=csharp#comparison-with-virtual-actors) comes as an exciting surprise:

>Many of the durable entities features are inspired by the [actor model](https://en.wikipedia.org/wiki/Actor_model). If you’re already familiar with actors, you might recognize many of the concepts described in this article. Durable entities are particularly similar to [virtual actors](https://research.microsoft.com/projects/orleans/), or grains, as popularized by the [Orleans project](http://dotnet.github.io/orleans/).

This represents an invitation to research into the same technology that has revolutionized [the billion-dollar gaming industry](https://hoopsomuah.com/2014/04/06/using-project-orleans-in-halo/)!

@[BryanWilhite](https://twitter.com/BryanWilhite)
