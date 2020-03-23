using System;
using System.Threading;
using System.Threading.Tasks;
using ScriptTrigger.BusinessLogic.ExecutionTriggerSource;

namespace ScriptTrigger.BusinessLogic
{
    public class ExecutionTrigger
    {
        private bool _isListeningBackingField;
        private Task _listeningTask;
        private CancellationTokenSource _listeningTaskCancellationTokenSource;
        public TimeSpan Delay { get; set; }
        public bool? LastCycleFired { get; set; }
        public string Parameter { get; set; }
        public ExecutionTriggerSourceTypeEnum SourceType { get; set; }

        public event EventHandler<EventArgs> Fire;
        public event EventHandler<EventArgs> LastCycleFireChanged;

        public ExecutionTrigger()
        {
            this._isListeningBackingField = false;
            this._listeningTask = null;
            this._listeningTaskCancellationTokenSource = null;
            this.Delay = TimeSpan.FromSeconds(2);
            this.LastCycleFired = null;
            this.Parameter = null;
            this.SourceType = ExecutionTriggerSourceTypeEnum.OpenPort;
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
                            cycleShouldFire = CheckTrigger();
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
                                Fire?.Invoke(this, EventArgs.Empty);
                            }

                            this.LastCycleFireChanged?.Invoke(this, EventArgs.Empty);
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

        private bool CheckTrigger()
        {
            switch (SourceType)
            {
                case ExecutionTriggerSourceTypeEnum.OpenPort:
                    return ExecutionTriggerSourceOpenPort.CheckTrigger(this.Parameter);

                default:
                    return false;
            }
        }
    }
}
