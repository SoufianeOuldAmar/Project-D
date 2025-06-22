using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlightGatewayTest
{
     public class FakeHandler : HttpMessageHandler
     {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage>? responder = null)
        {
            _responder = responder
              ?? new Func<HttpRequestMessage, HttpResponseMessage>(req =>
              {
                  var msg = new HttpResponseMessage(HttpStatusCode.OK);
                  msg.Content = new StringContent("[]", Encoding.UTF8, "application/json");
                  return msg;
              });
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
                var response = _responder(request);
                return Task.FromResult(response);
        }
     }
}
