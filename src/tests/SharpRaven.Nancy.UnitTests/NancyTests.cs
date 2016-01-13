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

using Nancy;
using Nancy.Testing;

using NSubstitute;

using NUnit.Framework;

using SharpRaven.Data;

namespace SharpRaven.Nancy.UnitTests
{
    [TestFixture]
    public class NancyTests
    {
        [Test]
        public void Get_InvokesRavenClientWithNancyContextJsonPacketFactory()
        {
            var guid = Guid.NewGuid().ToString();
            var browser = new Browser(cfg =>
            {
                cfg.Module<LogModule>();
                cfg.ApplicationStartup((container, pipelines) =>
                {
                    // Override the IRavenClient implementation so we don't perform
                    // any HTTP request and can retrieve the GUID so we can later
                    // assert that it is the same we sent in. @asbjornu
                    container.Register<IRavenClient, TestableRavenClient>();
                });
            });

            var response = browser.Get("/log", with => { with.FormValue("GUID", guid); });

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            response.Body["#message"]
                .ShouldExistOnce()
                .And.ShouldContain(
                    guid,
                    StringComparison.InvariantCultureIgnoreCase);
        }


        [Test]
        public void Post_InvokesRavenClient()
        {
            var ravenClient = Substitute.For<IRavenClient>();

            var browser = new Browser(cfg =>
            {
                cfg.Module<DefaultModule>();
                cfg.ApplicationStartup((container, pipelines) => container.Register(ravenClient));
            });

            TestDelegate throwing = () => browser.Post("/");

            var exception = Assert.Throws<Exception>(throwing);

            Assert.That(exception.InnerException, Is.TypeOf<RequestExecutionException>());
            Assert.That(exception.InnerException.InnerException, Is.TypeOf<DivideByZeroException>());
            ravenClient.Received(1).CaptureException(exception.InnerException.InnerException);
        }


        [Test]
        public void Post_InvokesRavenClientWithInjectedDsn()
        {
            var dsn = "http://my:test@example.com/987123/oijf98j23r";
            var browser = new Browser(cfg =>
            {
                cfg.Module<DefaultModule>();
                cfg.ApplicationStartup((container, pipelines) =>
                {
                    // Register the DSN so we verify that it can be injected. @asbjornu
                    container.Register(new Dsn(dsn));

                    // Override the IRavenClient implementation so we don't perform
                    // any HTTP request and can retrieve the GUID so we can later
                    // assert that it is the same we sent in. @asbjornu
                    container.Register<IRavenClient, TestableRavenClient>();
                });
            });

            TestDelegate throwing = () => browser.Post("/");

            var exception = Assert.Throws<Exception>(throwing);

            Assert.That(exception.InnerException, Is.TypeOf<RequestExecutionException>());
            Assert.That(exception.InnerException.InnerException, Is.TypeOf<DivideByZeroException>());
            // SentryRequestStartup.Initialize() should set the return value from IRavenClient.Send() in Exception.Data. @asbjornu
            var result = exception.InnerException.InnerException.Data[NancyConfiguration.SentryEventGuidKey];
            Assert.That(result, Is.EqualTo(dsn));
        }


        [Test]
        public void Post_InvokesRavenClientWithNancyContextJsonPacketFactory()
        {
            var guid = Guid.NewGuid().ToString();
            var browser = new Browser(cfg =>
            {
                cfg.Module<DefaultModule>();
                cfg.ApplicationStartup((container, pipelines) =>
                {
                    // Override the IRavenClient implementation so we don't perform
                    // any HTTP request and can retrieve the GUID so we can later
                    // assert that it is the same we sent in. @asbjornu
                    container.Register<IRavenClient, TestableRavenClient>();
                });
            });

            TestDelegate throwing = () => browser.Post("/", with =>
            {
                // Set the GUID which should be transferred to the SentryRequest
                // object in the NancyContextJsonPacketFactory.OnCreate() method.
                // @asbjornu
                with.FormValue("GUID", guid);
            });

            var exception = Assert.Throws<Exception>(throwing);

            Assert.That(exception.InnerException, Is.TypeOf<RequestExecutionException>());
            Assert.That(exception.InnerException.InnerException, Is.TypeOf<DivideByZeroException>());
            // SentryRequestStartup.Initialize() should set the return value from IRavenClient.Send() in Exception.Data. @asbjornu
            var loggedGuid = exception.InnerException.InnerException.Data[NancyConfiguration.SentryEventGuidKey];
            Assert.That(loggedGuid, Is.EqualTo(guid));
        }


        private class TestableRavenClient : RavenClient
        {
            private readonly Dsn dsn;


            public TestableRavenClient(IJsonPacketFactory jsonPacketFactory)
                : base(jsonPacketFactory)
            {
                // This constructor must exist so Nancy can inject the NancyContextJsonPacketFactory
                // that is registered in SentryRegistrations. @asbjornu
            }


            public TestableRavenClient(Dsn dsn, IJsonPacketFactory jsonPacketFactory)
                : base(dsn, jsonPacketFactory)
            {
                // This constructor must exist so Nancy can inject the Dsn and NancyContextJsonPacketFactory
                // that is registered in SentryRegistrations. @asbjornu
                this.dsn = dsn;
            }


            protected override string Send(JsonPacket packet)
            {
                if (this.dsn != null)
                    return this.dsn.ToString();

                // Retrieving the GUID from the JsonPacket verifies that
                // NancyContextJsonPacketFactory.OnCreate() has been invoked,
                // since it will copy the request data from the NancyContext.
                // @asbjornu
                dynamic data = packet.Request.Data;
                return data["GUID"];
            }
        }
    }
}