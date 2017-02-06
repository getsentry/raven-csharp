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
using System.IO;
using System.Linq;

namespace SharpRaven.Utilities
{
    /// <summary>
    /// Utility class for retreiving system information.
    /// </summary>
    public static class SystemUtil
    {
        /// <summary>
        /// Checks if a string is null or white space
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(string arg)
        {
            return string.IsNullOrEmpty(arg) || string.IsNullOrEmpty(arg.Trim());
        }

        /// <summary>
        /// Return all loaded modules.
        /// </summary>
        /// <returns>
        /// All loaded modules.
        /// </returns>
        public static IDictionary<string, string> GetModules()
        {
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                #if (!net35)
                .Where(q => !q.IsDynamic)
                #endif
                .Select(a => a.GetName())
                .OrderBy(a => a.Name);

            var dictionary = new Dictionary<string, string>();

            foreach (var assembly in assemblies)
            {
                if (dictionary.ContainsKey(assembly.Name))
                    continue;

                dictionary.Add(assembly.Name, assembly.Version.ToString());
            }

            return dictionary;
        }

        /// <summary>
        /// Writes the <paramref name="exception"/> to the <see cref="Console"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to write to the <see cref="Console"/>.</param>
        public static void WriteError(Exception exception)
        {
            if (exception == null)
                return;

            WriteError(exception.ToString());
        }

        /// <summary>
        /// Writes the <paramref name="error"/> to the <see cref="Console"/>.
        /// </summary>
        /// <param name="error">The error to write to the <see cref="Console"/>.</param>
        public static void WriteError(string error)
        {
            if (error == null)
                return;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ERROR] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(error);
        }

        /// <summary>
        /// Writes the <paramref name="description"/> and <paramref name="multilineData"/> to the <see cref="Console"/>.
        /// </summary>
        /// <param name="description">The text describing the <paramref name="multilineData"/>.</param>
        /// <param name="multilineData">The multi-line data to write to the <see cref="Console"/>.</param>
        public static void WriteError(string description, string multilineData)
        {
            if (multilineData == null)
                return;

            var lines = multilineData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length <= 0)
                return;

            WriteError(description);
            foreach (var line in lines)
            {
                WriteError(line);
            }
        }
        public static void CopyTo(this Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}