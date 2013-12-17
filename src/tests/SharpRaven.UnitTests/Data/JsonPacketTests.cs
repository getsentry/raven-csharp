#region License

// Copyright (c) 2013 The Sentry Team and individual contributors.
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

using SharpRaven.Data;

namespace SharpRaven.UnitTests.Data
{
    [TestFixture]
    public class JsonPacketTests
    {
        [Test]
        public void Constructor_NullException_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new JsonPacket(String.Empty, null));
            Assert.That(exception.ParamName, Is.EqualTo("exception"));
        }


        [Test]
        public void Constructor_NullProject_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new JsonPacket(null));
            Assert.That(exception.ParamName, Is.EqualTo("project"));
        }


        [Test]
        public void Constructor_ProjectAndException_EventIDIsNotNull()
        {
            var project = Guid.NewGuid().ToString();
            var json = new JsonPacket(project, new Exception("Error"));

            Assert.That(json.EventID, Is.Not.Null.Or.Empty, "EventID");
        }


        [Test]
        public void Constructor_ProjectAndException_ProjectIsEqual()
        {
            var project = Guid.NewGuid().ToString();
            var json = new JsonPacket(project, new Exception("Error"));

            Assert.That(json.Project, Is.EqualTo(project));
        }


        [Test]
        public void Constructor_Project_EventIDIsNotNull()
        {
            var project = Guid.NewGuid().ToString();
            var json = new JsonPacket(project);

            Assert.That(json.EventID, Is.Not.Null.Or.Empty, "EventID");
        }


        [Test]
        public void Constructor_Project_ProjectIsEqual()
        {
            var project = Guid.NewGuid().ToString();
            var json = new JsonPacket(project);

            Assert.That(json.Project, Is.EqualTo(project));
        }
    }
}