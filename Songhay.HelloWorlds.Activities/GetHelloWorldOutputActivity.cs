using Songhay.Models;
using System;
using System.Threading.Tasks;

namespace Songhay.HelloWorlds.Activities
{
    public class GetHelloWorldOutputActivity : GetHelloWorldActivity, IActivityOutput<ProgramArgs, string>
    {
        public Task<string> StartAsync(ProgramArgs input)
        {
            throw new NotImplementedException();
        }
    }
}
