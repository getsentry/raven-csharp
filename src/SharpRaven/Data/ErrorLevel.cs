namespace SharpRaven.Data {
    /// <summary>
    /// Indicates the severity of the error.
    /// </summary>
    public enum ErrorLevel {
        /// <summary>
        /// The error is fatal and more severe than a captured exception or regular <see cref="Error"/>. Errors at this severity
        /// will show up as dark red in the Sentry Stream.
        /// </summary>
        Fatal,
        
        /// <summary>
        /// The error is of the same severity as a captured exception. Errors at this severity
        /// will show up as bright red in the Sentry Stream.
        /// </summary>
        Error,
        
        /// <summary>
        /// The error is less severe than an a regular <see cref="Error"/>. Errors at this severity will show
        /// up as orange in the Sentry Stream.
        /// </summary>
        Warning,
        
        /// <summary>
        /// The error is less severe than a <see cref="Warning"/> and is probably expected. Errors at this severity
        /// will show up as blue in the Sentry Stream.
        /// </summary>
        Info,

        /// <summary>
        /// The error is less even severe than an <see cref="Info"/> and is just captured for debug purposes. Errors
        /// at this severity will show up as grey in the Sentry Stream.
        /// </summary>
        Debug
    }
}
