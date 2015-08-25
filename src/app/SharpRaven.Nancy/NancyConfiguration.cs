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
using System.Configuration;

namespace SharpRaven.Nancy
{
    /// <summary>
    /// Nancy-specific configuration for SharpRaven.
    /// </summary>
    public class NancyConfiguration : Configuration
    {
        private const string CaptureExceptionOnErrorKey = "captureExceptionOnError";
        private const string PipelineNameKey = "pipelineName";

        private static readonly NancyConfiguration settings =
            ConfigurationManager.GetSection("sharpRaven") as NancyConfiguration;

        /// <summary>
        /// Gets the &lt;captureExceptionOnError /&gt; configuration element.
        /// </summary>
        /// <value>
        /// The &lt;captureExceptionOnError /&gt; configuration element.
        /// </value>
        [ConfigurationProperty(CaptureExceptionOnErrorKey, IsKey = true)]
        public CaptureExceptionOnErrorElement CaptureExceptionOnError
        {
            get { return (CaptureExceptionOnErrorElement)base[CaptureExceptionOnErrorKey]; }
        }

        /// <summary>
        /// Gets the nancy context data slot.
        /// </summary>
        /// <value>
        /// The nancy context data slot.
        /// </value>
        public string NancyContextDataSlot
        {
            get { return "SharpRaven.Nancy.NancyContext"; }
        }

        /// <summary>
        /// Gets the &lt;pipelineName /&gt; configuration element.
        /// </summary>
        /// <value>
        /// The &lt;pipelineName /&gt; configuration element.
        /// </value>
        [ConfigurationProperty(PipelineNameKey, IsKey = true)]
        public PipelineNameElement PipelineName
        {
            get { return (PipelineNameElement)base[PipelineNameKey]; }
        }

        /// <summary>
        /// Gets the key for the GUID returned from Sentry after a successfully logged exception.
        /// </summary>
        /// <value>
        /// The key for the GUID returned from Sentry after a successfully logged exception.
        /// </value>
        public string SentryEventGuid
        {
            get { return "SharpRaven.Nancy.SentryEventGuid"; }
        }

        /// <summary>
        /// Gets the &lt;sharpRaven/&gt; configuration element.
        /// </summary>
        /// <value>
        /// The the &lt;sharpRaven/&gt; configuration element.
        /// </value>
        public new static NancyConfiguration Settings
        {
            get { return settings; }
        }

        #region Nested type: CaptureExceptionOnErrorElement

        /// <summary>
        /// The &lt;captureExceptionOnError /&gt; configuration element.
        /// </summary>
        public class CaptureExceptionOnErrorElement : ConfigurationElement
        {
            /// <summary>
            /// Gets or sets the value of the the &lt;captureExceptionOnError /&gt; configuration element.
            /// </summary>
            /// <value>
            ///   <c>true</c> if exceptions should be captured by SharpRaven; otherwise, <c>false</c>.
            /// </value>
            [ConfigurationProperty("value", DefaultValue = "true")]
            public bool Value
            {
                get { return (bool)this["value"]; }
                set { this["value"] = value; }
            }
        }

        #endregion

        #region Nested type: PipelineNameElement

        /// <summary>
        /// The &lt;pipelineName /&gt; configuration element.
        /// </summary>
        public class PipelineNameElement : ConfigurationElement
        {
            /// <summary>
            /// Gets or sets the value of the &lt;pipelineName /&gt; configuration element.
            /// </summary>
            /// <value>
            /// The value of the the &lt;pipelineName /&gt; configuration element.
            /// </value>
            [ConfigurationProperty("value", DefaultValue = "SharpRaven.Nancy")]
            public String Value
            {
                get { return (string)this["value"]; }
                set { this["value"] = value; }
            }
        }

        #endregion
    }
}