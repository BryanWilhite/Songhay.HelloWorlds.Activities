using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.HelloWorlds.Activities.Models;
using System.Diagnostics;

namespace Songhay.HelloWorlds.Activities
{
    public class GetHelloWorldReportActivity : IActivity
    {
        static GetHelloWorldReportActivity()
        {
            traceSource = TraceSources
                .Instance[ActivitiesGetter.TraceSourceName]
                .WithAllSourceLevels();
        }

        public void Start(string[] args)
        {
            traceSource.TraceInformation("Sorry, but the Hello Worlds reports are not yet available :(");
        }

        static readonly TraceSource traceSource;
    }
}
