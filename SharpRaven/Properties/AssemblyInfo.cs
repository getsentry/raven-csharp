using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("SharpRaven")]
[assembly: AssemblyDescription("SharpRaven is a C# client for Sentry https://www.getsentry.com")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Sentry")]
[assembly: AssemblyProduct("SharpRaven")]
[assembly: AssemblyCopyright("Copyright © Sentry 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("b5683941-1254-484e-b074-87cedd4fc78e")]

// AssemblyVersion the most formal version number and is akin to "API Version". It doesn't need to change unless there's breaking changes.
[assembly: AssemblyVersion("0.8.0.0")]

// AssemblyFileVersion is more informal and can be increased more rapidly and with less consideration than AssemblyVersion. 
[assembly: AssemblyFileVersion("0.8.0.0")]

// AssemblyInformationalVersion is even more informal than AssemblyFileVersion and doesn't need a certain format. It will be used as the $version$ replacement string in NuGet and can contain suffixes like "-alpha".
// [assembly: AssemblyInformationalVersion("0.8.0.0")]

[assembly: InternalsVisibleTo("SharpRaven.UnitTests")]