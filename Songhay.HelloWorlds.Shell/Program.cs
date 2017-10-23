using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.HelloWorlds.Activities;
using Songhay.HelloWorlds.Activities.Extensions;
using System;
using System.Diagnostics;

namespace Songhay.HelloWorlds.Shell
{
    class Program
    {
        static Program()
        {
            traceSource = TraceSources
                .Instance[ActivitiesGetter.TraceSourceName]
                .WithAllSourceLevels();
        }

        static void Main(string[] args)
        {
            using (var listener = new TextWriterTraceListener(Console.Out))
            {
                traceSource.Listeners.Add(listener);

                var activityName = args.ToActivityName();
                var activity = (new ActivitiesGetter()).GetActivity(activityName);
                activity.Start(args.ToActivityArgs());

                listener.Flush();
            }
        }

        static readonly TraceSource traceSource;
    }
}
