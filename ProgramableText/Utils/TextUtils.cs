using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.Utils
{
    public class TextUtils
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
    }
}
