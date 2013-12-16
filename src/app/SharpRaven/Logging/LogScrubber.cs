using System;
using System.Collections.Generic;
using SharpRaven.Logging.Filters;

namespace SharpRaven.Logging {
    /// <summary>
    /// Scrubs a JSON packet for sensitive information with <see cref="CreditCardFilter"/>, <see cref="PhoneNumberFilter"/> and <see cref="SocialSecurityFilter"/>.
    /// </summary>
    public class LogScrubber : IScrubber {
        /// <summary>
        /// Gets the list of filters 
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public List<IFilter> Filters { get; private set; }
    
        // Default scrubber implementation.
        /// <summary>
        /// Initializes a new instance of the <see cref="LogScrubber"/> class.
        /// </summary>
        public LogScrubber() {
            Filters = new List<IFilter>();

            // Add default scrubbers.
            Filters.AddRange(new IFilter[] {
                new CreditCardFilter(),
                new PhoneNumberFilter(),
                new SocialSecurityFilter()
            });
        }

        /// <summary>
        /// The main interface for scrubbing a JSON packet,
        /// called before compression (if enabled)
        /// </summary>
        /// <param name="input">The serialized JSON packet is given here.</param>
        /// <returns>
        /// Scrubbed JSON packet.
        /// </returns>
        public string Scrub(string input) {
            foreach (IFilter f in Filters) {
                input = f.Filter(input);
            }

            return input;
        }
    }
}
