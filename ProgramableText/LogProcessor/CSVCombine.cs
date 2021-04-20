using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    public class CSVCombine : ProgramNode
    {
        /// <summary>
        /// Each argument
        /// </summary>
        List<String> arguments = new List<string>();
        //TODO add min length check?
        String delimiter = ",";
        

        public override string calculate(string input)
        {
            String rtrn = "";

            List<String[]> lists = new List<string[]>();
            int longest = 0;
            for (int i=0;i<arguments.Count;i++)
            {
                String loadedInput = loadFileOrRegister(arguments[i]);
                lists.Add(loadedInput.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries));
                if (lists[i].Length> longest)
                {
                    longest = lists[i].Length;
                }
            }
            
            for (int i=0;i<longest;i++)
            {
                String line = lists.Select(x => {
                    if (x.Length > i) {
                        return x[i];
                        } else {
                        return "";
                    }
                    }).Aggregate((x, y) => x + delimiter + y);
                rtrn += line +  Environment.NewLine;
            }

            return rtrn;
        }



        public override ProgramNode createInstance()
        {
            return new CSVCombine();
        }

        public override string getOpName()
        {
            return "CSVCombine";
        }

        public override string createExample()
        {
            return getOpName() + "( Delimiter , FILE/REGISTER, FILE/REGISTER  , ... )";
        }

        public override string ToString()
        {
            //TODO: update this to csv the list
            return getOpName() + "( " + delimiter + "," + arguments.ToString() + " )";
        }

        public override void parseArgs(string[] args)
        {
            delimiter = loadString(args[0].Trim());
            int i = 1;
            while (i < args.Length)
            {
                arguments.Add (loadString(args[i].Trim()));
                i++;
            }
        }
    }
}
