using ScriptTrigger.CLI.BusinessLogic.ExecutionTriggerSource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using ScriptTrigger.CLI.BusinessLogic.Infrastructure;

namespace ScriptTrigger.CLI.BusinessLogic
{
    public class ExecutionTrigger : NotifyPropertyChangedBase
    {
        #region static

        public static List<Tuple<ExecutionTriggerSourceTypeEnum, string>> ExecutionTriggers { get; }
            = new List<Tuple<ExecutionTriggerSourceTypeEnum, string>>()
        {
            new Tuple<ExecutionTriggerSourceTypeEnum, string>(ExecutionTriggerSourceTypeEnum.None, "none"),
            new Tuple<ExecutionTriggerSourceTypeEnum, string>(ExecutionTriggerSourceTypeEnum.OpenPort, "open port"),
            //new Tuple<ExecutionTriggerSourceTypeEnum, string>(ExecutionTriggerSourceTypeEnum.DataChanges, "data changes"),
        };

        #endregion

        public ExecutionTrigger()
        {
            this._isListening = false;
            this._listeningTask = null;
            this._listeningTaskCancellationTokenSource = null;
            this._delay = TimeSpan.FromSeconds(2);
            this._sourceType = ExecutionTriggerSourceTypeEnum.OpenPort;
            this._executionTriggerSource = new ExecutionTriggerSourceOpenPort();
            this._lastCycleFired = null;
            this._value = null;
        }

        #region Fields / Events
        
        private Task _listeningTask;
        private CancellationTokenSource _listeningTaskCancellationTokenSource;
        private IExecutionTriggerSource _executionTriggerSource = null;

        public event EventHandler<ExecutionTriggerFiredEventArgs> Fire;
        public event EventHandler<ExecutionTriggerFiredEventArgs> FireChanged;

        #endregion

        #region Properties 

        private TimeSpan _delay;
        public TimeSpan Delay
        {
            get => Get(_delay);
            set => Set(ref _delay, value, nameof(Delay));
        }

        private bool? _lastCycleFired;
        public bool? LastCycleFired
        {
            get => Get(_lastCycleFired);
            set => Set(ref _lastCycleFired, value, nameof(LastCycleFired));
        }

        private string _value;
        public string Value
        {
            get => Get(_value);
            set => Set(ref _value, value, nameof(Value));
        }

        private ExecutionTriggerSourceTypeEnum _sourceType;
        public ExecutionTriggerSourceTypeEnum SourceType
        {
            get => Get(_sourceType);
            set => Set(ref _sourceType, 
                value, 
                SetExecutionTriggerSource, 
                nameof(SourceType), 
                nameof(SourceTypeAsString), 
                nameof(SourceTypeDisplayName));
        }

        public string SourceTypeAsString
        {
            get => Get(_sourceType).ToString();
            set => SourceType = EnumParse<ExecutionTriggerSourceTypeEnum>(value, true, ExecutionTriggerSourceTypeEnum.None);
        }

        public string SourceTypeDisplayName
        {
            get => ExecutionTriggers.FirstOrDefault(x => x.Item1 == SourceType)?.Item2;
            set => SourceType = ExecutionTriggers.FirstOrDefault(x => x.Item2 == value.ToLower())?.Item1 ?? ExecutionTriggerSourceTypeEnum.None;
        }

        private bool _isListening;
        public bool IsListening
        {
            get => Get(_isListening);
            set => Set(ref _isListening, value, MaintainListeningTask, nameof(IsListening));
        }

        #endregion

        private void SetExecutionTriggerSource(ExecutionTriggerSourceTypeEnum sourceType)
        {
            switch (sourceType)
            {
                case ExecutionTriggerSourceTypeEnum.OpenPort:
                    _executionTriggerSource = new ExecutionTriggerSourceOpenPort();
                    break;
                case ExecutionTriggerSourceTypeEnum.None:
                    _executionTriggerSource = new ExecutionTriggerSourceNone();
                    break;
            }
        }

        private void MaintainListeningTask(bool isListening)
        {
            if (isListening && this._listeningTask == null)
            {
                this._listeningTaskCancellationTokenSource = new CancellationTokenSource();
                this._listeningTaskCancellationTokenSource.Token.ThrowIfCancellationRequested();
                this._listeningTask = this.CreateListeningTask();
            }
            else if (!isListening && this._listeningTask != null)
            {
                this._listeningTaskCancellationTokenSource.Cancel();
                this._listeningTask.Wait();
                this._listeningTask = null;
                this._listeningTaskCancellationTokenSource = null;
            }
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
                                Fire?.Invoke(this, new ExecutionTriggerFiredEventArgs(true));
                            }
                            FireChanged?.Invoke(this, new ExecutionTriggerFiredEventArgs(cycleShouldFire));
                        }

                        await Task.Delay(this.Delay, this._listeningTaskCancellationTokenSource.Token)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }
                catch (Exception)
                {
                    this.IsListening = false;
                }
            }, this._listeningTaskCancellationTokenSource.Token);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _listeningTask.Dispose();
            _listeningTaskCancellationTokenSource.Dispose();
        }
    }
}
