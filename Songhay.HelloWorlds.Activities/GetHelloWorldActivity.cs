using Songhay.HelloWorlds.Activities.Models;
using System;
using System.Diagnostics;

namespace Songhay.HelloWorlds.Activities
{
    public class GetHelloWorldActivity : IActivity
    {
        static GetHelloWorldActivity()
        {
            traceSource = new TraceSource("rx", SourceLevels.All);
        }

        public void Start(string[] args)
        {
            if (args.Length < 1) throw new ArgumentException("The expected world name is not here.");

            var worldName = args[0];
            traceSource.TraceInformation($"Hello from world {worldName}!");
        }

        static readonly TraceSource traceSource;
    }
}
