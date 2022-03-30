using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProgramableText.Utils
{
    public static class TextUtils
    {
        public static String removeMultiSpaces(String str)
        {
            String rtrn = str;
            int lengthStart = 1;
            int lengthEnd = 0;
            while (lengthEnd < lengthStart)
            {
                lengthStart = rtrn.Length;
                rtrn = rtrn.Replace("  ", " ");
                lengthEnd = rtrn.Length;

            }
            return rtrn;
        }

        public static String removeHtmlTags(String astrInput)
        {
            return Regex.Replace(astrInput, @"<[^>]*>", String.Empty);
        }

        public static String[] splitOnNewLine(string astrInput)
        {
            return astrInput.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static String[] splitOnTab(string astrInput)
        {
            return astrInput.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static String[] splitOnComma(string astrInput)
        {
            return astrInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}
