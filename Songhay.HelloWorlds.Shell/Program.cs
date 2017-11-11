using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.HelloWorlds.Activities;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Songhay.HelloWorlds.Shell
{
    class Program
    {
        static Program() => traceSource = TraceSources.Instance.GetConfiguredTraceSource().WithAllSourceLevels();
        static readonly TraceSource traceSource;

        static void Main(string[] args)
        {
            Console.Write(FrameworkAssemblyUtility.GetAssemblyInfo(Assembly.GetExecutingAssembly(), true));

            using (var listener = new TextWriterTraceListener(Console.Out))
            {
                traceSource.Listeners.Add(listener);

                var getter = new MyActivitiesGetter(args);
                var activity = getter.GetActivity();

                if (getter.Args.IsHelpRequest())
                    Console.WriteLine(activity.DisplayHelp(getter.Args));

                activity.Start(getter.Args);

                listener.Flush();
            }

#if DEBUG
            Console.WriteLine(string.Format("{0}Press any key to continue...", Environment.NewLine));
            Console.ReadKey(false);
#endif

            Environment.Exit(0);
        }
    }
}
