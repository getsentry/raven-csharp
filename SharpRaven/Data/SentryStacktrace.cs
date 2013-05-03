using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using Newtonsoft.Json;

namespace SharpRaven.Data {
    public class SentryStacktrace {
        public SentryStacktrace(Exception e) {
            StackTrace trace = new StackTrace(e, true);

            Frames = (trace.GetFrames() ?? new StackFrame[0]).Reverse().Select(frame =>
            {
                int lineNo = frame.GetFileLineNumber();

                if (lineNo == 0)
                {
                    //The pdb files aren't currently available
                    lineNo = frame.GetILOffset();
                }

                var method = frame.GetMethod();
                return new ExceptionFrame()
                {
                    Filename = frame.GetFileName(),
                    Module = (method.DeclaringType != null) ? method.DeclaringType.FullName : null,
                    Function = method.Name,
                    Source = method.ToString(),
                    LineNumber = lineNo,
                    ColumnNumber = frame.GetFileColumnNumber()
                };
            }).ToList();
        }

        [JsonProperty(PropertyName = "frames")]
        public List<ExceptionFrame> Frames;

    }
}
