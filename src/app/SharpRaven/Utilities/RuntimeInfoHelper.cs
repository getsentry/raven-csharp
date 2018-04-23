using System;
using System.Reflection;
#if HAS_RUNTIME_INFORMATION
using System.Runtime.InteropServices;
#endif

namespace SharpRaven.Utilities
{
    internal static class RuntimeInfoHelper
    {
        public static string GetRuntimeVersion()
        {
#if HAS_RUNTIME_INFORMATION
            // Prefered API: netstandard2.0 and vNext
            // https://github.com/dotnet/corefx/blob/master/src/System.Runtime.InteropServices.RuntimeInformation/src/System/Runtime/InteropServices/RuntimeInformation/RuntimeInformation.cs
            // e.g: .NET Framework 4.7.2633.0, .NET Native, WebAssembly
            var version = RuntimeInformation.FrameworkDescription;
#else
            var mono = GetFromMono();
            var version = mono
                // Environment.Version: NET Framework 4, 4.5, 4.5.1, 4.5.2 = 4.0.30319.xxxxx
                // .NET Framework 4.6, 4.6.1, 4.6.2, 4.7, 4.7.1 =  4.0.30319.42000
                // Not recommended on NET45+
                ?? $".NET Framework {Environment.Version}";
#endif
            return version;
        }

        private static string GetFromMono()
        {
            // The implementation of Mono to RuntimeInformation:
            // https://github.com/mono/mono/blob/90b49aa3aebb594e0409341f9dca63b74f9df52e/mcs/class/corlib/System.Runtime.InteropServices.RuntimeInformation/RuntimeInformation.cs
            // e.g; Mono 5.10.0 (Visual Studio built mono)
            var monoVersion = Type.GetType("Mono.Runtime", false)
                ?.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static)
                ?.Invoke(null, null) as string;

            if (monoVersion != null)
            {
                monoVersion = "Mono " + monoVersion;
            }

            return monoVersion;
        }
    }
}
