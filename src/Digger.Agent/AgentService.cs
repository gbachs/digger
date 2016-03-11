using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace Digger.Agent
{
    public class AgentService : IDisposable
    {
        private readonly ICollector[] _collectors;

        protected bool IsDisposed { get; private set; }

        public AgentService(ICollectorFactory collectorFactory)
        {
            Ensure.That(collectorFactory, nameof(collectorFactory)).IsNotNull();

            _collectors = collectorFactory.Create().ToArray();
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

            var exceptions = new List<Exception>();

            foreach (var collector in _collectors.OfType<IDisposable>())
            {
                try
                {
                    collector.Dispose();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
                throw new AggregateException($"Exceptions occurred while disposing internal resources in {nameof(AgentService)}.", exceptions);
        }

        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public void Start()
        {
            foreach (var collector in _collectors)
                collector.Start();
        }

        public void Stop()
        {
            foreach (var collector in _collectors.Where(c => c.IsStarted))
                collector.Stop();
        }
    }
}