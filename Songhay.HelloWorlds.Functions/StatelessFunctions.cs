using System.Web;

namespace Songhay.HelloWorlds.Functions;

public static class StatelessFunctions
{
    static StatelessFunctions()
    {
        TraceSources.ConfiguredTraceSourceName = $"trace-{nameof(StatelessFunctions)}";
        TraceSource = TraceSources
            .Instance
            .GetConfiguredTraceSource()
            .WithSourceLevels()
            .ToReferenceTypeValueOrThrow();
    }

    static readonly TraceSource TraceSource;

    [FunctionName(FuncNameHttpTrigger)]
    public static async Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, Get, Post, Route = null)]
        HttpRequestMessage request,
        ILogger log)
    {
        log?.LogInformation($"{FuncNameHttpTrigger}: C# HTTP trigger function processed a request.");

        if (request.Content == null)
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                ReasonPhrase = "The expected Request Content is not here."
            };

        if (request.RequestUri == null)
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                ReasonPhrase = "The expected Request URI is not here."
            };

        var name = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("name");

        var requestBody = await request.Content.ReadAsStringAsync();
        dynamic? data = JsonConvert.DeserializeObject(requestBody);
        name ??= data?.name;

        return name != null
            ? new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent($"Hello, {name}") }
            : new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Please pass a name on the query string or in the request body." };
    }

    [FunctionName(FuncNameActivityTrigger)]
    public static async Task<HttpResponseMessage> RunActivity(
        [HttpTrigger(AuthorizationLevel.Anonymous, Post, Route = null)]
        HttpRequestMessage request,
        ILogger log)
    {
        log?.LogInformation($"{FuncNameActivityTrigger}: {nameof(RunActivity)} invoked...");

        if (request.Content == null)
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                ReasonPhrase = "The expected Request Content is not here."
            };

        var requestBody = await request.Content.ReadAsStringAsync();
        var jO = JObject.Parse(requestBody);

        var args = jO.GetValue<string>("args", throwException: false).Split(" ");
        if (!args.Any())
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                ReasonPhrase = $"{FuncNameActivityTrigger}: The expected Activity args are not here."
            };

        var getter = new MyActivitiesGetter(args);
        var activity = getter.GetActivity();

        if (activity == null)
        {
            log?.LogError($"{FuncNameActivityTrigger}: the expected Activity is not here [{nameof(args)}: {string.Join(",", args)}].");
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        if (getter.Args.IsHelpRequest()) log?.LogInformation(activity.DisplayHelp(getter.Args));
        else
        {
            var activityLog = activity.StartActivity(getter.Args, TraceSource);
            log?.LogInformation(activityLog);
        }

        return new HttpResponseMessage(HttpStatusCode.OK);
    }

    const string Get = nameof(HttpMethod.Get);
    const string Post = nameof(HttpMethod.Post);

    const string FuncNameHttpTrigger = "HttpTrigger";
    const string FuncNameActivityTrigger = "ActivityTrigger";
}
