using System;
using System.Linq;
using ScriptTrigger.CLI.BusinessLogic;

namespace ScriptTrigger.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            using ScriptTriggerImplementation scriptTrigger = new ScriptTriggerImplementation(args);
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
            Console.WriteLine("--------------------------------------------------");

            scriptTrigger.ExecutionTrigger.Wait();
        }
    }
}
