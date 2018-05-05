using System;

using NUnit.Framework;
using SharpRaven.Utilities;

namespace SharpRaven.UnitTests.Utilities
{
    public class RuntimeInfoHelperTests
    {
        [Test]
        public void GetRuntime_NotNull()
        {
            var runtime = RuntimeInfoHelper.GetRuntime();
            Assert.That(runtime, Is.Not.Null);
        }
    }
}
