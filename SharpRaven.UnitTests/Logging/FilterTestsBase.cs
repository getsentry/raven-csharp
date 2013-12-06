using System;

using NUnit.Framework;

using SharpRaven.Logging;

namespace SharpRaven.UnitTests.Logging
{
    public abstract class FilterTestsBase<TFilter>
        where TFilter : IFilter, new()
    {
        private readonly TFilter filter;


        protected FilterTestsBase()
        {
            this.filter = new TFilter();
        }


        protected void InvalidValueIsNotScrubbed(string invalidValue)
        {
            var input = String.Format(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. {0} Praesent est dui, ornare eget condimentum a, tincidunt sit amet lectus. Nulla pellentesque, tortor eget tempus malesuada.",
                invalidValue);

            var output = this.filter.Filter(input);

            Assert.That(output, Is.StringContaining(invalidValue));
        }


        protected void ValidValueIsScrubbed(string validValue)
        {
            var input = String.Format(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. {0} Praesent est dui, ornare eget condimentum a, tincidunt sit amet lectus. Nulla pellentesque, tortor eget tempus malesuada.",
                validValue);

            var output = this.filter.Filter(input);

            Assert.That(output, Is.Not.StringContaining(validValue));
        }
    }
}