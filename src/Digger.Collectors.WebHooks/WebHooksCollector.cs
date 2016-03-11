using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using EnsureThat;
using Microsoft.Owin.Hosting;
using Owin;

namespace Digger.Collectors.WebHooks
{
    public class WebHooksCollector : ICollector, IDisposable
    {
        private readonly string _baseAddress;
        private readonly Func<Type, object> _resolver;
        private IDisposable _host;

        protected bool IsDisposed { get; private set; }

        public WebHooksCollector(string baseAddress, Func<Type, object> resolver)
        {
            Ensure.That(baseAddress, nameof(baseAddress)).IsNotNullOrWhiteSpace();
            Ensure.That(resolver, nameof(resolver)).IsNotNull();

            _baseAddress = baseAddress;
            _resolver = resolver;
        }

        public bool IsStarted => _host != null;

        public void Start()
        {
            ThrowIfDisposed();

            if (_host != null)
                throw new InvalidOperationException("The host for WebHooks can not be started as it is already started.");

            _host = WebApp.Start(_baseAddress, app =>
            {
                var config = new HttpConfiguration();

                config.Services.Replace(typeof(IHttpControllerActivator), new ServiceActivator(_resolver));

                config.MapHttpAttributeRoutes();

                app.UseWebApi(config);
            });
        }

        public void Stop()
        {
            ThrowIfDisposed();

            _host?.Dispose();
            _host = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed || !disposing)
                return;

            _host?.Dispose();
            _host = null;
        }

        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}