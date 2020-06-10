using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    class GetFileName : ProgramNode
    {
        public const string GET_FILE_NAME = "GetFileName";

        public override string calculate(string input)
        {
            return LogProcessor.fileProcessing;
        }

        public override ProgramNode createInstance()
        {
            return new GetFileName();
        }

        public override string getOpName()
        {
            return GET_FILE_NAME;
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
