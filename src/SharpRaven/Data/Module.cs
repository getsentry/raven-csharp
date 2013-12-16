namespace SharpRaven.Data
{
    /// <summary>
    /// A module or library that might be in use and thus could relevant for debugging purposes.
    /// </summary>
    public class Module
    {
        /// <summary>
        /// Gets or sets the name of the module (or library).
        /// </summary>
        /// <value>
        /// The name of the module (or library).
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version of the module (or library).
        /// </summary>
        /// <value>
        /// The version of the module (or library).
        /// </value>
        public string Version { get; set; }
    }
}