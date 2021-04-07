using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    public class ReplaceIn : ProgramNode
    {
        /// <summary>
        /// The text to find
        /// </summary>
        String findText = "";
        String replaceText = "";

        String startText = "", endText = "";

        public override string calculate(string input)
        {
            String rtrn = "";

            String[] splitLines = input.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitLines.Length; i++)
            {
                String line = splitLines[i];

                if (line.Contains(findText) && line.Contains(startText) && line.Contains(endText))
                {
                    //may need to loop until all replacements
                    
                    String replacedLine = "";

                    String lineProgress = line;
                    int startOffset = 0 ,endIndex = 0 , offset = 0;

                    while (lineProgress.Contains(startText) && lineProgress.Contains(endText))
                    {
                        int index = lineProgress.IndexOf(startText) + startOffset;
                        lineProgress = line.Substring(index + startText.Length);
                        
                        offset = index + startText.Length;
                        endIndex = lineProgress.IndexOf(endText) + offset;

                        String internalLine = line.Substring(offset,endIndex-offset);

                        internalLine = internalLine.Replace(findText, replaceText);
                        replacedLine += line.Substring(startOffset, offset-startOffset) + internalLine;

                        //prepare for next loop
                        startOffset = endIndex + endText.Length;
                        lineProgress = line.Substring(endIndex+endText.Length);
                    }

                    replacedLine += line.Substring(endIndex, line.Length - endIndex);

                    rtrn += replacedLine + Environment.NewLine;
                }
            }

            return rtrn;
        }

        public override ProgramNode createInstance()
        {
            return new ReplaceIn();
        }

        public override string getOpName()
        {
            return "ReplaceIn";
        }

        public override string createExample()
        {
            return getOpName() + "( FIND , REPLACE , START , END )";
        }

        public override string ToString()
        {
            return getOpName() + "( " + findText + " , " + replaceText + " , " + startText + " , " + endText + " )";
        }

        public override void parseArgs(string[] args)
        {
            findText = LogProcessor.specialCharacterReplacement(args[0].Trim());
            replaceText = LogProcessor.specialCharacterReplacement(args[1].Trim());
            startText = LogProcessor.specialCharacterReplacement(args[2].Trim());
            endText = LogProcessor.specialCharacterReplacement(args[3].Trim());
        }
    }
}