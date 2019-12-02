using Songhay.Models;
using System;
using System.Collections.Generic;

namespace Songhay.HelloWorlds.Activities
{
    public class MyActivitiesGetter : ActivitiesGetter
    {
        public MyActivitiesGetter(string[] args) : base(args)
        {
            this.LoadActivities(new Dictionary<string, Lazy<IActivity>>
            {
                {
                    nameof(Activities.GetHelloWorldOutputActivity),
                    new Lazy<IActivity>(() => new Activities.GetHelloWorldOutputActivity())
                }
            });
        }
    }
}
