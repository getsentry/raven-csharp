#if !NET40 && !NET35
using System.Threading.Tasks;
#endif

namespace SharpRaven.Data
{
    /// <summary>
    /// Sends a request to Sentry
    /// </summary>
    public interface IRequester
    {
        void UseEventId(string eventId);

        /// <summary>
        /// Sends this request instance to Sentry
        /// </summary>
        string Request();
        /// <summary>
        /// Sends this feedback instance to Sentry
        /// </summary>
        /// <returns></returns>
        string SendFeedback();

#if !NET40 && !NET35
        /// <summary>
        /// Sends this request instance to Sentry
        /// </summary>
        /// <returns></returns>
        Task<string> RequestAsync();
        /// <summary>
        /// Sends this feedback instance to Sentry
        /// </summary>
        /// <returns></returns>
        Task<string> SendFeedbackAsync();
#endif
    }
}
