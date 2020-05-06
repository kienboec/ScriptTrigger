using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptTrigger.CLI.BusinessLogic.ExecutionTriggerSource
{
    public class ExecutionTriggerSourceNone : IExecutionTriggerSource
    {
        public bool CheckTrigger(string parameter)
            => false;
    }
}
