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
using System.Security.Principal;

using SharpRaven.Utilities;

namespace SharpRaven.Data
{
    /// <summary>
    /// A default implementation of <see cref="ISentryUserFactory"/>. Override the <see cref="OnCreate"/>
    /// method to adjust the values of the <see cref="SentryUser"/> before it is sent to Sentry.
    /// </summary>
    public class SentryUserFactory : ISentryUserFactory
    {
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns>
        /// If an HTTP context is available, an instance of <see cref="SentryUser"/>, otherwise <c>null</c>.
        /// </returns>
        public SentryUser Create()
        {
            SentryUser user;
            if (!SentryRequestFactory.HasHttpContext)
                user = new SentryUser(Environment.UserName);
            else
            {
                user = new SentryUser(GetPrincipal())
                {
                    IpAddress = GetIpAddress()
                };
            }

            return OnCreate(user);
        }


        /// <summary>
        /// Called when the <see cref="SentryUser"/> has been created. Can be overridden to
        /// adjust the values of the <paramref name="user"/> before it is sent to Sentry.
        /// </summary>
        /// <param name="user">The user information.</param>
        /// <returns>
        /// The <see cref="SentryUser"/>.
        /// </returns>
        protected virtual SentryUser OnCreate(SentryUser user)
        {
            return user;
        }

#if net35
        private static string GetIpAddress()
#else
        private static dynamic GetIpAddress()
#endif
        {
            try
            {
                return SentryRequestFactory.HttpContext.Request.UserHostAddress;
            }
            catch (Exception exception)
            {
                SystemUtil.WriteError(exception);
            }

            return null;
        }


        private static IPrincipal GetPrincipal()
        {
            try
            {
                return SentryRequestFactory.HttpContext.User as IPrincipal;
            }
            catch (Exception exception)
            {
                SystemUtil.WriteError(exception);
            }

            return null;
        }
    }
}