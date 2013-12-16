#region Disclaimer/Info

///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext@googlegroups.com 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Hosting;

namespace UnitTests.Subtext
{
    /// <summary>
    /// Used to simulate an HttpRequest.
    /// </summary>
    public class SimulatedHttpRequest : SimpleWorkerRequest
    {
        readonly string _host;
        readonly string _physicalFilePath;
        readonly int _port;
        Uri _referer;
        readonly string _verb;
        private readonly NameValueCollection _formVariables = new NameValueCollection();
        private readonly NameValueCollection _headers = new NameValueCollection();
        private readonly NameValueCollection _cookies = new NameValueCollection();


        /// <summary>
        /// Creates a new <see cref="SimulatedHttpRequest"/> instance.
        /// </summary>
        /// <param name="applicationPath">App virtual dir.</param>
        /// <param name="physicalAppPath">Physical Path to the app.</param>
        /// <param name="physicalFilePath">Physical Path to the file.</param>
        /// <param name="page">The Part of the URL after the application.</param>
        /// <param name="query">Query.</param>
        /// <param name="output">Output.</param>
        /// <param name="host">Host.</param>
        /// <param name="port">Port to request.</param>
        /// <param name="verb">The HTTP Verb to use.</param>
        public SimulatedHttpRequest(string applicationPath, string physicalAppPath, string physicalFilePath, string page,
                                    string query, TextWriter output, string host, int port, string verb)
            : base(applicationPath, physicalAppPath, page, query, output)
        {
            if (String.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException("host");
            }

            if (applicationPath == null)
            {
                throw new ArgumentNullException("applicationPath");
            }

            _host = host;
            _verb = verb;
            _port = port;
            _physicalFilePath = physicalFilePath;
        }

        public SimulatedHttpRequest(string applicationPath, string physicalAppPath, string page, string query)
            : this(applicationPath, physicalAppPath, @"c:\inetpub\" + page, page, query, new StringWriter(), "localhost", 80, "GET")
        {
        }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>The headers.</value>
        public NameValueCollection Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// Gets the cookies.
        /// </summary>
        /// <value>
        /// The cookies.
        /// </value>
        public NameValueCollection Cookies
        {
            get { return _cookies; }
        }

        /// <summary>
        /// Gets the format exception.
        /// </summary>
        /// <value>The format exception.</value>
        public NameValueCollection Form
        {
            get { return _formVariables; }
        }

        internal void SetReferer(Uri referer)
        {
            _referer = referer;
        }

        /// <summary>
        /// Returns the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The HTTP verb returned in the request
        /// header.
        /// </returns>
        public override string GetHttpVerbName()
        {
            return _verb;
        }

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        /// <returns></returns>
        public override string GetServerName()
        {
            return _host;
        }

        public override int GetLocalPort()
        {
            return _port;
        }

        /// <summary>
        /// Get all nonstandard HTTP header name-value pairs.
        /// </summary>
        /// <returns>An array of header name-value pairs.</returns>
        public override string[][] GetUnknownRequestHeaders()
        {
            if (_headers == null || _headers.Count == 0)
            {
                return null;
            }
            var headersArray = new string[_headers.Count][];
            for (int i = 0; i < _headers.Count; i++)
            {
                headersArray[i] = new string[2];
                headersArray[i][0] = _headers.Keys[i];
                headersArray[i][1] = _headers[i];
            }
            return headersArray;
        }

        public override string GetKnownRequestHeader(int index)
        {
            var requestHeader = (HttpRequestHeader)index;

            switch (requestHeader)
            {
                case HttpRequestHeader.Referer:
                    return _referer == null ? string.Empty : _referer.ToString();

                case HttpRequestHeader.ContentType:
                    if (_verb == "POST")
                    {
                        return "application/x-www-form-urlencoded";
                    }
                    break;

                case HttpRequestHeader.Cookie:
                    return Convert(_cookies);
            }

            return base.GetKnownRequestHeader(index);
        }

        /// <summary>
        /// Returns the virtual path to the currently executing
        /// server application.
        /// </summary>
        /// <returns>
        /// The virtual path of the current application.
        /// </returns>
        public override string GetAppPath()
        {
            string appPath = base.GetAppPath();
            return appPath;
        }

        public override string GetAppPathTranslated()
        {
            string path = base.GetAppPathTranslated();
            return path;
        }

        public override string GetUriPath()
        {
            string uriPath = base.GetUriPath();
            return uriPath;
        }

        public override string GetFilePathTranslated()
        {
            return _physicalFilePath;
        }

        public override string GetFilePath()
        {
            return CurrentExecutionPath ?? base.GetFilePath();
        }

        public override string GetPathInfo()
        {
            return "/";
        }

        public string CurrentExecutionPath
        {
            get;
            set;
        }

        
        /// <summary>
        /// Reads request data from the client (when not preloaded).
        /// </summary>
        /// <returns>The number of bytes read.</returns>
        public override byte[] GetPreloadedEntityBody()
        {
            string formText = Convert(_formVariables);
            return Encoding.UTF8.GetBytes(formText);
        }

        private static string Convert(NameValueCollection collection)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string key in collection.Keys)
            {
                sb.Append(String.Format(CultureInfo.InvariantCulture, "{0}={1}&", key, collection[key]));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a value indicating whether all request data
        /// is available and no further reads from the client are required.
        /// </summary>
        /// <returns>
        ///         <see langword="true"/> if all request data is available; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool IsEntireEntityBodyIsPreloaded()
        {
            return true;
        }
    }
}