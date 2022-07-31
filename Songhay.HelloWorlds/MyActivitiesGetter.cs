using Songhay.HelloWorlds.Activities;

namespace Songhay.HelloWorlds;

public sealed class MyActivitiesGetter : ActivitiesGetter
{
    public MyActivitiesGetter(string[] args) : base(args)
    {
        LoadActivities(new Dictionary<string, Lazy<IActivity?>>
        {
            {
                nameof(GetHelloWorldOutputActivity),
                new Lazy<IActivity?>(() => new GetHelloWorldOutputActivity())
            }
        });
    }
}
