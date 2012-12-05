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
            this.Frames = new List<ExceptionFrame>();

            string[] formattedStackTrace = e.ToString().Split('\n');

            for (int i = 0; i < trace.FrameCount; i++) {
                StackFrame frame = trace.GetFrame(i);

                Frames.Add(new ExceptionFrame() {
                    Filename = frame.GetMethod().Name,
                    Function = frame.GetMethod().DeclaringType.FullName,
                    Source = formattedStackTrace[i + 1],
                    LineNumber = frame.GetILOffset()
                });

            }
        }

        [JsonProperty(PropertyName = "frames")]
        public List<ExceptionFrame> Frames;

    }
}