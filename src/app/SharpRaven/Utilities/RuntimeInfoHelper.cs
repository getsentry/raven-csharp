using System;
using System.Reflection;

using SharpRaven.Data.Context;
#if HAS_RUNTIME_INFORMATION
using System.Runtime.InteropServices;
#endif
#if NET45PLUS_REGISTRY_VERSION
using Microsoft.Win32;
#endif

namespace SharpRaven.Utilities
{
    internal static class RuntimeInfoHelper
    {
        public static Runtime GetRuntime()
        {
#if HAS_RUNTIME_INFORMATION // .NET Core 2+, .NET Framework 4.5+
            // Prefered API: netstandard2.0 and vNext
            // https://github.com/dotnet/corefx/blob/master/src/System.Runtime.InteropServices.RuntimeInformation/src/System/Runtime/InteropServices/RuntimeInformation/RuntimeInformation.cs
            // e.g: .NET Framework 4.7.2633.0, .NET Native, WebAssembly
            var runtime = new Runtime
            {
                RawDescription = RuntimeInformation.FrameworkDescription
            };
#else
            var runtime = GetFromMono();
            runtime = runtime
                // Environment.Version: NET Framework 4, 4.5, 4.5.1, 4.5.2 = 4.0.30319.xxxxx
                // .NET Framework 4.6, 4.6.1, 4.6.2, 4.7, 4.7.1 =  4.0.30319.42000
                // Not recommended on NET45+
                ?? new Runtime
                {
                    Name = ".NET Framework",
                    Version = Environment.Version.ToString()
                };
#endif

#if NET45PLUS_REGISTRY_VERSION // .NET Framework 4.5 and later
            if (runtime.IsNetFx())
            {
                runtime.Build = Get45PlusLatestInstallationFromRegistry()?.ToString();
            }
#endif

#if !NETFX // Non .NET Framework (i.e: netstandard, netcoreapp)
            if (runtime.IsNetCore())
            {
                // RuntimeInformation.FrameworkDescription returns 4.6 on .NET Core 2.0 which is counter intuitive
                var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
                var assemblyPath = assembly.CodeBase.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                var netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
                if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
                {
                    runtime.Name = ".NET Core";
                    runtime.Version = assemblyPath[netCoreAppIndex + 1];
                }
            }
#endif
            return runtime;
        }

        private static Runtime GetFromMono()
        {
            // The implementation of Mono to RuntimeInformation:
            // https://github.com/mono/mono/blob/90b49aa3aebb594e0409341f9dca63b74f9df52e/mcs/class/corlib/System.Runtime.InteropServices.RuntimeInformation/RuntimeInformation.cs
            if (Type.GetType("Mono.Runtime", false)
                    ?.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static)
                    ?.Invoke(null, null) is string monoVersion)
            {
                return new Runtime
                {
                    // Send complete (raw) and let Sentry parse it. UI can display short version but details are not lost
                    // e.g; Mono 5.10.0 (Visual Studio built mono)
                    // e.g: Mono 5.10.1.47 (tarball Tue Apr 17 09:23:16 UTC 2018)
                    RawDescription = "Mono " + monoVersion
                };
            }

            return null;
        }

#if NET45PLUS_REGISTRY_VERSION
        // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed#to-find-net-framework-versions-by-querying-the-registry-in-code-net-framework-45-and-later
        private static int? Get45PlusLatestInstallationFromRegistry()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                return int.TryParse(ndpKey?.GetValue("Release")?.ToString(), out var releaseId)
                    ? releaseId
                    : (int?)null;
            }
        }
#endif
    }
}
