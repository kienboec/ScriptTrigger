using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ScriptTrigger.CLI.BusinessLogic.ExecutionAction;

namespace ScriptTrigger.CLI.BusinessLogic
{
    public class Executor : INotifyPropertyChanged
    {
        private ExecutionActionTypeEnum _type;
        private string _action;
        private string _output;
        private string _error;
        private int? _exitCode = null;
        private long _elapsedMs;
        private DateTime? _starTime = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public static List<Tuple<ExecutionActionTypeEnum, string>> ExecutionTypes { get; }
            = new List<Tuple<ExecutionActionTypeEnum, string>>()
            {
                new Tuple<ExecutionActionTypeEnum, string>(ExecutionActionTypeEnum.Script, "script"),
                new Tuple<ExecutionActionTypeEnum, string>(ExecutionActionTypeEnum.Command, "command"),
            };

        public ExecutionActionTypeEnum Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public string TypeDisplayName
        {
            get { return ExecutionTypes.FirstOrDefault(x => x.Item1 == Type)?.Item2; }
            set
            {
                if (Enum.TryParse(value ?? "None", out ExecutionActionTypeEnum type))
                {
                    this.Type = type;
                }
                else
                {
                    this.Type = ExecutionActionTypeEnum.None;
                }
            }
        }

        public string Action
        {
            get => _action;
            set
            {
                _action = value ?? string.Empty;
                OnPropertyChanged(nameof(Action));
            }
        }

        public string Output
        {
            get => _output;
            set
            {
                _output = value;
                OnPropertyChanged(nameof(Output));
                OnPropertyChanged(nameof(ExecutionOutput));
            }
        }

        public string Error
        {
            get => _error;
            set
            {
                _error = value;
                OnPropertyChanged(nameof(Error));
                OnPropertyChanged(nameof(ExecutionOutput));
            }

        }

        public int? ExitCode
        {
            get => _exitCode;
            set
            {
                _exitCode = value;
                OnPropertyChanged(nameof(ExitCode));
                OnPropertyChanged(nameof(ExecutionOutput));
            }
        }

        public long ElapsedMs
        {
            get => _elapsedMs;
            set
            {
                _elapsedMs = value;
                OnPropertyChanged(nameof(ElapsedMs));
                OnPropertyChanged(nameof(ExecutionOutput));
            }
        }

        public DateTime? StarTime
        {
            get => _starTime;
            set
            {
                _starTime = value;
                OnPropertyChanged(nameof(StarTime));
                OnPropertyChanged(nameof(ExecutionOutput));
            }
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
                    output.AppendLine("exit code : " + this.ExitCode.ToString());

                    if (this.StarTime.HasValue)
                    {
                        output.AppendLine("start time: " + this.StarTime.Value.ToString("u"));
                    }

                    output.AppendLine("elapsed ms: " + this.ElapsedMs.ToString());
                }

                return output.ToString();
            }
        }

        public event EventHandler<EventArgs> Executed;

        public void Execute()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.Action))
                {
                    if (Type == ExecutionActionTypeEnum.None)
                    {
                        // ignore
                    } else if (Type == ExecutionActionTypeEnum.Script)
                    {
                        ExecuteCommand(this.Action);
                    }
                    else if (Type == ExecutionActionTypeEnum.Command)
                    {
                        ExecuteCommand("cmd", "/c", this.Action);
                    }
                }
            }
            catch (Exception exc)
            {
                this.Error = exc.Message;
            }
            finally
            {
                Executed?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ExecuteCommand(string filename, params string[] args)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo(filename);
            foreach (string arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }

            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.StandardErrorEncoding = Encoding.UTF8;
            startInfo.StandardOutputEncoding = Encoding.UTF8;
            startInfo.StandardInputEncoding = Encoding.UTF8;

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


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
