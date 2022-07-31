namespace Songhay.HelloWorlds.Activities;

public class GetHelloWorldActivity : IActivity
{
    static GetHelloWorldActivity() => TraceSource = TraceSources
        .Instance
        .GetTraceSourceFromConfiguredName()
        .WithSourceLevels();

    internal static readonly TraceSource? TraceSource;

    public string DisplayHelp(ProgramArgs? args)
    {
        if(args == null) return string.Empty;

        if (!args.HelpSet.Any()) SetupHelp(args);

        return args.ToHelpDisplayText() ?? string.Empty;
    }

    public void Start(ProgramArgs? args)
    {
        var worldName = args.GetArgValue(ArgWorldName);
        TraceSource?.WriteLine(GetHelloWorldMessage(worldName));
    }

    internal static string GetHelloWorldMessage(string? worldName) => $"Hello from world {worldName}!";

    void SetupHelp(ProgramArgs? args)
    {
        if(args == null) return;

        var indentation = string.Join(string.Empty, Enumerable.Repeat(" ", 4).ToArray());

        args.HelpSet.Add(ArgWorldName, $"{ArgWorldName} <world name>{indentation}Returns a greeting for the specified world.");
        args.HelpSet.Add(ArgMoonList, $"{ArgMoonList} \"<list of moons>\"{indentation}A comma-separated list of moons.");
        args.HelpSet.Add(ArgMoonsRequired, $"{ArgMoonsRequired}{indentation}Indicates that moons are required for the world.");
    }

    const string ArgMoonList = "--moon-list";
    const string ArgMoonsRequired = "--moons-required";
    const string ArgWorldName = "--world-name";
}
