using ProgramableText.BlockProcessor;
using ProgramableText.BlockProcessor.Conditions;
using ProgramableText.LogProcessor.XMLProcessor;
using ProgramableText.Utils;
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

        public const String COMMENT_START = "//";

        public const String CLOSING_PARENTHSES = "SPC_CODE_CP";
        public const String COMMA = "SPC_CODE_COMMA";
        public const String SPACE = "SPC_CODE_SPACE";
        public const String TAB = "SPC_CODE_TAB";
        public const String SINGLE_QUOTE = "SPC_CODE_SQ";
        public const String NEWLINE = "SPC_CODE_NL";

        public const String DEFAULT_INPUT_TEXT = "_DEFAULT_INPUT_TEXT_";
        /// <summary>
        /// Lookup data structure
        /// </summary>
        public static Dictionary<String, ProgramNode> allNodes;
        public static Dictionary<String, BlockNode> allBlockNodes;

        public static List<ProgramNodeInterface> allProgramNodeInterfaces;

        public static Dictionary<String,Condition> allConditions;

        public static Dictionary<String, String> specialCharacters = new Dictionary<string, string>();

        public static Dictionary<String, FunctionalBlock> functionalBlocks = new Dictionary<string, FunctionalBlock>();

        public static List<ProgramNodeInterface> nodes;

        public static List<String> filesToProcess;
        //public static int step;
        //public static List<ProgramNodeInterface> subQueue;
        public static Boolean loopback = false;

        public static List<String> registers;
        public static Dictionary<String,LogVar> variables;

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
            ConfigFile.loadConfig();

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

            addAllNode(new SetOutput());
            addAllNode(new GetFileName());
            addAllNode(new RemoveDuplicates());
            //Register Code
            addAllNode(new ReadFromRegister());
            addAllNode(new WriteToRegister());
            addAllNode(new VariableTransform());
            addAllNode(new RegisterFindAndReplace());
            addAllNode(new RegisterFilter());

            addAllNode(new FindPlusLines());
            addAllNode(new LineInstanceCount());
            addAllNode(new ReplaceCount());
            addAllNode(new ReplaceIn());
            addAllNode(new StartLineWith());
            addAllNode(new EndLineWith());
            addAllNode(new LineTrim());
            addAllNode(new CSVCombine());
            addAllNode(new CallFunctionBlock());
            addAllNode(new GetWordInLine());
            addAllNode(new SyntaxParse());
            addAllNode(new LoadProgram());

            //xml
            addAllNode(new ReadXML());
            addAllNode(new GetAllElements());
            addAllNode(new GetParentNode());

            //Code

            addAllNode(new MultilineFindAndReplace());
            addAllNode(new IfStatement());
            addAllNode(new FunctionalBlock());
            addAllNode(new SimpleForeachReplace());
            addAllNode(new ConditionalForeachReplace());


            //conditions
            addCondition(new Contains());
            addCondition(new ContainsAny());


            //special characters
            specialCharacters.Add(CLOSING_PARENTHSES, ")");
            specialCharacters.Add(COMMA, ",");
            specialCharacters.Add(SPACE, " ");
            specialCharacters.Add(TAB, "\t");
            specialCharacters.Add(SINGLE_QUOTE, "'");
            specialCharacters.Add(NEWLINE, Environment.NewLine);
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
            String textLeft = removeComments(programText);

            //Split into lines
            String[] linesLeft = textLeft.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            
            while (textLeft.Length >=1 && linesLeft.Length >= 1)
            {
                try
                {
                    textLeft = textLeft.TrimStart(Environment.NewLine.ToCharArray());

                    linesLeft = textLeft.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);

                    if (linesLeft[0].Contains(BlockNode.START) && !linesLeft[0].Contains(")"))
                    {
                        BlockNode node;
                        textLeft = parseFirstBlockNode(textLeft, out node);
                        if (node != null)
                        {
                            nodes.Add(node);
                        }
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
                    addError(textLeft);
                    break;
                }

            }

        }

        public static string removeComments(string programText)
        {
            var split = programText.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            String processed = "";
            foreach(String line in split)
            {
                if (!line.Trim().StartsWith(COMMENT_START))
                {
                    processed += line + System.Environment.NewLine;
                }
            }
            return processed;
        }

        /// <summary>
        /// Returns non seperated arg string
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static String getArgString(String line)
        {
            return argReader.calculate(line);
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

        public static string parseFirstBlockNode(string program, out BlockNode node)
        {
            int firstBlockStart = int.MaxValue;
            int firstBlockEnd = int.MaxValue;
            String firstBlock = "";
            //Get the next block op name
            String opName = program.Trim().Split(BLOCK_NAME_SPLIT, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            if (allBlockNodes.ContainsKey(opName))
            {
                BlockNode foundNode = allBlockNodes[opName];

                int startLocation, endLocation;
                String block =
                BlockNode.getFullStartAndEndLocations
                    (program, foundNode.getOpName(), out startLocation, out endLocation);
                if (firstBlockStart > startLocation)
                {
                    firstBlockStart = startLocation;
                    firstBlockEnd = endLocation;
                    firstBlock = block;
                }
                //store the program in the list
                node = loadBlockProgram(firstBlock);
            } else
            {
                node = null;
                addError("Could not find block node with Op Name: " + opName);
            }
            
            String strippedProgram = program.Substring(firstBlockEnd);
            return strippedProgram;
        }
        public static BlockNode loadBlockProgram(String parsedBlock)
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
                //nodes.Add(createdNode);
                return createdNode;
            }
            else
            {
                //Throw Error
                errors += "Failed to parse line: " + firstLine;
                return null;
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
        public static void process(List<ProgramNodeInterface> nodes,Boolean reset, String startInput = DEFAULT_INPUT_TEXT)
        {
            if (reset)
            {
                registers = new List<string>();
                variables = new Dictionary<string, LogVar>();
                opSteps = new List<string>();
                outputSteps = new List<string>();
                functionalBlocks = new Dictionary<string, FunctionalBlock>();
                errors = "";
                output = "";
                AutoSave.saveLogProcessor(programText, inputText);
            }
            int step = 0;
            int lastLoadFilesStep = 0;
            

            // Process Text
            try
            {
                if (startInput.Equals(DEFAULT_INPUT_TEXT))
                {
                    startInput = inputText;
                }
                String processedText = startInput;
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

        public static String cachedFileText = "";
        public static String processNextFile(Boolean loopback, Boolean createFile = false)
        {
            String fileLocation = filesToProcess[0];
            try
            {
                //IF file doesn't exist, create it?
                if (createFile)
                {
                    if (!System.IO.File.Exists(fileLocation))
                    {
                        var fileStream = System.IO.File.CreateText(fileLocation);
                        fileStream.Close();
                    }
                }
                string text = System.IO.File.ReadAllText(fileLocation);
                cachedFileText = text;
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
            if (!text.Equals(cachedFileText))
            {
                System.IO.File.WriteAllText(fileProcessing, text);
            }

        }

        //Maybe move this to another location?

        public static string specialCharacterReplacement(String input)
        {
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
