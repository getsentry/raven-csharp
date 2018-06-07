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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using Newtonsoft.Json;
using SharpRaven.Serialization;
using SharpRaven.Utilities;

namespace SharpRaven.Data.Context
{
    /// <summary>
    /// Describes the device that caused the event. This is most appropriate for mobile applications.
    /// </summary>
    /// <seealso href="https://docs.sentry.io/clientdev/interfaces/contexts/"/>
    public class Device
    {
        [JsonProperty(PropertyName = "timezone", NullValueHandling = NullValueHandling.Ignore)]
        private string TimezoneSerializable => Timezone?.Id;

        /// <summary>
        /// The name of the device. This is typically a hostname.
        /// </summary>
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The family of the device.
        /// </summary>
        /// <remarks>
        /// This is normally the common part of model names across generations.
        /// </remarks>
        /// <example>
        /// iPhone, Samsung Galaxy
        /// </example>
        [JsonProperty(PropertyName = "family", NullValueHandling = NullValueHandling.Ignore)]
        public string Family { get; set; }
        /// <summary>
        /// The model name.
        /// </summary>
        /// <example>
        /// Samsung Galaxy S3
        /// </example>
        [JsonProperty(PropertyName = "model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }
        /// <summary>
        /// An internal hardware revision to identify the device exactly.
        /// </summary>
        [JsonProperty(PropertyName = "model_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ModelId { get; set; }
        /// <summary>
        /// The CPU architecture.
        /// </summary>
        [JsonProperty(PropertyName = "arch", NullValueHandling = NullValueHandling.Ignore)]
        public string Architecture { get; set; }
        /// <summary>
        /// If the device has a battery an integer defining the battery level (in the range 0-100).
        /// </summary>
        [JsonProperty(PropertyName = "battery_level", NullValueHandling = NullValueHandling.Ignore)]
        public short? BatteryLevel { get; set; }
        /// <summary>
        /// This can be a string portrait or landscape to define the orientation of a device.
        /// </summary>
        [JsonProperty(PropertyName = "orientation", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(LowerInvariantStringEnumConverter))]
        public DeviceOrientation? Orientation { get; set; }
        /// <summary>
        ///  A boolean defining whether this device is a simulator or an actual device.
        /// </summary>
        [JsonProperty(PropertyName = "simulator", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Simulator { get; set; }
        /// <summary>
        /// Total system memory available in bytes.
        /// </summary>
        [JsonProperty(PropertyName = "memory_size", NullValueHandling = NullValueHandling.Ignore)]
        public long? MemorySize { get; set; }
        /// <summary>
        /// Free system memory in bytes.
        /// </summary>
        [JsonProperty(PropertyName = "free_memory", NullValueHandling = NullValueHandling.Ignore)]
        public long? FreeMemory { get; set; }
        /// <summary>
        /// Memory usable for the app in bytes.
        /// </summary>
        [JsonProperty(PropertyName = "usable_memory", NullValueHandling = NullValueHandling.Ignore)]
        public long? UsableMemory { get; set; }
        /// <summary>
        /// Total device storage in bytes.
        /// </summary>
        [JsonProperty(PropertyName = "storage_size", NullValueHandling = NullValueHandling.Ignore)]
        public long? StorageSize { get; set; }
        /// <summary>
        /// Free device storage in bytes.
        /// </summary>
        [JsonProperty(PropertyName = "free_storage", NullValueHandling = NullValueHandling.Ignore)]
        public long? FreeStorage { get; set; }
        /// <summary>
        /// Total size of an attached external storage in bytes (e.g.: android SDK card).
        /// </summary>
        [JsonProperty(PropertyName = "external_storage_size", NullValueHandling = NullValueHandling.Ignore)]
        public long? ExternalStorageSize { get; set; }
        /// <summary>
        /// Free size of an attached external storage in bytes (e.g.: android SDK card).
        /// </summary>
        [JsonProperty(PropertyName = "external_free_storage", NullValueHandling = NullValueHandling.Ignore)]
        public long? ExternalFreeStorage { get; set; }
        /// <summary>
        /// A formatted UTC timestamp when the system was booted.
        /// </summary>
        /// <example>
        /// 018-02-08T12:52:12Z
        /// </example>
        [JsonProperty(PropertyName = "boot_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? BootTime { get; set; }
        /// <summary>
        /// The timezone of the device.
        /// </summary>
        /// <example>
        /// Europe/Vienna
        /// </example>
        [JsonIgnore]
        public TimeZoneInfo Timezone { get; set; }

        /// <summary>
        /// Clones this instance
        /// </summary>
        /// <returns></returns>
        internal Device Clone()
        {
            return new Device
            {
                Name = this.Name,
                Architecture = this.Architecture,
                BatteryLevel = this.BatteryLevel,
                BootTime = this.BootTime,
                ExternalFreeStorage = this.ExternalFreeStorage,
                ExternalStorageSize = this.ExternalStorageSize,
                Family = this.Family,
                FreeMemory = this.FreeMemory,
                FreeStorage = this.FreeStorage,
                MemorySize = this.MemorySize,
                Model = this.Model,
                ModelId = this.ModelId,
                Orientation = this.Orientation,
                Simulator = this.Simulator,
                StorageSize = this.StorageSize,
                Timezone = this.Timezone,
                UsableMemory = this.UsableMemory
            };
        }

        /// <summary>
        /// Creates a Device while attempting to detect the running device info.
        /// </summary>
        /// <returns>An instance if any property was successfuly set or null otherwise.</returns>
        internal static Device Create()
        {
            try
            {
                var device = new Device();

                device.Name = Environment.MachineName;
                device.Timezone = TimeZoneInfo.Local;
                device.Architecture = GetArchitecture();
                device.BootTime = new DateTimeOffset(
                    DateTime.UtcNow - TimeSpan.FromTicks(Stopwatch.GetTimestamp()),
                    TimeSpan.Zero);

                return device;
            }
            catch (Exception e)
            {
                SystemUtil.WriteError(e);
                return null;
            }
        }


        /// <summary>
        /// Retrieves the architecture of this device.
        /// </summary>
        /// <returns>Arch or null if unknown</returns>
        internal static string GetArchitecture() =>
#if HAS_RUNTIME_INFORMATION
                   // x-plat: Known results: X86, X64, Arm, Arm64,
                   RuntimeInformation.OSArchitecture.ToString();
#elif NET35
                   // Windows: Known results: AMD64, IA64, x86
                   Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine)
                       ?? ProcessorArchitectureNet35();
#else
                   // https://github.com/mono/mono/blob/cdea795c0e4706abee0841174c35799690f63ccb/mcs/class/corlib/System.Runtime.InteropServices.RuntimeInformation/RuntimeInformation.cs#L79
                   Environment.Is64BitOperatingSystem ? "X64" : "X86";
#endif

#if NET35

        // Will attempt to detect the architecture of the OS. Returns null if it cannot.
        private static string ProcessorArchitectureNet35()
        {
            bool? is64Bit = null;
            if (IntPtr.Size == 8)
            {
                is64Bit = true;
            }
            // Only XP SP2+ support this API
            else if (Environment.OSVersion.Version.Major == 5
                       && Environment.OSVersion.Version.Minor >= 1
                       || Environment.OSVersion.Version.Major >= 6)
            {
                // 32 bit process on Windows could be WoW64, make P/Invoke call to verify
                using (var p = Process.GetCurrentProcess())
                {
                    if (IsWow64Process(p.Handle, out var retVal))
                    {
                        is64Bit = retVal;
                    }
                }
            }

            return !is64Bit.HasValue ? null : is64Bit.Value ? "X64" : "X86";
        }

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool wow64Process);
#endif
    }
}
