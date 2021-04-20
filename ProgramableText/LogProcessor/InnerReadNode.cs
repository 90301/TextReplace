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
        Boolean nonRegex = true;
        Boolean singleLine = false;

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
            if (nonRegex)
            {
                if (singleLine)
                {
                    //split string into lines, then attempt to parse
                }
                else
                {
                    List<String> matches = coreFilter2(input);
                    if (matches.Count >= 1)
                    {
                        return coreFilter2(input).Aggregate((x, y) => x + Environment.NewLine + y);
                    } else {
                        return "";
                    }
                }

            } else {
                List<String> core = coreFilter(input);
                if (core.Count >= 1)
                {
                    return core.Aggregate((x, y) => x + Environment.NewLine + y);
                } else
                {
                    return "NO RESULTS FOUND FOR : " + ToString();
                }
            }
            return "";
        }

        public string[] getArrayResults(string input)
        {
            return calculate(input).Split(LogProcessor.LINE_SPLIT, StringSplitOptions.RemoveEmptyEntries);
        }
        public List<String> coreFilter2(String input)
        {
            String textRemaining = input;
            List<String> matches = new List<string>();
            while (textRemaining.Contains(start) && textRemaining.Contains(end))
            {
                int startIndex = textRemaining.IndexOf(start) + start.Length;
                String subString = textRemaining.Substring(startIndex);
                if (!subString.Contains(end))
                {
                    break;
                }
                int relEndIndex = subString.IndexOf(end);

                String match = subString.Substring(0, relEndIndex);
                matches.Add(match);

                int endIndex = startIndex + relEndIndex + end.Length;
                if (textRemaining.Length <= endIndex)
                {
                    break;
                }
                textRemaining = textRemaining.Substring(endIndex);
            }
            return matches;
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
            if (nonRegex)
            {
                //Load Args
                start = loadString(args[0]);
                end = loadString(args[1]);
            }
            else
            {
                if (args.Length >= 2)
                {
                    start = Escape(args[0]);
                    end = Escape(args[1]);
                }
                else
                {
                    if (args[0].Equals("("))
                    {
                        start = Escape("(");
                        end = Escape(")");
                    }
                }
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
