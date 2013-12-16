using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

using Newtonsoft.Json;

namespace SharpRaven.Data {
    /// <summary>
    /// Represents Sentry's version of an <see cref="Exception"/>'s stack trace.
    /// </summary>
    public class SentryStacktrace {
        /// <summary>
        /// Initializes a new instance of the <see cref="SentryStacktrace"/> class.
        /// </summary>
        /// <param name="e">The decimal.</param>
        public SentryStacktrace(Exception e) {
            StackTrace trace = new StackTrace(e, true);

            if (trace.GetFrames() != null)
            {
                int length = trace.GetFrames().Length;
                this.Frames = new ExceptionFrame[length];
                for (int i=0; i<length; i++) {
                    StackFrame frame = trace.GetFrame(length - i - 1);
                    this.Frames[i] = BuildExceptionFrame(frame);
                }
            }
        }

        private static ExceptionFrame BuildExceptionFrame(StackFrame frame)
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
        }

        /// <summary>
        /// Gets or sets the <see cref="Exception"/> frames.
        /// </summary>
        /// <value>
        /// The <see cref="Exception"/> frames.
        /// </value>
        [JsonProperty(PropertyName = "frames")]
        public ExceptionFrame[] Frames { get; set; }

    }
}
