namespace Api.Core.Utils
{
    public static class StringExtensions
    {
        public static bool HasNoValue(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool HasValue(this string value)
        {
            return !value.HasNoValue();
        }
    }
}
