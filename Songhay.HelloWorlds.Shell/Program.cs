using Songhay.HelloWorlds.Activities;
using Songhay.HelloWorlds.Activities.Extensions;

namespace Songhay.HelloWorlds.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            var activityName = args.ToActivityName();
            var activity = (new ActivitiesGetter()).GetActivity(activityName);
            activity.Start(args.ToActivityArgs());
        }
    }
}
