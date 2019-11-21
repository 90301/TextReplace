using ProgramableText.LogProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.BlockProcessor
{
    /// <summary>
    /// Block Data Format:
    /// 
    /// Block Start
    /// Stuff In Here
    /// Block End
    /// </summary>
    abstract class BlockNode : ProgramNodeInterface
    {
        public const String START = " Start";
        public const String END = " End";

        public abstract string calculate(string input);
        public abstract String getOpName();
        public abstract void parseBlocks(String input);
        public abstract BlockNode createInstance();
        public abstract string createExample();

        //Utilities
        //Nesting of the same block nodes not yet supported

        public static string getFullStartAndEndLocations(String inputText, String blockName, out int startLocation, out int endLocation)
        {
            String start = blockName + START;
            String end = blockName + END;
            startLocation = inputText.IndexOf(start);
            endLocation = inputText.IndexOf(end) + end.Length;
            int length = endLocation - startLocation;
            return inputText.Substring(startLocation, length);
        }

        public static String getInternalBlockText(String blockName,String inputText)
        {
            String start = blockName + START;
            String end = blockName + END;
            int startLocation = inputText.IndexOf(start) + start.Length;
            int endLocation = inputText.IndexOf(end);
            int length = endLocation - startLocation;

            return inputText.Substring(startLocation, length);
        }

        public string getStartBlock()
        {
            return getOpName() + START;
        }
        public string getEndBlock()
        {
            return getOpName() + END;
        }

        public String generateExample(List<String> args)
        {
            String start = getStartBlock();
            String end = getEndBlock();
            String argsToString = args.Select(x => x + START + Environment.NewLine + x + END)
                .Aggregate((x, y) => x + Environment.NewLine + y);
            String rtrn = start + Environment.NewLine;
            rtrn += argsToString + Environment.NewLine;
            rtrn += end;
            return rtrn;
        }

        
    }
}
