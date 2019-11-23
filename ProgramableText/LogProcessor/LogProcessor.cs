using ProgramableText.BlockProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    static class LogProcessor
    {

        private static readonly String[] OP_SPLIT = { Environment.NewLine };
        private static readonly String[] NAME_SPLIT = { "(" };
        private static readonly String[] ARG_SPLIT = { "," };


        /// <summary>
        /// Lookup data structure
        /// </summary>
        public static Dictionary<String, ProgramNode> allNodes;
        public static Dictionary<String, BlockNode> allBlockNodes;

        public static List<ProgramNodeInterface> allProgramNodeInterfaces; 

        public static List<ProgramNodeInterface> nodes;
        public static List<ExtraPass> extraPassNodes;
        public static List<ExtraPass> processedExtraPassNodes;

        public static InnerReadNode argReader = new InnerReadNode();

        //public static String[] lines;

        public static String errors, output, inputText, nodeToString, programText;

        public static List<String> outputSteps;
        static LogProcessor()
        {
            argReader.parseArgs(new String[] { "(", ")" });

            allNodes = new Dictionary<string, ProgramNode>();
            allBlockNodes = new Dictionary<string, BlockNode>();
            allProgramNodeInterfaces = new List<ProgramNodeInterface>();
            addAllNode(new InnerReadNode());
            addAllNode(new FilterNode());
            addAllNode(new WordSearch());
            addAllNode(new PlusBase());
            addAllNode(new ExtraPassNow());
            addAllNode(new DirectoryLoad());

            addAllNode(new MultilineFindAndReplace());
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

        public static void loadProgram(String program)
        {
            programText = program;
            //String programLines = loadBlocks(program);
            //lines = programLines.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            processProgram();
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

        public static void processProgram()
        {
            nodes = new List<ProgramNodeInterface>();
            extraPassNodes = new List<ExtraPass>();

            errors = "";
            output = "";

            //TODO: Update to "Remaining Program" model
            String textLeft = programText;

            //Split into lines
            String[] linesLeft = textLeft.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            
            while (textLeft.Length >=1 && linesLeft.Length >= 1)
            {

                if (linesLeft[0].Contains(BlockNode.START))
                {
                    textLeft = parseFirstBlockNode(textLeft);
                    linesLeft = textLeft.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
                } else
                {
                    //TODO move this code into a method
                    String line = linesLeft[0];
                    String lineClean = line.Trim();

                    String op = lineClean.Split(NAME_SPLIT, StringSplitOptions.RemoveEmptyEntries)[0];
                    ProgramNode node;

                    if (allNodes.ContainsKey(op))
                    {
                        node = (allNodes[op]).createInstance();
                    }
                    else
                    {
                        errors = "Failed to find OP: " + op;
                        textLeft = textLeft.Substring(textLeft.IndexOf(Environment.NewLine));
                        linesLeft = textLeft.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
                        continue;
                    }

                    //Load the arguments of the method, comma seperated (arg1,arg2)
                    node.parseArgs(
                        argReader.getArrayResults(lineClean)[0].Split(ARG_SPLIT, StringSplitOptions.RemoveEmptyEntries)
                        );
                    nodes.Add(node);

                    if (node.GetType().IsInstanceOfType(typeof(ExtraPass)))
                    {
                        extraPassNodes.Add((ExtraPass)node);
                    }

                    textLeft = textLeft.Substring(textLeft.IndexOf(Environment.NewLine));
                    linesLeft = textLeft.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
                }

            }

        }

        /// <summary>
        /// Returns remaining program lines (removes blocks)
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static string loadBlocks(string program)
        {
            String rtrn = program;
            while (allBlockNodes.Select(x => x)
                .Any(x => rtrn.Contains(x.Value.getStartBlock())))
            {
                rtrn = parseFirstBlockNode(rtrn);
            }
            return rtrn;
        }

        public static string parseFirstBlockNode(string program)
        {
            int firstBlockStart = int.MaxValue;
            int firstBlockEnd = int.MaxValue;
            String firstBlock = "";
            foreach (BlockNode node in allBlockNodes.Values)
            {
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
            }
            //store the program in the list
            loadBlockProgram(firstBlock);
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

        public static void addProcessedExtraPass(ExtraPass item)
        {
            processedExtraPassNodes.Add(item);
        }


        /// <summary>
        /// Process text
        /// </summary>
        public static void process()
        {
            processedExtraPassNodes = new List<ExtraPass>();

            outputSteps = new List<string>();
            errors = "";
            output = "";

            // Process Text
            String processedText = inputText;
            foreach (ProgramNodeInterface node in nodes)
            {

                processedText = node.calculate(processedText);
                outputSteps.Add(processedText);
                
            }

            processedText = processExtraPass(processedText);

            output = processedText;

        }

        public static String processExtraPass(String input)
        {
            string processedText = input;
            foreach (ExtraPass node in processedExtraPassNodes)
            {
                processedText = node.extraPass(processedText);
                outputSteps.Add(processedText);
            }
            processedExtraPassNodes.Clear();
            return processedText;
        }

    }
}
