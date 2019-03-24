using System.Text.RegularExpressions;

namespace BlinkReminder.Helpers
{
    internal static class TextValidator
    {
        private static readonly string onlyNums = "[0-9]";

        /// <summary>
        /// Checks whether the passed text composes only of numbers
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNumsOnly(string text)
        {
            return Regex.IsMatch(text, onlyNums);
        }
    }
}
