using Nancy;
using SharpRaven.Data;
using System.Threading;

namespace SharpRaven.Nancy
{
    /// <summary>
    /// The Raven Client, responsible for capturing exceptions and sending them to Sentry.
    /// </summary>
    public class RavenClient : SharpRaven.RavenClient
    {
        private NancyContext httpContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RavenClient" /> class. Sentry
        /// Data Source Name will be read from sharpRaven section in your app.config or
        /// web.config.
        /// </summary>
        /// <param name="httpContext">The optional <see cref="NancyContext" /> that will be used to fill <see cref="SentryRequest" /> that will be sent to Sentry.</param>
        /// <param name="jsonPacketFactory">The optional factory that will be used to create the <see cref="JsonPacket" /> that will be sent to Sentry.</param>
        public RavenClient(NancyContext httpContext = null, IJsonPacketFactory jsonPacketFactory = null)
            : base(new Dsn(Configuration.Settings.Dsn.Value), jsonPacketFactory)
        {
            this.httpContext = httpContext;
        }

        /// <summary>
        /// Sends the specified packet to Sentry.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID"/> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        protected override string Send(JsonPacket packet, Dsn dsn)
        {
            // looking for NancyContext
            this.httpContext = this.httpContext ?? Thread.GetData(
                Thread.GetNamedDataSlot(Configuration.Settings.NancyContextDataSlot)) as NancyContext;

            // get SentryRequest
            ISentryRequest sentryRequest = SharpRaven.Nancy.Data.SentryRequest.GetRequest(this.httpContext);

            // patch JsonPacket.Request with data on NancyContext
            packet.Request = sentryRequest;

            // patch JsonPacket.User with data on NancyContext
            packet.User = sentryRequest.GetUser();

            return base.Send(packet, dsn);
        }
    }
}
