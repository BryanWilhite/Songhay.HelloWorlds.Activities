using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Songhay.HelloWorlds.Activities;
using Songhay.HelloWorlds.Activities.Extensions;
using System;
using System.Diagnostics;

namespace Songhay.HelloWorlds.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection().AddLogging(builder =>
            {
                builder
                    .AddTraceSource(new SourceSwitch("rx-switch")
                    {
                        Level = SourceLevels.All
                    });
            });

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("logger loaded");

            var activityName = args.ToActivityName();
            var activity = (new ActivitiesGetter()).GetActivity(activityName);
            activity.Start(args.ToActivityArgs());

            Console.ReadKey();
        }
    }
}
