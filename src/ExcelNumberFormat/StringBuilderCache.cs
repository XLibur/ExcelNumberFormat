using System.Text;

namespace ExcelNumberFormat
{
    /// <summary>
    /// Reuses one <see cref="StringBuilder"/> per thread, so formatting a value allocates the result string and not a builder.
    /// A nested call finds the cache empty and gets a new builder, so re-entrancy is safe.
    /// </summary>
    internal static class StringBuilderCache
    {
        private const int MaxCachedCapacity = 360;

        [System.ThreadStatic]
        private static StringBuilder cached;

        public static StringBuilder Acquire()
        {
            var builder = cached;
            if (builder == null)
                return new StringBuilder(64);

            cached = null;
            return builder.Clear();
        }

        public static string GetStringAndRelease(StringBuilder builder)
        {
            var result = builder.ToString();
            if (builder.Capacity <= MaxCachedCapacity)
                cached = builder;

            return result;
        }
    }
}
