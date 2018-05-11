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

#if (!NET40) && (!NET35)

using System;
using System.Threading.Tasks;

using NUnit.Framework;

using SharpRaven.Data;

namespace SharpRaven.UnitTests.RavenClientTests
{
    [TestFixture]
    public class CaptureAsyncTests
    {
        #region SetUp/Teardown

        [SetUp]
        public void SetUp()
        {
            this.tester = new CaptureTester();
        }

        #endregion

        [Test]
        public void ClientEnvironmentIsIgnored()
        {
            this.tester.ClientEnvironmentIsIgnored(client => client.CaptureAsync(new SentryEvent("Test")));
        }


        [Test]
        public void ClientLoggerIsIgnored()
        {
            this.tester.ClientLoggerIsIgnored(async client => await client.CaptureAsync(new SentryEvent("Test")));
        }


        [Test]
        public void ClientReleaseIsIgnored()
        {
            this.tester.ClientReleaseIsIgnored(async client => await client.CaptureAsync(new SentryEvent("Test")));
        }


        [Test]
        public void ErrorLevelIsDebug()
        {
            this.tester.ErrorLevelIsDebug(async client => await client.CaptureAsync(new SentryEvent("Test") { Level = ErrorLevel.Debug }));
        }


        [Test]
        public async Task InvokesSendAndJsonPacketFactoryOnCreate()
        {
            var project = Guid.NewGuid().ToString();
            var client = this.tester.GetTestableRavenClient(project);
            var result = await client.CaptureAsync(new SentryEvent("Test"));

            Assert.That(result, Is.EqualTo(project));
        }


        [Test]
        public void OnlyDefaultTags()
        {
            this.tester.OnlyDefaultTags(async client => await client.CaptureAsync(new SentryEvent("Test")));
        }


        [Test]
        public void OnlyMessageTags()
        {
            this.tester.OnlyMessageTags(async (client, tags) => await client.CaptureAsync(new SentryEvent("Test")
            {
                Level = ErrorLevel.Info,
                Tags = tags
            }));
        }


        [Test]
        public async void ScrubberIsInvoked()
        {
            await this.tester.ScrubberIsInvokedAsync(async (client, message) => await client.CaptureAsync(new SentryEvent(message)));
        }


        [Test]
        public void SendsEnvironment()
        {
            this.tester.SendsEnvironment(async client => await client.CaptureAsync(new SentryEvent("Test")));
        }


        [Test]
        public void SendsLogger()
        {
            this.tester.SendsLogger(async client => await client.CaptureAsync(new SentryEvent("Test")));
        }


        [Test]
        public void SendsRelease()
        {
            this.tester.SendsRelease(async client => await client.CaptureAsync(new SentryEvent("Test")));
        }


        [Test]
        public void TagHandling()
        {
            this.tester.TagHandling(async (client, tags) => await client.CaptureAsync(new SentryEvent("Test")
            {
                Level = ErrorLevel.Info,
                Tags = tags
            }));
        }


        private CaptureTester tester;
    }
}

#endif