using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    public class GetParentNode : ProgramNode
    {
        /// <summary>
        /// The text to start each line
        /// </summary>
        String varName = "";
        //TODO add min length check?

        public override string calculate(string input)
        {
            String rtrn = "";
            XmlLog xmlLog = getOrCreateXmlLog(varName);
            rtrn = xmlLog.xmlDocument.FirstChild.Name;

            return rtrn;
        }

        public override ProgramNode createInstance()
        {
            return new GetParentNode();
        }

        public override string getOpName()
        {
            return "GetParentNode";
        }

        public override string createExample()
        {
            return getOpName() + "( VARNAME )";
        }

        public override string ToString()
        {
            return getOpName() + "( " + varName + " )";
        }

        public override void parseArgs(string[] args)
        {
            varName = loadString(args[0].Trim());
        }
    }
}
