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
using System.Diagnostics;
using System.IO;

using NUnit.Framework;

namespace SharpRaven.UnitTests.Integration
{
    /// <summary>
    /// NuGet tests. These are more integration tests than unit tests, but I can't bother with
    /// setting up TeamCity to run more than one test DLL.
    /// </summary>
    [TestFixture]
    [Category("NuGet")]
    public class NuGetTests
    {
        private static string MakeAbsolute(string relativePath)
        {
            string absolutePath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\..\", relativePath);

            return new DirectoryInfo(absolutePath).FullName;
        }


        [Test]
        public void Pack_Works()
        {
            string pathToNuGet = MakeAbsolute(@".nuget\NuGet.exe");
            string pathToNuSpec = MakeAbsolute(@"src\app\SharpRaven\SharpRaven.nuspec");

            //Console.WriteLine("pathToNuGet: " + pathToNuGet);
            //Console.WriteLine("pathToNuSpec: " + pathToNuSpec);

            ProcessStartInfo start = new ProcessStartInfo(pathToNuGet)
            {
                Arguments = String.Format(
                    "Pack {0} -Version {1} -Properties Configuration=Release -Properties \"ReleaseNotes=Test\"",
                    pathToNuSpec,
                    GetType().Assembly.GetName().Version),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            //Console.WriteLine("Arguments: " + start.Arguments);

            using (var process = new Process())
            {
                process.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
                process.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);
                process.StartInfo = start;
                Assert.That(process.Start(), Is.True, "The NuGet process couldn't start.");
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(2000);
                Assert.That(process.ExitCode, Is.EqualTo(0), "The NuGet process exited with an unexpected code.");
            }
        }
    }
}