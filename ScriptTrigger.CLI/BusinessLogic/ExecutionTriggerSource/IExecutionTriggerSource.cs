using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptTrigger.CLI.BusinessLogic.ExecutionTriggerSource
{
    public interface IExecutionTriggerSource
    {
        bool CheckTrigger(string parameter);
    }
}
