using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    class FilterNode : ProgramNode
    {

        String filterText;

        public override string calculate(string input)
        {
            String output = "";
            foreach (String line in input.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains(filterText))
                {
                    output += line + Environment.NewLine;
                }
                else
                {

                }

            }

            return output;
        }

        public override ProgramNode createInstance()
        {
            return new FilterNode();
        }

        public override string getOpName()
        {
            return "filter";
        }

        public override void parseArgs(string[] args)
        {
            this.filterText = args[0];
        }

        public override string ToString()
        {
            return getOpName() + "( " + this.filterText +" )";
        }
    }
}
