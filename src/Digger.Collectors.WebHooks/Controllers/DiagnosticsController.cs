using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Digger.Collectors.WebHooks.Controllers
{
    [RoutePrefix("diagnostics")]
    public class DiagnosticsController : ApiController
    {
        [Route]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                ServerTimeUtc = DateTime.UtcNow
            });
        }
    }
}