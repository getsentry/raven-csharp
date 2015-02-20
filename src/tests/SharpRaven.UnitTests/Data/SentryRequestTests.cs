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

using NUnit.Framework;

using SharpRaven.Data;
using SharpRaven.UnitTests.Utilities;

namespace SharpRaven.UnitTests.Data
{
    [TestFixture]
    public class SentryRequestTests
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            // Set the HTTP Context to null before so tests don't bleed data into each other. @asbjornu
            SentryRequestFactory.HttpContext = null;
        }


        [TearDown]
        public void TearDown()
        {
            // Set the HTTP Context to null before so tests don't bleed data into each other. @asbjornu
            SentryRequestFactory.HttpContext = null;
        }

        #endregion

        private static readonly ISentryRequestFactory requestFactory = new SentryRequestFactory();

        private static void SimulateHttpRequest(Action<SentryRequest> test)
        {
            using (var simulator = new HttpSimulator())
            {
                simulator.SetFormVariable("Form1", "Value1");
                simulator.SetHeader("UserAgent", "SharpRaven");
                simulator.SetCookie("Cookie", "Monster");

                using (simulator.SimulateRequest())
                {
                    var request = requestFactory.Create();
                    test.Invoke(request);
                }
            }
        }


        [Test]
        public void GetRequest_NoHttpContext_ReturnsNull()
        {
            var request = requestFactory.Create();
            Assert.That(request, Is.Null);
        }


        [Test]
        [Category("NoMono")]
        public void GetRequest_WithHttpContext_RequestHasCookies()
        {
            SimulateHttpRequest(request =>
            {
                Assert.That(request.Cookies, Has.Count.EqualTo(1));
                Assert.That(request.Cookies["Cookie"], Is.EqualTo("Monster"));
            });
        }


        [Test]
        [Category("NoMono")]
        public void GetRequest_WithHttpContext_RequestHasFormVariables()
        {
            SimulateHttpRequest(request =>
            {
                Assert.That(request.Data, Is.TypeOf<Dictionary<string, string>>());

                var data = (Dictionary<string, string>) request.Data;

                Assert.That(data, Has.Count.EqualTo(1));
                Assert.That(data["Form1"], Is.EqualTo("Value1"));
            });
        }


        [Test]
        [Category("NoMono")]
        public void GetRequest_WithHttpContext_RequestHasHeaders()
        {
            SimulateHttpRequest(request =>
            {
                Assert.That(request.Headers, Has.Count.EqualTo(3));
                Assert.That(request.Headers["UserAgent"], Is.EqualTo("SharpRaven"));
            });
        }


        [Test]
        [Category("NoMono")]
        public void GetRequest_WithHttpContext_RequestIsNotNull()
        {
            SimulateHttpRequest(request => Assert.That(request, Is.Not.Null));
        }
    }
}