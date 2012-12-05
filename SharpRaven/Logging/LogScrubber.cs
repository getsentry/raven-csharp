using System;
using System.Collections.Generic;
using SharpRaven.Logging.Filters;

namespace SharpRaven.Logging {
    public class LogScrubber : IScrubber {
        public List<IFilter> Filters { get; private set; }
    
        // Default scrubber implementation.
        public LogScrubber() {
            Filters = new List<IFilter>();

            // Add default scrubbers.
            Filters.AddRange(new IFilter[] {
                new CreditCardFilter(),
                new PhoneNumberFilter(),
                new SocialSecurityFilter()
            });
        }

        public string Scrub(string input) {
            foreach (IFilter f in Filters) {
                input = f.Filter(input);
            }

            return input;
        }
    }
}
