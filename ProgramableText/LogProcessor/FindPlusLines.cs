using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    public class FindPlusLines : ProgramNode
    {
        /// <summary>
        /// The text to find
        /// </summary>
        String findText = "";
        /// <summary>
        /// how many lines to include after a find
        /// </summary>
        int nextLineCount = 0;


        public override string calculate(string input)
        {
            String rtrn = "";
            int lineCountdown = 0;//how many lines remain
            String[] splitLines = input.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries);
            for (int i=0;i<splitLines.Length;i++)
            {
                String line = splitLines[i];
                if (line.Contains(findText))
                {
                    lineCountdown = nextLineCount;
                    rtrn += line + Environment.NewLine;
                } else if (lineCountdown >0)
                {
                    lineCountdown--;
                    rtrn += line + Environment.NewLine;
                }
            }

            return rtrn;
        }

        public override ProgramNode createInstance()
        {
            return new FindPlusLines();
        }

        public override string getOpName()
        {
            return "FindPlusLines";
        }

        public override string createExample()
        {
            return getOpName() + "( FIND , ADDITIONAL_LINES )";
        }

        public override string ToString()
        {
            return getOpName() + "( " + findText + " , " + nextLineCount + " )";
        }

        public override void parseArgs(string[] args)
        {
            findText = LogProcessor.specialCharacterReplacement(args[0].Trim());
            nextLineCount = loadInt(args[1]);
        }
    }
}
