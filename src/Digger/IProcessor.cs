using System.Threading.Tasks;

namespace Digger
{
    public interface IProcessor
    {
        Task ProcessAsync(IMeasurement measurement);
    }
}