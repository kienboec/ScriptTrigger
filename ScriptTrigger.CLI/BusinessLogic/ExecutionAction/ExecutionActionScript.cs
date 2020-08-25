using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptTrigger.CLI.BusinessLogic.ExecutionAction
{
    public class ExecutionActionScript : ExecutionActionBase
    {
        protected override void ExecuteInternal(string action)
        {
            this.Output = "started: " + DateTime.Now.ToString("O") + Environment.NewLine;
            this.Error = "";

            ProcessStartInfo startInfo = new ProcessStartInfo("cmd");
            startInfo.ArgumentList.Add("/c");
            startInfo.ArgumentList.Add(action);
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = false;

            StarTime = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Process process = Process.Start(startInfo);
            process.EnableRaisingEvents = false;

            if (!process.WaitForExit(60_000))
            {
                process.Close();
                Error += Environment.NewLine + "canceled!";
            }

            stopwatch.Stop();
            this.Output += "ended: " + DateTime.Now.ToString("O") + Environment.NewLine;
            this.ExitCode = process?.ExitCode ?? null;
            this.ElapsedMs = stopwatch.ElapsedMilliseconds;

        }
    }
}
