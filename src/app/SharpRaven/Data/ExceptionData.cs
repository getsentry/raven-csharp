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

namespace SharpRaven.Data
{
    /// <summary>
    /// Data class for containing <see cref="Exception.Data"/>.
    /// </summary>
    public class ExceptionData : Dictionary<string, object>
    {
        private readonly Exception exception;


        /// <summary>Initializes a new instance of the <see cref="ExceptionData"/> class.</summary>
        /// <param name="exception">The exception.</param>
        public ExceptionData(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            this.exception = exception;

            foreach (var k in exception.Data.Keys)
            {
                try
                {
                    var value = exception.Data[k];
                    var key = JsonPacketFactory.UniqueKey(this, k);
                    Add(key, value);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e);
                }
            }

            if (exception.InnerException == null)
                return;

            var exceptionData = new ExceptionData(exception.InnerException);
            exceptionData.AddTo(this);
        }


        /// <summary>Adds the current <see cref="ExceptionData"/> instance to the specified <paramref name="dictionary"/>.</summary>
        /// <param name="dictionary">The dictionary to current <see cref="ExceptionData"/> instance to.</param>
        public void AddTo(IDictionary<string, object> dictionary)
        {
            var key = String.Concat(this.exception.GetType().Name, "Data");
            key = JsonPacketFactory.UniqueKey(dictionary, key);
            dictionary.Add(key, this);
        }
    }
}