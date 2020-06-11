using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    /// <summary>
    /// Loads all text into the files processing list
    /// (one file per line)
    /// Then starts storing remaining operations into a processing queue to be repeated for every file.
    /// </summary>
    class LoadFiles : ProgramNode
    {
        public override string calculate(string input)
        {
            LogProcessor.filesToProcess = ProgramNode.splitTextToLines(input);
            //LogProcessor.subQueue = LogProcessor.nodes.Skip(0).ToList();
            return LogProcessor.processNextFile(false);
        }

        public override ProgramNode createInstance()
        {
            return new LoadFiles();
        }

        public override string getOpName()
        {
            return "loadFiles";
        }

        public override void parseArgs(string[] args)
        {
            //No Arguments yet
        }
    }
}
