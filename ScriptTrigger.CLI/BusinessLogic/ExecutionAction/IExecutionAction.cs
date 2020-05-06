using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptTrigger.CLI.BusinessLogic.ExecutionAction
{
    public interface IExecutionAction
    {
        void Execute(string action);
        string ExecutionOutput { get; }
    }
}
