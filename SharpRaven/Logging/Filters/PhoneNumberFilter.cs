using System;
using System.Text.RegularExpressions;

namespace SharpRaven.Logging.Filters {
    public class PhoneNumberFilter : IFilter {
        public string Filter(string input) {
            Regex phoneRegex = new Regex(@"1?\W*([2-9][0-8][0-9])\W*([2-9][0-9]{2})\W*([0-9]{4})(\se?x?t?(\d*))?");

            phoneRegex.Replace(input, new MatchEvaluator(delegate(Match m) {
                return "##-PHONE-TRUNC-##";
            }));

            return input;
        }
    }
}
