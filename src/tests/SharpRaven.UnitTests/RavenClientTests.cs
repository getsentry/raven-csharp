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
using System.Collections.Generic;

using NSubstitute;

using NUnit.Framework;

using SharpRaven.Data;
using SharpRaven.Logging;
using SharpRaven.UnitTests.Utilities;
#if (!net40)
using System.Threading.Tasks;

#endif

namespace SharpRaven.UnitTests
{
    [TestFixture]
    public class RavenClientTests
    {
        [Test]
        public void CaptureMessage_ClientEnvironmentIsIgnored()
        {
            var jsonPacketFactory = new TestableJsonPacketFactory(environment : "keep-me");
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory) { Environment = "foobar" };
            client.CaptureMessage("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Environment, Is.EqualTo("keep-me"));
        }


        [Test]
        public void CaptureMessage_ClientLoggerIsIgnored()
        {
            var jsonPacketFactory = new TestableJsonPacketFactory(logger : "keep-me");
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory) { Logger = "foobar" };
            client.CaptureMessage("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Logger, Is.EqualTo("keep-me"));
        }


        [Test]
        public void CaptureMessage_ClientReleaseIsIgnored()
        {
            var jsonPacketFactory = new TestableJsonPacketFactory(release : "keep-me");
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory) { Release = "foobar" };
            client.CaptureMessage("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Release, Is.EqualTo("keep-me"));
        }


        [Test]
        public void CaptureMessage_InvokesSend_AndJsonPacketFactoryOnCreate()
        {
            var project = Guid.NewGuid().ToString();
            var jsonPacketFactory = new TestableJsonPacketFactory(project);
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory);
            var result = client.CaptureMessage("Test");

            Assert.That(result, Is.EqualTo(project));
        }


        [Test]
        public void CaptureMessage_OnlyDefaultTags()
        {
            var client = new TestableRavenClient(dsnUri);
            client.Tags.Add("key", "value");
            client.Tags.Add("foo", "bar");
            // client.Tags = defaultTags;
            client.CaptureMessage("Test", ErrorLevel.Info);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Tags["key"], Is.EqualTo("value"));
            Assert.That(lastEvent.Tags["foo"], Is.EqualTo("bar"));
        }


        [Test]
        public void CaptureMessage_OnlyMessageTags()
        {
            var messageTags = new Dictionary<string, string> { { "key", "value" }, { "foo", "bar" } };

            var client = new TestableRavenClient(dsnUri);
            client.CaptureMessage("Test", ErrorLevel.Info, messageTags);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Tags["key"], Is.EqualTo("value"));
            Assert.That(lastEvent.Tags["foo"], Is.EqualTo("bar"));
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
        public void CaptureMessage_SendsEnvironment()
        {
            var client = new TestableRavenClient(dsnUri) { Environment = "foobar" };
            client.CaptureMessage("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Environment, Is.EqualTo("foobar"));
        }


        [Test]
        public void CaptureMessage_SendsLogger()
        {
            var client = new TestableRavenClient(dsnUri) { Logger = "foobar" };
            client.CaptureMessage("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Logger, Is.EqualTo("foobar"));
        }


        [Test]
        public void CaptureMessage_SendsRelease()
        {
            var client = new TestableRavenClient(dsnUri) { Release = "foobar" };
            client.CaptureMessage("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Release, Is.EqualTo("foobar"));
        }


        [Test]
        public void CaptureMessage_TagHandling()
        {
            var messageTags = new Dictionary<string, string> { { "key", "another value" } };

            var client = new TestableRavenClient(dsnUri);
            client.Tags.Add("key", "value");
            client.Tags.Add("foo", "bar");
            client.CaptureMessage("Test", ErrorLevel.Info, messageTags);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Tags["key"], Is.EqualTo("another value"));
            Assert.That(lastEvent.Tags["foo"], Is.EqualTo("bar"));
        }


#if (!net40)
        [Test]
        public async Task CaptureMessageAsync_InvokesSend_AndJsonPacketFactoryOnCreate()
        {
            const string dsnUri =
                "http://7d6466e66155431495bdb4036ba9a04b:4c1cfeab7ebd4c1cb9e18008173a3630@app.getsentry.com/3739";
            var project = Guid.NewGuid().ToString();
            var jsonPacketFactory = new TestableJsonPacketFactory(project);
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory);
            var result = await client.CaptureMessageAsync("Test");

            Assert.That(result, Is.EqualTo(project));
        }
#endif


        [Test]
        public void Constructor_NullDsn_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new RavenClient((Dsn)null));
            Assert.That(exception.ParamName, Is.EqualTo("dsn"));
        }


        [Test]
        public void Constructor_NullDsnString_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new RavenClient((string)null));
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


        [Test]
        public void Release_IsNullByDefault()
        {
            IRavenClient ravenClient = new RavenClient(TestHelper.DsnUri);

            Assert.That(ravenClient.Release, Is.Null);
        }


        private const string dsnUri =
            "http://7d6466e66155431495bdb4036ba9a04b:4c1cfeab7ebd4c1cb9e18008173a3630@app.getsentry.com/3739";

        private class TestableJsonPacketFactory : JsonPacketFactory
        {
            private readonly string environment;
            private readonly string logger;
            private readonly string project;
            private readonly string release;


            public TestableJsonPacketFactory(string project = null, string release = null, string environment = null, string logger = null)
            {
                this.project = project;
                this.release = release;
                this.environment = environment;
                this.logger = logger;
            }


            protected override JsonPacket OnCreate(JsonPacket jsonPacket)
            {
                jsonPacket.Project = this.project;
                jsonPacket.Release = this.release;
                jsonPacket.Environment = this.environment;
                jsonPacket.Logger = this.logger;
                return base.OnCreate(jsonPacket);
            }
        }

        private class TestableRavenClient : RavenClient
        {
            public TestableRavenClient(string dsn, IJsonPacketFactory jsonPacketFactory = null)
                : base(dsn, jsonPacketFactory)
            {
            }


            public JsonPacket LastPacket { get; private set; }


            protected override string Send(JsonPacket packet)
            {
                // TODO(dcramer): this is duped from RavenClient
                packet = PreparePacket(packet);
                LastPacket = packet;
                return packet.Project;
            }


#if(!net40)
            protected override Task<string> SendAsync(JsonPacket packet)
            {
                return Task.FromResult(packet.Project);
            }
#endif
        }
    }
}