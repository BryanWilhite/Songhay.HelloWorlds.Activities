using Microsoft.Extensions.Configuration;
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
        static void Main(string[] args)
        {
            Console.Write(FrameworkAssemblyUtility.GetAssemblyInfo(Assembly.GetExecutingAssembly(), true));
            Console.WriteLine(string.Empty);
            Console.WriteLine("Activities Assembly:");
            Console.Write(FrameworkAssemblyUtility.GetAssemblyInfo(typeof(MyActivitiesGetter).Assembly, true));

            Console.WriteLine("Loading configuration...");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("./appsettings.json");

            Console.WriteLine("Building configuration...");
            var configuration = builder.Build();
            TraceSources.ConfiguredTraceSourceName = configuration[DeploymentEnvironment.DefaultTraceSourceNameConfigurationKey];

            using (var listener = new TextWriterTraceListener(Console.Out))
            {
                var traceSource = TraceSources
                    .Instance
                    .GetTraceSourceFromConfiguredName()
                    .WithAllSourceLevels()
                    .EnsureTraceSource();
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
