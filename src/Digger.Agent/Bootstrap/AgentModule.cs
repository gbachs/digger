using System;
using System.IO;
using Digger.Agent.Config;
using Digger.Agent.Serialization;
using Newtonsoft.Json.Linq;
using Ninject;
using Ninject.Modules;

namespace Digger.Agent.Bootstrap
{
    public class AgentModule : NinjectModule
    {
        public override void Load()
        {
            var agentSettings = DiggerAgentSettings.ReadFromAppSettings();

            Kernel
                .Bind<AgentConfig>()
                .ToMethod(ctx =>
                {
                    var rawJson = File.ReadAllText(agentSettings.ConfigFilePath);
                    var serializer = ctx.Kernel.Get<IJsonSerializer>();
                    var json = serializer.Deserialize<JToken>(rawJson);

                    return new AgentConfig(json);
                })
                .InSingletonScope();

            Kernel
                .Bind<IJsonSerializer>()
                .To<DiggerJsonSerializer>();

            Kernel
                .Bind<ICollectorFactory>()
                .ToMethod(ctx =>
                {
                    var config = ctx.Kernel.Get<AgentConfig>();
                    var dispatcher = ctx.Kernel.Get<IDispatcher>();
                    var loggerResolver = ctx.Kernel.Get<LoggerResolver>();

                    return new JsonCollectorFactory(config.Collectors, dispatcher, loggerResolver);
                });

            Kernel
                .Bind<IDispatcher>()
                .ToMethod(ctx =>
                {
                    var config = ctx.Kernel.Get<AgentConfig>();
                    var dispatcherType = Type.GetType(config.DispatcherType, true, true);

                    return (IDispatcher)ctx.Kernel.Get(dispatcherType);
                });

            Kernel
                .Bind<AgentService>()
                .ToSelf()
                .InSingletonScope();
        }
    }
}