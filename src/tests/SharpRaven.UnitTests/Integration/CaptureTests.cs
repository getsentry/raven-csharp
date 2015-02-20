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
using System.Linq;

using NUnit.Framework;

using SharpRaven.Data;
using SharpRaven.Logging;

namespace SharpRaven.UnitTests.Integration
{
    [TestFixture]
    public class CaptureTests
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Initializing RavenClient.");
            this.ravenClient = new RavenClient(DsnUrl)
            {
                Logger = "C#",
                LogScrubber = new LogScrubber()
            };

            Helper.PrintInfo("Sentry Uri: " + this.ravenClient.CurrentDsn.SentryUri);
            Helper.PrintInfo("Port: " + this.ravenClient.CurrentDsn.Port);
            Helper.PrintInfo("Public Key: " + this.ravenClient.CurrentDsn.PublicKey);
            Helper.PrintInfo("Private Key: " + this.ravenClient.CurrentDsn.PrivateKey);
            Helper.PrintInfo("Project ID: " + this.ravenClient.CurrentDsn.ProjectID);
        }

        #endregion

        [Test]
        public void CaptureException_CanLogException_If_Send_Fails()
        {
            const string dsnUri = "http://a:b@totally.notexisting.xyz/666";

            Exception hookedException = null;

            this.ravenClient = new RavenClient(dsnUri)
            {
                ErrorOnCapture = exp => hookedException = exp
            };

            Helper.PrintInfo("In test client change!");
            Helper.PrintInfo("Sentry Uri: " + this.ravenClient.CurrentDsn.SentryUri);
            Helper.PrintInfo("Port: " + this.ravenClient.CurrentDsn.Port);
            Helper.PrintInfo("Public Key: " + this.ravenClient.CurrentDsn.PublicKey);
            Helper.PrintInfo("Private Key: " + this.ravenClient.CurrentDsn.PrivateKey);
            Helper.PrintInfo("Project ID: " + this.ravenClient.CurrentDsn.ProjectID);

            try
            {
                Helper.FirstLevelException();
            }
            catch (Exception e)
            {
                this.ravenClient.CaptureException(e);
            }

            Assert.That(hookedException, Is.Not.Null);
        }


        [Test]
        public void CaptureException_Doesnt_Fail_On_Error_During_Send()
        {
            const string dsnUri = "http://a:b@totally.notexisting.xyz/666";

            this.ravenClient = new RavenClient(dsnUri);

            Helper.PrintInfo("In test client change!");
            Helper.PrintInfo("Sentry Uri: " + this.ravenClient.CurrentDsn.SentryUri);
            Helper.PrintInfo("Port: " + this.ravenClient.CurrentDsn.Port);
            Helper.PrintInfo("Public Key: " + this.ravenClient.CurrentDsn.PublicKey);
            Helper.PrintInfo("Private Key: " + this.ravenClient.CurrentDsn.PrivateKey);
            Helper.PrintInfo("Project ID: " + this.ravenClient.CurrentDsn.ProjectID);

            try
            {
                Helper.FirstLevelException();
            }
            catch (Exception e)
            {
                Assert.DoesNotThrow(() => this.ravenClient.CaptureException(e));
            }
        }


        [Test]
        public void CaptureException_WithMessageFormat_ReturnsValidID()
        {
            object[] args = Enumerable.Range(0, 5).Select(i => Guid.NewGuid()).Cast<object>().ToArray();
            var message = new SentryMessage("A {0:N} B {1:N} C {2:N} D {3:N} F {4:N}.", args);
            var id = this.ravenClient.CaptureException(new Exception("Test without a stacktrace."), message);
            //Console.WriteLine("Sent packet: " + id);

            Assert.That(id, Is.Not.Null);
            Assert.That(Guid.Parse(id), Is.Not.Null);
        }


        [Test]
        public void CaptureException_WithStacktrace_ReturnsValidID()
        {
            try
            {
                Helper.FirstLevelException();
            }
            catch (Exception e)
            {
                //Console.WriteLine("Captured: " + e.Message);
                Dictionary<string, string> tags = new Dictionary<string, string>();
                Dictionary<string, string> extra = new Dictionary<string, string>();

                tags["TAG"] = "TAG1";
                extra["extra"] = "EXTRA1";

                var id = this.ravenClient.CaptureException(e, tags: tags, extra: extra);

                //Console.WriteLine("Sent packet: " + id);

                Assert.That(id, Is.Not.Null);
                Assert.That(Guid.Parse(id), Is.Not.Null);
            }
        }


        [Test]
        public void CaptureException_WithoutStacktrace_ReturnsValidID()
        {
            var id = this.ravenClient.CaptureException(new Exception("Test without a stacktrace."));
            //Console.WriteLine("Sent packet: " + id);

            Assert.That(id, Is.Not.Null.Or.Empty);
            Assert.That(Guid.Parse(id), Is.Not.Null);
        }


        [Test]
        public void CaptureMessage_CanLogException_If_Send_Fails()
        {
            const string dsnUri = "http://a:b@totally.notexisting.xyz/666";

            Exception hookedException = null;

            this.ravenClient = new RavenClient(dsnUri)
            {
                ErrorOnCapture = exp => hookedException = exp
            };

            Helper.PrintInfo("In test client change!");
            Helper.PrintInfo("Sentry Uri: " + this.ravenClient.CurrentDsn.SentryUri);
            Helper.PrintInfo("Port: " + this.ravenClient.CurrentDsn.Port);
            Helper.PrintInfo("Public Key: " + this.ravenClient.CurrentDsn.PublicKey);
            Helper.PrintInfo("Private Key: " + this.ravenClient.CurrentDsn.PrivateKey);
            Helper.PrintInfo("Project ID: " + this.ravenClient.CurrentDsn.ProjectID);

            this.ravenClient.CaptureMessage("Test message");

            Assert.NotNull(hookedException);
        }


        [Test]
        public void CaptureMessage_Doesnt_Fail_On_Error_During_Send()
        {
            const string dsnUri = "http://a:b@totally.notexisting.xyz/666";

            this.ravenClient = new RavenClient(dsnUri);

            Helper.PrintInfo("In test client change!");
            Helper.PrintInfo("Sentry Uri: " + this.ravenClient.CurrentDsn.SentryUri);
            Helper.PrintInfo("Port: " + this.ravenClient.CurrentDsn.Port);
            Helper.PrintInfo("Public Key: " + this.ravenClient.CurrentDsn.PublicKey);
            Helper.PrintInfo("Private Key: " + this.ravenClient.CurrentDsn.PrivateKey);
            Helper.PrintInfo("Project ID: " + this.ravenClient.CurrentDsn.ProjectID);

            Assert.DoesNotThrow(() => this.ravenClient.CaptureMessage("Test message"));
        }


        [Test]
        public void CaptureMessage_ReturnsValidID()
        {
            var id = this.ravenClient.CaptureMessage("Test");
            //Console.WriteLine("Sent packet: " + id);

            Assert.That(id, Is.Not.Null);
            Assert.That(Guid.Parse(id), Is.Not.Null);
        }


        [Test]
        public void CaptureMessage_WithCompression_ReturnsValidID()
        {
            this.ravenClient.Compression = true;
            var id = this.ravenClient.CaptureException(new Exception("Test without a stacktrace."));

            Assert.That(id, Is.Not.Null);
            Assert.That(Guid.Parse(id), Is.Not.Null);
        }


        [Test]
        public void CaptureMessage_WithFormat_ReturnsValidID()
        {
            object[] args = Enumerable.Range(0, 5).Select(i => Guid.NewGuid()).Cast<object>().ToArray();
            var message = new SentryMessage("Lorem %s ipsum %s dolor %s sit %s amet %s.", args);
            var id = this.ravenClient.CaptureMessage(message);
            //Console.WriteLine("Sent packet: " + id);

            Assert.That(id, Is.Not.Null);
            Assert.That(Guid.Parse(id), Is.Not.Null);
        }


        private const string DsnUrl =
            "https://7d6466e66155431495bdb4036ba9a04b:4c1cfeab7ebd4c1cb9e18008173a3630@app.getsentry.com/3739";

        private IRavenClient ravenClient;

        #region Nested type: Helper

        private static class Helper
        {
            public static void FirstLevelException()
            {
                try
                {
                    SecondLevelException();
                }
                catch (Exception e)
                {
                    throw new Exception("First Level Exception", e);
                }
            }


            public static void PrintInfo(string info)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[INFO] ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(info);
            }


            private static void PerformDivideByZero()
            {
                int i2 = 0;
                int i = 10 / i2;
            }


            private static void SecondLevelException()
            {
                try
                {
                    PerformDivideByZero();
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Second Level Exception", e);
                }
            }
        }

        #endregion
    }
}