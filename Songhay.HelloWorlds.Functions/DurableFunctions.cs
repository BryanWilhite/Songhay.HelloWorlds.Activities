using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Songhay.HelloWorlds.Functions
{
    public static class DurableFunctions
    {
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
            //[ActivityTrigger] DurableActivityContext context,
            ILogger log)
        {
            log?.LogInformation($"{FUNC_NAME_ORCH_FUNC}: {nameof(RunOrchestratedFunction)} invoked...");
            log?.LogInformation($"{FUNC_NAME_ORCH_FUNC}: {nameof(DurableOrchestrationContextBase.InstanceId)} {orchestrationContext.InstanceId}");

            return await Task.FromResult("");
        }

        const string GET = "get";
        const string POST = "post";

        const string FUNC_NAME_ORCH = "Orchestration";
        const string FUNC_NAME_ORCH_FUNC = "OrchestratedFunction";
        const string FUNC_NAME_ORCH_TRIGGER = "OrchestrationTrigger";
    }
}
