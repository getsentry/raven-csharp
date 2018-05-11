#if !NET40 && !NET35
using SharpRaven.Utilities;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SharpRaven.Data
{
    public class BackgroundRequester : IRequester
    {
        private readonly IRequester requester;
        private readonly BlockingCollection<IRequester> queue;


        internal BackgroundRequester(IRequester requester, BlockingCollection<IRequester> queue)
        {
            this.requester = requester ?? throw new ArgumentNullException(nameof(requester));
            this.queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }


        // If max amount of items was reached, the event won't be sent.
        private string EnqueueRequester()
        {
            var eventId = Guid.NewGuid().ToString("n");
            this.requester.UseEventId(eventId);

            var queued = this.queue.TryAdd(this.requester);

            if (queued)
            {
                return eventId;
            }
            else
            {
                SystemUtil.WriteError("An event was dropped due to queue overflow.");
                return null;
            }
        }


        public void UseEventId(string eventId) { /* No op */}

        public string Request() => EnqueueRequester();

        public string SendFeedback() => EnqueueRequester();

        public Task<string> RequestAsync() => Task.FromResult(EnqueueRequester());

        public Task<string> SendFeedbackAsync() => Task.FromResult(EnqueueRequester());
    }
}
#endif
