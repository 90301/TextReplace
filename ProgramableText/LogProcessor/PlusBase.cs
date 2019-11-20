using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{

    class PlusBase : ProgramNode , ExtraPass
    {
        public String cachedText = "";
        public override string calculate(string input)
        {
            cachedText = input;
            LogProcessor.addProcessedExtraPass(this);
            return LogProcessor.inputText;
        }

        public override ProgramNode createInstance()
        {
            return new PlusBase();
        }

        public override string getOpName()
        {
            return "plusBase";
        }

        public override void parseArgs(string[] args)
        {
            
        }

        public string extraPass(string input)
        {
            return cachedText + Environment.NewLine + input;
        }

        public override String ToString()
        {
            return getOpName() + "()";
        }
    }
}
