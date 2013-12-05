namespace SharpRaven.Logging {
    /// <summary>
    /// Interface for providing a 'filter' for log scrubbers.
    /// Used primarily for the default filter, but can be used
    /// by others if needed.
    /// </summary>
    public interface IFilter {
        /// <summary>
        /// Filters the specified input and returns the filtered <see cref="string"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// The filtered <see cref="string"/>.
        /// </returns>
        string Filter(string input);
    }
}
