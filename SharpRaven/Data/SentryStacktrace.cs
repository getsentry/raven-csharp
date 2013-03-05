using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace SharpRaven.Data {
    public class SentryStacktrace {
        public SentryStacktrace(Exception e) {
            StackTrace trace = new StackTrace(e);

            this.Frames = trace.GetFrames().Reverse().Select(frame =>
            {
                int lineNo = frame.GetFileLineNumber();

                if (lineNo == 0)
                {
                    //The pdb files aren't currently available
                    lineNo = frame.GetILOffset();
                }

                return new ExceptionFrame()
                {
                    Filename = frame.GetFileName(),
                    Module = frame.GetMethod().DeclaringType.FullName,
                    Function = frame.GetMethod().Name,
                    Source = frame.GetMethod().ToString(),
                    LineNumber = lineNo,
                    ColumnNumber = frame.GetFileColumnNumber()
                };
            }).ToList();
        }

        [JsonProperty(PropertyName = "frames")]
        public List<ExceptionFrame> Frames;

    }
}