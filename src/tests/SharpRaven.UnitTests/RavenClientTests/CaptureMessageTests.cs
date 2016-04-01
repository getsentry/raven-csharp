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

using NUnit.Framework;

using SharpRaven.Data;

namespace SharpRaven.UnitTests.RavenClientTests
{
    [TestFixture]
    public class CaptureMessageTests
    {
        #region SetUp/Teardown

        [SetUp]
        public void SetUp()
        {
            this.tester = new RavenClientTester();
        }

        #endregion

        [Test]
        public void ClientEnvironmentIsIgnored()
        {
            this.tester.ClientEnvironmentIsIgnored(client => client.CaptureMessage("Test"));
        }


        [Test]
        public void ClientLoggerIsIgnored()
        {
            this.tester.ClientLoggerIsIgnored(client => client.CaptureMessage("Test"));
        }


        [Test]
        public void ClientReleaseIsIgnored()
        {
            this.tester.ClientReleaseIsIgnored(client => client.CaptureMessage("Test"));
        }


        [Test]
        public void ErrorLevelIsDebug()
        {
            this.tester.ErrorLevelIsDebug(client => client.CaptureMessage("Test", ErrorLevel.Debug));
        }


        [Test]
        public void InvokesSendAndJsonPacketFactoryOnCreate()
        {
            this.tester.InvokesSendAndJsonPacketFactoryOnCreate(client => client.CaptureMessage("Test"));
        }


        [Test]
        public void OnlyDefaultTags()
        {
            this.tester.OnlyDefaultTags(client => client.CaptureMessage("Test"));
        }


        [Test]
        public void OnlyMessageTags()
        {
            this.tester.OnlyMessageTags((client, tags) => client.CaptureMessage("Test", ErrorLevel.Info, tags));
        }


        [Test]
        public void ScrubberIsInvoked()
        {
            this.tester.ScrubberIsInvoked((client, message) => client.CaptureMessage(message));
        }


        [Test]
        public void SendsEnvironment()
        {
            this.tester.SendsEnvironment(client => client.CaptureMessage("Test"));
        }


        [Test]
        public void SendsLogger()
        {
            this.tester.SendsLogger(client => client.CaptureMessage("Test"));
        }


        [Test]
        public void SendsRelease()
        {
            this.tester.SendsRelease(client => client.CaptureMessage("Test"));
        }


        [Test]
        public void TagHandling()
        {
            this.tester.TagHandling((client, tags) => client.CaptureMessage("Test", ErrorLevel.Info, tags));
        }


        private RavenClientTester tester;
    }
}