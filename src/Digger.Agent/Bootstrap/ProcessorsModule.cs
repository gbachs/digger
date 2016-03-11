using System;
using Digger.Agent.Config;
using Digger.Agent.Processors;
using Digger.Processors.InfluxDb;
using MyInfluxDbClient;
using Ninject;
using Ninject.Modules;

namespace Digger.Agent.Bootstrap
{
public class ProcessorsModule : NinjectModule
{
    public override void Load()
    {
            if (Environment.UserInteractive)
                RegisterProcessor<ConsoleProcessor>();
            else
                RegisterInfluxDbProcessor();
    }

        private void RegisterInfluxDbProcessor()
        {
            Kernel
                .Bind<InfluxDbMeasurementMappings>()
                .ToMethod(ctx =>
                {
                    var config = ctx.Kernel.Get<AgentConfig>().Processors.InfluxDb;
                    var databaseName = config.Database;
                    var mappings = new InfluxDbMeasurementMappings(new InfluxDbTargetInfo(databaseName));
                    //TODO: Extend config to define specific targets per measurement
                    //mappings.Register<PerformanceCounterMeasurement>(new InfluxDbTargetInfo(databaseName));
                    //mappings.Register<WinServiceStatusMeasurement>(new InfluxDbTargetInfo(databaseName));

                    return mappings;
                })
                .InSingletonScope();

            Kernel
                .Bind<IInfluxDbClient>()
                .ToMethod(ctx =>
                {
                    var config = ctx.Kernel.Get<AgentConfig>().Processors.InfluxDb;
                    return new InfluxDbClient(config.Host);
                })
                .InSingletonScope();

            RegisterProcessor<QueuedInfluxDbProcessor>();
        }

        private void RegisterProcessor<T>() where T : IProcessor
        {
            Kernel
                .Bind<IProcessor>()
                .To<T>()
                .InSingletonScope();
        }
    }
}