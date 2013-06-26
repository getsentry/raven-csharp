using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SharpRaven.Utilities {
    public static class SystemUtil {
        /// <summary>
        /// Return all loaded modules.
        /// </summary>
        /// <returns></returns>
        public static Module[] GetModules() {
            // Get primary module.
            Assembly curAssembly = Assembly.GetExecutingAssembly();
            // Return all module names.
            return curAssembly.GetModules();
        }
    }
}
