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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Newtonsoft.Json;

namespace SharpRaven.Data
{
    /// <summary>
    /// A default implementation of <see cref="ISentryRequestFactory"/>. Override the <see cref="OnCreate"/>
    /// method to adjust the values of the <see cref="SentryRequest"/> before it is sent to Sentry.
    /// </summary>
    public class SentryRequestFactory : ISentryRequestFactory
    {
        private static bool checkedForHttpContextProperty;

        /// <summary>
        /// Gets or sets the CurrentHttpContextProperty
        /// </summary>
        /// <value>
        /// The current httpcontext property
        /// </value>
        internal static dynamic CurrentHttpContextProperty { get; set; }

        [JsonIgnore]
        internal static bool HasCurrentHttpContextProperty
        {
            get { return CurrentHttpContextProperty != null; }
        }

        [JsonIgnore]
        internal static bool HasHttpContext
        {
            get { return HttpContext != null; }
        }

        /// <summary>
        /// Gets or sets the HTTP context.
        /// </summary>
        /// <value>
        /// The HTTP context.
        /// </value>
        internal static dynamic HttpContext
        {
            get
            {
                TryGetHttpContextPropertyFromAppDomain();

                // [Meilu] If the currentHttpcontext property is not available we couldnt retrieve it, dont continue
                if (!HasCurrentHttpContextProperty)
                    return null;

                try
                {
                    return CurrentHttpContextProperty.GetValue(null, null);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("An error occurred while retrieving the current HTTP context: {0}", exception);
                    return null;
                }
            }
        }


        /// <summary>
        /// Creates a new instance of <see cref="SentryRequest"/>
        /// for the current packet.
        /// </summary>
        /// <returns>A new instance of <see cref="SentryRequest"/> with information relating to the current HTTP request</returns>
        public ISentryRequest Create()
        {
            if (!HasHttpContext || HttpContext.Request == null)
                return OnCreate(null);

            var request = new SentryRequest
            {
                Url = HttpContext.Request.Url.ToString(),
                Method = HttpContext.Request.HttpMethod,
                Environment = Convert(x => x.Request.ServerVariables),
                Headers = Convert(x => x.Request.Headers),
                Cookies = Convert(x => x.Request.Cookies),
                Data = BodyConvert(),
                QueryString = HttpContext.Request.QueryString.ToString()
            };

            return OnCreate(request);
        }


        /// <summary>
        /// Called when the <see cref="SentryRequest"/> has been created. Can be overridden to
        /// adjust the values of the <paramref name="request"/> before it is sent to Sentry.
        /// </summary>
        /// <param name="request">The HTTP request information.</param>
        /// <returns>
        /// The <see cref="SentryRequest"/>.
        /// </returns>
        public virtual SentryRequest OnCreate(SentryRequest request)
        {
            return request;
        }


        private static object BodyConvert()
        {
            if (!HasHttpContext)
                return null;

            try
            {
                var mediaTypes = new Dictionary<string, IHttpRequestBodyConverter>
                {
                    { "FormMediaType", new FormHttpRequestBodyConverter() },
                    { "MultiPartFormMediaType", new MultiPartFormHttpRequestBodyConverter() },
                    { "JsonMediaType", new JsonHttpRequestBodyConverter() },
                    { "DefaultMediaType", new DefaultHttpRequestBodyConverter() }
                };

                foreach (var item in mediaTypes)
                {
                    var mediaType = item.Value;

                    if (!mediaType.Matches(HttpContext.Request.ContentType))
                        continue;

                    object data;
                    if (mediaType.TryConvert(HttpContext, out data))
                        return data;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return null;
        }


        private static IDictionary<string, string> Convert(Func<dynamic, NameObjectCollectionBase> collectionGetter)
        {
            if (!HasHttpContext)
                return null;

            IDictionary<string, string> dictionary = new Dictionary<string, string>();

            try
            {
                var collection = collectionGetter.Invoke(HttpContext);
                var keys = Enumerable.ToArray(collection.AllKeys);

                foreach (object key in keys)
                {
                    if (key == null)
                        continue;

                    var stringKey = key as string ?? key.ToString();

                    // NOTE: Ignore these keys as they just add duplicate information. [asbjornu]
                    if (stringKey.StartsWith("ALL_") || stringKey.StartsWith("HTTP_"))
                        continue;

                    var value = collection[stringKey];
                    var stringValue = value as string;

                    if (stringValue != null)
                    {
                        // Most dictionary values will be strings and go through this path.
                        dictionary.Add(stringKey, stringValue);
                    }
                    else
                    {
                        // HttpCookieCollection is an ugly, evil beast that needs to be treated with a sledgehammer.

                        try
                        {
                            // For whatever stupid reason, HttpCookie.ToString() doesn't return its Value, so we need to dive into the .Value property like this.
                            dictionary.Add(stringKey, value.Value);
                        }
                        catch (Exception exception)
                        {
                            dictionary.Add(stringKey, exception.ToString());
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return dictionary;
        }


        private static void TryGetHttpContextPropertyFromAppDomain()
        {
            if (checkedForHttpContextProperty)
                return;

            checkedForHttpContextProperty = true;

            try
            {
                var systemWeb = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(assembly => assembly.FullName.StartsWith("System.Web,"));

                if (systemWeb == null)
                    return;

                var httpContextType = systemWeb.GetExportedTypes()
                    .FirstOrDefault(type => type.Name == "HttpContext");

                if (httpContextType == null)
                    return;

                var currentHttpContextProperty = httpContextType.GetProperty("Current",
                                                                             BindingFlags.Static | BindingFlags.Public);

                if (currentHttpContextProperty == null)
                    return;

                CurrentHttpContextProperty = currentHttpContextProperty;
            }
            catch (Exception exception)
            {
                Console.WriteLine("An error occurred while retrieving the HTTP contextproperty: {0}", exception);
            }
        }
    }
}