using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProgramableText.LogProcessor
{

    /// <summary>
    /// This is a class that holds an XML 
    /// </summary>
    public class XmlLog : LogVar
    {

        public String varName;
        /// <summary>
        /// Holds the xmlDoc
        /// </summary>
        public XmlDocument xmlDocument;
        
        public XmlNode xmlElement;

        public string getVarName()
        {
            return varName;
        }

        public override String ToString()
        {
            return varName + Environment.NewLine + xmlDocument.OuterXml;
        }
        //TODO add min length check?
    }
}
