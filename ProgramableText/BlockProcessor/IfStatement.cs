using ProgramableText.BlockProcessor.Conditions;
using ProgramableText.LogProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.BlockProcessor
{
    class IfStatement : BlockNode
    {
        const String OP_NAME = "If Statement";
        const String CONDITION = "If";
        const String THEN = "Then";
        const String ELSE = "Else";

        String condCondition, condThen, condElse;

        Condition condition;
        List<ProgramNodeInterface> nodesThen,nodesElse;

        public override string calculate(string input)
        {
            if (condition.calculate(input))
            {
                if (nodesThen.Count >= 1)
                {
                    LogProcessor.LogProcessor.process(nodesThen, false , input);
                    return LogProcessor.LogProcessor.output;
                } else
                {
                    return input;
                }
            } else
            {
                if (nodesElse.Count >= 1)
                {
                    LogProcessor.LogProcessor.process(nodesElse, false, input);
                    return LogProcessor.LogProcessor.output;
                } else
                {
                    return input;
                }
            }
            
        }

        public override string createExample()
        {
            List<String> args = new List<string>();
            args.Add(CONDITION);
            args.Add(THEN);
            args.Add(ELSE);

            return generateExample(args);
        }

        public override BlockNode createInstance()
        {
            return new IfStatement();
        }

        public override string getOpName()
        {
            return OP_NAME;
        }

        public override void parseBlocks(string input)
        {
            condCondition = getInternalBlockText(CONDITION, input);

            condition = Condition.loadCondition(condCondition);

            condThen = getInternalBlockText(THEN, input);
            LogProcessor.LogProcessor.compileProgram(condThen,out nodesThen);

            condElse = getInternalBlockText(ELSE, input);
            LogProcessor.LogProcessor.compileProgram(condElse, out nodesElse);
        }

        public override string ToString()
        {
            return getOpName();
        }
    }
}
