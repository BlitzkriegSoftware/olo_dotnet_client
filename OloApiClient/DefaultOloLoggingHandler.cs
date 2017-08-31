using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

namespace OloApiClient
{
    /// <summary>
    /// Default Logging Handler for Requests
    /// <para>You can use this or write your own</para>
    /// </summary>
    public class DefaultOloLoggingHandler : DelegatingHandler
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="innerHandler">HttpMessageHandler</param>
        public DefaultOloLoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        /// <summary>
        /// The Handler that kicks in, for every request, and send it caputuring the input and output
        /// </summary>
        /// <param name="request">HttpRequestMessage</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>HttpResponseMessage</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();

            sb.Append("-".PadRight(20, '-'));

            sb.Append("\nRequest:");
            sb.Append(request.ToString());
            if (request.Content != null)
                sb.Append(await request.Content.ReadAsStringAsync());
            sb.Append("\n");

            var response = await base.SendAsync(request, cancellationToken);

            sb.Append("\nResponse:");
            sb.Append(response.ToString());
            if (response.Content != null)
                sb.Append(await response.Content.ReadAsStringAsync());
            sb.Append("\n");

            Trace.TraceInformation(sb.ToString());

            return response;
        }
    }



}
