using System;
using ScriptTrigger.CLI.BusinessLogic.Infrastructure;

namespace ScriptTrigger.CLI.BusinessLogic
{
    public class ScriptTriggerImplementation : NotifyPropertyChangedBase
    {
        public Executor Executor { get; }
        public ExecutionTrigger ExecutionTrigger { get; }
        public bool ShouldOnlyDrawHelp { get; }

        public ScriptTriggerImplementation() : this(null)
        {
        }

        public ScriptTriggerImplementation(string[] commandLineArgs)
        {
            var commandLineArgumentInterpreter = new CommandLineArgumentInterpreter(commandLineArgs);
            ExecutionTrigger = new ExecutionTrigger()
            {
                Delay = TimeSpan.FromSeconds(3),
                IsListening = commandLineArgumentInterpreter.ShouldListen,
                LastCycleFired = null,
                SourceTypeDisplayName = commandLineArgumentInterpreter.Trigger,
                Value = commandLineArgumentInterpreter.Value,
            };

            Executor = new Executor()
            {
                Action = commandLineArgumentInterpreter.Action,
                TypeDisplayName = commandLineArgumentInterpreter.Type,
            };
            
            this.ShouldOnlyDrawHelp = commandLineArgumentInterpreter.ShouldOnlyDrawHelp;

            this.ExecutionTrigger.Fire += (sender, args) => Executor.Execute();
            this.ExecutionTrigger.PropertyChanged += (sender, args) => DelegateRaisePropertyChanged(nameof(ExecutionTrigger));
            this.Executor.PropertyChanged += (sender, args) => DelegateRaisePropertyChanged(nameof(Executor));
        }

        public static string GetHelpMessage()
        {
            return "see help at: https://kienboec.github.io/ScriptTrigger/";
        }

        public string GetStateMessage()
        {
            return 
$@"Script Trigger
- Trigger: {ExecutionTrigger.SourceTypeDisplayName}
- Value  : {ExecutionTrigger.Value}
- Type   : {Executor.TypeDisplayName}
- Action : {Executor.Action}
--------------------------------------------------";
        }
    }
}
