using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

using Newtonsoft.Json;

namespace SharpRaven.Data {
    public class SentryStacktrace {
        public SentryStacktrace(Exception e) {
            StackTrace trace = new StackTrace(e, true);
            List<ExceptionFrame> frames = new List<ExceptionFrame>();

            if (trace.GetFrames() != null)
            {
                foreach (StackFrame frame in trace.GetFrames()) {
                    int lineNo = frame.GetFileLineNumber();

                    if (lineNo == 0)
                    {
                        //The pdb files aren't currently available
                        lineNo = frame.GetILOffset();
                    }

                    var method = frame.GetMethod();
                    frames.Insert(0, new ExceptionFrame()
                    {
                        Filename = frame.GetFileName(),
                        Module = (method.DeclaringType != null) ? method.DeclaringType.FullName : null,
                        Function = method.Name,
                        Source = method.ToString(),
                        LineNumber = lineNo,
                        ColumnNumber = frame.GetFileColumnNumber()
                    });
                }
            }
            this.Frames = frames;
        }

        [JsonProperty(PropertyName = "frames")]
        public List<ExceptionFrame> Frames;

    }
}
