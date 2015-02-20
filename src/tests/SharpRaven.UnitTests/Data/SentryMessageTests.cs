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

using SharpRaven.Data;

namespace SharpRaven.UnitTests.Data
{
    [TestFixture]
    public class SentryMessageTests
    {
        [Test]
        public void Message_IsImplicitlyConvertedTo_String()
        {
            string message = new SentryMessage("Hello");

            Assert.That(message, Is.EqualTo("Hello"));
        }


        [Test]
        public void Message_ReturnsMessage()
        {
            const string format = "Format something {0:N} in here";
            var arg = Guid.NewGuid();
            SentryMessage message = new SentryMessage(format, arg);

            Assert.That(message.Message, Is.EqualTo(format));
        }


        [Test]
        public void NullMessage_IsImplicitlyConvertedTo_NullString()
        {
            SentryMessage message = null;
            string stringMessage = message;

            Assert.That(stringMessage, Is.Null);
        }


        [Test]
        public void NullString_IsImplicitlyConvertedTo_NullMessage()
        {
            string stringMessage = null;
            SentryMessage message = stringMessage;

            Assert.That(message, Is.Null);
        }


        [Test]
        public void Parameters_ReturnsParameters()
        {
            const string format = "Format something {0:N} in here";
            var arg = Guid.NewGuid();
            SentryMessage message = new SentryMessage(format, arg);

            Assert.That(message.Parameters, Has.Length.EqualTo(1));
            Assert.That(message.Parameters[0], Is.EqualTo(arg));
        }


        [Test]
        public void String_IsImplicitlyConvertedTo_Message()
        {
            SentryMessage message = "Hello";

            Assert.That(message.ToString(), Is.EqualTo("Hello"));
        }


        [Test]
        public void ToString_ReturnsFormattedString()
        {
            Guid arg = Guid.NewGuid();
            SentryMessage message = new SentryMessage("Format something {0:N} in here", arg);

            Assert.That(message.ToString(), Is.StringContaining(arg.ToString("N")));
        }


        [Test]
        public void ToString_ReturnsMessage()
        {
            string stringMessage = Guid.NewGuid().ToString("N");
            SentryMessage message = new SentryMessage(stringMessage);

            Assert.That(message.ToString(), Is.EqualTo(stringMessage));
        }
    }
}