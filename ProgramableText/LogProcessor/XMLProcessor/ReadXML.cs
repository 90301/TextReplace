using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProgramableText.LogProcessor
{
    public class ReadXML : ProgramNode
    {
        /// <summary>
        /// The text to start each line
        /// </summary>
        String varName = "";
        //TODO add min length check?

        public override string calculate(string input)
        {

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(input);
                getOrCreateXmlLog(varName, doc);
            } catch (Exception e)
            {
                return e.Message;
            }
            return input;
        }


        public override ProgramNode createInstance()
        {
            return new ReadXML();
        }

        public override string getOpName()
        {
            return "ReadXML";
        }

        public override string createExample()
        {
            return getOpName() + "( VARNAME )";
        }

        public override string ToString()
        {
            return getOpName() + "( " + varName+  " )";
        }

        public override void parseArgs(string[] args)
        {
            varName = loadString(args[0].Trim());
        }
    }
}
