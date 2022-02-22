# Songhay System Activities example

This is a simple demonstration of the Songhay System Activities ‘tiny’ architecture, running in the shell and in a serverless context.

Songhay System Activities is part of the `SonghayCore` [repo](https://github.com/BryanWilhite/SonghayCore). It is intended to encapsulate and decouple “business logic” running in:

- any .NET Shell
- any .NET ASP.NET context, including Azure Functions
- any .NET Service context of [a modular desktop application](https://prismlibrary.github.io/docs/wpf/legacy/Modules.html)

## key class definitions

Songhay System Activities depends on these key definitions:

- The `IActivity` [interface](https://github.com/BryanWilhite/SonghayCore/blob/master/SonghayCore/Models/IActivity.cs). This interface defines the _Activity_.
- The `IActivity` lazy loading [extension methods](https://github.com/BryanWilhite/SonghayCore/blob/master/SonghayCore/Extensions/IActivityExtensions.Lazy.cs), featuring support for `Dictionary<string, Lazy<IActivity>>` that is used in the `ActivitiesGetter` [class](./Songhay.HelloWorlds.Activities/ActivitiesGetter.cs).
- The `IActivityOutput` [interface](https://github.com/BryanWilhite/SonghayCore/blob/master/SonghayCore/Models/IActivityOutput.cs) extends `IActivity` with fundamental input/output, allowing the Activity to support both synchronous activity (usually for a Shell application) and asynchronous output (usually for everything else).
- The `ActivitiesGetter` [class](./Songhay.HelloWorlds.Activities/ActivitiesGetter.cs). The central store of the Activities. It reveals the convention that an Activity is called by its class name from the command line. This implies that the first argument of the shell command refers to the name of the Activity and all other arguments are passed directly to the Activity.

## running from the command line

The `tasks.json` [file](../.vscode/tasks.json) for Visual Studio Code contains the commands used to run builds. For example, to run the release from [the shell project folder](./Songhay.HelloWorlds.Shell) enter:

```bash
dotnet run --configuration Release -- GetHelloWorldActivity --world-name Saturn
```

## history and some FAQs

Songhay Shell Activities is a replacement for the retired Data Activity Runner (DAR), previously on CodePlex.

### why are you using `TraceSource`?

The use of `TraceSource` is detailed in my ‘.NET Core `TextWriterTraceListener`’ [sample](https://github.com/BryanWilhite/dotnet-core/tree/master/dotnet-console-textwritertracelistener).

### why are you not using an IoC container?

Based on my current and often limited understanding, the “opportunity” to use an IoC container comes when there is a need to use multiple instances of the `ActivitiesGetter` [class](./Songhay.HelloWorlds.Activities/ActivitiesGetter.cs). This is a level of real-world complexity beyond the scope of the intent here.

Later I may discover through the “kindness” of others that my use of lazy-loading dictionary to get instances of `IActivity` by default should actually be replaced by an IoC container with, say, [hierarchy features](http://unitycontainer.org/tutorials/hierarchies.html).

@[BryanWilhite](https://twitter.com/BryanWilhite)
