﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    class WriteToRegister : ProgramNode
    {
        int registerNumber = 0;
        Boolean overwrite = true;
        Boolean addNewline = true;
        public override string calculate(string input)
        {
            while (LogProcessor.registers.Count <= registerNumber)
            {
                LogProcessor.registers.Add("");
            }
            if (overwrite)
            {
                if (input.Contains(Environment.NewLine))
                {
                    LogProcessor.registers[registerNumber] = input;
                }
                else
                {
                    LogProcessor.registers[registerNumber] = input + Environment.NewLine;
                }
            } else
            {
                if (input.Contains(Environment.NewLine))
                {
                    LogProcessor.registers[registerNumber] += input;
                } else
                {
                    LogProcessor.registers[registerNumber] += input + Environment.NewLine;
                }
            }
            return input;
        }

        public override ProgramNode createInstance()
        {
            return new WriteToRegister();
        }

        public override string getOpName()
        {
            return "writeToRegister";
        }

        public override string createExample()
        {
            return getOpName() + "( REGISTER , OVERWRITE )"; 
        }

        public override string ToString()
        {
            return getOpName() + "( " + registerNumber + " , " +overwrite +" )";
        }

        public override void parseArgs(string[] args)
        {
            registerNumber = int.Parse(args[0].Trim());
            overwrite = loadBoolean(args[1]);

            if (registerNumber < 0)
            {
                registerNumber = 0;
            }
        }
    }
}
