using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    /// <summary>
    /// Searches for words that match a substring (case insensitive)
    /// EX: WordSearch(ID)
    /// accID
    /// acctId
    /// 
    /// </summary>
    class WordSearch : ProgramNode

    {
        String searchFor = "";
        public override string calculate(string input)
        {
            String rtrn = "";
            String searchString = searchFor.ToLower();
            List<String> results = ProgramNode.splitTextToWords(input).Select(x => x).Where(x => x.ToLower().Contains(searchString)).ToList();
            //Eliminate Duplicates (may make this a seperate operation later)
            HashSet<String> noDuplicates = new HashSet<string>();
            results.ForEach(x => noDuplicates.Add(x));

            //Output results, one on each line
            if (noDuplicates.Count >= 1)
                rtrn = noDuplicates.Select(x => x).Aggregate((x, y) => x + Environment.NewLine + y);
            return rtrn;

        }

        public override ProgramNode createInstance()
        {
            return new WordSearch();
        }

        public override string getOpName()
        {
            return "wordSearch";
        }

        public override void parseArgs(string[] args)
        {
            searchFor = args[0];
        }

        public override string ToString()
        {            
            return getOpName() + "( " + searchFor + " )";
        }
    }
}
