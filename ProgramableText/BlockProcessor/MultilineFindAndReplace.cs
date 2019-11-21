using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.BlockProcessor
{
    class MultilineFindAndReplace : BlockNode
    {

        const String FIND = "Find";
        const String REPLACE = "Replace";

        String findText, replaceText;
        public override string calculate(string input)
        {
            return input.Replace(findText, replaceText);
        }

        public override string createExample()
        {
            List<String> args = new List<string>();
            args.Add(FIND);
            args.Add(REPLACE);

            return generateExample(args);
        }

        public override BlockNode createInstance()
        {
            return new MultilineFindAndReplace();
        }

        public override string getOpName()
        {
            return "MultilineFindAndReplace";
        }

        public override void parseBlocks(string input)
        {
            findText = getInternalBlockText(FIND, input);
            replaceText = getInternalBlockText(REPLACE, input);
        }

        public override string ToString()
        {
            return getOpName();
        }

    }
}
