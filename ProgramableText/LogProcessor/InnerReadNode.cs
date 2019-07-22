using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    /// <summary>
    /// Get's the values inside of certain values
    /// EX: textoutside (textinside)
    /// ||| ---------- |||
    /// VVV turns into VVV
    /// textinside
    /// </summary>
    public class InnerReadNode : ProgramNode
    {
        string start, end;
        Boolean multiMatch;

        static List<String> escapeChars = new List<string>();

        public static void setupEscapeChars()
        {
            escapeChars.Add("(");
            escapeChars.Add(")");
            escapeChars.Add("[");
            escapeChars.Add("]");
        }
        public override string calculate(string input)
        {
            return coreFilter(input).Aggregate((x, y) => x + Environment.NewLine + y);
        }

        public string[] getArrayResults(string input)
        {
            return coreFilter(input).ToArray();
        }

        public List<String> coreFilter(String input)
        {
            List<String> matches = new List<string>();

            //only one match
            //List<string> halfMatches = input.Split(start, StringSplitOptions.None).ToList();

            //multiple matches
            String regex = start + "(.*?)" + end;
            //from Match match in Regex.Matches(input,regex)
            //select match.ToString();

            MatchCollection matchesRegex = Regex.Matches(input, regex);

            foreach (Match match in matchesRegex)
            {
                matches.Add(match.Groups[1].Value);
            }
            return matches;
        }

        public override ProgramNode createInstance()
        {
            return new InnerReadNode();
        }

        public override string getOpName()
        {
            return "innerRead";
        }

        public override void parseArgs(string[] args)
        {
            if (args.Length >=2) {
            start = Escape(args[0]);
            end = Escape(args[1]);
            } else
            {
                if (args[0].Equals("("))
                {
                    start = Escape("(");
                    end = Escape(")");
                }
            }


            if (args.Length > 2)
            {
                Boolean.TryParse(args[2],out multiMatch);
            }
        }

        private string Escape(string v)
        {
            String escapeV = v;
            foreach (String escape in escapeChars) {
                escapeV = escapeV.Replace(escape, "\\" + escape);
            }

            return escapeV;

        }

        public override string ToString()
        {
            return getOpName() + "( " + this.start + " , " + this.end + " )";
        }

    }
}
