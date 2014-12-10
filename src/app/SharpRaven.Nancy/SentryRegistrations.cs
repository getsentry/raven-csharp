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

using System.Collections.Generic;

using Nancy;
using Nancy.Bootstrapper;

using SharpRaven.Data;
using SharpRaven.Nancy.Data;

namespace SharpRaven.Nancy
{
    /// <summary>
    /// SharpRaven-specific implementation of <see cref="IRegistrations"/> to expose a different
    /// default implementation of <see cref="IJsonPacketFactory"/> so <see cref="NancyContext"/>
    /// specific data can be sent to Sentry.
    /// </summary>
    public class SentryRegistrations : IRegistrations
    {
        /// <summary>
        /// Gets the collection registrations to register for this startup task
        /// </summary>
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the instance registrations to register for this startup task
        /// </summary>
        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get
            {
                return new[]
                {
                    //new InstanceRegistration(typeof(Dsn), new Dsn(Configuration.Settings.Dsn.Value)),
                    // TODO: I don't like to register the concrete instance like this, see the above TODO. @asbjornu
                    new InstanceRegistration(typeof(IRavenClient),
                                             new RavenClient(new Dsn(NancyConfiguration.Settings.Dsn.Value),
                                                             new NancyContextJsonPacketFactory())),
                };
            }
        }

        /// <summary>
        /// Gets the type registrations to register for this startup task
        /// </summary>
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                return new[]
                {
                    new TypeRegistration(typeof(IJsonPacketFactory),
                                         typeof(NancyContextJsonPacketFactory),
                                         Lifetime.Singleton),
                    // TODO: I'd like to register the RavenClient like this so it's more composable and IoC friendly, but I'm seemingly unable to override their registration, which makes testing impossible. @asbjornu
                    /*new TypeRegistration(typeof(IRavenClient),
                                         typeof(RavenClient),
                                         Lifetime.PerRequest),*/
                };
            }
        }
    }
}