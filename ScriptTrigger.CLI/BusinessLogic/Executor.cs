using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScriptTrigger.CLI.BusinessLogic.ExecutionAction;
using ScriptTrigger.CLI.BusinessLogic.Infrastructure;

namespace ScriptTrigger.CLI.BusinessLogic
{
    public class Executor : NotifyPropertyChangedBase
    {
        #region static 

        public static List<Tuple<ExecutionActionTypeEnum, string>> ExecutionTypes { get; }
            = new List<Tuple<ExecutionActionTypeEnum, string>>()
            {
                new Tuple<ExecutionActionTypeEnum, string>(ExecutionActionTypeEnum.Script, "script"),
                new Tuple<ExecutionActionTypeEnum, string>(ExecutionActionTypeEnum.Command, "command"),
            };

        #endregion

        #region Fields / Events

        private IExecutionAction _executionAction;
        public event EventHandler<EventArgs> Executed;

        #endregion

        #region Property

        private ExecutionActionTypeEnum _type = ExecutionActionTypeEnum.None;
        public ExecutionActionTypeEnum Type
        {
            get => Get(_type);
            set => Set(ref _type, value, SetExecutionAction, nameof(Type), nameof(TypeDisplayName));
        }

        public string TypeDisplayName
        {
            get => ExecutionTypes.FirstOrDefault(x => x.Item1 == Type)?.Item2;
            set => Type = EnumParse(value, true, ExecutionActionTypeEnum.None);
        }

        private string _action;
        public string Action
        {
            get => Get(_action);
            set => Set(ref _action, value, nameof(Action));
        }

        #endregion

        private void SetExecutionAction(ExecutionActionTypeEnum type)
        {
            _executionAction = type switch
            {
                ExecutionActionTypeEnum.Script => (IExecutionAction) new ExecutionActionScript(),
                ExecutionActionTypeEnum.Command => new ExecutionActionCommand(),
                ExecutionActionTypeEnum.None => new ExecutionActionNone(),
                _ => new ExecutionActionNone()
            };
        }

        public string ExecutionOutput => this._executionAction?.ExecutionOutput;

        public async Task Execute()
        {
            await Task.Run(() =>
            {
                _executionAction.Execute(Action);
                Executed?.Invoke(this, EventArgs.Empty);
            }).ConfigureAwait(false);
        }
    }
}
