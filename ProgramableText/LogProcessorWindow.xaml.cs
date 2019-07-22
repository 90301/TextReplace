
using System;
using System.Collections.Generic;
using System.Windows;

namespace ProgramableText
{
    /// <summary>
    /// Interaction logic for LogProcessor.xaml
    /// </summary>
    public partial class LogProcessorWindow : Window
	{
        private readonly String[] OP_SPLIT = { Environment.NewLine };
        private readonly String[] NAME_SPLIT = { "(" };
        private readonly String[] ARG_SPLIT = { "," };

        /// <summary>
        /// Lookup data structure
        /// </summary>
        Dictionary<String, ProgramableText.LogProcessor.ProgramNode> allNodes;

        List<ProgramableText.LogProcessor.ProgramNode> nodes ;

        ProgramableText.LogProcessor.InnerReadNode argReader = new ProgramableText.LogProcessor.InnerReadNode();

        public LogProcessorWindow()
		{
			InitializeComponent();

            argReader.parseArgs(new String[] { "(", ")" });

            allNodes = new Dictionary<string, ProgramableText.LogProcessor.ProgramNode>();
            addAllNode(new ProgramableText.LogProcessor.InnerReadNode());
            addAllNode(new ProgramableText.LogProcessor.FilterNode());

        }

        public void addAllNode(ProgramableText.LogProcessor.ProgramNode node)
        {
            allNodes.Add(node.getOpName(), node);
        } 

        private void ProcessBtn_Click(object sender, RoutedEventArgs e)
        {
            nodes = new List<LogProcessor.ProgramNode>();


            string[] lines = this.FilterProgram.Text.Split(OP_SPLIT,StringSplitOptions.RemoveEmptyEntries);
            foreach (String line in lines) {
                String lineClean = line.Trim();

                String op = lineClean.Split(NAME_SPLIT,StringSplitOptions.RemoveEmptyEntries)[0];

                ProgramableText.LogProcessor.ProgramNode node = (allNodes[op]).createInstance();

                //Load the arguments of the method, comma seperated (arg1,arg2)
                node.parseArgs(
                    argReader.getArrayResults(lineClean)[0].Split(ARG_SPLIT,StringSplitOptions.RemoveEmptyEntries)
                    );
                nodes.Add(node);
            }
        }
    }
}
