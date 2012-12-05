using System;
using SharpRaven.Data;

namespace SharpRaven.Logging {
    /// <summary>
    /// Interface for providing a 'filter' for log scrubbers.
    /// Used primarily for the default filter, but can be used
    /// by others if needed.
    /// </summary>
    public interface IFilter {
        string Filter(string input);
    }
}
