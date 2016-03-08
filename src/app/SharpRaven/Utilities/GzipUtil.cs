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

using System.IO;
using System.IO.Compression;
using System.Text;
#if (!net40)
using System.Threading.Tasks;

#endif

namespace SharpRaven.Utilities
{
    /// <summary>
    /// GZip Utilities.
    /// </summary>
    internal class GzipUtil
    {
        /// <summary>
        /// Compress a JSON string with base-64 encoded gzip compressed string.
        /// </summary>
        /// <param name="json">The JSON to write.</param>
        /// <param name="stream">The stream.</param>
        public static void Write(string json, Stream stream)
        {
            // Encode into data byte-stream with UTF8.
            byte[] data = Encoding.UTF8.GetBytes(json);

            using (GZipStream gzip = new GZipStream(stream, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
        }


#if(!net40)
        /// <summary>
        /// Compress a JSON string with base-64 encoded gzip compressed string.
        /// </summary>
        /// <param name="json">The JSON to write.</param>
        /// <param name="stream">The stream.</param>
        public static async Task WriteAsync(string json, Stream stream)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            using (GZipStream gzip = new GZipStream(stream, CompressionMode.Compress))
            {
                await gzip.WriteAsync(data, 0, data.Length);
            }
        }
#endif
    }
}