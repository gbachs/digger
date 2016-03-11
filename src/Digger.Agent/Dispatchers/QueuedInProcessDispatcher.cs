using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Digger.Extensions;
using EnsureThat;

namespace Digger.Agent.Dispatchers
{
    public class QueuedInProcessDispatcher : IDispatcher, IDisposable
    {
        private readonly IProcessor _processor;
        private readonly ConcurrentQueue<Func<Task>> _queue = new ConcurrentQueue<Func<Task>>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _worker;

        protected bool IsDisposed { get; private set; }

        public QueuedInProcessDispatcher(IProcessor processor)
        {
            Ensure.That(processor, nameof(processor)).IsNotNull();

            _processor = processor;

            _worker = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    Func<Task> work;
                    if (_queue.TryDequeue(out work))
                        await work().ForAwait();

                    SpinWait.SpinUntil(() => !_queue.IsEmpty);
                }
            }, _cancellationTokenSource.Token);
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

        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public Task DispatchAsync(IMeasurement measurement)
        {
            _queue.Enqueue(() => _processor.ProcessAsync(measurement));

            return Task.FromResult(0);
        }
    }
}