using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Songhay.HelloWorlds.Functions;

public static class DurableFunctions
{
    static DurableFunctions()
    {
        TraceSources.ConfiguredTraceSourceName = $"trace-{nameof(DurableFunctions)}";
        TraceSource = TraceSources
            .Instance
            .GetConfiguredTraceSource()
            .WithSourceLevels()
            .ToReferenceTypeValueOrThrow();
    }

    static readonly TraceSource TraceSource;

    [FunctionName(FuncNameOrchTrigger)]
    public static async Task<HttpResponseMessage> TriggerOrchestration(
        [HttpTrigger(AuthorizationLevel.Anonymous,
            nameof(HttpMethod.Get),
            nameof(HttpMethod.Post),
            Route = null)]
        HttpRequestMessage request,
        [DurableClient]
        IDurableOrchestrationClient client,
        ILogger log)
    {
        log?.LogInformation($"{FuncNameOrchTrigger}: {nameof(TriggerOrchestration)} invoked...");

        if (request.Content == null)
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                ReasonPhrase = "The expected Request Content is not here."
            };

        var requestBody = await request.Content.ReadAsStringAsync();
        var jO = JObject.Parse(requestBody);

        var args = jO.GetValue<string>("args", throwException: false).Split(" ");
        if (args.Length < 2)
            return request.CreateResponse(HttpStatusCode.BadRequest,
                $"{FuncNameOrchTrigger}: The expected Activity args are not here.");

        var instanceId = await client.StartNewAsync(FuncNameOrch, args);

        log.LogInformation($"{FuncNameOrchTrigger}: orchestration started [{nameof(instanceId)}: {instanceId}].");

        return client.CreateCheckStatusResponse(request, instanceId);
    }

    [FunctionName(FuncNameOrch)]
    public static async Task<string> RunOrchestration(
        [OrchestrationTrigger] IDurableOrchestrationContext context,
        ILogger log)
    {
        log?.LogInformation($"{FuncNameOrch}: {nameof(RunOrchestration)} invoked...");
        log?.LogInformation($"{FuncNameOrch}: {nameof(IDurableOrchestrationContext.InstanceId)} {context.InstanceId}");

        var args = context.GetInput<string[]>();

        if (args == null || !args.Any())
        {
            const string errorMessage = $"{FuncNameOrch}: The expected Activity args are not here.";
            log?.LogError(errorMessage);
            throw new NullReferenceException(errorMessage);
        }

        var className = args.First();

        //fan out
        foreach (var planetName in args.Skip(1))
        {
            var pair = new KeyValuePair<string, string>(className, planetName);
            await context.CallActivityAsync<string>(FuncNameOrchFunc, pair);
        }

        return context.InstanceId;
    }

    [FunctionName(FuncNameOrchFunc)]
    public static async Task<string?> RunOrchestratedFunction(
        [ActivityTrigger] IDurableActivityContext context,
        ILogger log)
    {
        log?.LogInformation($"{FuncNameOrchFunc}: {nameof(RunOrchestratedFunction)} invoked...");
        log?.LogInformation("{FuncNameOrch}: {InstanceIdName} {ContextInstanceId}",
            FuncNameOrch, nameof(IDurableOrchestrationContext.InstanceId), context.InstanceId);

        var pair = context.GetInput<KeyValuePair<string?, string?>>();

        log?.LogInformation("getting activity for {PairName} `{PairKey}, {PairValue}`",
            nameof(pair), pair.Key ?? "[null]", pair.Value ?? "[null]");

        var getter = new MyActivitiesGetter(
            new[]
            {
                pair.Key.ToReferenceTypeValueOrThrow(),
                pair.Value.ToReferenceTypeValueOrThrow()
            });

        var activity = getter.GetActivity();

        if (activity == null)
        {
            var errorMessage = $"{FuncNameOrchFunc}: the expected Activity is not here [{nameof(pair)} `{pair.Key ?? "[null]"}, {pair.Value ?? "[null]"}`].";
            log?.LogError(errorMessage);
            throw new NullReferenceException(errorMessage);
        }

        var activityOutput = await activity.StartActivityAsync<string, string>(pair.Value, TraceSource);

        log?.LogInformation(activityOutput.Log);

        return activityOutput.Output;
    }

    const string FuncNameOrch = "Orchestration";
    const string FuncNameOrchFunc = "OrchestratedFunction";
    const string FuncNameOrchTrigger = "OrchestrationTrigger";
}
