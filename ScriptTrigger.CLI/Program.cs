using System;
using System.Linq;
using ScriptTrigger.CLI.BusinessLogic;

namespace ScriptTrigger.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            using ScriptTriggerImplementation scriptTrigger = new ScriptTriggerImplementation();
            if (scriptTrigger.ShouldOnlyDrawHelp)
            {
                Console.WriteLine(ScriptTriggerImplementation.GetHelpMessage());
                return;
            }

            scriptTrigger.ExecutionTrigger.FireChanged += (sender, args) =>
            {
                var stateString = args.Triggered ? "fire" : "idle";
                var stateColor = args.Triggered ? ConsoleColor.Green : ConsoleColor.DarkRed;

                Console.Write("state change: ");
                var fg = Console.ForegroundColor;
                Console.ForegroundColor = stateColor;
                Console.Write($"{stateString,10}");
                Console.ForegroundColor = fg;
                Console.WriteLine($" ({DateTime.Now.ToShortTimeString()})");
            };

            scriptTrigger.Executor.Executed += (sender, eventArgs) =>
            {
                Console.WriteLine("<execution ended>");

            };

            Console.WriteLine(scriptTrigger.GetStateMessage());
            #pragma warning disable CA1303 // Literale nicht als lokalisierte Parameter übergeben
            Console.WriteLine("--------------------------------------------------");
            #pragma warning restore CA1303 // Literale nicht als lokalisierte Parameter übergeben

            scriptTrigger.ExecutionTrigger.Wait();
        }
    }
}
