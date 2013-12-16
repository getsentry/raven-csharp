using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SharpRaven.Data {
    /// <summary>
    /// Represents Sentry's version of <see cref="System.Diagnostics.StackFrame" />.
    /// </summary>
    public class ExceptionFrame {
        /// <summary>
        /// Gets or sets the absolute path.
        /// </summary>
        /// <value>
        /// The absolute path.
        /// </value>
        [JsonProperty(PropertyName = "abs_path")]
        public string AbsolutePath { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        [JsonProperty(PropertyName = "filename")]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the module.
        /// </summary>
        /// <value>
        /// The module.
        /// </value>
        [JsonProperty(PropertyName = "module")]
        public string Module { get; set; }

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        /// <value>
        /// The function.
        /// </value>
        [JsonProperty(PropertyName = "function")]
        public string Function { get; set; }

        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>
        /// The vars.
        /// </value>
        [JsonProperty(PropertyName = "vars")]
        public Dictionary<string, string> Vars { get; set; }

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
        /// Gets or sets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        [JsonProperty(PropertyName = "lineno")]
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the column number.
        /// </summary>
        /// <value>
        /// The column number.
        /// </value>
        [JsonProperty(PropertyName = "colno")]
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [information application].
        /// </summary>
        /// <value>
        /// <c>true</c> if [information application]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(PropertyName = "in_app")]
        public bool InApp { get; set; }

        /// <summary>
        /// Gets or sets the post context.
        /// </summary>
        /// <value>
        /// The post context.
        /// </value>
        [JsonProperty(PropertyName = "post_context")]
        public List<string> PostContext { get; set; }
    }
}