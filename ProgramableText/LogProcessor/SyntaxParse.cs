using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramableText.Utils;

namespace ProgramableText.LogProcessor
{

    class SyntaxParse : ProgramNode
    {
        //The first line must contain this string (optional) ie class
        String filterFirst;
        //the last line must contain this string (optional) ie end class, we will hold off on this functionality
        //String filterLast;
        //the character to count up
        String first;
        //the character to count down
        String last;

        //optionally dump the matches into a list
        String listToPopulate,listFirstLines;

        public override string calculate(string input)
        {
            List<String> blocksParsed = new List<String>();
            List<String> firstLines = new List<String>();
            String[] lines = input.Split(ProgramNode.NEWLINE, StringSplitOptions.None);

            //Start out by finding all indexes for matches for up and down counts
            List<int> indexsUp = input.AllIndexesOf(first);

            List<int> indexsDown = input.AllIndexesOf(last);

            List<int> indexsLine = input.AllIndexesOf(Environment.NewLine);
            //then use that to find what line the first block starts
            int count = 0;
            int upCounter = 0;
            int downCounter = 0;

            int upIndex, downIndex;

            while (upCounter < indexsUp.Count)
            {
                int upCharLocation = indexsUp[upCounter];
                int lineNumUp = getLine(indexsLine, upCharLocation);
                String firstLine = lines[lineNumUp];
                if (firstLine.Contains(first))
                {
                    //start parsing down counting
                    count++;
                    upCounter++;
                    while (count > 0 && downCounter<indexsDown.Count)
                    {
                        int nextDownCharLocation = indexsDown[downCounter];

                        int nextUpCharLocation;
                        if (upCounter < indexsUp.Count)
                        {
                            nextUpCharLocation = indexsUp[upCounter];
                        } else
                        {
                            nextUpCharLocation = int.MaxValue;
                        }

                        if (nextDownCharLocation < nextUpCharLocation)
                        {
                            count--;
                            downCounter++;
                            if (count == 0)
                            {
                                int length = nextDownCharLocation - upCharLocation - first.Length;
                                blocksParsed.Add(input.Substring(upCharLocation+first.Length, length));
                                if (listFirstLines!= null && listFirstLines.Length>=1)
                                {
                                    firstLines.Add(firstLine);
                                }
                            }
                        }
                        else
                        {
                            upCounter++;
                            count++;

                        }
                    }
                }
            }
            //Once we find the first block, we must find the final block, for every extra first we find, we must increment the number of lasts we need



            if (listToPopulate != null && listToPopulate.Length >=1)
            {
                //TODO populate the list
                ListVar listVar = new ListVar();
                listVar.varName = listToPopulate;
                listVar.list = blocksParsed;
                LogProcessor.variables[listVar.varName] = listVar;
            }
            
            return blocksParsed.Select(x => x.ToString()).Aggregate((x,y) => x + Environment.NewLine + y);
        }

        public static int getLine(List<int> indexsLine, int location)
        {
            //returns the line the match was found on
            for (int i=0;i<indexsLine.Count();i++)
            {
                if (indexsLine[i]<location)
                {
                    return i;
                }
            }
            return 0;
        }

        public override ProgramNode createInstance()
        {
            return new SyntaxParse();
        }

        public override string getOpName()
        {
            return "SyntaxParse";
        }
        public override string createExample()
        {
            return getOpName() + "( { , } , FILTERFIRST, LISTNAME )";
        }
        public override void parseArgs(string[] args)
        {
            first = ProgramNode.loadString(args[0].Trim());
            last = ProgramNode.loadString(args[1].Trim());
            filterFirst = ProgramNode.loadString(args[2].Trim());
            listToPopulate = ProgramNode.loadString(args[3].Trim());
            if (args.Length>=5)
                listFirstLines = ProgramNode.loadString(args[4].Trim());
        }
    }
}
