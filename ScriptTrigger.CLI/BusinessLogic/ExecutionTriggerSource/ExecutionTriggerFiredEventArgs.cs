using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptTrigger.CLI.BusinessLogic.ExecutionTriggerSource
{
    public class ExecutionTriggerFiredEventArgs : EventArgs
    {
        public bool Triggered { get; set; }

        public ExecutionTriggerFiredEventArgs(bool triggered)
        {
            Triggered = triggered;
        }
    }
}
