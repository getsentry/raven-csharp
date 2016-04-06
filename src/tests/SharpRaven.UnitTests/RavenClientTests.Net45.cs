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

#if (!net40)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using NSubstitute;

using NUnit.Framework;

using SharpRaven.Data;
using SharpRaven.Logging;
using SharpRaven.UnitTests.Utilities;

namespace SharpRaven.UnitTests
{
    [TestFixture]
    public partial class RavenClientTests
    {
        [Test]
        public async Task CaptureMessageAsync_ClientEnvironmentIsIgnored()
        {
            var jsonPacketFactory = new TestableJsonPacketFactory(environment : "keep-me");
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory) { Environment = "foobar" };
            await client.CaptureMessageAsync("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Environment, Is.EqualTo("keep-me"));
        }


        [Test]
        public async Task CaptureMessageAsync_ClientLoggerIsIgnored()
        {
            var jsonPacketFactory = new TestableJsonPacketFactory(logger : "keep-me");
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory) { Logger = "foobar" };
            await client.CaptureMessageAsync("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Logger, Is.EqualTo("keep-me"));
        }


        [Test]
        public async Task CaptureMessageAsync_ClientReleaseIsIgnored()
        {
            var jsonPacketFactory = new TestableJsonPacketFactory(release : "keep-me");
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory) { Release = "foobar" };
            await client.CaptureMessageAsync("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Release, Is.EqualTo("keep-me"));
        }


        [Test]
        public async Task CaptureMessageAsync_InvokesSend_AndJsonPacketFactoryOnCreate()
        {
            var project = Guid.NewGuid().ToString();
            var jsonPacketFactory = new TestableJsonPacketFactory(project);
            var client = new TestableRavenClient(dsnUri, jsonPacketFactory);
            var result = await client.CaptureMessageAsync("Test");

            Assert.That(result, Is.EqualTo(project));
        }


        [Test]
        public async Task CaptureMessageAsync_OnlyDefaultTags()
        {
            var client = new TestableRavenClient(dsnUri);
            client.Tags.Add("key", "value");
            client.Tags.Add("foo", "bar");
            // client.Tags = defaultTags;
            await client.CaptureMessageAsync("Test", ErrorLevel.Info);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Tags["key"], Is.EqualTo("value"));
            Assert.That(lastEvent.Tags["foo"], Is.EqualTo("bar"));
        }


        [Test]
        public async Task CaptureMessageAsync_OnlyMessageTags()
        {
            var messageTags = new Dictionary<string, string> { { "key", "value" }, { "foo", "bar" } };

            var client = new TestableRavenClient(dsnUri);
            await client.CaptureMessageAsync("Test", ErrorLevel.Info, messageTags);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Tags["key"], Is.EqualTo("value"));
            Assert.That(lastEvent.Tags["foo"], Is.EqualTo("bar"));
        }


        [Test]
        public async Task CaptureMessageAsync_ScrubberIsInvoked()
        {
            string message = Guid.NewGuid().ToString("n");

            var client = new RavenClient(TestHelper.DsnUri);
            client.LogScrubber = Substitute.For<IScrubber>();
            client.LogScrubber.Scrub(Arg.Any<string>())
                .Returns(c =>
                {
                    string json = c.Arg<string>();
                    Assert.That(json, Is.StringContaining(message));
                    return json;
                });

            await client.CaptureMessageAsync(message);

            // Verify that we actually received a Scrub() call:
            client.LogScrubber.Received().Scrub(Arg.Any<string>());
        }


        [Test]
        public async Task CaptureMessageAsync_SendsEnvironment()
        {
            var client = new TestableRavenClient(dsnUri) { Environment = "foobar" };
            await client.CaptureMessageAsync("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Environment, Is.EqualTo("foobar"));
        }


        [Test]
        public async Task CaptureMessageAsync_SendsLogger()
        {
            var client = new TestableRavenClient(dsnUri) { Logger = "foobar" };
            await client.CaptureMessageAsync("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Logger, Is.EqualTo("foobar"));
        }


        [Test]
        public async Task CaptureMessageAsync_SendsRelease()
        {
            var client = new TestableRavenClient(dsnUri) { Release = "foobar" };
            await client.CaptureMessageAsync("Test");

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Release, Is.EqualTo("foobar"));
        }


        [Test]
        public async Task CaptureMessageAsync_TagHandling()
        {
            var messageTags = new Dictionary<string, string> { { "key", "another value" } };

            var client = new TestableRavenClient(dsnUri);
            client.Tags.Add("key", "value");
            client.Tags.Add("foo", "bar");
            await client.CaptureMessageAsync("Test", ErrorLevel.Info, messageTags);

            var lastEvent = client.LastPacket;

            Assert.That(lastEvent.Tags["key"], Is.EqualTo("another value"));
            Assert.That(lastEvent.Tags["foo"], Is.EqualTo("bar"));
        }
    }
}

#endif