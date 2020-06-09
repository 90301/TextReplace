using ProgramableText.LogProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.BlockProcessor.Conditions
{
    abstract class Condition : ProgramNodeInterface
    {
        public abstract Boolean calculate(string input);

        public abstract string createExample();

        public abstract String getOpName();
        public abstract void paseCondition(String input);

        public abstract Condition createInstance();

        public static Condition loadCondition(String input)
        {
            //Simplistic conditions, only allow one per if statement
            String line = input.Trim();
            String op = LogProcessor.LogProcessor.getOpName(line);
            Condition condition = LogProcessor.LogProcessor.getConditionByName(op, LogProcessor.LogProcessor.allConditions);
            if (condition != null)
            {
                String argText = LogProcessor.LogProcessor.getArgString(line);
                condition.paseCondition(argText);
                return condition;
            } else
            {
                return null;
            }
        }


        string ProgramNodeInterface.calculate(string input)
        {
            //this code probably shouldn't run
            Console.Error.WriteLine("Condition ran with string output instead of Boolean.");
            throw new NotImplementedException();
            return "" + calculate(input);
            
        }

        
    }
}
