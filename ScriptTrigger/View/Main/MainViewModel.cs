using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ScriptTrigger.BusinessLogic;

namespace ScriptTrigger.View.Main
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ExecutionTrigger _trigger;
        private readonly Executor _executor;
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand ToggleListeningCommand { get; }
        public List<Tuple<ExecutionTriggerSourceTypeEnum, string>> ExecutionTriggers { get; }

        public MainViewModel()
        {
            _executor = new Executor();
            _trigger = new ExecutionTrigger();
            
            _trigger.Fire += (sender, args) => this._executor.Execute();
            _trigger.LastCycleFireChanged += (sender, args) =>
            {
                OnPropertyChanged(nameof(LastCycleFired));
                OnPropertyChanged(nameof(LastFiredStateBackground));
            };

            ExecutionTriggers = new List<Tuple<ExecutionTriggerSourceTypeEnum, string>>()
            {
                new Tuple<ExecutionTriggerSourceTypeEnum, string>(ExecutionTriggerSourceTypeEnum.OpenPort, "open port"),
            };
            this.ToggleListeningCommand = new RelayCommand((parameter) => this.IsListening = !this.IsListening);

            InterpretCommandLineArgs();
        }

        private void InterpretCommandLineArgs()
        {
            try
            {
                string valueArg = null;
                string triggerArg = null;
                string scriptArg = null;
                bool shouldListenArg = false;

                foreach (string arg in Environment.GetCommandLineArgs())
                {
                    if (arg.StartsWith("-v=", StringComparison.InvariantCulture) ||
                        arg.StartsWith("-v:", StringComparison.InvariantCulture))
                    {
                        valueArg = arg.Substring(3);
                    }
                    else if (arg.StartsWith("-t=", StringComparison.InvariantCulture) ||
                             arg.StartsWith("-t:", StringComparison.InvariantCulture))
                    {
                        triggerArg = arg.Substring(3);
                    }
                    else if (arg.StartsWith("-s=", StringComparison.InvariantCulture) ||
                             arg.StartsWith("-s:", StringComparison.InvariantCulture))
                    {
                        scriptArg = arg.Substring(3);
                    }
                    else if (arg.StartsWith("-l=", StringComparison.InvariantCulture) ||
                             arg.StartsWith("-l:", StringComparison.InvariantCulture))
                    {
                        if (!bool.TryParse(arg.Substring(3), out shouldListenArg))
                        {
                            shouldListenArg = false;
                        }
                    }
                }

                this.Value = valueArg ?? string.Empty;
                this.SelectedExecutionTrigger =
                    this.ExecutionTriggers.FirstOrDefault(x => x.Item1.ToString() == triggerArg) ??
                    this.ExecutionTriggers.First();
                this.Script = scriptArg ?? string.Empty;

                if (shouldListenArg)
                {
                    this.IsListening = true;
                }
            }
            catch
            {
                // ignored
            }
        }

        public string Value
        {
            get => _trigger.Parameter;
            set
            {
                _trigger.Parameter = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public string Script
        {
            get => this._executor.Script;
            set
            {
                this._executor.Script = value;
                OnPropertyChanged(nameof(Script));
            }
        }

        public Tuple<ExecutionTriggerSourceTypeEnum, string> SelectedExecutionTrigger
        {
            get => this.ExecutionTriggers.FirstOrDefault(x => x.Item1 == _trigger.SourceType);
            set
            {
                _trigger.SourceType = value?.Item1 ?? ExecutionTriggerSourceTypeEnum.OpenPort;
                OnPropertyChanged(nameof(SelectedExecutionTrigger));
            }
        }

        private bool IsListening
        {
            get => this._trigger.IsListening;
            set
            {
                this._trigger.IsListening = value;
                OnPropertyChanged(nameof(IsListening));
                OnPropertyChanged(nameof(IsListeningText));
                OnPropertyChanged(nameof(IsListeningVisible));
            }
        }
        public string IsListeningText => this._trigger.IsListening ? "On" : "Off";
        public Visibility IsListeningVisible => this._trigger.IsListening ? Visibility.Visible : Visibility.Collapsed;

        private bool? LastCycleFired
        {
            get => this._trigger.LastCycleFired;
            set
            {
                this._trigger.LastCycleFired = value;
                OnPropertyChanged(nameof(LastCycleFired));
                OnPropertyChanged(nameof(LastFiredStateBackground));
            }
        }

        public SolidColorBrush LastFiredStateBackground =>
            this.LastCycleFired.HasValue 
                ? (this.LastCycleFired.Value ? new SolidColorBrush(Colors.DarkSeaGreen) : new SolidColorBrush(Colors.Coral))
                : new SolidColorBrush(Colors.Beige);

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
