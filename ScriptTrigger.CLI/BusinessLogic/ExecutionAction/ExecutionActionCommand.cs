﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ScriptTrigger.CLI.BusinessLogic.ExecutionAction
{
    public class ExecutionActionCommand : ExecutionActionBase
    {
        protected override void ExecuteInternal(string action)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(action)
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                StandardInputEncoding = Encoding.UTF8
            };


            StarTime = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Process process = Process.Start(startInfo);
            this.Output = process?.StandardOutput?.ReadToEnd();
            this.Error = process?.StandardError?.ReadToEnd();
            this.ExitCode = process?.ExitCode ?? null;
            stopwatch.Stop();
            this.ElapsedMs = stopwatch.ElapsedMilliseconds;
        }
    }
}
