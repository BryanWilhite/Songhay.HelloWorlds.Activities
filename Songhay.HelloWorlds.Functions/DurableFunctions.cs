﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.HelloWorlds.Activities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Songhay.HelloWorlds.Functions
{
    public static class DurableFunctions
    {
        static DurableFunctions()
        {
            TraceSources.ConfiguredTraceSourceName = $"trace-{nameof(DurableFunctions)}";
            traceSource = TraceSources
               .Instance
               .GetConfiguredTraceSource()
               .WithSourceLevels()
               .EnsureTraceSource();
        }

        static readonly TraceSource traceSource;

        [FunctionName(FUNC_NAME_ORCH_TRIGGER)]
        public static async Task<HttpResponseMessage> TriggerOrchestration(
            [HttpTrigger(AuthorizationLevel.Anonymous, GET, POST, Route = null)]
            HttpRequestMessage request,
            [OrchestrationClient]
            DurableOrchestrationClient client,
            ILogger log)
        {
            log?.LogInformation($"{FUNC_NAME_ORCH_TRIGGER}: {nameof(TriggerOrchestration)} invoked...");

            var requestBody = await request.Content.ReadAsStringAsync();
            var jO = JObject.Parse(requestBody);

            var args = jO.GetValue<string>("args", throwException: false).Split(" ");
            if ((args == null) || (args.Count() < 2))
                return request.CreateResponse(HttpStatusCode.BadRequest,
                    $"{FUNC_NAME_ORCH_TRIGGER}: The expected Activity args are not here.");

            string instanceId = await client.StartNewAsync(FUNC_NAME_ORCH, args);

            log.LogInformation($"{FUNC_NAME_ORCH_TRIGGER}: orchestration started [{nameof(instanceId)}: {instanceId}].");

            return client.CreateCheckStatusResponse(request, instanceId);
        }

        [FunctionName(FUNC_NAME_ORCH)]
        public static async Task<string> RunOrchestration(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log)
        {
            log?.LogInformation($"{FUNC_NAME_ORCH}: {nameof(RunOrchestration)} invoked...");
            log?.LogInformation($"{FUNC_NAME_ORCH}: {nameof(DurableOrchestrationContextBase.InstanceId)} {context.InstanceId}");

            var args = context.GetInput<string[]>();

            if ((args == null) || !args.Any())
            {
                var errorMessage = $"{FUNC_NAME_ORCH}: The expected Activity args are not here.";
                log?.LogError(errorMessage);
                throw new NullReferenceException(errorMessage);
            }

            var className = args.First();
            var tasks = args
                .Skip(1)
                .Select(planetName => context.CallActivityAsync<string>(FUNC_NAME_ORCH_FUNC, new KeyValuePair<string, string>(className, planetName)));
            //fan out
            await Task.WhenAll(tasks);

            return context.InstanceId;
        }

        [FunctionName(FUNC_NAME_ORCH_FUNC)]
        public static async Task<string> RunOrchestratedFunction(
            [ActivityTrigger] DurableActivityContext context,
            ILogger log)
        {
            log?.LogInformation($"{FUNC_NAME_ORCH_FUNC}: {nameof(RunOrchestratedFunction)} invoked...");
            log?.LogInformation($"{FUNC_NAME_ORCH_FUNC}: {nameof(DurableActivityContext.InstanceId)} {context.InstanceId}");

            var pair = context.GetInput<KeyValuePair<string, string>>();

            log?.LogInformation($"getting activity for {nameof(pair)} `{pair.Key ?? "[null]"}, {pair.Value ?? "[null]"}`");

            var getter = new MyActivitiesGetter(new[] { pair.Key, pair.Value });
            var activity = getter.GetActivity();

            if (activity == null)
            {
                var errorMessage = $"{FUNC_NAME_ORCH_FUNC}: the expected Activity is not here [{nameof(pair)} `{pair.Key ?? "[null]"}, {pair.Value ?? "[null]"}`].";
                log?.LogError(errorMessage);
                throw new NullReferenceException(errorMessage);
            }

            var activityOutput = await activity.StartActivityAsync<string, string>(pair.Value, traceSource);

            log?.LogInformation(activityOutput.Log);

            return activityOutput.Output;
        }

        const string GET = "get";
        const string POST = "post";

        const string FUNC_NAME_ORCH = "Orchestration";
        const string FUNC_NAME_ORCH_FUNC = "OrchestratedFunction";
        const string FUNC_NAME_ORCH_TRIGGER = "OrchestrationTrigger";
    }
}
