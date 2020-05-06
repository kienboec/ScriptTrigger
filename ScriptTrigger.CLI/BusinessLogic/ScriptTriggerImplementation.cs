using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ScriptTrigger.CLI.BusinessLogic.ExecutionAction;

namespace ScriptTrigger.CLI.BusinessLogic
{
    public class ScriptTriggerImplementation : INotifyPropertyChanged
    {
        private CommandLineArgumentInterpreter CommandLineArgumentInterpreter { get; }
        public Executor Executor { get; }
        public ExecutionTrigger ExecutionTrigger { get; }
        public bool ShouldDrawHelp { get; set; }

        
        
        public ScriptTriggerImplementation()
        {
            CommandLineArgumentInterpreter = new CommandLineArgumentInterpreter();
            ExecutionTrigger = new ExecutionTrigger();
            Executor = new Executor();

            this.ExecutionTrigger.SourceType = ExecutionTrigger.GetTypeByTypeNameOrNone(CommandLineArgumentInterpreter.Trigger);
            this.ExecutionTrigger.Value = CommandLineArgumentInterpreter.Value;
            this.ExecutionTrigger.Delay = TimeSpan.FromSeconds(3);
            this.ExecutionTrigger.Fire += (sender, args) => Executor.Execute();
            this.ExecutionTrigger.IsListening = CommandLineArgumentInterpreter.ShouldListen;

            this.Executor.TypeDisplayName = CommandLineArgumentInterpreter.Type;
            this.Executor.Action = CommandLineArgumentInterpreter.Action;

            

            this.ShouldDrawHelp = CommandLineArgumentInterpreter.ShouldDrawHelp;
        }

        public string GetHelpMessage()
        {
            return 
$@"Script Trigger
- Trigger: {ExecutionTrigger.SourceTypeDisplayName}
- Value  : {ExecutionTrigger.Value}
- Type   : {Executor.TypeDisplayName}
- Action : {Executor.Action}
--------------------------------------------------";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
