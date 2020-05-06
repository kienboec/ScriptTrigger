using System;
using System.Linq;
using ScriptTrigger.CLI.BusinessLogic;

namespace ScriptTrigger.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            ScriptTriggerImplementation scriptTrigger = new ScriptTriggerImplementation();
            
            scriptTrigger.ExecutionTrigger.PropertyChanged += (sender, args) => {
                if (args.PropertyName == nameof(ExecutionTrigger.LastCycleFired))
                {
                    string stateString = "unknown";
                    ConsoleColor stateColor = ConsoleColor.DarkGray;
                    if (scriptTrigger.ExecutionTrigger.LastCycleFired.HasValue)
                    {
                        stateString = scriptTrigger.ExecutionTrigger.LastCycleFired.Value
                            ? "fire"
                            : "idle";
                        stateColor = scriptTrigger.ExecutionTrigger.LastCycleFired.Value
                            ? ConsoleColor.Green
                            : ConsoleColor.DarkRed;
                    }

                    Console.Write("state change: ");
                    var fg = Console.ForegroundColor;
                    Console.ForegroundColor = stateColor;
                    Console.Write($"{stateString,10}");
                    Console.ForegroundColor = fg;
                    Console.WriteLine($" ({DateTime.Now.ToShortTimeString()})");
                }
            };

            scriptTrigger.Executor.Executed += (sender, eventArgs) =>
            {
                Console.WriteLine("execution ended"); 

            };

            if (scriptTrigger.ShouldDrawHelp)
            {
                Console.WriteLine(scriptTrigger.GetHelpMessage());
                Console.WriteLine("--------------------------------------------------");
                return;
            }

            scriptTrigger.ExecutionTrigger.Wait();
        }
    }
}
