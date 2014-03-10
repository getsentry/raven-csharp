#region License

// Copyright (c) 2013 The Sentry Team and individual contributors.
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

using System.Collections.Generic;

using Newtonsoft.Json;

namespace SharpRaven.Data
{
    /// <summary>
    /// Represents Sentry's version of <see cref="System.Diagnostics.StackFrame" />.
    /// </summary>
    public class ExceptionFrame
    {
        /// <summary>
        /// Gets or sets the absolute path.
        /// </summary>
        /// <value>
        /// The absolute path.
        /// </value>
        [JsonProperty(PropertyName = "abs_path")]
        public string AbsolutePath { get; set; }

        /// <summary>
        /// Gets or sets the column number.
        /// </summary>
        /// <value>
        /// The column number.
        /// </value>
        [JsonProperty(PropertyName = "colno")]
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        [JsonProperty(PropertyName = "filename")]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        /// <value>
        /// The function.
        /// </value>
        [JsonProperty(PropertyName = "function")]
        public string Function { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [information application].
        /// </summary>
        /// <value>
        /// <c>true</c> if [information application]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(PropertyName = "in_app")]
        public bool InApp { get; set; }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        [JsonProperty(PropertyName = "lineno")]
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the module.
        /// </summary>
        /// <value>
        /// The module.
        /// </value>
        [JsonProperty(PropertyName = "module")]
        public string Module { get; set; }

        /// <summary>
        /// Gets or sets the post context.
        /// </summary>
        /// <value>
        /// The post context.
        /// </value>
        [JsonProperty(PropertyName = "post_context")]
        public List<string> PostContext { get; set; }

        /// <summary>
        /// Gets or sets the preference context.
        /// </summary>
        /// <value>
        /// The preference context.
        /// </value>
        [JsonProperty(PropertyName = "pre_context")]
        public List<string> PreContext { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        [JsonProperty(PropertyName = "context_line")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>
        /// The vars.
        /// </value>
        [JsonProperty(PropertyName = "vars")]
        public Dictionary<string, string> Vars { get; set; }
    }
}