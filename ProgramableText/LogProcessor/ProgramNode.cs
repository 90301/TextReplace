using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

/// <summary>
/// This is an abstract class that holds all functional processes
/// </summary>
namespace ProgramableText.LogProcessor
{
    public abstract class ProgramNode : ProgramNodeInterface
    {
        public static readonly String[] NEWLINE = new string[] { Environment.NewLine, "\r\n", "\r", "\n" };
        public static readonly String[] WORDS = new string[] { Environment.NewLine, " ", "=", ">", "<", "/", "'", "\"" };
        public static readonly String[] ATTRIBUTES = new string[] { Environment.NewLine, " " };
        public static readonly String[] TRUE = new string[] { "true", "t", "1" };
        public static readonly String[] FALSE = new string[] { "false", "f", "0" };
        public static readonly String[] BAR = new string[] { "|" };

        /// <summary>
        /// Calculates a given functional node
        /// Null or an Empty String outputs will be removed automatically.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public abstract String calculate(String input);

        public abstract String getOpName();

        public abstract void parseArgs(String[] args);

        public abstract ProgramNode createInstance();

        // Utilities:

        public static List<String> splitTextToLines(String text)
        {
            return text.Split(NEWLINE, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static List<String> splitTextToWords(String text)
        {
            return text.Split(WORDS, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static List<String> splitTextToAttributes(String text)
        {
            return text.Split(ATTRIBUTES, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static String getAttribute(List<String> attributes, String attribute)
        {
            return attributes.Select(x => x).Where(x => x.Contains(attribute)).First();
        }

        /// <summary>
        /// Utility to remove nulls
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<String> removeNullorEmptyLines(List<String> lines)
        {
            //TODO have a version of this that only takes
            return lines.Select(x => x).Where(x => x.Length >= 1).ToList();
        }

        public virtual string createExample()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return generateToString(0);
        }

        public String generateToString(int argCount)
        {
            String commas = "";
            for (int i = 1; i < argCount; i++)
            {
                commas += " ,";
            }
            commas += " ";
            return this.getOpName() + "(" + commas + ")";
        }

        public static String loadString(String str)
        {
            return LogProcessor.specialCharacterReplacement(str.Trim());
        }
        public static int loadInt(String str)
        {
            return int.Parse(str.Trim());
        }
        public static Boolean loadBoolean(String str)
        {
            String processed = str.Trim().ToLower();
            if (TRUE.Contains(processed))
            {
                return true;
            }
            else if (FALSE.Contains(processed))
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public static String loadFileOrRegister(String input)
        {
            if (input.Contains("/") || input.Contains("\\"))
            {
                //file input
                return System.IO.File.ReadAllText(input);
            }
            else
            {
                int register = loadInt(input);
                return LogProcessor.registers[register];
            }
        }


        public static XmlLog getOrCreateXmlLog(String varName)
        {
            if (LogProcessor.variables.ContainsKey(varName))
            {
                LogProcessor.variables.TryGetValue(varName, out LogVar logVar);
                if (logVar.GetType().IsInstanceOfType(new XmlLog()))
                {
                    XmlLog xmlLog = ((XmlLog)logVar);
                    xmlLog.varName = varName;
                    return ((XmlLog)logVar);
                }
                else
                {
                    XmlLog xmlLog = new XmlLog();
                    xmlLog.varName = varName;
                    LogProcessor.variables.Add(varName, xmlLog);
                }
                return null;
            }
            else
            {
                XmlLog xmlLog = new XmlLog();
                xmlLog.varName = varName;
                LogProcessor.variables.Add(varName, xmlLog);
                return xmlLog;
            }
        }
        public static XmlLog getOrCreateXmlLog(string varName, XmlDocument doc)
        {
            XmlLog xmlLog = getOrCreateXmlLog(varName);
            xmlLog.xmlDocument = doc;
            return xmlLog;
        }
    }

}
