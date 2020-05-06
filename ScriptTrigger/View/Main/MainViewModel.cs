using ScriptTrigger.CLI.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ScriptTrigger.CLI.BusinessLogic.ExecutionAction;
using ScriptTrigger.CLI.BusinessLogic.ExecutionTriggerSource;
using ScriptTrigger.View.Infrastructure;

namespace ScriptTrigger.View.Main
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ScriptTriggerImplementation _scriptTrigger;
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand ToggleListeningCommand { get; }
        public RelayCommand ExecuteCommand { get; }
        public RelayCommand ExitCommand { get; }
        public MainViewModel()
        {
            _scriptTrigger = new ScriptTriggerImplementation();
            _scriptTrigger.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ExecutionTrigger.LastCycleFired))
                {
                    OnPropertyChanged(nameof(LastCycleFired));
                    OnPropertyChanged(nameof(LastFiredStateBackground));
                }
            };

            _scriptTrigger.Executor.Executed += (sender, args) =>
            {
                OnPropertyChanged(nameof(ExecutionOutput));
            };

            this.ToggleListeningCommand = new RelayCommand((p) => this.IsListening = !this.IsListening);
            this.ExecuteCommand = new RelayCommand(p => _scriptTrigger.Executor.Execute());
            this.ExitCommand = new RelayCommand((p) => Application.Current.Shutdown(0));
        }

        public string Value
        {
            get => _scriptTrigger.ExecutionTrigger.Value;
            set
            {
                _scriptTrigger.ExecutionTrigger.Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public string Action
        {
            get => this._scriptTrigger.Executor.Action;
            set
            {
                this._scriptTrigger.Executor.Action = value;
                OnPropertyChanged(nameof(Action));
            }
        }

        public bool? LastCycleFired
        {
            get => this._scriptTrigger.ExecutionTrigger.LastCycleFired;
            set
            {
                this._scriptTrigger.ExecutionTrigger.LastCycleFired = value;
                OnPropertyChanged(nameof(LastCycleFired));
                OnPropertyChanged(nameof(LastFiredStateBackground));
            }
        }

        public Tuple<ExecutionTriggerSourceTypeEnum, string> SelectedExecutionTrigger
        {
            get => this.ExecutionTriggers.FirstOrDefault(x => x.Item1 == _scriptTrigger.ExecutionTrigger.SourceType);
            set
            {
                _scriptTrigger.ExecutionTrigger.SourceType = value?.Item1 ?? ExecutionTriggerSourceTypeEnum.OpenPort;
                OnPropertyChanged(nameof(SelectedExecutionTrigger));
            }
        }

        public Tuple<ExecutionActionTypeEnum, string> SelectedExecutionType
        {
            get => this.ExecutionTypes.FirstOrDefault(x => x.Item1 == _scriptTrigger.Executor.Type);
            set
            {
                _scriptTrigger.Executor.Type = value?.Item1 ?? ExecutionActionTypeEnum.Script;
                OnPropertyChanged(nameof(SelectedExecutionType));
            }
        }
        public bool IsListening
        {
            get => this._scriptTrigger.ExecutionTrigger.IsListening;
            set
            {
                this._scriptTrigger.ExecutionTrigger.IsListening = value;

                OnPropertyChanged(nameof(IsListeningText));
                OnPropertyChanged(nameof(IsListeningVisible));
            }
        }

        public SolidColorBrush LastFiredStateBackground =>
            this.LastCycleFired.HasValue
                ? (this.LastCycleFired.Value ? new SolidColorBrush(Colors.DarkSeaGreen) : new SolidColorBrush(Colors.Coral))
                : new SolidColorBrush(Colors.Beige);

        public string ExecutionOutput => this._scriptTrigger.Executor.ExecutionOutput;
        public List<Tuple<ExecutionTriggerSourceTypeEnum, string>> ExecutionTriggers => ExecutionTrigger.ExecutionTriggers;
        public List<Tuple<ExecutionActionTypeEnum, string>> ExecutionTypes => Executor.ExecutionTypes;

        public string IsListeningText => this._scriptTrigger.ExecutionTrigger.IsListening ? "On" : "Off";
        public Visibility IsListeningVisible => this._scriptTrigger.ExecutionTrigger.IsListening ? Visibility.Visible : Visibility.Collapsed;


        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
