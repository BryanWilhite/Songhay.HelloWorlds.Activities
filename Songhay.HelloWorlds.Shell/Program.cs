using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.HelloWorlds.Activities;
using Songhay.HelloWorlds.Activities.Extensions;
using System;
using System.Diagnostics;
using System.Reflection;

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
            Console.Write(FrameworkAssemblyUtility.GetAssemblyInfo(Assembly.GetExecutingAssembly(), true));

            using (var listener = new TextWriterTraceListener(Console.Out))
            {
                traceSource.Listeners.Add(listener);

                var activityName = args.ToActivityName();
                var activity = (new ActivitiesGetter()).GetActivity(activityName);
                activity.Start(args.ToActivityArgs());

                listener.Flush();
            }

#if DEBUG
            Console.WriteLine(string.Format("{0}Press any key to continue...", Environment.NewLine));
            Console.ReadKey(false);
#endif

            Environment.Exit(0);
        }

        static readonly TraceSource traceSource;
    }
}
