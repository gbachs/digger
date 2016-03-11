using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using EnsureThat;

namespace Digger.Collectors.WebHooks
{
    internal class ServiceActivator : IHttpControllerActivator
    {
        private readonly Func<Type, IHttpController> _controllerResolver;

        internal ServiceActivator(Func<Type, object> resolver)
        {
            Ensure.That(resolver, nameof(resolver)).IsNotNull();

            _controllerResolver = controllerType => (IHttpController)resolver(controllerType);
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            return _controllerResolver(controllerType);
        }
    }
}