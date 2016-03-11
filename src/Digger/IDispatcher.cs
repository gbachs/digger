using System.Threading.Tasks;

namespace Digger
{
    public interface IDispatcher
    {
        Task DispatchAsync(IMeasurement measurement);
    }
}