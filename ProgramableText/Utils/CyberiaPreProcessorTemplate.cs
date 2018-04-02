using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.Utils
{
    public class CyberiaPreProcessorTemplate
    {
        public string templateName { get; set; } = "";
        public string templateText { get; set; } = "";


        /// <summary>
        /// TODO make this load from a file.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<String,CyberiaPreProcessorTemplate> getBasicTemplates()
        {
            Dictionary<String,CyberiaPreProcessorTemplate> templates = new Dictionary<String,CyberiaPreProcessorTemplate>();

            CyberiaPreProcessorTemplate c1 = new CyberiaPreProcessorTemplate();
            c1.templateName = "SQL Var List";
            c1.templateText += "sql varlist gen blockstart" + Environment.NewLine;
            c1.templateText += "" + Environment.NewLine;
            c1.templateText += "" + Environment.NewLine;
            c1.templateText += "sql varlist gen blockend" + Environment.NewLine;

            templates[c1.templateName] = (c1);

            CyberiaPreProcessorTemplate c2 = new CyberiaPreProcessorTemplate();
            c2.templateName = "Foreach Replace";
            c2.templateText += "foreach varlist blockstart" + Environment.NewLine;
            c2.templateText += ""+Environment.NewLine;
            c2.templateText += "foreach content blockstart" + Environment.NewLine;
            c2.templateText += ""+Environment.NewLine;
            c2.templateText += "foreach content blockend" + Environment.NewLine;
            c2.templateText += "foreach varlist blockend" + Environment.NewLine;
            templates[c2.templateName] = (c2);

            return templates;

        }
    }
}
