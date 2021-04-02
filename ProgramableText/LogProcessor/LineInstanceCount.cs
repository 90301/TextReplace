using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    public class LineInstanceCount : ProgramNode
    {
        /// <summary>
        /// The text to find
        /// </summary>
        String findText = "";
        /// <summary>
        /// how many lines to include after a find
        /// </summary>
        int countRequirement = 1;
        /// <summary>
        /// How to do the comparison (typically > or < or != or ==)
        /// </summary>
        string compareSymbol = ">";
        /// <summary>
        /// Output Style - how it returns the line
        /// line - return the entire unaltered line (acts as a filter)
        /// count - returns the count of how many instances were found
        /// </summary>
        string outputStyle = "line";

        const string OUTPUT_STYLE_LINE = "line";
        const string OUTPUT_STYLE_COUNT = "count";

        public override string calculate(string input)
        {
            String rtrn = "";

            String[] splitLines = input.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries);
            for (int i=0;i<splitLines.Length;i++)
            {
                String line = splitLines[i];

                if (line.Contains(findText))
                {
                    int count = (line.Length - line.Replace(findText, "").Length) / findText.Length;
                    if ((count > countRequirement && compareSymbol == ">") 
                        || (count < countRequirement && compareSymbol == "<")
                        || (count != countRequirement && compareSymbol == "!=")
                        || (count == countRequirement && compareSymbol == "=="))
                    {
                        if (outputStyle.Contains(OUTPUT_STYLE_LINE))
                        {
                            rtrn += line + Environment.NewLine;
                        } else if (outputStyle.Contains(OUTPUT_STYLE_COUNT))
                        {
                            rtrn += count + Environment.NewLine;
                        }
                    }

                   
                }
            }

            return rtrn;
        }

        public override ProgramNode createInstance()
        {
            return new LineInstanceCount();
        }

        public override string getOpName()
        {
            return "LineInstanceCount";
        }

        public override string createExample()
        {
            return getOpName() + "( FIND , COUNT , > , line )";
        }

        public override string ToString()
        {
            return getOpName() + "( " + findText + " , " + countRequirement + " , " + compareSymbol + " , " + outputStyle + " )";
        }

        public override void parseArgs(string[] args)
        {
            findText = LogProcessor.specialCharacterReplacement(args[0].Trim());
            countRequirement = loadInt(args[1]);
            compareSymbol = LogProcessor.specialCharacterReplacement(args[2].Trim());
            outputStyle = LogProcessor.specialCharacterReplacement(args[3].Trim()).ToLower();
        }
    }
}
