using Songhay.HelloWorlds.Activities;
using System;
using System.Linq;

namespace Songhay.HelloWorlds.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1) throw new ArgumentException("The expected Activity name is not here.");

            var activityName = args[0];
            var activitiesGetter = new ActivitiesGetter();
            var activity = activitiesGetter.GetActivity(activityName);
            if (activity == null) throw new NullReferenceException("The expected Activity is not here.");
            activity.Start(args.Skip(1).ToArray());
        }
    }
}
