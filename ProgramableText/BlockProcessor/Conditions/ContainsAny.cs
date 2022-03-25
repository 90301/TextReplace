using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramableText.LogProcessor;

namespace ProgramableText.BlockProcessor.Conditions
{
    /// <summary>
    /// Checks to see if a string contains any of the strings in a register
    /// </summary>
    class ContainsAny : Condition
    {
        public const String CONTAINS_ANY = "ContainsAny";

        int register;
        //String searchStr = "";
        public override bool calculate(string input)
        {

            string[] searchStrings = Utils.TextUtils.splitOnNewLine(LogProcessor.LogProcessor.registers[register]);

            foreach (String search in searchStrings)
            {
                if (input.Contains(search))
                {
                    return true;
                }
            }
            return false;
        }

        public override string createExample()
        {
            return getOpName() + "()";
        }

        public override Condition createInstance()
        {
            return new Contains();
        }

        public override string getOpName()
        {
            return CONTAINS_ANY;
        }

        public override void paseCondition(string input)
        {
            register = ProgramNode.loadInt(input.Trim());
            
        }

        public override string ToString()
        {
            return getOpName() + " ( " + register  + " ) ";
        }
    }
}
