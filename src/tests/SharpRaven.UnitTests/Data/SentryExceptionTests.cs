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
using SharpRaven.UnitTests.Utilities;

namespace SharpRaven.UnitTests.Data
{
    internal class PrivateException : ApplicationException
    {
    }

    [TestFixture]
    public class SentryExceptionTests
    {
        [Test]
        public void Constructor_DivideByZeroException_ModuleEqualsUnitTests()
        {
            var exception = TestHelper.GetException();
            var sentryException = new SentryException(exception);

            Assert.That(sentryException.Module, Is.EqualTo("SharpRaven.UnitTests"));
        }


        [Test]
        public void Constructor_DivideByZeroException_StackTraceIsNotNull()
        {
            var exception = TestHelper.GetException();
            var sentryException = new SentryException(exception);

            Assert.That(sentryException.Stacktrace, Is.Not.Null);
        }


        [Test]
        public void Constructor_InvalidOperationException_TypeIsInvalidOperationException()
        {
            var exception = new InvalidOperationException("Invalid");
            var sentryException = new SentryException(exception);

            Assert.That(sentryException.Type, Is.EqualTo("System.InvalidOperationException"));
        }


        [Test]
        public void Constructor_InvalidOperationException_ValueIsEqualToMessage()
        {
            const string message = "Invalid";
            var exception = new InvalidOperationException(message);
            var sentryException = new SentryException(exception);

            Assert.That(sentryException.Value, Is.EqualTo(message));
        }


        [Test]
        public void Constructor_NullException_DoesNotThrow()
        {
            var sentryException = new SentryException(null);

            Assert.That(sentryException.Type, Is.Null);
            Assert.That(sentryException.Value, Is.Null);
            Assert.That(sentryException.Module, Is.Null);
            Assert.That(sentryException.Stacktrace, Is.Null);
        }


        [Test]
        public void Constructor_PrivateException_TypeIsPrivateException()
        {
            var exception = new PrivateException();
            var sentryException = new SentryException(exception);

            Assert.That(sentryException.Type, Is.EqualTo("SharpRaven.UnitTests.Data.PrivateException"));
        }


        [Test]
        [Category("NoMono")]
        public void ToString_StringIsEqualTo_ExceptionToString()
        {
            var exception = TestHelper.GetException();
            var sentryException = new SentryException(exception);
            string exceptionString = exception.ToString();
            string stacktraceString = sentryException.ToString();

            Console.WriteLine(exceptionString);
            Console.WriteLine();
            Console.WriteLine(stacktraceString);

            Assert.True(stacktraceString.Contains(exceptionString.Substring(0,30)));
        }
    }
}