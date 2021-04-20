using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    /**
     * Replaces the Xth item, or FIRST / LAST item
    **/
    public class ReplaceCount : ProgramNode
    {
        /// <summary>
        /// The text to find
        /// </summary>
        String findText = "";
        String replaceText = "";
        /// <summary>
        /// how many lines to include after a find
        /// </summary>
        int countRequirement = 1;
        /// <summary>
        /// How to do the comparison (typically > or < or != or ==)
        /// </summary>
        String countType = COUNT_LAST;
        const String COUNT_LAST = "LAST";
        const String COUNT_FIRST = "FIRST";

        Boolean countIsNumber = false;

        public override string calculate(string input)
        {
            String rtrn = "";

            String[] splitLines = input.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries);

            for (int i=0;i<splitLines.Length;i++)
            {
                String line = splitLines[i];

                if (line.Contains(findText))
                {
                    //find all instances
                    int index;
                    string preReplace = "";
                    string textReplaced = "";
                    string replacedLine = "";
                    if (countType.Contains(COUNT_LAST)) {
                        index = line.LastIndexOf(findText);
                        textReplaced = line.Substring(index);
                        preReplace = textReplaced;
                        textReplaced = textReplaced.Replace(findText, replaceText);
                        replacedLine = line.Substring(0, index) + textReplaced;

                    } else if (countType.Contains(COUNT_FIRST))
                    {
                        index = line.IndexOf(findText);
                        textReplaced = line.Substring(0,index + findText.Length);
                        preReplace = textReplaced;
                        textReplaced = textReplaced.Replace(findText, replaceText);
                        replacedLine = textReplaced + line.Substring(index + findText.Length);

                    } else if (countIsNumber)
                    {
                        //TODO add functionality to find the Xth instance
                        replacedLine = "Replace Count Numerical Functionality not implemented yet.";

                    }

                    rtrn += replacedLine + Environment.NewLine;

                } else
                {
                    rtrn += line + Environment.NewLine;
                }
            }

            return rtrn;
        }

        public override ProgramNode createInstance()
        {
            return new ReplaceCount();
        }

        public override string getOpName()
        {
            return "ReplaceCount";
        }

        public override string createExample()
        {
            return getOpName() + "( FIND , REPLACE , LAST )";
        }

        public override string ToString()
        {
            return getOpName() + "( " + findText + " , " + replaceText + " , " + countType + " )";
        }

        public override void parseArgs(string[] args)
        {
            findText = loadString(args[0].Trim());
            replaceText = loadString(args[1].Trim());
            countType = loadString(args[2].Trim());
            if (countType.Any(char.IsDigit)) {
                countRequirement = loadInt(countType);
                countIsNumber = true;
            } else
            {
                countIsNumber = false;
            }
        }
    }
}
