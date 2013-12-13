using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SharpRaven.Utilities {
    /// <summary>
    /// Utility class for retreiving system information.
    /// </summary>
    public static class SystemUtil {
        /// <summary>
        /// Return all loaded modules.
        /// </summary>
        /// <returns>
        /// All loaded modules.
        /// </returns>
        public static Module[] GetModules() {
            // Get primary module.
            Assembly curAssembly = Assembly.GetExecutingAssembly();
            // Return all module names.
            return curAssembly.GetModules();
        }
    }
}
