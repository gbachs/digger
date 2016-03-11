using System.Collections.Generic;

namespace Digger
{
    public interface ICollectorFactory
    {
        IEnumerable<ICollector> Create();
    }
}