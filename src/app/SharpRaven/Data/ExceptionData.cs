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

using Newtonsoft.Json;

using SharpRaven.Utilities;

namespace SharpRaven.Data
{
    /// <summary>
    /// Data class for containing <see cref="Exception.Data"/>.
    /// </summary>
    public class ExceptionData : Dictionary<string, object>
    {
        private readonly string exceptionType;


        /// <summary>Initializes a new instance of the <see cref="ExceptionData"/> class.</summary>
        /// <param name="exception">The exception.</param>
        public ExceptionData(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            this.exceptionType = exception.GetType().FullName;

            foreach (var k in exception.Data.Keys)
            {
                try
                {
                    var value = exception.Data[k];
                    var key = k as string ?? k.ToString();
                    Add(key, value);
                }
                catch (Exception e)
                {
                    SystemUtil.WriteError(e);
                }
            }

            if (exception.InnerException == null)
                return;

            var exceptionData = new ExceptionData(exception.InnerException);

            if (exceptionData.Count == 0)
            {
                return;
            }

            exceptionData.AddTo(this);
        }


        /// <summary>Gets the type of the exception.</summary>
        /// <value>The type of the exception.</value>
        [JsonProperty("type")]
        public string ExceptionType
        {
            get { return this.exceptionType; }
        }


        private void AddTo(IDictionary<string, object> dictionary)
        {
            var key = String.Concat(ExceptionType, '.', "Data");
            key = UniqueKey(dictionary, key);
            dictionary.Add(key, this);
        }


        private static string UniqueKey(IDictionary<string, object> dictionary, object key)
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
    }
}