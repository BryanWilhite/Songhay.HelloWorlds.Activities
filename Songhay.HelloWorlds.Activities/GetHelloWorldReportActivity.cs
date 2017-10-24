using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using System.Diagnostics;

namespace Songhay.HelloWorlds.Activities
{
    public class GetHelloWorldReportActivity : IActivity
    {
        static GetHelloWorldReportActivity() => traceSource = TraceSources.Instance[ActivitiesGetter.TraceSourceName].WithAllSourceLevels();
        static readonly TraceSource traceSource;

        public void Start(string[] args)
        {
            traceSource.TraceInformation("Sorry, but the Hello Worlds reports are not yet available :(");
        }
    }
}
