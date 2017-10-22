using Songhay.HelloWorlds.Activities.Extensions;
using Songhay.HelloWorlds.Activities.Models;
using System;
using System.Collections.Generic;

namespace Songhay.HelloWorlds.Activities
{
    public class ActivitiesGetter
    {
        public ActivitiesGetter()
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

        public IActivity GetActivity(string activityName)
        {
            return this._activities.GetActivity(activityName);
        }

        Dictionary<string, Lazy<IActivity>> _activities;
    }
}
