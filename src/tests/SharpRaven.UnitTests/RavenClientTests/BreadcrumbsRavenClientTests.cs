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

using System.Collections.Generic;

using NSubstitute;

using NUnit.Framework;

using SharpRaven.Data;
using SharpRaven.UnitTests.Utilities;

namespace SharpRaven.UnitTests.RavenClientTests {
    [TestFixture]
    public class BreadcrumbsRavenClientTests {

        [Test]
        public void Should_Call_JsonFactory_to_Breadcrumbs() {
            var breadcrumbsRecord = new BreadcrumbsRecord();

            var jsonPacketFactory = Substitute.For<IJsonPacketFactory>();

            IRavenClient ravenClient = new RavenClientTestable(TestHelper.DsnUri, jsonPacketFactory);
            
            ravenClient.AddTrail(breadcrumbsRecord);

            ravenClient.Capture(new SentryEvent(new SentryMessage("foo")));

            jsonPacketFactory.Received().Create(Arg.Any<string>(),
                                                Arg.Any<SentryEvent>(),
                                                Arg.Is<List<BreadcrumbsRecord>>(br => br.Contains(breadcrumbsRecord)));
        }

        [Test]
        public void Should_Call_JsonFactory_to_Breadcrumbs_Null_When_NotUsed() {
            var jsonPacketFactory = Substitute.For<IJsonPacketFactory>();

            IRavenClient ravenClient = new RavenClientTestable(TestHelper.DsnUri, jsonPacketFactory);
            
            ravenClient.Capture(new SentryEvent(new SentryMessage("foo")));

            jsonPacketFactory.Received().Create(Arg.Any<string>(),
                                                Arg.Any<SentryEvent>(),
                                                null);
        }

        [Test]
        public void Should_Call_JsonFactory_to_Breadcrumbs_Null_When_Not_Informed() {
            var jsonPacketFactory = Substitute.For<IJsonPacketFactory>();

            IRavenClient ravenClient = new RavenClientTestable(TestHelper.DsnUri, jsonPacketFactory);

            ravenClient.AddTrail(null);

            ravenClient.Capture(new SentryEvent(new SentryMessage("foo1")));

            jsonPacketFactory.Received().Create(Arg.Any<string>(),
                                                Arg.Any<SentryEvent>(),
                                                null);
        }

        [Test]
        public void Should_RestartTrails_When_Call_ResetTrails() {
            var jsonPacketFactory = Substitute.For<IJsonPacketFactory>();

            IRavenClient ravenClient = new RavenClientTestable(TestHelper.DsnUri, jsonPacketFactory);
            
            ravenClient.AddTrail(new BreadcrumbsRecord());
            ravenClient.RestartTrails();

            ravenClient.Capture(new SentryEvent(new SentryMessage("foo")));

            jsonPacketFactory.Received().Create(Arg.Any<string>(),
                                                Arg.Any<SentryEvent>(),
                                                null);
        }

        [Test]
        public void Shouldnot_Register_Trails_When_IgnoreBreadcrumb() {
            var jsonPacketFactory = Substitute.For<IJsonPacketFactory>();

            IRavenClient ravenClient = new RavenClientTestable(TestHelper.DsnUri, jsonPacketFactory);
            ravenClient.IgnoreBreadcrumbs = true;

            ravenClient.AddTrail(new BreadcrumbsRecord());

            ravenClient.Capture(new SentryEvent(new SentryMessage("foo")));

            jsonPacketFactory.Received().Create(Arg.Any<string>(),
                                                Arg.Any<SentryEvent>(),
                                                null);
        }


        [Test]
        public void Should_RestartTrails_After_Send_Package() {
            var breadcrumbsRecord = new BreadcrumbsRecord();

            var jsonPacketFactory = Substitute.For<IJsonPacketFactory>();

            IRavenClient ravenClient = new RavenClientTestable(TestHelper.DsnUri, jsonPacketFactory);

            ravenClient.AddTrail(breadcrumbsRecord);

            ravenClient.Capture(new SentryEvent(new SentryMessage("foo")));
            jsonPacketFactory.Received().Create(Arg.Any<string>(),
                                                Arg.Any<SentryEvent>(),
                                                Arg.Is<List<BreadcrumbsRecord>>(br => br.Contains(breadcrumbsRecord)));

            ravenClient.Capture(new SentryEvent(new SentryMessage("foo")));
            jsonPacketFactory.Received().Create(Arg.Any<string>(),
                                                Arg.Any<SentryEvent>(),
                                                null);
        }

        public class RavenClientTestable : RavenClient {
            public RavenClientTestable(string dsnUri, IJsonPacketFactory jsonPacketFactory1)
                : base(dsnUri, jsonPacketFactory1) {

            }
            
            protected override string Send(JsonPacket packet) {
                return null;
            }
        }
    }
}