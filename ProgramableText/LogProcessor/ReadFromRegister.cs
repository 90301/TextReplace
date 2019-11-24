using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    class ReadFromRegister : ProgramNode
    {
        int registerNumber = 0;
        public override string calculate(string input)
        {
            return LogProcessor.registers[registerNumber];
        }

        public override ProgramNode createInstance()
        {
            return new ReadFromRegister();
        }

        public override string getOpName()
        {
            return "readFromRegister";
        }

        public override void parseArgs(string[] args)
        {
            registerNumber = int.Parse(args[0].Trim());

            if (registerNumber < 0)
            {
                registerNumber = 0;
            }
        }
    }
}
