using System.Text.RegularExpressions;

namespace SharpRaven.Logging.Filters
{
    /// <summary>
    /// An <see cref="IFilter"/> implementation for masking phone numbers in a logged message
    /// </summary>
    public class PhoneNumberFilter : IFilter
    {
        /// <summary>
        /// An <see cref="IFilter"/> implementation for masking phone numbers in a logged 
        /// </summary>
        public string Filter(string input)
        {
            Regex phoneRegex = new Regex(@"1?\W*([2-9][0-8][0-9])\W*([2-9][0-9]{2})\W*([0-9]{4})(\se?x?t?(\d*))?");
            return phoneRegex.Replace(input, delegate { return "##-PHONE-TRUNC-##"; });
        }
    }
}