using Songhay.HelloWorlds.Activities;
using System.Reflection;

namespace Songhay.HelloWorlds.Shell;

static class Program
{
    static void DisplayCredits()
    {
        Console.Write(ProgramAssemblyUtility.GetAssemblyInfo(Assembly.GetExecutingAssembly(), true));
        Console.WriteLine(string.Empty);
        Console.WriteLine("Activities Assembly:");
        Console.Write(ProgramAssemblyUtility.GetAssemblyInfo(typeof(TopTenActivity).Assembly, true));
    }

    static void Main(string[] args)
    {
        DisplayCredits();

#if DEBUG
        Console.WriteLine($"{Environment.NewLine}Press any key to continue...");
        Console.ReadKey(false);
#endif

        Environment.Exit(0);
    }
}
