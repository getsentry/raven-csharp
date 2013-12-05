using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SharpRaven.Data {
    public class ExceptionFrame {
        [JsonProperty(PropertyName = "abs_path")]
        public string AbsolutePath { get; set; }

        [JsonProperty(PropertyName = "filename")]
        public string Filename { get; set; }

        [JsonProperty(PropertyName = "module")]
        public string Module { get; set; }

        [JsonProperty(PropertyName = "function")]
        public string Function { get; set; }

        [JsonProperty(PropertyName = "vars")]
        public Dictionary<string, string> Vars { get; set; }

        [JsonProperty(PropertyName = "pre_context")]
        public List<string> PreContext { get; set; }

        [JsonProperty(PropertyName = "context_line")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "lineno")]
        public int LineNumber { get; set; }

        [JsonProperty(PropertyName = "colno")]
        public int ColumnNumber { get; set; }

        [JsonProperty(PropertyName = "in_app")]
        public bool InApp { get; set; }

        [JsonProperty(PropertyName = "post_context")]
        public List<string> PostContext { get; set; }
    }
}