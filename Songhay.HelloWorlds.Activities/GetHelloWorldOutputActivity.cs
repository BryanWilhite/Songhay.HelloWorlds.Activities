using Songhay.Extensions;
using Songhay.Models;
using System.Threading.Tasks;

namespace Songhay.HelloWorlds.Activities
{
    public class GetHelloWorldOutputActivity : GetHelloWorldActivity, IActivityWithTask<string, string>
    {
        public async Task<string> StartAsync(string input)
        {
            var message = GetHelloWorldMessage(input);
            traceSource?.WriteLine(message);
            return await Task.FromResult(message);
        }
    }
}
