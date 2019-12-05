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
    public static class StatelessFunctions
    {
        static StatelessFunctions()
        {
            TraceSources.ConfiguredTraceSourceName = $"trace-{nameof(StatelessFunctions)}";
            traceSource = TraceSources
               .Instance
               .GetConfiguredTraceSource()
               .WithSourceLevels()
               .EnsureTraceSource();
        }

        static readonly TraceSource traceSource;

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
        public static async Task<HttpResponseMessage> RunActivity(
            [HttpTrigger(AuthorizationLevel.Anonymous, POST, Route = null)]
            HttpRequestMessage request,
            ILogger log)
        {
            log?.LogInformation($"{FUNC_NAME_ACTIVITY_TRIGGER}: {nameof(RunActivity)} invoked...");

            var requestBody = await request.Content.ReadAsStringAsync();
            var jO = JObject.Parse(requestBody);

            var args = jO.GetValue<string>("args", throwException: false).Split(" ");
            if ((args == null) || (!args.Any()))
                return request.CreateResponse(HttpStatusCode.BadRequest,
                    $"{FUNC_NAME_ACTIVITY_TRIGGER}: The expected Activity args are not here.");

            var getter = new MyActivitiesGetter(args);
            var activity = getter.GetActivity();

            if (activity == null)
            {
                log?.LogError($"{FUNC_NAME_ACTIVITY_TRIGGER}: the expected Activity is not here [{nameof(args)}: {string.Join(",", args)}].");
                return request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (getter.Args.IsHelpRequest()) log?.LogInformation(activity.DisplayHelp(getter.Args));
            else
            {
                var activityLog = activity.StartActivity(getter.Args, traceSource);
                log?.LogInformation(activityLog);
            }

            return request.CreateResponse(HttpStatusCode.OK);
        }

        const string GET = "get";
        const string POST = "post";

        const string FUNC_NAME_HTTP_TRIGGER = "HttpTrigger";
        const string FUNC_NAME_ACTIVITY_TRIGGER = "ActivityTrigger";
    }
}
