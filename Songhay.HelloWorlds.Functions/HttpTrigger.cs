using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.HelloWorlds.Activities;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        [FunctionName("HttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, GET, POST, Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await req.GetRawBodyStringAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        [FunctionName("ActivityTrigger")]
        public static async Task<IActionResult> RunActivity(
            [HttpTrigger(AuthorizationLevel.Anonymous, POST, Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Activity trigger invoked...");

            var requestBody = await req.GetRawBodyStringAsync();
            var jO = JObject.Parse(requestBody);

            var args = jO["args"]?.Value<string>()?.Split(" ");
            if ((args == null) || (!args.Any()))
                return new BadRequestObjectResult("The expected Activity args are not here.");

            var getter = new MyActivitiesGetter(args);
            var activity = getter.GetActivity();

            if (activity == null) return new NotFoundResult();

            if (getter.Args.IsHelpRequest())
                log.LogInformation(activity.DisplayHelp(getter.Args));

            using (var writer = new StringWriter())
            using (var listener = new TextWriterTraceListener(writer))
            {
                traceSource.Listeners.Add(listener);

                try
                {
                    activity.Start(getter.Args);
                }
                finally
                {
                    listener.Flush();
                    log.LogInformation(writer.ToString());
                }
            }

            return (ActionResult)new OkResult();
        }

        const string GET = "get";
        const string POST = "post";
    }
}
