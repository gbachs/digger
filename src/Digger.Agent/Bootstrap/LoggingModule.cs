using Digger.Agent.Logging;
using Ninject.Modules;
using Serilog;

namespace Digger.Agent.Bootstrap
{
    public class LoggingModule : NinjectModule
    {
        public override void Load()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .ReadFrom.AppSettings()
                .CreateLogger();

            LoggerFactory.Resolve = t => new SerilogLogger(Log.Logger.ForContext(t));

            Kernel
                .Bind<LoggerResolver>()
                .ToConstant(LoggerFactory.Resolve);

            Kernel
                .Bind<ILogger>()
                .ToMethod(ctx => LoggerFactory.Resolve(ctx.Request.ParentContext.Request.Service));
        }
    }
}