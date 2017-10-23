using Songhay.Extensions;
using Songhay.HelloWorlds.Activities.Models;
using System;
using System.Collections.Generic;

namespace Songhay.HelloWorlds.Activities
{
    public class ActivitiesGetter
    {
        static ActivitiesGetter()
        {
            TraceSourceName = "rx-trace";
        }

        public ActivitiesGetter()
        {
            this.SetupActivities();
        }

        public static string TraceSourceName { get; private set; }

        public IActivity GetActivity(string activityName)
        {
            return this._activities.GetActivity(activityName);
        }

        void SetupActivities()
        {
            this._activities = new Dictionary<string, Lazy<IActivity>>
            {
                {
                    nameof(Activities.GetHelloWorldActivity),
                    new Lazy<IActivity>(() => new Activities.GetHelloWorldActivity())
                },
                {
                    nameof(Activities.GetHelloWorldReportActivity),
                    new Lazy<IActivity>(() => new Activities.GetHelloWorldReportActivity())
                }
            };
        }

        Dictionary<string, Lazy<IActivity>> _activities;
    }
}
