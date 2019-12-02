using Songhay.Models;
using System.Threading.Tasks;

namespace Songhay.HelloWorlds.Activities
{
    public class GetHelloWorldOutputActivity : GetHelloWorldActivity, IActivityOutput<string, string>
    {
        public Task<string> StartAsync(string input) => Task.FromResult(GetHelloWorldMessage(input));
    }
}
