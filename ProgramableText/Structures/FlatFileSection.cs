using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramableText.Utils;

namespace ProgramableText.Structures
{
    public class FlatFileSection
    {

        public FlatFileSection(string sectionText)
        {
            String[] lines = TextUtils.splitOnNewLine(sectionText);
            //take the first line, that's the condition
            this.conditionString = lines[0];
            string[] splitCondition = this.conditionString.Split('=');
            this.fieldName = splitCondition[0].Trim();
            this.fieldValue = splitCondition[1].Trim();
            //remaining lines are converted to format list.
            string[] linesFormat = lines.Skip(1).ToArray();
            formatList.Clear();
            //String[] lines = TextUtils.splitOnNewLine(textFormat);
            foreach (string line in linesFormat)
            {
                String[] fields = line.Split(new[] { "," }, StringSplitOptions.None);
                if (fields.Length >= 2)
                {
                    String fieldNameLocal = fields[0].Trim();
                    String fieldTextSize = fields[1].Trim();
                    int fieldSize = Int32.Parse(fieldTextSize);
                    if (fieldNameLocal.Equals(fieldName))
                    {
                        fieldNumber = formatList.Count;
                    }
                    formatList.Add(new KeyValuePair<string, int>(fieldNameLocal, fieldSize));
                }
            }

            //condition

        }

        //conditions
        public string conditionString { get; set; } //The condition string (not processed yet.)
        
        //TODO move this into a map or something
        public string fieldName { get; set; }
        public int fieldNumber { get; set; }
        public string fieldValue { get; set; }


        public List<KeyValuePair<String, int>> formatList { get; set; } = new List<KeyValuePair<string, int>>();

        /// <summary>
        /// Lines of the CSV file (Used only when group by is enabled)
        /// </summary>
        public List<String> contentLines { get; set; } = new List<string>();

        /// <summary>
        /// Retruns parsed line (csv text)
        /// </summary>
        /// <param name="line"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static String parseSections(String fullFlatFile, List<FlatFileSection> section)
        {

            int lastLineSection = -1;
            string outputString = "";

            if (FlatFileReader.groupSections)
            {
                foreach (FlatFileSection flatFileSection in section)
                {
                    flatFileSection.contentLines.Clear();
                    
                }
            }

            foreach (string line in TextUtils.splitOnNewLine(fullFlatFile))
            {
                String csvLine = "";
                //if the last section used works, then just use that, if not, try all the sections one by one
                if (lastLineSection >=0 && section[lastLineSection].parseLine(line, out csvLine))
                {
                    if (FlatFileReader.groupSections)
                    {
                        section[lastLineSection].contentLines.Add(csvLine);
                    }
                    else
                    {
                        outputString += csvLine + Environment.NewLine;
                    }
                }
                else
                {
                    int sectionNumber = 0;
                    foreach (FlatFileSection flatFileSection in section)
                    {
                        if (flatFileSection.parseLine(line, out csvLine))
                        {
                            if (FlatFileReader.groupSections)
                            {
                                flatFileSection.contentLines.Add(csvLine);
                            }
                            else
                            {
                                outputString += flatFileSection.getHeaderText() + Environment.NewLine;
                                outputString += csvLine + Environment.NewLine;
                            }
                            
                            lastLineSection = sectionNumber;
                            break;
                        }
                        sectionNumber++;
                    }
                }
                
            }

            //group by output
            if (FlatFileReader.groupSections)
            {
                foreach (FlatFileSection flatFileSection in section)
                {
                    if (flatFileSection.contentLines.Count >= 1)
                    {
                        outputString += flatFileSection.getHeaderText() + Environment.NewLine;
                        outputString += flatFileSection.contentLines.Aggregate((i, j) => i + Environment.NewLine + j) + Environment.NewLine;
                    }
                }

            }

            return outputString;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="outputLine"></param>
        /// <returns>Is this a valid line for this section?</returns>
        public Boolean parseLine(String line, out String outputLine)
        {

            outputLine = "";
            int lastPosition = 0;
            int fieldNum = 0;
            foreach (var formKeyValuePair in formatList)
            {
                
                String value = line.Substring(lastPosition, formKeyValuePair.Value);
                outputLine += value + ",";
                lastPosition += formKeyValuePair.Value;
                //conditional code
                if (fieldNum == this.fieldNumber && !value.Equals(this.fieldValue))
                {
                    return false;
                }
                fieldNum++;
            }
            if (outputLine.Length >= 1)
            {
                outputLine = outputLine.Remove(outputLine.Length - 1, 1);
            }
            return true;
        }

        public String getHeaderText()
        {
            return formatList.Select(x => x.Key).Aggregate((i, j) => i + "," + j);
        }
                    
    }
}
