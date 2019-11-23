using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    class ProcessFiles : ProgramNode
    {
        public override string calculate(string input)
        {
            LogProcessor.saveChanges(input);
            if (LogProcessor.filesToProcess.Count >= 1)
            {
                String text = LogProcessor.processNextFile();
                return text;
            }

            return "Files finished processing";
        }

        public override ProgramNode createInstance()
        {
            return new ProcessFiles();
        }

        public override string getOpName()
        {
            return "processFiles";
        }

        public override void parseArgs(string[] args)
        {
            //no args
        }
    }
}
