using ProgramableText.LogProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.BlockProcessor
{
    /// <summary>
    /// This creates text based on inputs from each line
    /// 
    /// </summary>
    class SimpleForeachReplace : BlockNode
    {
        const String DELIMITER = "Delimiter";
        const String PARAMETERS = "Parameters";
        const String TEXT = "Text";

        String DelimiterText, ParameterText, CreationText;

        String[] Delimiter;
        List<String> VarList;

        public override string calculate(string input)
        {
            //TODO add CASE support to select text based on conditions
            //EX var2 include Id => var3 = Int

            String rtrn = "";
            String[] splitLines = input.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries);

            foreach (String line in splitLines)
            {
                //process text line by delimiters
                List<String> replacements;
                if (Delimiter[0].Length>=1 && line.Contains(Delimiter[0]))
                {
                    replacements = line.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries).ToList();
                } else
                {
                    replacements = new List<string>();
                    replacements.Add(line);
                }
                String editingBlock = CreationText;
                for (int i = 0; i < VarList.Count; i++)
                {
                    String oldStr = VarList[i];
                    String newStr;
                    if (replacements.Count > i)
                    {
                        newStr = replacements[i];
                    } else
                    {
                        newStr = "";
                    }
                    editingBlock = editingBlock.Replace(oldStr, newStr);
                }
                rtrn += editingBlock + Environment.NewLine;
            }

            return rtrn;
        }

        public override string createExample()
        {
            List<String> args = new List<string>();
            args.Add(DELIMITER);
            args.Add(PARAMETERS);
            args.Add(TEXT);
           

            return generateExample(args);
        }

        public override BlockNode createInstance()
        {
            return new SimpleForeachReplace();
        }

        public override string getOpName()
        {
            return "SimpleForeachReplace";
        }

        public override void parseBlocks(string input)
        {
            DelimiterText = getInternalBlockText(DELIMITER, input);
            ParameterText = getInternalBlockText(PARAMETERS, input);
            CreationText = getInternalBlockText(TEXT, input);

            //Parse Delimiter
            Delimiter = new String[] { ProgramNode.loadString(DelimiterText) };
            //Parse Parameter Text
            VarList = ParameterText.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries).Select(x=> ProgramNode.loadString(x)).ToList();
        }

        public override string ToString()
        {
            return getOpName();
        }

    }
}
