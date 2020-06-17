using ProgramableText.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{

    /// <summary>
    /// Finds text from the first register
    /// Replaces text with text from the 2nd register
    /// </summary>
    class RegisterFindAndReplace : ProgramNode
    {

        int registerFind, registerReplace;
        public bool lineByLine = true;

        public override string calculate(string input)
        {
            String rtrn = input;
            if (lineByLine)
            {
                string[] findLines = TextUtils.splitOnNewLine(LogProcessor.registers[registerFind]);
                string[] replaceLines = TextUtils.splitOnNewLine(LogProcessor.registers[registerReplace]);

                //Dictionary<String, String> replacements = new Dictionary<string, string>();
                for (int i=0;i<findLines.Length;i++)
                {
                    rtrn = rtrn.Replace(findLines[i], replaceLines[i]);
                }

            }
            else
            {
                rtrn = input.Replace(
                LogProcessor.registers[registerFind], LogProcessor.registers[registerReplace]);
            }

            return rtrn;

        }

        public override ProgramNode createInstance()
        {
            return new RegisterFindAndReplace();
        }

        public override string getOpName()
        {
            return "RegisterFindAndReplace";
        }

        public override void parseArgs(string[] args)
        {
            registerFind = int.Parse(args[0].Trim());
            registerReplace = int.Parse(args[1].Trim());
        }

        public override string ToString()
        {
            return getOpName() + "(" + registerFind + ","+ registerReplace + ")";
        }
    }
}
