using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    class SetOutput : ProgramNode
    {
        public const string SET_OUTPUT = "SetOutput";
        string output = "OUTPUT";
        Boolean concat = false;
        public override string calculate(string input)
        {
            if (concat)
            {
                return input + Environment.NewLine + output;
            }
            else
            {
                return output;
            }
        }

        public override ProgramNode createInstance()
        {
            return new SetOutput();
        }

        public override string getOpName()
        {
            return SET_OUTPUT;
        }

        public override void parseArgs(string[] args)
        {
            //ARGS (TEXT , (optional) add text Boolean)
            if (args[0].Length == 0)
            {
                output = "";
            }
            else
            {
                output = LogProcessor.specialCharacterReplacement(args[0]);
            }
            if (args.Length >= 2)
            {
                concat = ProgramNode.loadBoolean(args[1]);
            }
        }

        public override string ToString()
        {
            return getOpName() + "(" + output + ")";
        }
    }
}
