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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using NSubstitute;

using NUnit.Framework;

using SharpRaven.Data;
using SharpRaven.Logging;
using SharpRaven.UnitTests.Utilities;
#if (!net40) && (!net35)
using System.Threading.Tasks;

#endif

namespace SharpRaven.UnitTests.RavenClientTests
{
    [TestFixture]
    public class CaptureTester
    {
        [Test]
        public void AllMethodsAreTested()
        {
            var testerMethods = GetType()
                .GetMethods(BindingFlags.DeclaredOnly)
                .Where(m => !m.GetCustomAttributes(typeof(TestAttribute), false).Any())
                .Where(m => !m.GetCustomAttributes(typeof(IgnoreAttribute), false).Any())
                .ToList();

            var ravenClientTests = new[]
            {
#if (!net40) && (!net35)
                typeof(CaptureAsyncTests),
                typeof(CaptureExceptionAsyncTests),
                typeof(CaptureMessageAsyncTests),
#endif
                typeof(CaptureExceptionTests),
                typeof(CaptureMessageTests),
                typeof(CaptureTests)
            };

            var sb = new StringBuilder();

            foreach (var ravenClientTestType in ravenClientTests)
            {
                var testMethods = ravenClientTestType
                    .GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any())
                    .ToList();

                foreach (var testerMethod in testerMethods.Where(testerMethod => testMethods.All(m => m.Name != testerMethod.Name)))
                {
                    sb.AppendFormat("{0}.{1} is missing or not properly decorated with a [Test] attribute.", ravenClientTestType.FullName,
                                    testerMethod.Name);
                    sb.AppendLine();
                }
            }

            if (sb.Length > 0)
                Assert.Fail(sb.ToString());
        }


        private const string dsnUri =
            "http://7d6466e66155431495bdb4036ba9a04b:4c1cfeab7ebd4c1cb9e18008173a3630@app.getsentry.com/3739";


        public void ClientEnvironmentIsIgnored(Action<IRavenClient> capture)
        {
            var jsonPacketFactory = new TestableJsonPacketFactory(environment : "keep-me");
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory) { Environment = "foobar" };
            capture.Invoke(client);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Environment, Is.EqualTo("keep-me"));
        }


        public void ClientLoggerIsIgnored(Action<IRavenClient> capture)
        {
            var jsonPacketFactory = new TestableJsonPacketFactory(logger : "keep-me");
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory) { Logger = "foobar" };
            capture.Invoke(client);
            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Logger, Is.EqualTo("keep-me"));
        }


        public void ClientReleaseIsIgnored(Action<IRavenClient> capture)
        {
            var jsonPacketFactory = new TestableJsonPacketFactory(release : "keep-me");
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory) { Release = "foobar" };
            capture.Invoke(client);
            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Release, Is.EqualTo("keep-me"));
        }


        public void ErrorLevelIsDebug(Action<IRavenClient> capture)
        {
            var client = new TestableRavenClient(dsnUri);
            capture.Invoke(client);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Level, Is.EqualTo(ErrorLevel.Debug));
        }


        [Ignore]
        public IRavenClient GetTestableRavenClient(string project)
        {
            var jsonPacketFactory = new TestableJsonPacketFactory(project);
            return new TestableRavenClient(dsnUri, jsonPacketFactory);
        }


        public void InvokesSendAndJsonPacketFactoryOnCreate(Func<IRavenClient, string> capture)
        {
            var project = Guid.NewGuid().ToString();
            var client = GetTestableRavenClient(project);
            var result = capture.Invoke(client);

            Assert.That(result, Is.EqualTo(project));
        }


        public void OnlyDefaultTags(Action<IRavenClient> capture)
        {
            var client = new TestableRavenClient(dsnUri);
            client.Tags.Add("key", "value");
            client.Tags.Add("foo", "bar");
            capture.Invoke(client);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Tags["key"], Is.EqualTo("value"));
            Assert.That(lastEvent.Tags["foo"], Is.EqualTo("bar"));
        }


        public void OnlyMessageTags(Action<IRavenClient, IDictionary<string, string>> capture)
        {
            var messageTags = new Dictionary<string, string> { { "key", "value" }, { "foo", "bar" } };
            var client = new TestableRavenClient(dsnUri);
            capture.Invoke(client, messageTags);
            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Tags["key"], Is.EqualTo("value"));
            Assert.That(lastEvent.Tags["foo"], Is.EqualTo("bar"));
        }


        public void ScrubberIsInvoked(Action<IRavenClient, string> capture)
        {
            var message = Guid.NewGuid().ToString("n");
            var ravenClient = GetScrubbingRavenClient(message);

            capture.Invoke(ravenClient, message);

            // Verify that we actually received a Scrub() call:
            ravenClient.LogScrubber.Received().Scrub(Arg.Any<string>());
        }


#if (!net40) && (!net35)
        public async Task ScrubberIsInvokedAsync(Func<IRavenClient, string, Task> captureAsync)
        {
            var message = Guid.NewGuid().ToString("n");
            var ravenClient = GetScrubbingRavenClient(message);

            await captureAsync.Invoke(ravenClient, message).ConfigureAwait(false);

            // Verify that we actually received a Scrub() call:
            ravenClient.LogScrubber.Received().Scrub(Arg.Any<string>());
        }
#endif


        public void SendsEnvironment(Action<IRavenClient> capture)
        {
            var client = new TestableRavenClient(dsnUri) { Environment = "foobar" };
            capture.Invoke(client);
            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Environment, Is.EqualTo("foobar"));
        }


        public void SendsLogger(Action<IRavenClient> capture)
        {
            var client = new TestableRavenClient(dsnUri) { Logger = "foobar" };
            capture.Invoke(client);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Logger, Is.EqualTo("foobar"));
        }


        public void SendsRelease(Action<IRavenClient> capture)
        {
            var client = new TestableRavenClient(dsnUri) { Release = "foobar" };
            capture.Invoke(client);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Release, Is.EqualTo("foobar"));
        }


        public void TagHandling(Action<IRavenClient, IDictionary<string, string>> capture)
        {
            var messageTags = new Dictionary<string, string> { { "key", "another value" } };

            var client = new TestableRavenClient(dsnUri);
            client.Tags.Add("key", "value");
            client.Tags.Add("foo", "bar");
            capture.Invoke(client, messageTags);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Tags["key"], Is.EqualTo("another value"));
            Assert.That(lastEvent.Tags["foo"], Is.EqualTo("bar"));
        }


        private static RavenClient GetScrubbingRavenClient(string message)
        {
            var ravenClient = new RavenClient(TestHelper.DsnUri)
            {
                LogScrubber = Substitute.For<IScrubber>()
            };
#if (!net35)
            ravenClient.LogScrubber
                .Scrub(Arg.Any<string>())
                .Returns(c =>
                {
                    var json = c.Arg<string>();
                    Assert.That(json, Is.StringContaining(message));
                    return json;
                });
#endif

            return ravenClient;
        }


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


#if (!net40) && (!net35)
            protected override Task<string> SendAsync(JsonPacket packet, CancellationToken ct)
            {
                packet = PreparePacket(packet);
                LastPacket = packet;
                return Task.FromResult(packet.Project);
            }
#endif
        }
    }
}