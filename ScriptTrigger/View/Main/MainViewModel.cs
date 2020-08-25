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
using ScriptTrigger.CLI.BusinessLogic.Infrastructure;
using ScriptTrigger.View.Infrastructure;

namespace ScriptTrigger.View.Main
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        public static List<Tuple<ExecutionTriggerSourceTypeEnum, string>> ExecutionTriggers => ExecutionTrigger.ExecutionTriggers;
        public static List<Tuple<ExecutionActionTypeEnum, string>> ExecutionTypes => Executor.ExecutionTypes;

        private ScriptTriggerImplementation _scriptTrigger;

        public RelayCommand ToggleListeningCommand { get; }
        public RelayCommand ExecuteCommand { get; }
        public RelayCommand ExitCommand { get; }

        public MainViewModel()
        {
            _scriptTrigger = new ScriptTriggerImplementation();
            _scriptTrigger.ExecutionTrigger.FireChanged += (sender, args) =>
            {
                DelegateRaisePropertyChanged(
                    nameof(LastCycleFired),
                    nameof(LastFiredStateBackground));
            };

            _scriptTrigger.Executor.Executed += (sender, args) =>
            {
                DelegateRaisePropertyChanged(nameof(ExecutionOutput));
            };

            this.ToggleListeningCommand = new RelayCommand((p) => this.IsListening = !this.IsListening);
            this.ExecuteCommand = new RelayCommand(async p => await _scriptTrigger.Executor.Execute().ConfigureAwait(false));
            this.ExitCommand = new RelayCommand((p) => Application.Current.Shutdown(0));
        }

        public string Value
        {
            get => Get(_scriptTrigger.ExecutionTrigger.Value);
            set => Propagate(_scriptTrigger.ExecutionTrigger.Value = value, nameof(Value));
        }

        public string Action
        {
            get => Get(this._scriptTrigger.Executor.Action);
            set => Propagate(this._scriptTrigger.Executor.Action = value, nameof(Action));
        }

        public bool? LastCycleFired
        {
            get => Get(this._scriptTrigger.ExecutionTrigger.LastCycleFired);
            set => Propagate(this._scriptTrigger.ExecutionTrigger.LastCycleFired = value,
                nameof(LastCycleFired),
                nameof(LastFiredStateBackground));
        }

        public Tuple<ExecutionTriggerSourceTypeEnum, string> SelectedExecutionTrigger
        {
            get => Get(ExecutionTriggers.FirstOrDefault(x => x.Item1 == _scriptTrigger.ExecutionTrigger.SourceType));
            set => Propagate(
                _scriptTrigger.ExecutionTrigger.SourceType = value?.Item1 ?? ExecutionTriggerSourceTypeEnum.OpenPort,
                nameof(SelectedExecutionTrigger));
        }

        public Tuple<ExecutionActionTypeEnum, string> SelectedExecutionType
        {
            get => Get(ExecutionTypes.FirstOrDefault(x => x.Item1 == _scriptTrigger.Executor.Type));
            set => Propagate(_scriptTrigger.Executor.Type = value?.Item1 ?? ExecutionActionTypeEnum.None, 
                nameof(SelectedExecutionType));
        }
        
        public string ExecutionOutput => Get(this._scriptTrigger.Executor.ExecutionOutput);
        
        public bool IsListening
        {
            get => Get(this._scriptTrigger.ExecutionTrigger.IsListening);
            set => Propagate(this._scriptTrigger.ExecutionTrigger.IsListening = value,
                    nameof(IsListeningText),
                    nameof(IsListeningVisible));

        }

        public SolidColorBrush LastFiredStateBackground => 
            Get(this.LastCycleFired.HasValue
                ? (this.LastCycleFired.Value 
                    ? new SolidColorBrush(Colors.DarkSeaGreen) 
                    : new SolidColorBrush(Colors.Coral))
                : new SolidColorBrush(Colors.Beige));

        public string IsListeningText =>
            Get(this._scriptTrigger.ExecutionTrigger.IsListening 
                ? "On" 
                : "Off");

        public Visibility IsListeningVisible =>
            Get(this._scriptTrigger.ExecutionTrigger.IsListening 
                ? Visibility.Visible 
                : Visibility.Collapsed);


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _scriptTrigger?.Dispose();
                _scriptTrigger = null;
            }
        }
    }
}
