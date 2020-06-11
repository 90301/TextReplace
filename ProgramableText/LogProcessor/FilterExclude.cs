using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    class FilterExclude : ProgramNode
    {

        String[] filterText;

        public override string calculate(string input)
        {
            String output = "";
            foreach (String line in input.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!filterText.Any(x => line.ToLower().Contains(x.ToLower())))
                {
                    output += line + Environment.NewLine;
                }

            }

            return output;
        }

        public override ProgramNode createInstance()
        {
            return new FilterExclude();
        }

        public override string getOpName()
        {
            return "filterExclude";
        }

        public override void parseArgs(string[] args)
        {
            this.filterText = args;
            for (int i=0;i<this.filterText.Length;i++)
            {
                this.filterText[i] = LogProcessor.specialCharacterReplacement(this.filterText[i]);
            }
        }

        public override string ToString()
        {
            String argsToString;
            if (filterText != null && filterText.Length >= 1)
            {
                argsToString = this.filterText.Select(x => x.ToLower()).Aggregate((x, y) => (x + "," + y));
            } else
            {
                argsToString = "";
            }
            return getOpName() + "( " + argsToString + " )";
        }
    }
}
