using Songhay.HelloWorlds.Client.Models;
using System;

namespace Songhay.HelloWorlds.Client.Activities
{
    public class GetHelloWorldActivity : IActivity
    {
        public void Start(string[] args)
        {
            if (args.Length < 1) throw new ArgumentException("The expected world name is not here.");

            var worldName = args[0];
            Console.WriteLine($"Hello from world {worldName}!");
        }
    }
}
