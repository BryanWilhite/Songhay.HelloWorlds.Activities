﻿using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using System.Diagnostics;
using System.Linq;

namespace Songhay.HelloWorlds.Activities
{
    public class GetHelloWorldActivity : IActivity
    {
        static GetHelloWorldActivity() => traceSource = TraceSources
            .Instance
            .GetTraceSourceFromConfiguredName()
            .WithSourceLevels();

        internal static readonly TraceSource traceSource;

        public string DisplayHelp(ProgramArgs args)
        {
            if (!args.HelpSet.Any()) this.SetupHelp(args);

            return args.ToHelpDisplayText();
        }

        public void Start(ProgramArgs args)
        {
            var worldName = args.GetArgValue(argWorldName);
            traceSource?.TraceInformation(GetHelloWorldMessage(worldName));
        }

        internal static string GetHelloWorldMessage(string worldName) => $"Hello from world {worldName}!";

        void SetupHelp(ProgramArgs args)
        {
            var indentation = string.Join(string.Empty, Enumerable.Repeat(" ", 4).ToArray());
            args.HelpSet.Add(argWorldName, $"{argWorldName} <world name>{indentation}Returns a greeting for the specified world.");
            args.HelpSet.Add(argMoonList, $"{argMoonList} \"<list of moons>\"{indentation}A comma-separated list of moons.");
            args.HelpSet.Add(argMoonsRequired, $"{argMoonsRequired}{indentation}Indicates that moons are required for the world.");
        }

        const string argMoonList = "--moon-list";
        const string argMoonsRequired = "--moons-required";
        const string argWorldName = "--world-name";
    }
}
