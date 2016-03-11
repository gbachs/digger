using System.Threading.Tasks;
using Digger.Extensions;
using EnsureThat;

namespace Digger.Agent.Dispatchers
{
    public class ImmediateInProcessDispatcher : IDispatcher
    {
        private readonly IProcessor _processor;

        public ImmediateInProcessDispatcher(IProcessor processor)
        {
            Ensure.That(processor, nameof(processor)).IsNotNull();

            _processor = processor;
        }

        public async Task DispatchAsync(IMeasurement measurement)
        {
            await _processor.ProcessAsync(measurement).ForAwait();
        }
    }
}