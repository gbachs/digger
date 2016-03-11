using System;
using System.Threading;
using System.Threading.Tasks;
using Digger.Extensions;
using EnsureThat;

namespace Digger
{
    public abstract class AsyncPollingCollectorBase : ICollector, IDisposable
    {
        private readonly IDispatcher _dispatcher;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _worker;

        public string Name { get; }
        public TimeSpan CollectInterval { get; }
        protected ILogger Logger { get; }
        protected bool IsDisposed { get; private set; }

        public bool IsStarted => _cancellationTokenSource != null;

        protected AsyncPollingCollectorBase(string name, TimeSpan collectInterval, IDispatcher dispatcher, ILogger logger)
        {
            Ensure.That(name, nameof(name)).IsNotNullOrWhiteSpace();
            Ensure.That(dispatcher, nameof(dispatcher)).IsNotNull();
            Ensure.That(logger, nameof(logger)).IsNotNull();

            Name = name;
            CollectInterval = collectInterval;
            Logger = logger;
            _dispatcher = dispatcher;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed || !disposing)
                return;

            CleanResources();
        }

        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public void Start()
        {
            Logger.Info("Starting collection of name={name} intervall={collectIntervall}", Name, CollectInterval);

            ThrowIfDisposed();

            if (_cancellationTokenSource != null)
                throw new InvalidOperationException("The Collector can not be started as it's already started.");

            _cancellationTokenSource = new CancellationTokenSource();
            _worker = Task.Run(async () =>
            {
                await OnStartingAsync().ForAwait();

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        Logger.Debug("Collecting");
                        await OnCollectAsync(_dispatcher).ForAwait();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error while collecting name={name}", Name);
                    }
                    
                    await Task.Delay(CollectInterval).ForAwait();
                }
            }, _cancellationTokenSource.Token);
        }

        protected abstract Task OnStartingAsync();

        protected abstract Task OnCollectAsync(IDispatcher dispatcher);

        public void Stop()
        {
            ThrowIfDisposed();

            CleanResources();
        }

        private void CleanResources()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            if (_worker != null)
            {
                if (_worker.IsCanceled || _worker.IsCompleted)
                    _worker.Dispose();
                _worker = null;
            }
        }
    }
}