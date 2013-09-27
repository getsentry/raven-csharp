using System;
using SharpRaven.Data;
using System.Text.RegularExpressions;

namespace SharpRaven.Logging.Filters {
    public class CreditCardFilter : IFilter {

        public string Filter(string input) {
            Regex cardRegex = new Regex(@"\b(?:\d[ -]*?){13,16}\b", RegexOptions.IgnoreCase);
            
            cardRegex.Replace(input, new MatchEvaluator(delegate(Match m) {
                if (IsValidCreditCardNumber(m.Value))
                    return "####-CC-TRUNCATED-####";
                return m.Value;
            }));

            return input;
        }


        /// Extremely fast Luhn algorithm implementation, based on 
        /// pseudo code from Cliff L. Biffle (http://microcoder.livejournal.com/17175.html)
        /// 
        /// Copyleft Thomas @ Orb of Knowledge:
        /// http://orb-of-knowledge.blogspot.com/2009/08/extremely-fast-luhn-function-for-c.html 
        /// 
        private bool IsValidCreditCardNumber(string number) {
            number.Replace("-", String.Empty);
            number.Replace(" ", String.Empty);

            int[] deltas = new int[] { 0, 1, 2, 3, 4, -4, -3, -2, -1, 0 };
            int checksum = 0;
            char[] chars = number.ToCharArray();
            for (int i = chars.Length - 1; i > -1; i--) {
                int j = ((int)chars[i]) - 48;
                checksum += j;
                if (((i - chars.Length) % 2) == 0)
                    checksum += deltas[j];
            }

            return ((checksum % 10) == 0);
        }
    }
}
