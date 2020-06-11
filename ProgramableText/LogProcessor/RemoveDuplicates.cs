using ProgramableText.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    /// <summary>
    /// Removes duplicate lines
    /// </summary>
    class RemoveDuplicates : ProgramNode
    {
        public override string calculate(string input)
        {
            HashSet<String> lineSet = new HashSet<string>();

            foreach (String line in TextUtils.splitOnNewLine(input))
            {
                lineSet.Add(line);
            }

            if (lineSet.Count >= 1)
            {
                return lineSet.Select(x => x).Aggregate((x, y) => x + Environment.NewLine + y);
            } else
            {
                return "";
            }
        }

        public override ProgramNode createInstance()
        {
            return new RemoveDuplicates();
        }

        public override string getOpName()
        {
            return "RemoveDuplicates";
        }

        public override void parseArgs(string[] args)
        {
            // No Args Yet
        }

        public override string ToString()
        {
            return getOpName() + "()";
        }
    }
}
