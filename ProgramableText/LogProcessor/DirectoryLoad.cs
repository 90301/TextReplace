using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    class DirectoryLoad : ProgramNode
    {

        String fileLocation, filterBy;
        Boolean subDirectories;
        public override string calculate(string input)
        {

            SearchOption serarchOptions = SearchOption.TopDirectoryOnly;
            String filter = "*.*";
            if (subDirectories == true)
            {
                serarchOptions = SearchOption.AllDirectories;
            }
            if (filterBy != null)
            {
                filter = filterBy;
            }
            List<String> files = Directory.GetFiles(fileLocation, filter, serarchOptions).ToList();

            return files.Select(x => x).Aggregate((x, y) => x + Environment.NewLine + y);

        }

        public override ProgramNode createInstance()
        {
            return new DirectoryLoad();
        }

        public override string getOpName()
        {
            return "DirectoryLoad";
        }

        public override void parseArgs(string[] args)
        {
            fileLocation = args[0].Trim();
            if (args.Length >= 2)
            {
                subDirectories = ProgramNode.loadBoolean(args[1]);
            }
            if (args.Length >= 3)
            {
                filterBy = args[2].Trim();
            }
        }

        public override string ToString()
        {
            return generateToString(3);
        }

        public override String createExample()
        {
            return this.getOpName() + "( DIRECTORY , SUBDIRECTORIES , FILTER )";
        }
    }
}
