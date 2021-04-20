using ProgramableText.LogProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.BlockProcessor
{
    class FunctionalBlock : BlockNode
    {
        const String OP_NAME = "Functional Block";
        const String FUNCTION_NAME = "Function Name";
        const String FUNCTION_NODES = "Function Nodes";

        String functionName, nodeText;

        List<ProgramNodeInterface> nodes;
        Boolean run = false;

        public override string calculate(string input)
        {
            if (run)
            {
                if (nodes.Count >= 1)
                {
                    LogProcessor.LogProcessor.process(nodes, false);
                    return LogProcessor.LogProcessor.output;
                }
                else
                {
                    return input;
                }
            } else
            {
                //Ignore the first run, this is just loading this into the file
                LogProcessor.LogProcessor.functionalBlocks[functionName] =  this;
                run = true;
                return input;
            }
        }

        public override string createExample()
        {
            List<String> args = new List<string>();
            args.Add(FUNCTION_NAME);
            args.Add(FUNCTION_NODES);

            return generateExample(args);
        }

        public override BlockNode createInstance()
        {
            return new FunctionalBlock();
        }

        public override string getOpName()
        {
            return OP_NAME;
        }

        public override void parseBlocks(string input)
        {
            functionName = getInternalBlockText(FUNCTION_NAME, input);
            functionName = functionName.Trim();

            nodeText = getInternalBlockText(FUNCTION_NODES, input);
            LogProcessor.LogProcessor.compileProgram(nodeText, out nodes);

            LogProcessor.LogProcessor.functionalBlocks[functionName] = this;
            run = false;
        }

        public override string ToString()
        {
            return getOpName();
        }
    }
}
