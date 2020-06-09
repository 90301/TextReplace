using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramableText.LogProcessor;

namespace ProgramableText.BlockProcessor.Conditions
{
    class Contains : Condition
    {
        public const String CONTAINS = "Contains";

        String searchStr = "";
        public override bool calculate(string input)
        {
            return input.Contains(searchStr);
        }

        public override string createExample()
        {
            return CONTAINS + "()";
        }

        public override Condition createInstance()
        {
            return new Contains();
        }

        public override string getOpName()
        {
            return CONTAINS;
        }

        public override void paseCondition(string input)
        {
            searchStr = LogProcessor.LogProcessor.specialCharacterReplacement(input);
            
        }

        public override string ToString()
        {
            return getOpName();
        }
    }
}
