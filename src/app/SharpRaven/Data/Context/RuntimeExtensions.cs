namespace SharpRaven.Data.Context
{
    internal static class RuntimeExtensions
    {
        public static bool IsNetFx(this Runtime runtime) => runtime.IsRuntime(".NET Framework");
        public static bool IsNetCore(this Runtime runtime) => runtime.IsRuntime(".NET Core");

        private static bool IsRuntime(this Runtime runtime, string runtimeName)
        {
            return runtime?.Name?.StartsWith(runtimeName) == true
                   || runtime?.RawDescription?.StartsWith(runtimeName) == true;
        }
    }
}
