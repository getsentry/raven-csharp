using System;

namespace SharpRaven.Logging {
    /// <summary>
    /// Interface for plugging into RavenClient to provide methods to filter logs.
    /// </summary>
    public interface IScrubber {
        /// <summary>
        /// The main interface for scrubbing a JSON packet,
        /// called before compression (if enabled)
        /// </summary>
        /// <param name="input">The serialized JSON packet is given here.</param>
        /// <returns>Scrubbed JSON packet.</returns>
        public string Scrub(string input);
    }
}
