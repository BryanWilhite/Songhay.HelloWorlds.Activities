# Songhay Shell Activities

This is a simple demonstration of the Songhay System Shell Activities architecture. Songhay Shell Activities is a replacement for the retired Data Activity Runner (DAR), previously on CodePlex.

This new approach depends on these key definitions:

The `IActivity` [interface](https://github.com/BryanWilhite/Songhay.HelloWorlds.Activities/blob/master/Songhay.HelloWorlds.Activities/Models/IActivity.cs). This interface defines the _Activity_. Its associated [extension methods](https://github.com/BryanWilhite/Songhay.HelloWorlds.Activities/blob/master/Songhay.HelloWorlds.Activities/Extensions/IActivityExtensions.cs) intend to make the console `Program` [class](./Songhay.HelloWorlds.Shell/Program.cs) as simple as possible:

```c#
static void Main(string[] args)
{
    using (var listener = new TextWriterTraceListener(Console.Out))
    {
        traceSource.Listeners.Add(listener);

        var activityName = args.ToActivityName();
        var activity = (new ActivitiesGetter()).GetActivity(activityName);
        activity.Start(args.ToActivityArgs());

        listener.Flush();
    }
}
```

The `ActivitiesGetter` [class](./Songhay.HelloWorlds.Activities/ActivitiesGetter.cs) is the central store of all the Activities. It reveals the convention that an Activity is called by its class name from the command line. This implies that the first argument of the shell command refers to the name of the Activity and all other arguments are passed directly to the Activity.

## Why are you using `TraceSource`?

The use of `TraceSource` is detailed in my ‘.NET Core `TextWriterTraceListener`’ [sample](https://github.com/BryanWilhite/dotnet-core/tree/master/dotnet-console-textwritertracelistener).
