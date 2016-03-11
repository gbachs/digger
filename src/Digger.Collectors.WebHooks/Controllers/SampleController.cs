using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Digger.Collectors.WebHooks.Requests;
using EnsureThat;

namespace Digger.Collectors.WebHooks.Controllers
{
    [RoutePrefix("sample")]
    public class SampleController : ApiController
    {
        private readonly IDispatcher _dispatcher;
        private readonly ILogger _logger;

        public SampleController(IDispatcher dispatcher, ILogger logger)
        {
            Ensure.That(dispatcher, nameof(dispatcher)).IsNotNull();

            _dispatcher = dispatcher;
            _logger = logger;
        }

        [Route]
        [HttpPost]
        public HttpResponseMessage Post(SampleMeasurementRequest request)
        {
            _logger.Debug("Recieved SampleMeasurementRequest");

            var point = new MeasurementPoint(request.Name)
                .AddTag("host", request.MachineName ?? Environment.MachineName)
                .AddField("value", request.Value)
                .AddTimeStamp(SysTime.Now());

            var measurement = new WebHookMeasurement(point);

            _dispatcher.DispatchAsync(measurement);

            return Request.CreateResponse(HttpStatusCode.Created);
        }
    }
}