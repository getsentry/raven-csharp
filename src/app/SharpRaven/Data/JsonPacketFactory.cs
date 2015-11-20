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
using System.Reflection;

namespace SharpRaven.Data
{
    /// <summary>
    /// A default implementation of <see cref="IJsonPacketFactory"/>. Override the <see cref="OnCreate"/>
    /// method to adjust the values of the <see cref="JsonPacket"/> before it is sent to Sentry.
    /// </summary>
    public class JsonPacketFactory : IJsonPacketFactory
    {
        /// <summary>
        /// Creates a new instance of
        /// <see cref="JsonPacket" /> for the specified
        /// <paramref name="project" />.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured <paramref name="message" />. Default <see cref="ErrorLevel.Info" />.</param>
        /// <param name="tags">The tags to annotate the captured <paramref name="message" /> with.</param>
        /// <param name="fingerprint">The custom fingerprint to annotate the captured <paramref name="message" /> with.</param>
        /// <param name="extra">The extra metadata to send with the captured <paramref name="message" />.</param>
        /// <returns>
        /// A new instance of <see cref="JsonPacket" /> for the specified <paramref name="project" />.
        /// </returns>
        public JsonPacket Create(string project,
                                 SentryMessage message,
                                 ErrorLevel level = ErrorLevel.Info,
                                 IDictionary<string, string> tags = null,
                                 string[] fingerprint = null,
                                 object extra = null)
        {
            var json = new JsonPacket(project)
            {
                Message = message != null ? message.ToString() : null,
                MessageObject = message,
                Level = level,
                Tags = tags,
                Fingerprint = fingerprint,
                Extra = Convert(extra)
            };

            return OnCreate(json);
        }


        /// <summary>
        /// Creates a new instance of
        /// <see cref="JsonPacket" /> for the specified
        /// <paramref name="project" />, with the
        /// given
        /// <paramref name="exception" />.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="exception">The <see cref="Exception" /> to capture.</param>
        /// <param name="message">The optional messge to capture. Default: <see cref="Exception.Message" />.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured <paramref name="exception" />. Default: <see cref="ErrorLevel.Error" />.</param>
        /// <param name="tags">The tags to annotate the captured <paramref name="exception" /> with.</param>
        /// <param name="fingerprint">The custom fingerprint to annotate the captured <paramref name="message" /> with.</param>
        /// <param name="extra">The extra metadata to send with the captured <paramref name="exception" />.</param>
        /// <returns>
        /// A new instance of
        /// <see cref="JsonPacket" /> for the specified
        /// <paramref name="project" />, with the
        /// given
        /// <paramref name="exception" />.
        /// </returns>
        public JsonPacket Create(string project,
                                 Exception exception,
                                 SentryMessage message = null,
                                 ErrorLevel level = ErrorLevel.Error,
                                 IDictionary<string, string> tags = null,
                                 string[] fingerprint = null,
                                 object extra = null)
        {
            var json = new JsonPacket(project, exception)
            {
                Message = message != null ? message.ToString() : exception.Message,
                MessageObject = message,
                Level = level,
                Tags = tags,
                Fingerprint = fingerprint,
                Extra = Convert(extra, exception)
            };

            return OnCreate(json);
        }


        /// <summary>
        /// Called when the <see cref="JsonPacket"/> has been created. Can be overridden to
        /// adjust the values of the <paramref name="jsonPacket"/> before it is sent to Sentry.
        /// </summary>
        /// <param name="jsonPacket">The json packet.</param>
        /// <returns>
        /// The <see cref="JsonPacket"/>.
        /// </returns>
        protected virtual JsonPacket OnCreate(JsonPacket jsonPacket)
        {
            return jsonPacket;
        }


        internal static string UniqueKey(IDictionary<string, object> dictionary, object key)
        {
            var stringKey = key as string ?? key.ToString();

            if (!dictionary.ContainsKey(stringKey))
                return stringKey;

            for (var i = 0; i < 10000; i++)
            {
                var newKey = String.Concat(stringKey, i);
                if (!dictionary.ContainsKey(newKey))
                    return newKey;
            }

            throw new ArgumentException(String.Format("Unable to find a unique key for '{0}'.", stringKey), "key");
        }


        private static IDictionary<string, object> Convert(object extra, Exception exception = null)
        {
            if (exception == null && extra == null)
                return null;

            var result = new Dictionary<string, object>();

            if (extra != null)
            {
                foreach (var property in extra.GetType().GetProperties())
                {
                    try
                    {
                        var value = property.GetValue(extra, BindingFlags.Default, null, null, null);
                        var key = UniqueKey(result, property.Name);
                        result.Add(key, value);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: " + e);
                    }
                }
            }

            if (exception == null)
                return result;

            var exceptionData = new ExceptionData(exception);
            exceptionData.AddTo(result);

            return result;
        }
    }
}