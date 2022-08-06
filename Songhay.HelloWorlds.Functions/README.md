# Azure Functions (Q4 2022)

Here is the current view of the serverless cloud technology of this Studio: Azure Functions. The leading update here is the full acceptance of .NET 6.0. This is the state of the art and is no longer a recommendation. Starting with the “recommendations for the enterprise” section below, the notes here try to deep-dive into Azure Functions details. A more introductory overview can be seen in [my notes](https://github.com/BryanWilhite/dotnet-core/tree/master/dotnet-azure-functions-quickstart#azure-functions-quickstart) for the Azure Functions sample in my self-educational `dotnet-core` repo. This introductory overview makes Azure Functions Core Tools [[GitHub](https://github.com/Azure/azure-functions-core-tools)] and Azurite [[GitHub](https://github.com/azure/azurite#npm)] so familiar that the expectation here is that we will run `Songhay.HelloWorlds.Functions` locally from Visual Studio Code in the following cycle:

- run **Azurite: Start** inside Visual Studio Code
- run and debug **Attach to .NET Functions** (defined in the conventional `launch.json` [file](https://github.com/BryanWilhite/Songhay.HelloWorlds.Activities/blob/master/.vscode/launch.json#L32)) inside Visual Studio Code
- call the local endpoints with, say, [Thunder Client](https://betterprogramming.pub/vs-code-awesome-tools-thunder-client-extension-for-you-apis-f2e6a3671c3b) inside Visual Studio Code
- run **Azurite: Close** inside Visual Studio Code

## recommendations for the enterprise

The current recommendations are:

- minimize the use of the `Microsoft.AspNetCore.Mvc` namespace
- review the importance of why [Azure Durable Functions](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp) exists
- consider that effective usage of [Azure Durable Functions](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp) requires some understanding of [the event-sourcing design pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing) and how it applies to how Azure Functions _replays_ functions for scalability
- verify the assertion that using `Task.WhenAll` over `IEnumerable<Task<T>>` in an Orchestration is an anti-pattern
- verify the assertion that Durable Entities can be the replacement for some temporary-table-based designs in SQL server
- consider the possibility that ‘advanced usage’ of Durable Entities (beyond temporary-table designs) requires an understanding of the [Actor Model](https://en.wikipedia.org/wiki/Actor_model) in general and Microsoft’s [Orleans](https://dotnet.github.io/orleans/) product in particular

## minimize the use of the `Microsoft.AspNetCore.Mvc` namespace

The `Microsoft.AspNetCore.*` namespaces contain loads of convenience methods that appear to be _transitional_ and _not permanent_ in the world of Azure Functions. For example, capturing query strings with `Microsoft.AspNetCore.Mvc` helpers is baked in (using `HttpRequest`):

```csharp
string name = req.Query["name"];
```

Doing the same without `Microsoft.AspNetCore.*` namespaces (using `HttpUtility` [📖 [docs](https://docs.microsoft.com/en-us/dotnet/api/system.web.httputility?view=net-6.0)]):

```csharp
var name = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("name");
```

Also, the use of `BadRequestObjectResult` (or `OkObjectResult`) can be replaced by this pattern:

```csharp
return new HttpResponseMessage(HttpStatusCode.BadRequest)
{
    ReasonPhrase = "The expected Request is not here."
};
```

or

```csharp
return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent($"Hello, {name}") };
```

However, modern ASP.NET will no longer tolerate this return value:

```csharp
return request.CreateResponse(HttpStatusCode.OK);
```

Calling `CreateResponse` will produce an error like `Synchronous operations are disallowed. Call WriteAsync or set AllowSynchronousIO to true instead.` [📖 [docs](https://docs.microsoft.com/en-us/dotnet/core/compatibility/aspnetcore#change-description)].

Minimizing the use of code dedicated to the ASP.NET MVC product reduces our responsibility to track compatibility changes with said product.

## review the importance of why Azure Durable Functions exists

Back in 2018, Microsoft’s Mark Heath takes the time to explain why we need Azure Durable Functions in “[10 Reasons to Use Durable Functions](https://markheath.net/post/10-reasons-durable-functions).” What I am seeing in this article is the following:

The reasons to use Azure Durable Functions are largely logistical and operational. Stateless Azure Functions can technically do almost everything that Azure Durable Functions can do as long as you do _not_ want to:

- run Functions in parallel in order to aggregate the output of these Functions ([fan-out/fan-in](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-cloud-backup?tabs=csharp))
- formally define and monitor a workflow of named, long-running steps (the _Orchestration_ concept [📖 [docs](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-orchestrations?tabs=csharp)])
- stop the Orchestration of Functions when it runs too long (`IDurableOrchestrationClient.TerminateAsync` [📖 [docs](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.webjobs.extensions.durabletask.idurableorchestrationclient.terminateasync?view=azure-dotnet#microsoft-azure-webjobs-extensions-durabletask-idurableorchestrationclient-terminateasync(system-string-system-string))])
- formally define a hierarchy of workflows (the _Sub-Orchestration_ concept [📖 [docs](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-sub-orchestrations?tabs=csharp)])
- share state among Functions from trigger input (`IDurableOrchestrationContext.GetInput<TInput>`)
- trigger a function in an orchestration that can wait for an external event (`IDurableOrchestrationContext.WaitForExternalEvent`)

The fan out, fan in scenario is one, leading technical argument for Azure Durable Functions. GitHub [issue #815](https://github.com/Azure/Azure-Functions/issues/815), “C# `Parallel.ForEach` not executing in parallel,” betrays the technical price we pay for using a serverless solution:

>This is basically expected behavior as the level of asynchrony you’ll get will depend on the number of cores the machine has, and you can't control that when you're deployed to the consumption plan.
>
>The dynamic scale capabilities of Azure Functions will never take a single execution and transparently distribute that execution over multiple threads or machines. If you have a scenario where a large amount of work has to be done based on a single event then you should split your work up across multiple function executions so that the system can scale it out. One of the easiest ways to do this is with queues.
>
>—[Paul Batum](https://github.com/paulbatum), Microsoft

Azure Durable Functions and plain-old Azure Functions both have retry policies, respectively, see:

- “[Automatic retry on failure](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-error-handling?tabs=csharp#automatic-retry-on-failure)” (`IDurableOrchestrationContext.CallActivityWithRetryAsync`)
- “[Retry policies (preview)](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-error-pages?tabs=csharp#retry-policies-preview)” (`FixedDelayRetry` or `ExponentialBackoffRetry` attributes)

## consider that effective usage of Azure Durable Functions requires some understanding of the event-sourcing design pattern and how it applies to how Azure Functions replays functions for scalability

Whenever the Azure Functions runtime encounters an `async` call, it puts the running code to “sleep” and “wakes” it up when the `async` call completes, _replaying_ the code up to the last `async` call. The statement in the previous sentence can open up a world of hurt that can only be mitigated by designing for simplicity and studying [the event-sourcing design pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing).

## verify the assertion that using `Task.WhenAll` over `IEnumerable<Task<T>>` in an Orchestration is an anti-pattern

Empirical evidence informs us that the following Durable Orchestration may not complete:

```csharp
[FunctionName(BadOrchestrationFunctionName)]
public static async Task<List<string>> RunOrchestrator2(
    [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
{
    log.LogInformation($"{nameof(context.IsReplaying)}: {context.IsReplaying}");
    var cities = await context.CallActivityAsync<string[]>(ActivityFunction1Name, null);

    var tasks = cities.Select(city =>
    {
        log.LogInformation($"setting {city} task...");
        var t = context.CallActivityAsync<string>(ActivityFunction2Name, city);
        log.LogInformation($"returning {city} task...");
        return t;
    }); // to make this function behave correctly, add `.ToArray();`

    await Task.WhenAll(tasks);

    /*
        ⚠ Reminder: close Visual Studio and delete under `%LocalAppData%\Temp\Azurite` to remove orchestrations that do not complete ♻
    */

    List<string> outputs = tasks.Select(t => t.Result).ToList();

    return outputs;
}
```

Very similar to ensuring the execution of SQL commands in Entity Framework, calling `.ToArray()` (or `.ToList()`) on `IEnumerable<Task<T>>` ensures that all Tasks complete.

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
