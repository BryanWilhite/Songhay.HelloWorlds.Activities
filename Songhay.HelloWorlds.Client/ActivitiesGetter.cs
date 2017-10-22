using Songhay.HelloWorlds.Activities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    nameof(Activities.GetHelloWorldActivity),
                    new Lazy<IActivity>(() => new Activities.GetHelloWorldReportActivity())
                }
            };
        }

        public IActivity GetActivity(string activityName)
        {
            if (string.IsNullOrEmpty(activityName)) throw new ArgumentNullException("The expected Activity name is not here.");
            if (!this._activities.Keys.Contains(activityName)) throw new ArgumentNullException($"The expected Activity name, {activityName}, is not here.");

            return this._activities[activityName].Value;
        }

        Dictionary<string, Lazy<IActivity>> _activities;
    }
}
