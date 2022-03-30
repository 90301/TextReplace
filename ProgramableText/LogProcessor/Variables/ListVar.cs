using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProgramableText.LogProcessor
{

    /// <summary>
    /// This is a class that holds a List and some ways of navigating it
    /// </summary>
    public class ListVar : LogVar
    {

        public String varName;
        /// <summary>
        /// Holds the xmlDoc
        /// </summary>
        public List<String> list;
        
        public int listLocation = 0;

        public string getVarName()
        {
            return varName;
        }

        public override String ToString()
        {
            return varName + Environment.NewLine + list;
        }
        //TODO add min length check?
    }
}
