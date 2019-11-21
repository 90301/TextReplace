using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using ProgramableText.Structures;
using ProgramableText.Utils;

namespace ProgramableText
{
    /// <summary>
    /// Supported Languages:
    /// Java
    /// C#
    /// SQL
    /// </summary>
    public class CyberiaPreProcessor
    {
        public String LanguageUsing { get; set; }
        public static bool useTabDelimiter { get; set; } = false;

        public const String LANGUAGE_JAVA = "JAVA";
        public const String LANGUAGE_C_SHARP = "CSHARP";
        public const String LANGUAGE_SQL = "SQL";
        public const String LANGUAGE_XML = "XML";

        #region Directives

        public static List<String> LANGUAGES = new List<String>();

        public static List<String> DIRECTIVES = new List<String>();

        public static List<String> VARIABLE_MODS = new List<String>();

        public static List<String> VARIABLE_TYPES = new List<String>();

        public static List<String> VARIABLE_PREFIXES = new List<String>();

        public const String BLOCK_START = "blockstart";
        public const String BLOCK_END = "blockend";

        public const String DEV_ONLY = "devonly";

        public const String PROD_ONLY = "prodonly";

        public const String SQL_FLATFILE = "sql flatfile";

        public const String FOR_EACH_VARLIST = "foreach varlist";//This is a list of variable (start / end) (CSV seperated for multi-replace)

        /// <summary>
        /// This is going to be repeated for each variable in the var list with the VARNAME being replaced with the variable and
        /// NO_PREFIX_VARNAME being replaced with the variablename with the prefix being stripped.
        /// </summary>
        public const String FOR_EACH_CONTENT = "foreach content";

        /// <summary>
        /// Turns SAS XML Vars to SQL parameters
        /// </summary>
        public const String SQL_VARLIST_GEN = "sql varlist gen";

        static CyberiaPreProcessor()
        {
            DIRECTIVES.Add(DEV_ONLY);
            DIRECTIVES.Add(PROD_ONLY);

            VARIABLE_MODS.Add("public");
            VARIABLE_MODS.Add("private");
            VARIABLE_MODS.Add("protected");
            VARIABLE_MODS.Add("static");
            VARIABLE_MODS.Add("const");

            List<String> modValues = new List<string>();
            modValues.Add("a");//parameter
            modValues.Add("l");//local
            modValues.Add("i");//instance
            foreach (String modValue in modValues)
            {
                VARIABLE_PREFIXES.Add(modValue + "str");//string
                VARIABLE_PREFIXES.Add(modValue + "int");//int
                VARIABLE_PREFIXES.Add(modValue + "bol");//boolean
                VARIABLE_PREFIXES.Add(modValue + "col");//collection
                VARIABLE_PREFIXES.Add(modValue + "arr");//Array
                VARIABLE_PREFIXES.Add(modValue + "bus");//buisness class
                VARIABLE_PREFIXES.Add(modValue + "dt");//date time
                VARIABLE_PREFIXES.Add(modValue + "obj");//object

            }

            VARIABLE_TYPES.Add("integer");
            VARIABLE_TYPES.Add("string");
            VARIABLE_TYPES.Add("boolean");
            VARIABLE_TYPES.Add("long");
            VARIABLE_TYPES.Add("datetime");

            LANGUAGES.Add(LANGUAGE_C_SHARP);
            LANGUAGES.Add(LANGUAGE_JAVA);
            LANGUAGES.Add(LANGUAGE_SQL);
            LANGUAGES.Add(LANGUAGE_XML);
        }

        /// <summary>
        /// Strips a variable name
        /// </summary>
        /// <param name="varNameLine"></param>
        /// <returns></returns>
        public String variableNameStripping(String varNameLine)
        {
            string rtrn = "";
            //TODO add SQL string capibility to var replacement
            //TODO don't replace var define with variable.
            String[] varSplit = varNameLine.Split(new String[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            //remove public / private / protected

            foreach (string varPart in varSplit)
            {
                //contains?
                if (
                    !VARIABLE_MODS.Exists(x => x.Equals(varPart.ToLower()))
                    && !VARIABLE_TYPES.Exists(x => x.Equals(varPart.ToLower()))
                    && !varPart.Contains("<")// removes any variable types such as List<>
                    )
                {
                    rtrn = varPart.Replace(";", "");
                    
                }
            }

            return rtrn;
        }

        public String removePrefix(String varName)
        {
            String rtrn = varName;
            foreach (var prefix in VARIABLE_PREFIXES)
            {
                rtrn = rtrn.Replace(prefix, "");
            }
            return rtrn;
        }

        #endregion

        #region messages

        public string devMessage()
        {
            String devMessage = "This file was generated by the Cyberia Pre-Processor" + Environment.NewLine;
            devMessage += "Language of output file: " + LanguageUsing + Environment.NewLine;
            devMessage += "File Generated on: " + DateTime.Now + Environment.NewLine;

            return codeComment(devMessage);
        }

#endregion
        /// <summary>
        /// Processes input code (.cyberia files in the future)
        /// into devCode and prodCode
        /// </summary>
        /// <param name="input"></param>
        /// <param name="devCode"></param>
        /// <param name="prodCode"></param>
        public void processText(String input, out String devCode, out String prodCode)
        {
            string scratchpad = input;
            devCode = "" + devMessage();
            prodCode = "" + devMessage();

            //line by line pre-processing
            String devByLines, prodByLines;

            foreach (String block in getBlocksForDirective(input, SQL_FLATFILE))
            {
                scratchpad = sqlFlatFile(scratchpad, block);
            }
            foreach (String block in getBlocksForDirective(scratchpad, FOR_EACH_VARLIST))
            {
                //for each content is nested in each foreach varlist
                String listResult = foreachVarList(scratchpad, block);
                scratchpad += listResult;

            }
            foreach (String block in getBlocksForDirective(scratchpad, SQL_VARLIST_GEN))
            {
                //SQL Var List Gen (XML -> var defines)
                String listResult = sqlVarList(scratchpad, block);
                scratchpad = listResult;

            }


            lineByLinePreProcess(scratchpad, out devByLines, out prodByLines);

            devCode += devByLines;
            prodCode += prodByLines;
        }



        protected void lineByLinePreProcess(string input, out string devByLines, out string prodByLines)
        {
            devByLines = "";
            prodByLines = "";

            String[] lines = input.Split(NEW_LINE_SPLIT,StringSplitOptions.None);

            foreach (String line in lines)
            {
                //find any pre-processor directives? do those
                List<String> directives = findDirectives(line);
                //DEV / Prod only
                if (directives.Count >= 1)
                {


                    if (directives.Contains(DEV_ONLY))
                    {
                        devByLines += line + Environment.NewLine;

                    }
                    else if (directives.Contains(PROD_ONLY))
                    {
                        prodByLines += line + Environment.NewLine;
                    }
                }
                else
                {
                    //No Processor directives found
                    devByLines += line + Environment.NewLine;
                    prodByLines += line + Environment.NewLine;
                }


            }

        }

        public static readonly string[] NEW_LINE_SPLIT = new[] { "\r\n", "\r", "\n" };

        /// <summary>
        /// 
        /// Code inspired by:
        /// https://stackoverflow.com/questions/1757065/java-splitting-a-comma-separated-string-but-ignoring-commas-in-quotes
        /// </summary>
        /// <param name="line">The line to split</param>
        /// <param name="escape">the escape character</param>
        /// <returns></returns>
        public String[] SplitOnCommaWithoutEscape(String input, Char escape1,Char escape2 )
        {
            List<String> result = new List<String>();
            int start = 0;
            Boolean inQuotes = false;
            for (int current = 0; current < input.Length; current++)
            {
                if (input[current] == escape1) inQuotes = true; // toggle state
                if (input[current] == escape2) inQuotes = false; // toggle state
                Boolean atLastChar = (current == input.Length - 1);
                if (atLastChar)
                {
                    result.Add(input.Substring(start));
                }
                else if (input[current] == ',' && !inQuotes)
                {
                    result.Add(input.Substring(start, current-start));
                    start = current + 1;
                }
            }
            return result.ToArray();
        }

        #region SQL Specific funtions

        /// <summary>
        /// Returns the result of 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public string sqlFlatFile(string fullText,string block)
        {
            //remove variables

            string varname = getSimpleVarValue("var", block);
            string stringVarName = getSimpleVarValue("sql_string_var", block);
            string tables = getSimpleVarValue("tables", block);
            string order = getSimpleVarValue("order", block);
            //parse acutal code

            String sqlQuery = "Select Concat(";

            List<String> onlySqlVars = block.Split(NEW_LINE_SPLIT, StringSplitOptions.RemoveEmptyEntries).Where(x => x.Contains(",")).ToList();

            List<String> allOutputVars = new List<string>();

            foreach (String sqlVarLine in onlySqlVars)
            {
                //split by commas
                //first two are always the same, anything else is an operation that should be performed
                String[] commaStrings = SplitOnCommaWithoutEscape(sqlVarLine, '(', ')');//sqlVarLine.Split(',');

                if (commaStrings.Length >= 2)
                {
                    string varName = commaStrings[0];
                    int length = int.Parse(commaStrings[1]);
                    string padCharacters = "' '";//defaults to space
                    string usedVarName = varName;
                    Boolean rightPad = true;

                    usedVarName = "isnull(" + usedVarName + ",''" + ")"; //default add isNull

                    if (commaStrings.Length > 2)
                    {
                        
                        for (int i = 2; i < commaStrings.Length; i++)
                        {
                            string extraCommand = commaStrings[i];
                            if (extraCommand.Contains("remove"))
                            {
                                List<String> args = parseFunction("remove",extraCommand);
                                //remove these characters from the text pulled by sql
                                foreach (String arg in args)
                                {
                                    usedVarName = "replace(" + usedVarName + ",\'" + arg + "\',\'\')";
                                }
                                
                            }
                            if (extraCommand.Contains("pad"))
                            {
                                List<String> args = parseFunction("pad", extraCommand);
                                padCharacters = args[0];
                            }
                            if (extraCommand.Contains("leftPad"))
                            {
                                rightPad = false;
                            }
                        }
                     }//end of special conditions
                       
                        

                        //run the variables through the process of padding
                        //TODO a bunch of cool SQL generation utilities
                        string padVarOutput = "replicate(" + padCharacters + "," + length + " - Len("+usedVarName+"))";
                        string actualVarOutput = usedVarName;
                    if (rightPad)
                    {
                        allOutputVars.Add(actualVarOutput);
                        allOutputVars.Add(padVarOutput);
                    }
                    else
                    {
                        allOutputVars.Add(padVarOutput);
                        allOutputVars.Add(actualVarOutput);
                        
                    }
                    //replicate args
                }


            }//end foreach

            sqlQuery += allOutputVars.Aggregate((i, j) => i + "," + j);

            sqlQuery += ")";//END of select
            if (tables.Length >= 1)
                sqlQuery += " FROM " + tables;
            if (order.Length>=1)
            sqlQuery += " ORDER BY " + order;


            fullText = varReplace(fullText,varname,sqlQuery);
            fullText = varReplace(fullText, stringVarName, StringToSqlString(sqlQuery));
            return fullText;
        }

        private List<string> parseFunction(string functionName, string str)
        {
            str = str.Replace(functionName+"(", "").Replace(")", "");
            return str.Split('|').ToList();
        }

        private string getSimpleVarValue(string var, string block)
        {
            if (block.Contains(var))
            {
                Regex findVarNameRegex = new Regex( "(" + var + "[^\\n]+)");
                Match m = findVarNameRegex.Match(block);
                Group g = m.Groups[0];
                String varName = g.ToString();
                varName = varName.Trim();
                varName = varName.Remove(0, var.Length);
                varName = varName.Trim();
                return varName;
            }
            else
            {
                return "";
            }
        }

        public static string varReplace(string fullText, String varName, String replaceWith)
        {
            String rtrn = fullText.Replace("var " + varName, "varDefineTempStr").Replace(varName, replaceWith).Replace("varDefineTempStr","var " + varName);
            return rtrn;
        }

        public static String StringToSqlString(String inputText)
        {
            String rtrn = inputText;
            rtrn = rtrn.Replace("\t", " "); //remove tabs
            rtrn = rtrn.Replace(Environment.NewLine, ""); //remove
            rtrn = rtrn.Replace("'", "''"); //escape single quotes
            for (int i = 0; i < 10; i++)
            {
                rtrn = rtrn.Replace("  ", " "); //replace double spaces with single spaces.
            }
            return rtrn;
        }

        public static String sqlVarList(String fullText, String block)
        {
            String rtrn = fullText;

            String xmlBlock = TextUtils.removeMultiSpaces(block);
            /*
        <parameter ID="@FROM_EMPLOYER_ACCOUNT_ID" sfwDataType="int" />
        <parameter ID="@TO_EMPLOYER_ACCOUNT_ID" sfwDataType="int" />
        <parameter ID="@aintNoOfQuarterForDelinquency" sfwDataType="int" />
        */
            //step 1 Load in XML
            List<SQL_Variable> sqlVars = new List<SQL_Variable>();
            var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlBlock), settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.AttributeCount >= 2)
                            {
                                //Step 2, create objects
                                String varName = reader.GetAttribute("ID");
                                String varType = reader.GetAttribute("sfwDataType");
                                SQL_Variable sqlVar = new SQL_Variable();
                                sqlVar.varName = varName;
                                sqlVar.varType = varType;
                                sqlVar.setDefaultValue();
                                sqlVars.Add(sqlVar);


                            }
                            break;
                    }
                }
            }
            
            

            //step 3, output SQL defines
            if (sqlVars.Count >= 1)
            {
                String sqlVarDefine = sqlVars.Select(x => x.getVarDeclare())
                    .Aggregate((i, j) => i + Environment.NewLine + j);
                String replaceFrom = addDirectives(block, SQL_VARLIST_GEN);
                rtrn = rtrn.Replace(replaceFrom, sqlVarDefine);
            }
            return rtrn;
        }
        #endregion


        #region Advanced Find and Replace
        public string foreachVarList(string fullText, string block)
        {
            String replaceBlockWith = "";// codeComment(block);

            //seperate out content and varlist
            List<String> blocks = getBlocksForDirective(block, FOR_EACH_CONTENT);

            string metadataOnly = removeDirectives(block, FOR_EACH_CONTENT);

            String[] splitStr = metadataOnly.Split(NEW_LINE_SPLIT,StringSplitOptions.RemoveEmptyEntries);
            
            String[] replaceVars;
            if ((splitStr[0].Contains(",") && !useTabDelimiter) || (splitStr[0].Contains("\t") && useTabDelimiter))
            {
                if (useTabDelimiter)
                {
                    replaceVars = TextUtils.splitOnTab(splitStr[0]);
                }
                else
                {
                    replaceVars = TextUtils.splitOnComma(splitStr[0]);
                }
            }
            else
            {
                replaceVars = new [] {splitStr[0]};
            }

            String[] replaceWithVars = splitStr.Skip(1).ToArray();
            //first line contains find and replace

            List<String> replacedBlocks = new List<string>();

                //blocks -> [block][block]
                foreach (String contentBlock in blocks)
                {
                String blockReplicate = "";
                      
                foreach (String replaceWithCSV in replaceWithVars)
                        {
                            // x,y,z -> [x][y][z]
                        String[] replaceWith;

                        if (useTabDelimiter)
                        {
                            replaceWith = TextUtils.splitOnTab(replaceWithCSV);
                        }
                        else
                        {
                            replaceWith = TextUtils.splitOnComma(replaceWithCSV);
                        }

                        String tempBlock = contentBlock;

                        for (int i = 0; i < replaceVars.Length; i++)
                        {
                            tempBlock = tempBlock.Replace(replaceVars[i], replaceWith[i]);
                        }

                            blockReplicate += tempBlock; // + Environment.NewLine;
                        }
                        replacedBlocks.Add(blockReplicate);
            }
            replaceBlockWith += replacedBlocks.Select(x => x).Aggregate((i, j) => i + j);


            return replaceBlockWith;

        }


        #endregion

        /// <summary>
        /// Adds Directives back to the text. (used after processing)
        /// </summary>
        /// <param name="block"></param>
        /// <param name="directive"></param>
        /// <returns></returns>
        public static String addDirectives(String block, String directive)
        {
            String directiveStart = directive + " " + BLOCK_START;
            String directiveEnd = directive + " " + BLOCK_END;

            return directiveStart + block + directiveEnd;
        }

        public string removeDirectives(String str, String directive)
        {
            String rtrn = str;

            String directiveStart = directive + " " + BLOCK_START;
            String directiveEnd = directive + " " + BLOCK_END;

            string[] splitOnDirectives = str.Split(new[] { directiveStart }, StringSplitOptions.None);

            foreach (String block in splitOnDirectives.Where(x => x.Contains(directiveEnd)))
            {
                String parsedBlock = block.Remove(block.IndexOf(directiveEnd, 0));
                rtrn = rtrn.Replace(parsedBlock,"");
            }

            rtrn = rtrn.Replace(directiveStart, "");
            rtrn = rtrn.Replace(directiveEnd, "");

            return rtrn;
        }

        /// <summary>
        /// NESTING NOT SUPPORTED
        /// </summary>
        /// <param name="directive"></param>
        /// <returns></returns>
        public static List<String> getBlocksForDirective(String str,String directive)
        {
            List<String> blocks = new List<string>();
            String directiveStart = directive +" " + BLOCK_START;
            String directiveEnd = directive +" "+ BLOCK_END;

            string[] splitOnDirectives = str.Split(new [] {directiveStart},StringSplitOptions.None);

            foreach (String block in splitOnDirectives.Where(x=>x.Contains(directiveEnd)))
            {
                String parsedBlock = block.Remove(block.IndexOf(directiveEnd, 0));
                blocks.Add(parsedBlock);
            }

            return blocks;

        }

        public List<string> findDirectives(string str)
        {
            List<String> directivesFound = new List<string>();
            foreach (String directive in DIRECTIVES)
            {
                if (findDirective(str,directive))
                {
                    directivesFound.Add(directive);
                }
            }

            return directivesFound;
        }

        public static bool findDirective(string str, string directive)
        {
            if (str.ToLower().Contains(directive))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public String codeComment(String strToComment)
        {
            String commentStart = "", commentEnd = "";
            if (LanguageUsing.Equals(LANGUAGE_C_SHARP) || LanguageUsing.Equals(LANGUAGE_JAVA) || LanguageUsing.Equals(LANGUAGE_SQL))
            {
                commentStart = "/*";
                commentEnd = "*/";
            } else if (LanguageUsing.Equals(LANGUAGE_XML))
            {
                commentStart = "<!--";
                commentEnd = "-->";
            }
            return commentStart + Environment.NewLine + strToComment + Environment.NewLine + commentEnd + Environment.NewLine;
        }
        public String codeCommentLine(String strToComment)
        {
            if (LanguageUsing.Equals(LANGUAGE_SQL))
            {
                //SQL support
                return "--" + strToComment;
            }
            else if (LanguageUsing.Equals(LANGUAGE_C_SHARP) || LanguageUsing.Equals(LANGUAGE_JAVA))
            {
                //Java / C# support
                return "//" + strToComment;
            }
            else
            {
                //XML support
                return "<!-- " + strToComment + " -->";
            }
        }
    }
}
