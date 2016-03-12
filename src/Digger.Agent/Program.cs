using Topshelf;

namespace Digger.Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            AgentRuntime.Bootstrap(typeof(Program).Assembly);

            HostFactory.New(x =>
            {
                x.UseSerilog();
                x.Service<AgentService>(sc =>
                {
                    sc.ConstructUsing(AgentRuntime.Resolve<AgentService>);
                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());
                    sc.WhenShutdown(s => s.Dispose());
                });

                //More options exists. Like instance name, which you could supply using
                //command line and then host more instances of the same service. NOTE! Don't
                //forget that you would have to configure a different host for the second server.
                x.SetServiceName("Digger.Agent");
                x.SetDescription("Digger Agent service for collecting measurement data.");

                //Here you can take advantage of specifying specific user-accounts, service accounts etc
                //used to run the service and thereby limit access to resources like file-system,
                //databases etc.
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.EnableShutdown();
            }).Run();
        }
    }
}
