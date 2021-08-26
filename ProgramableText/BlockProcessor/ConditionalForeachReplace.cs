using ProgramableText.BlockProcessor.Conditions;
using ProgramableText.LogProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.BlockProcessor
{
    /// <summary>
    /// This creates text conditionally based on inputs from each line
    /// 
    /// 
    /// Syntax:
    /// 
    /// C_VAR_1 <-- varname
    /// VAR1X Contains(ID) Long
    /// VAR1X Contains(Date) XMLSerializableDate
    /// NEXT <-- Next keyword to indicate the next line will be a variable
    /// </summary>
    class ConditionalForeachReplace : BlockNode
    {
        const String DELIMITER = "Delimiter";
        const String PARAMETERS = "Parameters";
        const String CONDITIONALS = "Conditionals";
        const String TEXT = "Text";

        const String NEXT = "NEXT";

        String DelimiterText, ParameterText, ConditionalText , CreationText;

        String[] Delimiter;
        List<String> VarList;
        List<Conditional> Conditionals;

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
                List<String> conditionalReplacements = new List<String>();
                //Determine Conditional Values
                if (Conditionals.Count>=1)
                {
                    foreach (Conditional c in Conditionals)
                    {
                        conditionalReplacements.Add(c.Compute(replacements));
                    }
                }

                String editingBlock = CreationText;

                for (int i = 0; i < conditionalReplacements.Count; i++)
                {
                    String oldStr = Conditionals[i].varName;
                    String newStr;
                    if (replacements.Count > i)
                    {
                        newStr = conditionalReplacements[i];
                    }
                    else
                    {
                        newStr = "";
                    }
                    editingBlock = editingBlock.Replace(oldStr, newStr);
                }

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
            args.Add(CONDITIONALS);
            args.Add(TEXT);
           

            return generateExample(args);
        }

        public override BlockNode createInstance()
        {
            return new ConditionalForeachReplace();
        }

        public override string getOpName()
        {
            return "ConditionalForeachReplace";
        }

        public override void parseBlocks(string input)
        {
            DelimiterText = getInternalBlockText(DELIMITER, input);
            ParameterText = getInternalBlockText(PARAMETERS, input);
            ConditionalText = getInternalBlockText(CONDITIONALS, input);
            CreationText = getInternalBlockText(TEXT, input);

            //Parse Delimiter
            Delimiter = new String[] { ProgramNode.loadString(DelimiterText) };
            //Parse Parameter Text
            VarList = ParameterText.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries).Select(x=> ProgramNode.loadString(x)).ToList();

            var cSplit = ConditionalText.Split(ProgramNode.NEWLINE, StringSplitOptions.RemoveEmptyEntries).Select(x => ProgramNode.loadString(x)).ToList();

            Conditionals = new List<Conditional>();

            Conditional workingConditional = new Conditional();
            foreach (String line in cSplit)
            {
                if (workingConditional.varName.Length==0)
                {
                    //Set the varname to the first line
                    workingConditional.varName = line;
                    continue;
                } else if (line.Contains(NEXT))
                {
                    //This is a NEXT Statement, to advance the variable
                    Conditionals.Add(workingConditional);
                    workingConditional = new Conditional();
                    continue;
                } else
                {
                    //The format of the line:
                    //VAR_LOOKUP | CONDITION | VAR_VALUE
                    String[] cLineSplit = line.Split(ProgramNode.BAR, StringSplitOptions.RemoveEmptyEntries);
                    if (cLineSplit.Length >=2)
                    {
                        //Size 2 is allowed, because text can be blank
                        if (VarList.Contains(cLineSplit[0])) {
                            workingConditional.varLookups.Add(VarList.IndexOf(cLineSplit[0]));
                        } else
                        {
                            //Attempt to parse this as an integer
                            workingConditional.varLookups.Add(ProgramNode.loadInt(cLineSplit[0]));
                        }

                        //Attept to load condition

                        Condition c = Condition.loadCondition(cLineSplit[1]);
                        workingConditional.conditions.Add(c);

                        String result = "";
                        if (cLineSplit.Length>=3)
                        {
                            result = ProgramNode.loadString(cLineSplit[2]);
                        }
                        workingConditional.values.Add(result);

                    }
                }
            }

        }

        public override string ToString()
        {
            return getOpName();
        }

    }

    public class Conditional
    {
        public String varName = "";
        public List<Condition> conditions = new List<Condition>();
        public List<String> values = new List<string>();
        public List<int> varLookups = new List<int>();

        public string Compute(List<string> replacements)
        {
            for (int i = 0; i< conditions.Count;i++)
            {
                if (conditions[i].calculate(replacements[varLookups[i]]))
                {
                    return values[i];
                }
            }
            return "";
        }
    }
}
