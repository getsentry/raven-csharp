using System;
using System.Reflection;
#if HAS_RUNTIME_INFORMATION
using System.Runtime.InteropServices;
#elif NET45
using Microsoft.Win32;
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
#elif NET45
            var mono = GetFromMono();
            var version = mono
                // Windows only: Get latest installation of .NET Framework 4.5 or later
                ?? Get45PlusLatestInstallationFromRegistry();
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

        // Only Windows, .NET Framework 4.5 forward
#if NET45
        // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed#to-find-net-framework-versions-by-querying-the-registry-in-code-net-framework-45-and-later
        private static string Get45PlusLatestInstallationFromRegistry()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

            try
            {
                using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
                {
                    return int.TryParse(ndpKey?.GetValue("Release")?.ToString(), out var releaseId)
                        ? $".NET Framework {GetFor45PlusVersion(releaseId)}"
                        : null;
                }
            }
            catch (PlatformNotSupportedException)
            {
                return null;
            }

            string GetFor45PlusVersion(int releaseKey)
            {
                switch (releaseKey)
                {
                    // NOTE: 4.7.1 and forward support netstandard 2.0 and should use a different API!
                    case int _ when releaseKey >= 461308: return $"4.7.1 or higher ({releaseKey})";
                    case int _ when releaseKey >= 460798: return "4.7";
                    case int _ when releaseKey >= 394802: return "4.6.2";
                    case int _ when releaseKey >= 394254: return "4.6.1";
                    case int _ when releaseKey >= 393295: return "4.6";
                    case int _ when releaseKey >= 379893: return "4.5.2";
                    case int _ when releaseKey >= 378675: return "4.5.1";
                    case int _ when releaseKey >= 378389: return "4.5";
                    default: return null;
                }
            }
        }
#endif
    }
}
