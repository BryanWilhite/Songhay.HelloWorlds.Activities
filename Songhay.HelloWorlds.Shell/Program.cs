using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.HelloWorlds.Activities;
using Songhay.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Songhay.HelloWorlds.Shell
{
    class Program
    {
        static void DisplayCredits()
        {
            Console.Write(ProgramAssemblyUtility.GetAssemblyInfo(Assembly.GetExecutingAssembly(), true));
            Console.WriteLine(string.Empty);
            Console.WriteLine("Activities Assembly:");
            Console.Write(ProgramAssemblyUtility.GetAssemblyInfo(typeof(MyActivitiesGetter).Assembly, true));
        }

        static void Main(string[] args)
        {
            DisplayCredits();
            var configuration = ProgramUtility.LoadConfiguration(Directory.GetCurrentDirectory());
            TraceSources.ConfiguredTraceSourceName = configuration[DeploymentEnvironment.DefaultTraceSourceNameConfigurationKey];

            using (var listener = new TextWriterTraceListener(Console.Out))
            {
                ProgramUtility.InitializeTraceSource(listener);

                var getter = new MyActivitiesGetter(args);
                var activity = getter.GetActivity();

                if (getter.Args.IsHelpRequest())
                    Console.WriteLine(activity.DisplayHelp(getter.Args));
                else
                    activity.Start(getter.Args);

                listener.Flush();
            }

#if DEBUG
            Console.WriteLine($"{Environment.NewLine}Press any key to continue...");
            Console.ReadKey(false);
#endif

            Environment.Exit(0);
        }
    }
}
