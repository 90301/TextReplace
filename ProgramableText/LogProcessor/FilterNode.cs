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
            if (input.Contains(filterText))
            {
                return input;
            } else
            {
                return null;
            }
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
