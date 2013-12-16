using System;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace SharpRaven.Utilities {
    class GzipUtil {
        /// <summary>
        /// Compress a JSON string with base-64 encoded gzip compressed string.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>json base64 data stream</returns>
        public static string CompressEncode(string json) {
            // Encode into data byte-stream with UTF8.
            byte[] data = UTF8Encoding.UTF8.GetBytes(json);

            using (MemoryStream memory = new MemoryStream()) {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress)) {
                    gzip.Write(data, 0, data.Length);
                }

                return Convert.ToBase64String(memory.ToArray());
            }
        }
    }
}
