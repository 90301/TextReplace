﻿using ProgramableText.BlockProcessor;
using ProgramableText.BlockProcessor.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    static class LogProcessor
    {
        public static readonly String[] LINE_SPLIT = { Environment.NewLine };
        public static readonly String[] OP_SPLIT = { Environment.NewLine };
        public static readonly String[] NAME_SPLIT = { "(" };
        public static readonly String[] ARG_SPLIT = { "," };
        public static readonly String[] BLOCK_NAME_SPLIT = { BlockNode.START };

        public const String CLOSING_PARENTHSES = "SPC_CODE_CP";
        public const String COMMA = "SPC_CODE_COMMA";
        public const String SPACE = "SPC_CODE_SPACE";

        /// <summary>
        /// Lookup data structure
        /// </summary>
        public static Dictionary<String, ProgramNode> allNodes;
        public static Dictionary<String, BlockNode> allBlockNodes;

        public static List<ProgramNodeInterface> allProgramNodeInterfaces;

        public static Dictionary<String,Condition> allConditions;

        public static Dictionary<String, String> specialCharacters;

        public static List<ProgramNodeInterface> nodes;

        public static List<String> filesToProcess;
        //public static int step;
        //public static List<ProgramNodeInterface> subQueue;
        public static Boolean loopback = false;

        public static List<String> registers;

        public static String fileProcessing;

        public static InnerReadNode argReader = new InnerReadNode();

        //public static String[] lines;

        public static String errors, output, inputText, nodeToString, programText;
        // --- --- --- Debug --- --- ---
        public static List<String> outputSteps;
        public static List<String> opSteps;

        public static Boolean debugMode = false;
        static LogProcessor()
        {
            argReader.parseArgs(new String[] { "(", ")" });

            allNodes = new Dictionary<string, ProgramNode>();
            allBlockNodes = new Dictionary<string, BlockNode>();
            allProgramNodeInterfaces = new List<ProgramNodeInterface>();
            allConditions = new Dictionary<string, Condition>();
            specialCharacters = new Dictionary<string, string>();

            addAllNode(new InnerReadNode());
            addAllNode(new FilterNode());
            addAllNode(new FilterExclude());
            addAllNode(new WordSearch());
            addAllNode(new DirectoryLoad());
            addAllNode(new LoadFiles());
            addAllNode(new ProcessFiles());
            addAllNode(new NextFile());
            addAllNode(new ReadFromRegister());
            addAllNode(new WriteToRegister());
            addAllNode(new VariableTransform());
            addAllNode(new SetOutput());
            addAllNode(new GetFileName());
            addAllNode(new RemoveDuplicates());
            addAllNode(new RegisterFindAndReplace());
            addAllNode(new FindPlusLines());
            addAllNode(new LineInstanceCount());
            addAllNode(new ReplaceCount());
            addAllNode(new ReplaceIn());
            addAllNode(new StartLineWith());
            addAllNode(new EndLineWith());
            addAllNode(new LineTrim());

            addAllNode(new MultilineFindAndReplace());
            addAllNode(new IfStatement());

            //conditions
            addCondition(new Contains());

            //special characters
            specialCharacters.Add(CLOSING_PARENTHSES, ")");
            specialCharacters.Add(COMMA, ",");
            specialCharacters.Add(SPACE, " ");
        }

        public static void addAllNode(ProgramNode node)
        {
            allNodes.Add(node.getOpName(), node);
            allProgramNodeInterfaces.Add(node);
        }
        public static void addAllNode(BlockNode node)
        {
            allBlockNodes.Add(node.getOpName(), node);
            allProgramNodeInterfaces.Add(node);
        }

        public static void addCondition(Condition node)
        {
            allConditions.Add(node.getOpName(), node);
            allProgramNodeInterfaces.Add(node);
        }

        public static void loadProgram(String program)
        {
            programText = program;
            //String programLines = loadBlocks(program);
            //lines = programLines.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            compileProgram(programText,out nodes);
            processNodeToString();
        }


        public static void processNodeToString()
        {
            nodeToString = "";
            foreach (ProgramNodeInterface node in nodes)
            {
                nodeToString += node.ToString() + Environment.NewLine;
            }
        }
        #region Compile
        public static void compileProgram(String programText, out List<ProgramNodeInterface> nodes)
        {
            nodes = new List<ProgramNodeInterface>();

            errors = "";
            output = "";

            //TODO: Update to "Remaining Program" model
            String textLeft = programText;

            //Split into lines
            String[] linesLeft = textLeft.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            
            while (textLeft.Length >=1 && linesLeft.Length >= 1)
            {
                try
                {
                    textLeft = textLeft.TrimStart(Environment.NewLine.ToCharArray());

                    linesLeft = textLeft.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);

                    if (linesLeft[0].Contains(BlockNode.START))
                    {
                        textLeft = parseFirstBlockNode(textLeft);
                        linesLeft = textLeft.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        //TODO move this code into a method
                        String line = linesLeft[0].Trim();
                        String op = getOpName(line);

                        ProgramNode node = getOpByName(op, allNodes);

                        if (node == null)
                        {
                            errors += "Failed to find OP: " + op;
                            textLeft = nextLine(textLeft, out linesLeft);
                            continue;
                        }

                        //Load the arguments of the method, comma seperated (arg1,arg2)
                        node.parseArgs(
                            splitArgString(getArgString(line))
                        );
                        nodes.Add(node);

                        textLeft = nextLine(textLeft, out linesLeft);
                    }

                } catch (Exception e)
                {
                    addError(e.Message);
                    addError(e.StackTrace);
                }

            }

        }
        /// <summary>
        /// Returns non seperated arg string
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static String getArgString(String line)
        {
            return argReader.getArrayResults(line)[0];
        }
        /// <summary>
        /// Splits the arg string into an array of args
        /// </summary>
        /// <param name="argString"></param>
        /// <returns></returns>
        public static String[] splitArgString(String argString)
        {
            return argString.Split(ARG_SPLIT, StringSplitOptions.RemoveEmptyEntries);
        }

        public static String getOpName(String line)
        {
            String lineClean = line.Trim();

            String op = lineClean.Split(NAME_SPLIT, StringSplitOptions.RemoveEmptyEntries)[0];

            return op;
        }

        public static T getOpByName<T>(String op,Dictionary<String,T> nodeDictionary) where T : ProgramNode
        {
            if (nodeDictionary.ContainsKey(op))
            {
                T node = (T)nodeDictionary[op].createInstance();

                return node;
            }
            else
            {
                errors += "Failed to find OP: " + op;
                return null;
            }
        }
        public static Condition getConditionByName(String op, Dictionary<String, Condition> nodeDictionary)
        {
            if (nodeDictionary.ContainsKey(op))
            {
                Condition node = nodeDictionary[op].createInstance();

                return node;
            }
            else
            {
                errors += "Failed to find Condition OP: " + op;
                return null;
            }
        }
        public static String nextLine(String textLeft, out String[] linesLeft)
        {
            int index;
            if (textLeft.Contains(Environment.NewLine))
            {
                index = textLeft.IndexOf(Environment.NewLine);
            } else
            {
                index = textLeft.Length;
            }
            textLeft = textLeft.Substring(index);
            linesLeft = textLeft.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            return textLeft;
        }

        public static string parseFirstBlockNode(string program)
        {
            int firstBlockStart = int.MaxValue;
            int firstBlockEnd = int.MaxValue;
            String firstBlock = "";
            //Get the next block op name
            String opName = program.Trim().Split(BLOCK_NAME_SPLIT, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            if (allBlockNodes.ContainsKey(opName))
            {
                BlockNode node = allBlockNodes[opName];

                int startLocation, endLocation;
                String block =
                BlockNode.getFullStartAndEndLocations
                    (program, node.getOpName(), out startLocation, out endLocation);
                if (firstBlockStart > startLocation)
                {
                    firstBlockStart = startLocation;
                    firstBlockEnd = endLocation;
                    firstBlock = block;
                }
                //store the program in the list
                loadBlockProgram(firstBlock);
            } else
            {
                addError("Could not find block node with Op Name: " + opName);
            }
            
            String strippedProgram = program.Substring(firstBlockEnd);
            return strippedProgram;
        }
        public static void loadBlockProgram(String parsedBlock)
        {
            //TODO search for nested programs.
            String firstLine = parsedBlock.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries)[0];
            String nodeName = firstLine.Replace(BlockNode.START, "").Trim();
            BlockNode node;
            if (allBlockNodes.TryGetValue(nodeName, out node))
            {
                
                BlockNode createdNode = node.createInstance();
                String internalBlock = BlockNode.getInternalBlockText(createdNode.getOpName(), parsedBlock);
                createdNode.parseBlocks(internalBlock);
                nodes.Add(createdNode);

            }
            else
            {
                //Throw Error
                errors += "Failed to parse line: " + firstLine;
                return;
            }
        }

        public static void loadInputText(String input)
        {
            inputText = input;
        }

        #endregion
        /// <summary>
        /// Process text
        /// </summary>
        public static void process(List<ProgramNodeInterface> nodes,Boolean reset)
        {
            if (reset)
            {
                registers = new List<string>();
                opSteps = new List<string>();
                outputSteps = new List<string>();
                errors = "";
                output = "";
            }
            int step = 0;
            int lastLoadFilesStep = 0;
            

            // Process Text
            try
            {
                String processedText = inputText;
                while (step < nodes.Count || loopback)
                {
                    if (loopback)
                    {
                        loopback = false;
                        step = lastLoadFilesStep;
                    } else
                    {
                        if (nodes[step] is LoadFiles)
                        {
                            lastLoadFilesStep = step + 1;
                        }
                    }
                    ProgramNodeInterface node = nodes[step];
                    
                    processedText = node.calculate(processedText);
                    if (debugMode)
                    {
                        outputSteps.Add(processedText);
                        opSteps.Add(nodes[step].ToString());
                    }

                    step++;

                }

                output = processedText;

            } catch (Exception e)
            {
                errors += e.Message + Environment.NewLine + e.StackTrace;
            }

        }

        public static String processNextFile(Boolean loopback)
        {
            String fileLocation = filesToProcess[0];
            try
            {
                string text = System.IO.File.ReadAllText(fileLocation);
                fileProcessing = fileLocation;
                filesToProcess.RemoveAt(0);
                LogProcessor.loopback = loopback;
                return text;
            } catch (Exception e)
            {
                addError("Failed to open file: " + fileLocation);
                addError(e.Message);
                addError(e.StackTrace);
            }
            return "";
        }

        public static void saveChanges(String text)
        {
            System.IO.File.WriteAllText(fileProcessing,text);

        }

        //Maybe move this to another location?

        public static string specialCharacterReplacement(String input)
        {
            //input = input.Replace(CLOSING_PARENTHSES, ")");
            //input = input.Replace(COMMA, ",");
            foreach (String key in specialCharacters.Keys)
            {
                input = input.Replace(key, specialCharacters[key]);
            }

            return input;
        }

        public static void addError(String err)
        {
            errors += err + Environment.NewLine;
        }

    }
}
