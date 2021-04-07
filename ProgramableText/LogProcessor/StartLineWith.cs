using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    public class StartLineWith : ProgramNode
    {
        /// <summary>
        /// The text to start each line
        /// </summary>
        String startText = "";
        //TODO add min length check?

        public override string calculate(string input)
        {
            String rtrn = "";

            String[] splitLines = input.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries);
            for (int i=0;i<splitLines.Length;i++)
            {
                String line = splitLines[i];

                rtrn += startText + line + Environment.NewLine;
            }

            return rtrn;
        }

        public override ProgramNode createInstance()
        {
            return new StartLineWith();
        }

        public override string getOpName()
        {
            return "StartLineWith";
        }

        public override string createExample()
        {
            return getOpName() + "( TEXT )";
        }

        public override string ToString()
        {
            return getOpName() + "( " + startText + " )";
        }

        public override void parseArgs(string[] args)
        {
            startText = LogProcessor.specialCharacterReplacement(args[0].Trim());
        }
    }
}
