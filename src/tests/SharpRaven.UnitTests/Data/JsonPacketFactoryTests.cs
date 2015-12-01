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

using Newtonsoft.Json;

using NUnit.Framework;

using SharpRaven.Data;

namespace SharpRaven.UnitTests.Data
{
    [TestFixture]
    public class JsonPacketFactoryTests
    {
        #region SetUp/Teardown

        [SetUp]
        public void SetUp()
        {
            this.jsonPacketFactory = new JsonPacketFactory();
        }

        #endregion

        [Test]
        public void Create_InvokesOnCreate()
        {
            var project = Guid.NewGuid().ToString("N");
            var factory = new TestableJsonPacketFactory(project);
            var json = factory.Create(String.Empty, (SentryMessage)null);

            Assert.That(json.Project, Is.EqualTo(project));
        }


        [Test]
        public void Create_Project_EventIDIsValidGuid()
        {
            var project = Guid.NewGuid().ToString();
            var json = this.jsonPacketFactory.Create(project, (SentryMessage)null);

            Assert.That(json.EventID, Is.Not.Null.Or.Empty, "EventID");
            Assert.That(Guid.Parse(json.EventID), Is.Not.Null);
        }


        [Test]
        public void Create_Project_ModulesHasCountGreaterThanZero()
        {
            var project = Guid.NewGuid().ToString();
            var json = this.jsonPacketFactory.Create(project, (SentryMessage)null);

            Assert.That(json.Modules, Has.Count.GreaterThan(0));
        }


        [Test]
        public void Create_Project_ProjectIsEqual()
        {
            var project = Guid.NewGuid().ToString();
            var json = this.jsonPacketFactory.Create(project, (SentryMessage)null);

            Assert.That(json.Project, Is.EqualTo(project));
        }


        [Test]
        public void Create_Project_ServerNameEqualsMachineName()
        {
            var project = Guid.NewGuid().ToString();
            var json = this.jsonPacketFactory.Create(project, (SentryMessage)null);

            Assert.That(json.ServerName, Is.EqualTo(Environment.MachineName));
        }


        [Test]
        public void Create_ProjectAndException_DataPropertyIsSavedInExtras()
        {
            var project = Guid.NewGuid().ToString();
            var exception = new Exception("Error");
            exception.Data.Add("key", "value");
            var json = this.jsonPacketFactory.Create(project, exception);

            Assert.That(json.Extra.Keys, Has.Exactly(1).EqualTo("ExceptionData"));
            Assert.That(json.Extra["ExceptionData"], Is.TypeOf<ExceptionData>());
            Assert.That(json.Extra["ExceptionData"], Has.Exactly(1).EqualTo(new KeyValuePair<string, object>("key", "value")));
        }


        [Test]
        public void Create_ProjectAndException_DataPropertyIsSavedInExtrasAlongWithExtrasObject()
        {
            var project = Guid.NewGuid().ToString();
            var exception = new Exception("Error");
            exception.Data.Add("key", "value");
            var json = this.jsonPacketFactory.Create(project, exception, extra: new { key2 = "value2" });

            Assert.That(json.Extra, Has.Exactly(1).EqualTo(new KeyValuePair<string, object>("key2", "value2")));
            Assert.That(json.Extra.Keys, Has.Exactly(1).EqualTo("ExceptionData"));
            Assert.That(json.Extra["ExceptionData"], Is.TypeOf<ExceptionData>());
            Assert.That(json.Extra["ExceptionData"], Has.Exactly(1).EqualTo(new KeyValuePair<string, object>("key", "value")));
        }


        [Test]
        public void Create_ProjectAndException_ExtraObjectIsSavedWithoutUnnecessaryProperties()
        {
            var project = Guid.NewGuid().ToString();
            var exception = new Exception("Error");
            var json = this.jsonPacketFactory.Create(project, exception, extra: new Dictionary<string, string> { { "key", "value2" } });

            Assert.That(json.Extra.Keys, Has.No.Member("Count"));
        }


        [Test]
        public void Create_ProjectAndException_DataPropertyIsSavedInExtrasAlongWithExtrasObjectEvenWithTheSameKey()
        {
            var project = Guid.NewGuid().ToString();
            var exception = new Exception("Error");
            exception.Data.Add("key", "value");
            var json = this.jsonPacketFactory.Create(project, exception, extra: new { ExceptionData = "ExceptionValue" });

            Assert.That(json.Extra, Has.Exactly(1).EqualTo(new KeyValuePair<string, object>("ExceptionData", "ExceptionValue")));
            Assert.That(json.Extra.Keys, Has.Exactly(1).EqualTo("ExceptionData0"));
            Assert.That(json.Extra["ExceptionData0"], Is.TypeOf<ExceptionData>());
            Assert.That(json.Extra["ExceptionData0"], Has.Exactly(1).EqualTo(new KeyValuePair<string, object>("key", "value")));
        }


        [Test]
        public void Create_ProjectAndException_EventIDIsValidGuid()
        {
            var project = Guid.NewGuid().ToString();
            var json = this.jsonPacketFactory.Create(project, new Exception("Error"));

            Assert.That(json.EventID, Is.Not.Null.Or.Empty, "EventID");
            Assert.That(Guid.Parse(json.EventID), Is.Not.Null);
        }


        [Test]
        public void Create_ProjectAndException_MessageEqualsExceptionMessage()
        {
            var project = Guid.NewGuid().ToString();
            Exception exception = new Exception("Error");
            var json = this.jsonPacketFactory.Create(project, exception);

            Assert.That(json.Message, Is.EqualTo(exception.Message));
        }


        [Test]
        public void Create_ProjectAndException_ModulesHasCountGreaterThanZero()
        {
            var project = Guid.NewGuid().ToString();
            var json = this.jsonPacketFactory.Create(project, new Exception("Error"));

            Assert.That(json.Modules, Has.Count.GreaterThan(0));
        }


        [Test]
        public void Create_ProjectAndException_NestedExceptionsAndExtraAreStoredInExtrasObject()
        {
            var project = Guid.NewGuid().ToString();
            var exception = Helper.GetException();
            var json = this.jsonPacketFactory.Create(project, exception, extra: new { ExtraKey = "ExtraValue" });

            var s = JsonConvert.SerializeObject(json, Formatting.Indented);

            Console.WriteLine(s);

            Assert.That(json.Extra, Has.Exactly(1).EqualTo(new KeyValuePair<string, object>("ExtraKey", "ExtraValue")));
            Assert.That(json.Extra.Keys, Has.Exactly(1).EqualTo("ExceptionData"));
            Assert.That(json.Extra["ExceptionData"], Is.TypeOf<ExceptionData>());

            var exceptionData = (ExceptionData)json.Extra["ExceptionData"];
            Assert.That(exceptionData,
                        Has.Exactly(1).EqualTo(new KeyValuePair<string, object>("FirstLevelExceptionKey", "FirstLevelExceptionValue")));
            Assert.That(exceptionData.Keys, Has.Exactly(1).EqualTo("InvalidOperationExceptionData"));
            Assert.That(exceptionData["InvalidOperationExceptionData"], Is.TypeOf<ExceptionData>());

            var invalidOperationExceptionData = (ExceptionData)exceptionData["InvalidOperationExceptionData"];
            Assert.That(invalidOperationExceptionData,
                        Has.Exactly(1).EqualTo(new KeyValuePair<string, object>("InvalidOperationExceptionKey",
                                                                                "InvalidOperationExceptionValue")));
            Assert.That(invalidOperationExceptionData.Keys, Has.Exactly(1).EqualTo("DivideByZeroExceptionData"));

            var divideByZeroExceptionData = (ExceptionData)invalidOperationExceptionData["DivideByZeroExceptionData"];
            Assert.That(divideByZeroExceptionData,
                        Has.Exactly(1).EqualTo(new KeyValuePair<string, object>("DivideByZeroKey", "DivideByZeroValue")));
        }


        [Test]
        public void Create_ProjectAndException_ProjectIsEqual()
        {
            var project = Guid.NewGuid().ToString();
            var json = this.jsonPacketFactory.Create(project, new Exception("Error"));

            Assert.That(json.Project, Is.EqualTo(project));
        }


        [Test]
        public void Create_ProjectAndException_ServerNameEqualsMachineName()
        {
            var project = Guid.NewGuid().ToString();
            var json = this.jsonPacketFactory.Create(project, new Exception("Error"));

            Assert.That(json.ServerName, Is.EqualTo(Environment.MachineName));
        }


        private IJsonPacketFactory jsonPacketFactory;

        private class TestableJsonPacketFactory : JsonPacketFactory
        {
            private readonly string project;


            public TestableJsonPacketFactory(string project)
            {
                this.project = project;
            }


            protected override JsonPacket OnCreate(JsonPacket jsonPacket)
            {
                jsonPacket.Project = this.project;
                return base.OnCreate(jsonPacket);
            }
        }
    }
}