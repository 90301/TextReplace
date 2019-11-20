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

        public static List<ProgramNode> nodes;
        public static List<ExtraPass> extraPassNodes;
        public static List<ExtraPass> processedExtraPassNodes;

        public static InnerReadNode argReader = new InnerReadNode();

        public static String[] lines;

        public static String errors, output, inputText, nodeToString;

        public static List<String> outputSteps;
        static LogProcessor()
        {
            argReader.parseArgs(new String[] { "(", ")" });

            allNodes = new Dictionary<string, ProgramNode>();
            addAllNode(new InnerReadNode());
            addAllNode(new FilterNode());
            addAllNode(new WordSearch());
            addAllNode(new PlusBase());
            addAllNode(new ExtraPassNow());
        }

        public static void addAllNode(ProgramNode node)
        {
            allNodes.Add(node.getOpName(), node);
        }

        public static void loadProgram(String program)
        {
            lines = program.Split(OP_SPLIT, StringSplitOptions.RemoveEmptyEntries);
            processProgram();
            processNodeToString();
        }
        public static void loadInputText(String input)
        {
            inputText = input;
        }

        public static void addProcessedExtraPass(ExtraPass item)
        {
            processedExtraPassNodes.Add(item);
        }


        public static void processNodeToString()
        {
            nodeToString = "";
            foreach (ProgramNode node in nodes)
            {
                nodeToString += node.ToString() + Environment.NewLine;
            }
        }

        public static void processProgram()
        {
            nodes = new List<ProgramNode>();
            extraPassNodes = new List<ExtraPass>();
            

            errors = "";
            output = "";


            foreach (String line in lines)
            {

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
                    return;
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
            }
        }

        public static void process()
        {
            processedExtraPassNodes = new List<ExtraPass>();

            outputSteps = new List<string>();
            errors = "";
            output = "";

            // Process Text
            String processedText = inputText;
            foreach (ProgramNode node in nodes)
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
