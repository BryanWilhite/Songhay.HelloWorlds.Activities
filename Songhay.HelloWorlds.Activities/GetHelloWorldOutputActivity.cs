using Songhay.Models;
using System.Threading.Tasks;

namespace Songhay.HelloWorlds.Activities
{
    public class GetHelloWorldOutputActivity : GetHelloWorldActivity, IActivityWithOutput<string, string>
    {
        public async Task<string> StartAsync(string input)
        {
            var message = GetHelloWorldMessage(input);
            traceSource?.TraceInformation(message);
            return await Task.FromResult(message);
        }
    }
}
