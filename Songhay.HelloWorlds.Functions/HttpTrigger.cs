using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.HelloWorlds.Activities;
using Songhay.Models;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Songhay.HelloWorlds.Functions
{
    public static class HttpTrigger
    {
        static HttpTrigger()
        {
            TraceSources.ConfiguredTraceSourceName = $"trace-{nameof(HttpTrigger)}";
            traceSource = TraceSources
               .Instance
               .GetConfiguredTraceSource()
               .WithSourceLevels()
               .EnsureTraceSource();
        }

        static readonly TraceSource traceSource;

        public const string FUNC_NAME_HTTP_TRIGGER = "HttpTrigger";
        public const string FUNC_NAME_ACTIVITY_TRIGGER = "ActivityTrigger";
        public const string FUNC_NAME_ORCH = "Orchestration";
        public const string FUNC_NAME_ORCH_FUNC = "OrchestratedFunction";
        public const string FUNC_NAME_ORCH_TRIGGER = "OrchestrationTrigger";

        [FunctionName(FUNC_NAME_HTTP_TRIGGER)]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, GET, POST, Route = null)]
            HttpRequestMessage request,
            ILogger log)
        {
            log?.LogInformation($"{FUNC_NAME_HTTP_TRIGGER}: C# HTTP trigger function processed a request.");

            string name = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("name");

            string requestBody = await request.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? request.CreateResponse(HttpStatusCode.OK, $"Hello, {name}")
                : request.CreateErrorResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body");
        }

        [FunctionName(FUNC_NAME_ACTIVITY_TRIGGER)]
        public static async Task<IActionResult> RunActivity(
            [HttpTrigger(AuthorizationLevel.Anonymous, POST, Route = null)]
            HttpRequestMessage req,
            ILogger log)
        {
            log?.LogInformation($"{FUNC_NAME_ACTIVITY_TRIGGER}: {nameof(RunActivity)} invoked...");

            var requestBody = await req.Content.ReadAsStringAsync();
            var jO = JObject.Parse(requestBody);

            var args = jO.GetValue<string>("args", throwException: false).Split(" ");
            if ((args == null) || (!args.Any()))
                return new BadRequestObjectResult($"{FUNC_NAME_ACTIVITY_TRIGGER}: The expected Activity args are not here.");

            var getter = new MyActivitiesGetter(args);
            var activity = getter.GetActivity();

            if (activity == null)
            {
                log?.LogError($"{FUNC_NAME_ACTIVITY_TRIGGER}: the expected Activity is not here [{nameof(args)}: {string.Join(",", args)}].");
                return new NotFoundResult();
            }

            if (getter.Args.IsHelpRequest())
                log?.LogInformation(activity.DisplayHelp(getter.Args));
            else
                StartActivity(getter.Args, activity, log);

            return (ActionResult)new OkResult();
        }

        [FunctionName(FUNC_NAME_ORCH)]
        public static async Task<HttpResponseMessage> RunOrchestration(
            [HttpTrigger(AuthorizationLevel.Anonymous, GET, POST, Route = null)]
            HttpRequestMessage req,
            [OrchestrationClient]
            DurableOrchestrationClient client,
            ILogger log)
        {
            log?.LogInformation($"{FUNC_NAME_ORCH}: {nameof(RunOrchestration)} invoked...");

            string instanceId = await client.StartNewAsync(FUNC_NAME_ORCH_FUNC, "Nuget");

            log.LogInformation($"{FUNC_NAME_ORCH}: orchestration started [{nameof(instanceId)}: {instanceId}].");

            return client.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName(FUNC_NAME_ORCH_FUNC)]
        public static async Task<string> RunOrchestratedFunction(
            [OrchestrationTrigger] DurableOrchestrationContext orchestrationContext,
            [ActivityTrigger] DurableActivityContext context,
            ILogger log)
        {
            log?.LogInformation($"{FUNC_NAME_ORCH_FUNC}: {nameof(RunOrchestratedFunction)} invoked...");
            log?.LogInformation($"{FUNC_NAME_ORCH_FUNC}: {nameof(DurableOrchestrationContextBase.InstanceId)} {orchestrationContext.InstanceId}");

            return await Task.FromResult("");
        }

        internal static void StartActivity(ProgramArgs args, IActivity activity, ILogger log)
        {
            using (var writer = new StringWriter())
            using (var listener = new TextWriterTraceListener(writer))
            {
                traceSource.Listeners.Add(listener);

                try
                {
                    activity.Start(args);
                }
                finally
                {
                    listener.Flush();
                    log?.LogInformation(writer.ToString());
                }
            }
        }

        const string GET = "get";
        const string POST = "post";
    }
}
