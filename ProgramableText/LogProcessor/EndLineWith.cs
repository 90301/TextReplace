using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    public class EndLineWith : ProgramNode
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

                rtrn += line + startText +  Environment.NewLine;
            }

            return rtrn;
        }

        public override ProgramNode createInstance()
        {
            return new EndLineWith();
        }

        public override string getOpName()
        {
            return "EndLineWith";
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
            startText = loadString(args[0].Trim());
        }
    }
}
