using System;
using System.Diagnostics;
using System.Reflection;

namespace SharpRaven.Utilities
{
    internal static class EntryAssemblyNameLocator
    {
        private static AssemblyName entryAssemblyName;
        private static bool entryAssemblyNameLoaded;

        /// <summary>
        /// Retrieves the <see cref="AssemblyName"/> of the Entry Assembly
        /// </summary>
        /// <remarks>The result of the first lookup is cached and returned on subsequent calls</remarks>
        /// <returns></returns>
        public static AssemblyName GetAssemblyName()
        {
            if (entryAssemblyNameLoaded)
            {
                return entryAssemblyName;
            }

            try
            {
                entryAssemblyName =
                    // Fastest/Simpler API but returns null when entry is unmanaged code
                    Assembly.GetEntryAssembly()?.GetName()
                    ?? GetAssemblyNameFromEntryMethod();
            }
            catch (Exception e)
            {
                SystemUtil.WriteError(e);
            }
            finally
            {
                entryAssemblyNameLoaded = true;
            }

            return entryAssemblyName;
        }

        internal static AssemblyName GetAssemblyNameFromEntryMethod()
        {
            var entryMethod = GetApplicationEntryMethod();
            return entryMethod?.Module?.Assembly?.GetName();
        }

        internal static MethodBase GetApplicationEntryMethod()
        {
            var frames = new StackTrace().GetFrames();
            MethodBase entryMethod = null;

            for (var i = 0; i < frames.Length; i++)
            {
                var stackFrame = frames[i];
                var method = stackFrame.GetMethod() as MethodInfo;
                if (!method?.IsStatic == null)
                {
                    continue;
                }

                if (method.Name == "Main"
                    && (method.ReturnType == typeof(int)
                        || method.ReturnType == typeof(void)))
                {
                    entryMethod = method;
                    break;
                }

                if (method.Name == "InvokeMethod" && method.DeclaringType == typeof(RuntimeMethodHandle)
                    || method.Name == "InternalInvoke" && method.DeclaringType?.Name == "MonoMethod")
                {
                    entryMethod = i == 0
                        ? method
                        : frames[i - 1].GetMethod();

                    break;
                }
            }

            return entryMethod;
        }

    }
}
