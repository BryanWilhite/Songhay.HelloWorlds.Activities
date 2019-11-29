using Songhay.Models;
using System;
using System.Threading.Tasks;

namespace Songhay.HelloWorlds.Activities
{
    public class GetHelloWorldOutputActivity : GetHelloWorldActivity, IActivityOutput<string>
    {
        public Task<string> StartAsync(ProgramArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
