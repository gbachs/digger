using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Digger.Extensions;
using Digger.Processors.InfluxDb.Extensions;
using EnsureThat;
using MyInfluxDbClient;

namespace Digger.Processors.InfluxDb
{
    public class QueuedInfluxDbProcessor : IProcessor, IDisposable
    {
        private const int MaxBatchSize = 25;
        private static readonly TimeSpan MaxDurationForCheckout = TimeSpan.FromSeconds(5);
        private readonly ConcurrentQueue<IMeasurement> _queue = new ConcurrentQueue<IMeasurement>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _worker;

        protected bool IsDisposed { get; private set; }

        public QueuedInfluxDbProcessor(IInfluxDbClient client, InfluxDbMeasurementMappings dbMappings)
        {
            Ensure.That(client, nameof(client)).IsNotNull();
            Ensure.That(dbMappings, nameof(dbMappings)).IsNotNull();

            _worker = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    SpinWait.SpinUntil(() => !_queue.IsEmpty);

                    foreach (var measurement in CheckoutMeasurements())
                    {
                        var databaseInfo = dbMappings.GetDbTargetInfoOrDefault(measurement);
                        await client.WriteAsync(
                            databaseInfo.DatabaseName,
                            measurement.Points.ToInfluxPoints(),
                            databaseInfo.WriteOptions).ForAwait();
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        private IEnumerable<IMeasurement> CheckoutMeasurements()
        {
            var pointsByMeasurementType = new Dictionary<string, IMeasurement>();
            var started = DateTime.Now;

            while (!_queue.IsEmpty)
            {
                var duration = DateTime.Now.Subtract(started);
                if (duration >= MaxDurationForCheckout)
                    break;

                IMeasurement measurement;
                if (!_queue.TryDequeue(out measurement))
                    continue;

                IMeasurement existingMeasurement;
                if (!pointsByMeasurementType.TryGetValue(measurement.Type, out existingMeasurement))
                    pointsByMeasurementType[measurement.Type] = measurement;
                else
                    existingMeasurement.Points.Add(measurement.Points);

                if (pointsByMeasurementType.Count >= MaxBatchSize)
                    break;
            }

            return pointsByMeasurementType.Values;
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

        public Task ProcessAsync(IMeasurement measurement)
        {
            _queue.Enqueue(measurement);

            return Task.FromResult(0);
        }
    }
}