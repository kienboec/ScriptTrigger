using System.Diagnostics;

namespace ScriptTrigger.BusinessLogic
{
    public class Executor
    {
        public string Script { get; set; }

        public void Execute()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.Script))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(this.Script);
                    Process.Start(startInfo);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
