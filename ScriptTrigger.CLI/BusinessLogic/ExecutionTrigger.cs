using ScriptTrigger.CLI.BusinessLogic.ExecutionTriggerSource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptTrigger.CLI.BusinessLogic
{
    public class ExecutionTrigger : INotifyPropertyChanged
    {
        public static List<Tuple<ExecutionTriggerSourceTypeEnum, string>> ExecutionTriggers { get; }
            = new List<Tuple<ExecutionTriggerSourceTypeEnum, string>>()
        {
            new Tuple<ExecutionTriggerSourceTypeEnum, string>(ExecutionTriggerSourceTypeEnum.None, "none"),
            new Tuple<ExecutionTriggerSourceTypeEnum, string>(ExecutionTriggerSourceTypeEnum.OpenPort, "open port"),
            new Tuple<ExecutionTriggerSourceTypeEnum, string>(ExecutionTriggerSourceTypeEnum.DataChanges, "data changes"),
        };

        private bool _isListeningBackingField;
        private Task _listeningTask;
        private CancellationTokenSource _listeningTaskCancellationTokenSource;
        private TimeSpan _delay;
        private bool? _lastCycleFired;
        private string _value;
        private IExecutionTriggerSource _executionTriggerSource = null;
        private ExecutionTriggerSourceTypeEnum _sourceType;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ExecutionTriggerFiredEventArgs> Fire;

        public TimeSpan Delay
        {
            get => _delay; set
            {
                _delay = value;
                OnPropertyChanged(nameof(Delay));
            }
        }

        public bool? LastCycleFired
        {
            get => _lastCycleFired;
            set
            {
                _lastCycleFired = value;
                OnPropertyChanged(nameof(LastCycleFired));
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public ExecutionTriggerSourceTypeEnum SourceType
        {
            get => _sourceType;
            set
            {
                _sourceType = value;

                switch (_sourceType)
                {
                    case ExecutionTriggerSourceTypeEnum.OpenPort:
                        _executionTriggerSource = new ExecutionTriggerSourceOpenPort();
                        break;

                }

                OnPropertyChanged(nameof(SourceType));
            }
        }

        public string SourceTypeDisplayName
            => ExecutionTriggers.FirstOrDefault(x => x.Item1 == SourceType)?.Item2;

        public ExecutionTrigger()
        {
            this._isListeningBackingField = false;
            this._listeningTask = null;
            this._listeningTaskCancellationTokenSource = null;
            this._delay = TimeSpan.FromSeconds(2);
            this._sourceType = ExecutionTriggerSourceTypeEnum.OpenPort;
            this._executionTriggerSource = new ExecutionTriggerSourceOpenPort();
            this._lastCycleFired = null;
            this._value = null;
        }

        public bool IsListening
        {
            get => _isListeningBackingField;
            set
            {
                _isListeningBackingField = value;
                if (_isListeningBackingField && this._listeningTask == null)
                {
                    this._listeningTaskCancellationTokenSource = new CancellationTokenSource();
                    this._listeningTaskCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    this._listeningTask = this.CreateListeningTask();
                }
                else if (!this._isListeningBackingField && this._listeningTask != null)
                {
                    this._listeningTaskCancellationTokenSource.Cancel();
                    this._listeningTask.Wait();
                    this._listeningTask = null;
                    this._listeningTaskCancellationTokenSource = null;
                }
            }
        }
        public static ExecutionTriggerSourceTypeEnum GetTypeByTypeNameOrNone(string type = "None")
        {
            return
                ExecutionTriggers.FirstOrDefault(x => x.Item1.ToString() == type)?.Item1
                ?? ExecutionTriggerSourceTypeEnum.None;
        }

        public void Wait(bool setListening = true)
        {
            if (!IsListening && setListening)
            {
                this.IsListening = true;
            }

            this._listeningTask.Wait(this._listeningTaskCancellationTokenSource.Token);
        }

        private Task CreateListeningTask()
        {
            return Task.Run(async () =>
            {
                try
                {
                    while (this.IsListening)
                    {
                        bool cycleShouldFire;
                        try
                        {
                            cycleShouldFire = this._executionTriggerSource.CheckTrigger(this.Value);
                        }
                        catch
                        {
                            cycleShouldFire = false;
                        }

                        if (cycleShouldFire != this.LastCycleFired)
                        {
                            this.LastCycleFired = cycleShouldFire;
                            if (cycleShouldFire)
                            {
                                Fire?.Invoke(this, new ExecutionTriggerFiredEventArgs());
                            }
                        }

                        await Task.Delay(this.Delay, this._listeningTaskCancellationTokenSource.Token)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {

                }
                catch (Exception)
                {
                    this.IsListening = false;
                }
            }, this._listeningTaskCancellationTokenSource.Token);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
