using System;
using System.Collections.Generic;
using System.Text;
using ScriptTrigger.CLI.BusinessLogic.Infrastructure;

namespace ScriptTrigger.CLI.BusinessLogic.ExecutionAction
{
    public abstract class ExecutionActionBase : NotifyPropertyChangedBase, IExecutionAction
    {
        public void Execute(string action)
        {
            try
            {
                ExecuteInternal(action);
            }
            catch (Exception exc)
            {
                this.Error = exc.Message;
            }
        }

        protected abstract void ExecuteInternal(string action);

        private string _output;
        public string Output
        {
            get => Get(_output);
            set => Set(ref _output, value, nameof(Output), nameof(ExecutionOutput));
        }

        private string _error;
        public string Error
        {
            get => Get(_error);
            set => Set(ref _error, value, nameof(Error), nameof(ExecutionOutput));
        }

        private int? _exitCode = null;
        public int? ExitCode
        {
            get => Get(_exitCode);
            set => Set(ref _exitCode, value, nameof(ExitCode), nameof(ExecutionOutput));
        }

        private long _elapsedMs;
        public long ElapsedMs
        {
            get => Get(_elapsedMs);
            set => Set(ref _elapsedMs, value, nameof(ElapsedMs), nameof(ExecutionOutput));
        }

        private DateTime? _starTime = null;
        public DateTime? StarTime
        {
            get => Get(_starTime);
            set => Set(ref _starTime, value, nameof(StarTime), nameof(ExecutionOutput));
        }

        public string ExecutionOutput
        {
            get
            {
                StringBuilder output = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(this.Output))
                {
                    output.AppendLine(Output);
                }

                if (!string.IsNullOrWhiteSpace(this.Output))
                {
                    if (output.Length > 0)
                    {
                        output.AppendLine();
                        output.AppendLine("-------------------");
                    }

                    output.AppendLine("Error: ");
                    output.AppendLine(Error);
                }

                if (this.ExitCode != null)
                {
                    output.AppendLine();
                    output.AppendLine($"exit code : {this.ExitCode}");

                    if (this.StarTime.HasValue)
                    {
                        output.AppendLine("start time: " + this.StarTime.Value.ToString("u", Internationalization.DefaultCulture));
                    }

                    output.AppendLine($"elapsed ms: {this.ElapsedMs}");
                }

                return output.ToString();
            }
        }
    }
}
