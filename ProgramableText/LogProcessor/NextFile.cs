using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    /// <summary>
    /// Moves to the next file. DOES NOT save changes.
    /// </summary>
    class NextFile : ProgramNode
    {
        public override string calculate(string input)
        {
            if (LogProcessor.filesToProcess != null && LogProcessor.filesToProcess.Count >= 1)
            {
                String text = LogProcessor.processNextFile();
                return text;
            }
            return "Files Finished Processing";
        }

        public override ProgramNode createInstance()
        {
            return new NextFile();
        }

        public override string getOpName()
        {
            return "nextFile";
        }

        public override void parseArgs(string[] args)
        {
            //No Args
        }
    }
}
