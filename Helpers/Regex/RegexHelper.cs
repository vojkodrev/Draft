using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Helpers.Regex
{
    public class RegexHelper
    {
        public static MatchCollection Match(string pattern, string input)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, RegexOptions.Multiline);
            return regex.Matches(input);
        }
    }
}
