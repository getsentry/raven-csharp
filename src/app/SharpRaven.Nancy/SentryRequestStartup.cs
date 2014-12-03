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
using System.Threading;
using System.Runtime.Remoting.Messaging;

using Nancy;
using Nancy.Bootstrapper;

namespace SharpRaven.Nancy
{
    using PipelineItem = PipelineItem<Func<NancyContext, Exception, Response>>;

    /// <summary>
    /// SharpRaven's <see cref="IRequestStartup"/> implementation.
    /// Used to register exception handling to the start of the error handling pipeline.
    /// </summary>
    public class SentryRequestStartup : IRequestStartup
    {
        private readonly IRavenClient ravenClient;


        /// <summary>
        /// Initializes a new instance of the <see cref="SentryRequestStartup"/> class.
        /// </summary>
        /// <param name="ravenClient">The raven client.</param>
        /// <exception cref="System.ArgumentNullException">ravenClient</exception>
        public SentryRequestStartup(IRavenClient ravenClient)
        {
            if (ravenClient == null)
                throw new ArgumentNullException("ravenClient");

            this.ravenClient = ravenClient;
        }


        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        /// <param name="context">The current context</param>
        public void Initialize(IPipelines pipelines, NancyContext context)
        {
            // on each request store the NancyContext to the LogicalCallContext
            var nancyContextDataSlot = NancyConfiguration.Settings.NancyContextDataSlot;
            CallContext.LogicalSetData(nancyContextDataSlot, context);

            var name = NancyConfiguration.Settings.PipelineName.Value;
            var sharpRaven = new PipelineItem(name, (nancyContext, exception) =>
            {
                if (NancyConfiguration.Settings.CaptureExceptionOnError.Value)
                {
                    var guid = this.ravenClient.CaptureException(exception);

                    if (guid != null)
                    {
                        context.Items.Add(NancyConfiguration.Settings.SentryEventGuid, guid);
                        exception.Data.Add(NancyConfiguration.Settings.SentryEventGuid, guid);
                    }
                }

                return null;
            });

            pipelines.OnError.AddItemToStartOfPipeline(sharpRaven);
        }
    }
}