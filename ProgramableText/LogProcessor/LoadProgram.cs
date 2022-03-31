using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    /// <summary>
    /// Load a program
    /// </summary>
    class LoadProgram : ProgramNode
    {
        String programLocation;
        List<ProgramNodeInterface> nodes;
        public override string calculate(string input)
        {
            //TODO add working directory code
            String program;
            if (programLocation == null || programLocation.Length == 0)
            {
                program = input;
            }
            else
            {
                program = System.IO.File.ReadAllText(programLocation);
            }
            LogProcessor.compileProgram(program, out nodes);

            LogProcessor.process(nodes, false, input);
            return LogProcessor.output;
        }

        public override ProgramNode createInstance()
        {
            return new LoadProgram();
        }
        public override string createExample()
        {
            return getOpName()+"( FileLocation )";
        }
        public override string getOpName()
        {
            return "LoadProgram";
        }

        public override void parseArgs(string[] args)
        {
            if (args.Length >= 1)
                programLocation = loadString(args[0]);
        }
    }
}
