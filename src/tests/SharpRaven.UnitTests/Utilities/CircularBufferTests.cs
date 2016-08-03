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

using SharpRaven.Utilities;

namespace SharpRaven.UnitTests.Utilities
{
    [TestFixture]
    public class CircularBufferTests
    {
        [Test]
        public void Should_Create_Empty()
        {
            var circularBuffer = new CircularBuffer<string>();

            Assert.That(circularBuffer.IsEmpty(), Is.True);
        }

        [Test]
        public void Should_Put_One_Item()
        {
            var circularBuffer = new CircularBuffer<string>();

            circularBuffer.Add("One");

            Assert.That(circularBuffer.ToList().Count, Is.EqualTo(1));
        }

        [Test]
        public void Should_Retain_Last_Two_Items()
        {
            var circularBuffer = new CircularBuffer<string>(2);

            circularBuffer.Add("One");
            circularBuffer.Add("Two");
            circularBuffer.Add("Three");

            Assert.That(circularBuffer.ToList().Count, Is.EqualTo(2));
            Assert.That(circularBuffer.ToList()[0], Is.EqualTo("Two"));
            Assert.That(circularBuffer.ToList()[1], Is.EqualTo("Three"));
        }

        [Test]
        public void Should_Be_Empty_When_Clear()
        {
            var circularBuffer = new CircularBuffer<string>(2);

            circularBuffer.Add("One");

            circularBuffer.Clear();

            Assert.That(circularBuffer.ToList().Count, Is.EqualTo(0));
        }
    }
}