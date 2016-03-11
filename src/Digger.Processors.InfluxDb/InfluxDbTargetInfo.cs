using EnsureThat;
using MyInfluxDbClient;

namespace Digger.Processors.InfluxDb
{
    public class InfluxDbTargetInfo
    {
        public string DatabaseName { get; }
        public WriteOptions WriteOptions { get; }

        public InfluxDbTargetInfo(string databaseName, WriteOptions writeOptions = null)
        {
            Ensure.That(databaseName, nameof(databaseName)).IsNotNull();

            DatabaseName = databaseName;
            WriteOptions = writeOptions;
        }
    }
}