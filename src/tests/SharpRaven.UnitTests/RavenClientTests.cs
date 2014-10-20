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

using NSubstitute;

using NUnit.Framework;

using SharpRaven.Data;
using SharpRaven.Logging;
using SharpRaven.UnitTests.Utilities;

namespace SharpRaven.UnitTests
{
    [TestFixture]
    public class RavenClientTests
    {
        private class TestableRavenClient : RavenClient
        {
            public TestableRavenClient(string dsn, IJsonPacketFactory jsonPacketFactory = null)
                : base(dsn, jsonPacketFactory)
            {
            }


            protected override string Send(JsonPacket packet, Dsn dsn)
            {
                return packet.Project;
            }
        }

        private class TestableJsonPacketFactory : JsonPacketFactory
        {
            private readonly string project;


            public TestableJsonPacketFactory(string project)
            {
                this.project = project;
            }


            protected override JsonPacket OnCreate(JsonPacket jsonPacket)
            {
                jsonPacket.Project = this.project;
                return base.OnCreate(jsonPacket);
            }
        }


        [Test]
        public void CaptureMessage_InvokesSend_AndJsonPacketFactoryOnCreate()
        {
            const string dsnUri =
                "http://7d6466e66155431495bdb4036ba9a04b:4c1cfeab7ebd4c1cb9e18008173a3630@app.getsentry.com/3739";
            var project = Guid.NewGuid().ToString();
            var jsonPacketFactory = new TestableJsonPacketFactory(project);
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory);
            var result = client.CaptureMessage("Test");

            Assert.That(result, Is.EqualTo(project));
        }


        [Test]
        public void CaptureMessage_ScrubberIsInvoked()
        {
            string message = Guid.NewGuid().ToString("n");

            IRavenClient ravenClient = new RavenClient(TestHelper.DsnUri);
            ravenClient.LogScrubber = Substitute.For<IScrubber>();
            ravenClient.LogScrubber.Scrub(Arg.Any<string>())
                       .Returns(c =>
                       {
                           string json = c.Arg<string>();
                           Assert.That(json, Is.StringContaining(message));
                           return json;
                       });

            ravenClient.CaptureMessage(message);

            // Verify that we actually received a Scrub() call:
            ravenClient.LogScrubber.Received().Scrub(Arg.Any<string>());
        }


        [Test]
        public void Constructor_NullDsnString_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new RavenClient((string)null));
            Assert.That(exception.ParamName, Is.EqualTo("dsn"));
        }


        [Test]
        public void Constructor_NullDsn_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new RavenClient((Dsn)null));
            Assert.That(exception.ParamName, Is.EqualTo("dsn"));
        }


        [Test]
        public void Constructor_StringDsn_CurrentDsnEqualsDsn()
        {
            IRavenClient ravenClient = new RavenClient(TestHelper.DsnUri);
            Assert.That(ravenClient.CurrentDsn.ToString(), Is.EqualTo(TestHelper.DsnUri));
        }


        [Test]
        public void Logger_IsRoot()
        {
            IRavenClient ravenClient = new RavenClient(TestHelper.DsnUri);
            Assert.That(ravenClient.Logger, Is.EqualTo("root"));
        }
    }
}