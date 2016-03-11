using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject;

namespace Digger.Agent
{
    internal static class AgentRuntime
    {
        private static readonly IKernel Kernel = new StandardKernel();

        internal static void Bootstrap(params Assembly[] assembliesWithIoCConfig)
        {
            Kernel.Load(assembliesWithIoCConfig);
        }

        internal static object Resolve(Type type)
        {
            return Kernel.Get(type);
        }

        internal static T Resolve<T>()
        {
            return Kernel.Get<T>();
        }

        internal static IEnumerable<T> ResolveAll<T>()
        {
            return Kernel.GetAll<T>();
        }
    }
}