using System;

namespace ScriptTrigger.CLI.BusinessLogic
{
    public class CommandLineArgumentInterpreter
    {
        public bool ShouldDrawHelp { get; } = false;
        public bool HasInvalidParameters { get; } = false;
        public string Value { get; } = null;
        public string Trigger { get; } = null;
        public string Type { get; } = null;
        public string Action { get; } = null;
        public bool ShouldListen { get; } = false;

        public CommandLineArgumentInterpreter()
        {
            try
            {
                foreach (string arg in Environment.GetCommandLineArgs())
                {
                    if (arg.StartsWith("-h", StringComparison.InvariantCulture) ||
                        arg.StartsWith("/?", StringComparison.InvariantCulture) ||
                        arg.StartsWith("--help", StringComparison.InvariantCulture))
                    {
                        ShouldDrawHelp = true;
                    }
                    else if (arg.StartsWith("-v=", StringComparison.InvariantCulture) ||
                        arg.StartsWith("-v:", StringComparison.InvariantCulture))
                    {
                        Value = arg.Substring(3);
                    }
                    else if (arg.StartsWith("-t=", StringComparison.InvariantCulture) ||
                             arg.StartsWith("-t:", StringComparison.InvariantCulture))
                    {
                        Trigger = arg.Substring(3);
                    }
                    else if (arg.StartsWith("-s=", StringComparison.InvariantCulture) ||
                             arg.StartsWith("-s:", StringComparison.InvariantCulture))
                    {
                        Type = "Script";
                        Action = arg.Substring(3);
                    }
                    else if (arg.StartsWith("-c=", StringComparison.InvariantCulture) ||
                             arg.StartsWith("-c:", StringComparison.InvariantCulture))
                    {
                        Type = "Command";
                        Action = arg.Substring(3);
                    }
                    else if (arg.StartsWith("-e=", StringComparison.InvariantCulture) ||
                             arg.StartsWith("-e:", StringComparison.InvariantCulture))
                    {
                        Type = arg.Substring(3);
                    }
                    else if (arg.StartsWith("-a=", StringComparison.InvariantCulture) ||
                             arg.StartsWith("-a:", StringComparison.InvariantCulture))
                    {
                        Action = arg.Substring(3);
                    }
                    else if (arg.StartsWith("-l=", StringComparison.InvariantCulture) ||
                             arg.StartsWith("-l:", StringComparison.InvariantCulture))
                    {
                        if (!bool.TryParse(arg.Substring(3), out var shouldListen))
                        {
                            ShouldListen = false;
                        }
                        else
                        {
                            ShouldListen = shouldListen;
                        }
                    }
                    else
                    {
                        ShouldDrawHelp = true;
                        HasInvalidParameters = true;
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
