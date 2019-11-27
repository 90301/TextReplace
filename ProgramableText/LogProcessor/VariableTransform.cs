using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    public class VariableTransform : ProgramNode
    {
        /// <summary>
        /// Transform Variables from Start , Length format
        /// to
        /// Start / End format
        /// </summary>
        public const String OP_SUBSTRING_REPLACE = "substringreplace";
        public const int OP_VALUE_SUBSTRING_REPLACE = 0;

        public int op = 0;

        public List<string> variables;

        public static readonly String[] QUOTES = new string[] { "\"", "\"" };
        public override string calculate(string input)
        {
            String output = "";
            List<String> lines = splitTextToLines(input);

            String oldStart = variables[0].Trim();
            String oldLength = variables[1].Trim();
            String newStart = variables[2].Trim();
            String newEnd = variables[3].Trim();

            InnerReadNode innerReadValue = new InnerReadNode();
            innerReadValue.parseArgs(QUOTES);

            foreach (String line in lines)
            {
                if (line.Contains(oldStart) && line.Contains(oldLength))
                {
                    List<String> attributes = splitTextToAttributes(line);

                    String startAttribute = getAttribute(attributes, oldStart);
                    String lengthAttribute = getAttribute(attributes, oldLength);

                    int startVal,endVal,lengthVal;

                    startVal = int.Parse(innerReadValue.calculate(startAttribute).Trim());
                    lengthVal = int.Parse(innerReadValue.calculate(lengthAttribute).Trim());
                    endVal = startVal + lengthVal;

                    String newStartReplace = newStart + "=\"" + startVal + "\"";
                    String newEndReplace = newEnd + "=\"" + endVal + "\"";

                    String updatedLine = line.Replace(startAttribute, newStartReplace).Replace(lengthAttribute, newEndReplace);
                    output += updatedLine + Environment.NewLine;

                } else
                {
                    output += line + Environment.NewLine;
                }
            }

            return output;
        }

        public override ProgramNode createInstance()
        {
            return new VariableTransform();
        }

        public override string getOpName()
        {
            return "variableTransform";
        }

        public override void parseArgs(string[] args)
        {
            String opString = args[0].Trim().ToLower();
            variables = new List<string>();

            switch (opString)
            {
                case (OP_SUBSTRING_REPLACE):
                    op = OP_VALUE_SUBSTRING_REPLACE;
                    break;
                default:
                    op = 0;
                    break;  
            }

            if (op== OP_VALUE_SUBSTRING_REPLACE)
            {
                variables = args.Skip(1).ToList();
                //Variables:
                //Start
                //Length
                //New Start
                //New End
            }
            
        }

        public override string createExample()
        {
            return getOpName() + "(" + OP_SUBSTRING_REPLACE + ",OLD_START,OLD_LENGTH,NEW_START,NEW_END)";
        }
    }
}
