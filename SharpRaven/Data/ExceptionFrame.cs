using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SharpRaven.Data {
    public class ExceptionFrame {
        [JsonProperty(PropertyName = "abs_path")]
        public string AbsolutePath;

        [JsonProperty(PropertyName = "filename")]
        public string Filename;

        [JsonProperty(PropertyName = "module")]
        public string Module;

        [JsonProperty(PropertyName = "function")]
        public string Function;

        [JsonProperty(PropertyName = "vars")]
        public Dictionary<string, string> Vars;

        [JsonProperty(PropertyName = "pre_context")]
        public List<string> PreContext;

        [JsonProperty(PropertyName = "context_line")]
        public string Source;

        [JsonProperty(PropertyName = "lineno")]
        public int LineNumber;

        [JsonProperty(PropertyName = "in_app")]
        public bool InApp;

        [JsonProperty(PropertyName = "post_context")]
        public List<string> PostContext;
    }
}