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
using System.Net;
#if (!net35)
using System.Threading.Tasks;
#endif

using Newtonsoft.Json;

using SharpRaven.Utilities;

#if !(net40) && !(net35)

namespace SharpRaven.Data
{
    public partial class Requester
    {
        /// <summary>
        /// Executes the <c>async</c> HTTP request to Sentry.
        /// </summary>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        public async Task<string> RequestAsync()
        {
            using (var s = await this.webRequest.GetRequestStreamAsync())
            {
                if (this.Client.Compression)
                    await GzipUtil.WriteAsync(this.data.Scrubbed, s);
                else
                {
                    using (var sw = new StreamWriter(s))
                    {
                        await sw.WriteAsync(this.data.Scrubbed);
                    }
                }
            }

            using (var wr = (HttpWebResponse)await this.webRequest.GetResponseAsync())
            {
                using (var responseStream = wr.GetResponseStream())
                {
                    if (responseStream == null)
                        return null;

                    using (var sr = new StreamReader(responseStream))
                    {
                        var content = await sr.ReadToEndAsync();
                        var response = JsonConvert.DeserializeObject<dynamic>(content);
                        return response.id;
                    }
                }
            }
        }

        /// <summary>
        /// Sends the user feedback asynchronously to sentry
        /// </summary>
        /// <returns>An empty string if succesful, otherwise the server response</returns>
        public async Task<string> SendFeedbackAsync()
        {
            using (var s = await this.webRequest.GetRequestStreamAsync())
            {
                using (var sw = new StreamWriter(s))
                {
                    await sw.WriteAsync(feedback.ToString());
                }
            }
            using (var wr = (HttpWebResponse)await this.webRequest.GetResponseAsync())
            {
                using (var responseStream = wr.GetResponseStream())
                {
                    if (responseStream == null)
                        return null;

                    using (var sr = new StreamReader(responseStream))
                    {
                        var response = await sr.ReadToEndAsync();
                        return response;
                    }
                }
            }
        }
    }
}

#endif