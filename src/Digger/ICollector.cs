namespace Digger
{
    public interface ICollector
    {
        bool IsStarted { get; }

        void Start();
        void Stop();
    }
}