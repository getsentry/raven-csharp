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

using Nancy;
using Nancy.Bootstrapper;

namespace SharpRaven.Nancy
{
    /// <summary>
    /// SharpRaven's <see cref="IApplicationStartup"/> implementation.
    /// Used to register exception handling to the start of the error handling pipeline.
    /// </summary>
    public class ApplicationStartup : IApplicationStartup
    {
        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
            var value = Configuration.Settings.PipelineName.Value;
            var sharpRaven = new PipelineItem<Func<NancyContext, Exception, Response>>(value, (context, exception) =>
            {
                var nancyContextDataSlot = Configuration.Settings.NancyContextDataSlot;
                var localDataStoreSlot = Thread.GetNamedDataSlot(nancyContextDataSlot);
                Thread.SetData(localDataStoreSlot, context);

                if (Configuration.Settings.CaptureExceptionOnError.Value)
                {
                    // TODO: We should retrieve an IRavenClient instance from the application container. @asbjornu
                    IRavenClient client = new RavenClient(context);
                    client.CaptureException(exception);
                }

                return null;
            });

            pipelines.OnError.AddItemToStartOfPipeline(sharpRaven);
        }
    }
}