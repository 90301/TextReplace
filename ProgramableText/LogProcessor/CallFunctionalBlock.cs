using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    class CallFunctionBlock : ProgramNode
    {
        String functionName = "";
        public override string calculate(string input)
        {
            if (LogProcessor.functionalBlocks.ContainsKey(functionName))
            {
                return LogProcessor.functionalBlocks[functionName].calculate(input);
            } else
            {
                String allFunctionsLoaded = LogProcessor.functionalBlocks.Keys.Aggregate((x, y) => x + System.Environment.NewLine + y);
                //output an error
                return "Function: '" + functionName + "' was not found. Here are all the loaded functions:" + System.Environment.NewLine + allFunctionsLoaded; 
            }
        }

        public override ProgramNode createInstance()
        {
            return new CallFunctionBlock();
        }

        public override string getOpName()
        {
            return "CallFunctionBlock";
        }

        public override string createExample()
        {
            return getOpName() + "( FUNCTION )";
        }

        public override void parseArgs(string[] args)
        {
            functionName = loadString(args[0].Trim());
        }

        public override string ToString()
        {
            return getOpName() + "(" + functionName + ")";
        }
    }
}
