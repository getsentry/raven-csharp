#region License

// Copyright (c) 2014 The Sentry Team and individual contributors.
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
// 
//     1. Redistributions of source code must retain the above copyright notice, this list of
//        conditions and the following disclaimer.
// 
//     2. Redistributions in binary form must reproduce the above copyright notice, this list of
//        conditions and the following disclaimer in the documentation and/or other materials
//        provided with the distribution.
// 
//     3. Neither the name of the Sentry nor the names of its contributors may be used to
//        endorse or promote products derived from this software without specific prior written
//        permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;

using NUnit.Framework;

namespace SharpRaven.UnitTests
{
    [TestFixture]
    public class DsnTests
    {
        [Test]
        public void Constructor_EmptyDsn_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Dsn("      "));
            Assert.That(exception.ParamName, Is.EqualTo("dsn"));
        }


        [Test]
        public void Constructor_InvalidDsn_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(() => new Dsn("oijweofijw$%"));
            Assert.That(exception.ParamName, Is.EqualTo("dsn"));
        }


        [Test]
        public void Constructor_NullDsn_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Dsn(null));
            Assert.That(exception.ParamName, Is.EqualTo("dsn"));
        }


        [Test]
        public void Constructor_ValidHttpUri_SentryUriHasHttpScheme()
        {
            const string dsnUri =
                "http://7d6466e66155431495bdb4036ba9a04b:4c1cfeab7ebd4c1cb9e18008173a3630@app.getsentry.com/3739";
            var dsn = new Dsn(dsnUri);

            Assert.That(dsn.SentryUri, Is.Not.Null);
            Assert.That(dsn.SentryUri.Scheme, Is.EqualTo("http"));
        }


        [Test]
        public void Constructor_ValidHttpsUri_SentryUriHasHttpsScheme()
        {
            var dsn = new Dsn(TestHelper.DsnUri);

            Assert.That(dsn.SentryUri, Is.Not.Null);
            Assert.That(dsn.SentryUri.Scheme, Is.EqualTo("https"));
        }


        [Test]
        public void Constructor_ValidHttpsUri_UriIsEqualToDsn()
        {
            var dsn = new Dsn(TestHelper.DsnUri);

            Assert.That(dsn.Uri, Is.Not.Null);
            Assert.That(dsn.Uri.ToString(), Is.EqualTo(TestHelper.DsnUri));
        }


        [Test]
        public void Constructor_ValidUriButInvalidDsn_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(() => new Dsn("http://example.com/"));
            Assert.That(exception.ParamName, Is.EqualTo("dsn"));
        }


        [Test]
        public void ToString_ReturnsStringEqualToDsn()
        {
            var dsn = new Dsn(TestHelper.DsnUri);
            Assert.That(dsn.ToString(), Is.EqualTo(TestHelper.DsnUri));
        }
    }
}