using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    class ExtraPassNow : ProgramNode
    {
        public override string calculate(string input)
        {
            return LogProcessor.processExtraPass(input);
        }

        public override ProgramNode createInstance()
        {
            return new ExtraPassNow();
        }

        public override string getOpName()
        {
            return "extraPassNow";
        }

        public override void parseArgs(string[] args)
        {

        }

        public override string ToString()
        {
            return getOpName() + "()";
        }
    }
}
