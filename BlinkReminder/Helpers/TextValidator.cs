using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlinkReminder.Helpers
{
    static class TextValidator
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
