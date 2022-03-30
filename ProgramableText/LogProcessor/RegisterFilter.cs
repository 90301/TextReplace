﻿using ProgramableText.Utils;
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
    class RegisterFilter : ProgramNode
    {

        int register;
        public bool any = true;

        public override string calculate(string input)
        {
            String rtrn = "";
            string[] splitLines = TextUtils.splitOnNewLine(input);
            string[] searchStrings = TextUtils.splitOnNewLine(LogProcessor.registers[register]);

            if (any)
            {
                foreach (String line in splitLines)
                {
                    foreach (String search in searchStrings)
                    {
                        if (line.Contains(search))
                        {
                            rtrn += line + Environment.NewLine;
                            break;
                        }
                    }
                }
            } else
            {
                //None
                foreach (String line in splitLines)
                {
                    Boolean matchNotFound = true;
                    foreach (String search in searchStrings)
                    {
                        if (line.Contains(search))
                        {
                            matchNotFound = false;
                            break;
                        }
                    }
                    if (matchNotFound)
                    {
                        rtrn += line + Environment.NewLine;
                    }
                }
            }

            return rtrn;

        }

        public override ProgramNode createInstance()
        {
            return new RegisterFilter();
        }

        public override string getOpName()
        {
            return "RegisterFilter";
        }

        public override string createExample()
        {
            return getOpName() + "( REGISTER , TRUE )";
        }

        public override void parseArgs(string[] args)
        {
            
            register = loadInt(args[0].Trim());
            if (args.Length>=2)
                any = loadBoolean(args[1].Trim());
        }

        public override string ToString()
        {
            return getOpName() + "(" + register + ","+ any + ")";
        }
    }
}
