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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly : AssemblyTitle("SharpRaven")]
[assembly : AssemblyDescription("SharpRaven is a C# client for Sentry https://www.getsentry.com")]
[assembly : AssemblyConfiguration("")]
[assembly : AssemblyCompany("Sentry")]
[assembly : AssemblyProduct("SharpRaven")]
[assembly : AssemblyCopyright("Copyright © Sentry 2013")]
[assembly : AssemblyTrademark("")]
[assembly : AssemblyCulture("")]
[assembly : ComVisible(false)]
[assembly : Guid("b5683941-1254-484e-b074-87cedd4fc78e")]

// AssemblyVersion the most formal version number and is akin to "API Version". It doesn't need to change unless there's breaking changes.

[assembly : AssemblyVersion("1.0.0.0")]

// AssemblyFileVersion is more informal and can be increased more rapidly and with less consideration than AssemblyVersion. 

[assembly : AssemblyFileVersion("1.0.0.0")]

// AssemblyInformationalVersion is even more informal than AssemblyFileVersion and doesn't need a certain format. It will be used as the $version$ replacement string in NuGet and can contain suffixes like "-alpha".
// [assembly: AssemblyInformationalVersion("0.8.0.0")]

[assembly : InternalsVisibleTo("SharpRaven.UnitTests")]