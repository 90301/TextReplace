using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProgramableText.LogProcessor.XMLProcessor
{
    public class GetAllElements : ProgramNode
    {
        String XPath;
        String varName;
        public override string calculate(string input)
        {
            String rtrn = "";
            XmlLog xmlLog = getOrCreateXmlLog(varName);

            foreach (XmlNode topNode in xmlLog.xmlDocument.ChildNodes)
            {
                foreach (XmlNode node in topNode.ChildNodes)
                {
                    rtrn += node.Name + Environment.NewLine;
                }
            }
            return rtrn;
        }

        public override ProgramNode createInstance()
        {
            return new GetAllElements();
        }

        public override string getOpName()
        {
            return "GetAllElements";
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
