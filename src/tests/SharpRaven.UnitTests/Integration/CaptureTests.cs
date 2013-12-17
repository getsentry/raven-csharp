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
using System.Collections.Generic;

using NUnit.Framework;

using SharpRaven.Logging;

namespace SharpRaven.UnitTests.Integration
{
    [TestFixture]
    public class CaptureTests
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Initializing RavenClient.");
            this.ravenClient = new RavenClient(DsnUrl)
            {
                Logger = "C#",
                LogScrubber = new LogScrubber()
            };

            PrintInfo("Sentry Uri: " + this.ravenClient.CurrentDsn.SentryUri);
            PrintInfo("Port: " + this.ravenClient.CurrentDsn.Port);
            PrintInfo("Public Key: " + this.ravenClient.CurrentDsn.PublicKey);
            PrintInfo("Private Key: " + this.ravenClient.CurrentDsn.PrivateKey);
            PrintInfo("Project ID: " + this.ravenClient.CurrentDsn.ProjectID);
        }


        private const string DsnUrl =
            "https://7d6466e66155431495bdb4036ba9a04b:4c1cfeab7ebd4c1cb9e18008173a3630@app.getsentry.com/3739";

        private RavenClient ravenClient;


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


        private static void FirstLevelException()
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


        private static void PerformDivideByZero()
        {
            int i2 = 0;
            int i = 10 / i2;
        }


        private static void PrintInfo(string info)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[INFO] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(info);
        }


        [Test]
        public void CaptureWithStacktrace()
        {
            Console.WriteLine("Causing division by zero exception.");
            try
            {
                FirstLevelException();
            }
            catch (Exception e)
            {
                Console.WriteLine("Captured: " + e.Message);
                Dictionary<string, string> tags = new Dictionary<string, string>();
                Dictionary<string, string> extras = new Dictionary<string, string>();

                tags["TAG"] = "TAG1";
                extras["extra"] = "EXTRA1";

                var id = this.ravenClient.CaptureException(e, tags, extras);
                Console.WriteLine("Sent packet: " + id);
            }
        }


        [Test]
        public void CaptureWithoutStacktrace()
        {
            Console.WriteLine("Send exception without stacktrace.");
            var id = this.ravenClient.CaptureException(new Exception("Test without a stacktrace."));
            Console.WriteLine("Sent packet: " + id);
        }
    }
}