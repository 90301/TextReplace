using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{

    /// <summary>
    /// Used to get the first of last word in a line
    /// Can also get the first / last X words in a line
    /// 
    /// Useful for doing conversions, such as SQL => CS
    /// </summary>
    public class GetWordInLine : ProgramNode
    {
        /// <summary>
        /// The text to delimit by, defaults to space
        /// </summary>
        String[] Delimiter = { " " };
        /// <summary>
        /// how many words to include
        /// </summary>
        int WordCount = 1;
        /// <summary>
        /// Direction to read from / FIRST / LAST
        /// </summary>
        string Direction = DIRECTION_LAST;

        const string DIRECTION_FIRST = "FIRST";
        const string DIRECTION_LAST = "LAST";

        public override string calculate(string input)
        {
            String rtrn = "";

            String[] splitLines = input.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries);
            for (int i=0;i<splitLines.Length;i++)
            {
                String line = splitLines[i];

                String[] dLine = line.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries);
                String rtrnLine = "";
                int start, end, change;
                
                if (dLine.Length==1)
                {
                    rtrn += dLine[0] + Environment.NewLine;
                    continue;
                }

                if (Direction == DIRECTION_FIRST)
                {
                    start = 0;
                    end = dLine.Length-1;
                    change = 1;
                } else
                {
                    start = dLine.Length -1;
                    end = 0;
                    change = -1;
                }
                int count = 0;
                
                for (int e=start;e!=end;e+=change)
                {
                    count++;
                    if (count > WordCount)
                        break;

                    if (rtrnLine.Length == 0)
                    {
                        rtrnLine = dLine[e];
                    }
                    else
                    {
                        if (change == 1)
                        {
                            rtrnLine = rtrnLine + " " + dLine[e];
                        } else
                        {
                            rtrnLine = dLine[e] + " " + rtrnLine;
                        }

                    }
                    rtrn += rtrnLine + Environment.NewLine;
                }
            }

            return rtrn;
        }

        public override ProgramNode createInstance()
        {
            return new GetWordInLine();
        }

        public override string getOpName()
        {
            return "GetWordInLine";
        }

        public override string createExample()
        {
            return getOpName() + "( LAST/FIRST , [WORD_COUNT] , [OPTIONAL_DELIMITER] )";
        }

        public override string ToString()
        {
            return getOpName() + "( " + Direction + " , " + WordCount + " , " + Delimiter + " )";
        }

        public override void parseArgs(string[] args)
        {
            Direction = loadString(args[0]).ToUpper();
            if (args.Length>=2)
            WordCount = loadInt(args[1]);
            if (args.Length >= 3)
                Delimiter = new String[] { loadString(args[2]) };
        }
    }
}
