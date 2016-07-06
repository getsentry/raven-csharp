namespace SharpRaven.Data
{
    /// <summary>
    /// Indicates the descriptions of individual breadcrumb types, and what their data properties look like.
    /// </summary>
    public enum BreadcrumbsType
    {
        /// <summary>
        /// Describes a navigation breadcrumb. A navigation event can be a URL change in a web application, or a UI transition in a mobile or desktop application, etc.
        /// Its data property has the following sub-properties:
        /// from
        /// A string representing the original application state / location.
        /// to
        /// A string representing the new application state / location.
        /// </summary>
        Navigation,

        /// <summary>
        /// Describes an HTTP request breadcrumb. This represents an HTTP request transmitted from your application. This could be an AJAX request from a web application, or a server-to-server HTTP request to an API service provider, etc.
        /// 
        /// Its data property has the following sub-properties:
        /// 
        /// url
        /// The request URL.
        /// method
        /// The HTTP request method.
        /// status_code
        /// The HTTP status code of the response.
        /// reason
        /// A text that describes the status code.
        /// </summary>
        Http
    }
}