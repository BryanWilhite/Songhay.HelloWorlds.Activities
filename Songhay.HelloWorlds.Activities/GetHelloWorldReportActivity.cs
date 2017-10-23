using Songhay.HelloWorlds.Activities.Models;
using System;
using System.Diagnostics;

namespace Songhay.HelloWorlds.Activities
{
    public class GetHelloWorldReportActivity : IActivity
    {
        static GetHelloWorldReportActivity()
        {
            traceSource = new TraceSource("rx", SourceLevels.All);
        }

        public void Start(string[] args)
        {
            traceSource.TraceInformation("Sorry, but the Hello Worlds reports are not yet available :(");
        }

        static readonly TraceSource traceSource;
    }
}
