
using ProgramableText.LogProcessor;
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
        Dictionary<String, ProgramNode> allNodes;

        List<ProgramNode> nodes ;

        InnerReadNode argReader = new InnerReadNode();

        public LogProcessorWindow()
		{
			InitializeComponent();

            InnerReadNode.setupEscapeChars();

            argReader.parseArgs(new String[] { "(", ")" });

            allNodes = new Dictionary<string, ProgramNode>();
            addAllNode(new InnerReadNode());
            addAllNode(new FilterNode());
            addAllNode(new WordSearch());

            this.AllOpList.ItemsSource = allNodes.Values;

        }

        public void addAllNode(ProgramNode node)
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
                ProgramNode node;

                if (allNodes.ContainsKey(op))
                {
                    node = (allNodes[op]).createInstance();
                } else
                {
                    this.OutputBox.Text = "Failed to find OP: " + op;
                    return;
                }

                //Load the arguments of the method, comma seperated (arg1,arg2)
                node.parseArgs(
                    argReader.getArrayResults(lineClean)[0].Split(ARG_SPLIT,StringSplitOptions.RemoveEmptyEntries)
                    );
                nodes.Add(node);
            }
            // Process Text
            String processedText = this.LogText.Text;
            String nodeToString = "";
            foreach (ProgramNode node in nodes)
            {
                processedText = node.calculate(processedText);
                nodeToString += node.ToString() + Environment.NewLine;
            }

            this.ProgramReader.Text = nodeToString;

            this.OutputBox.Text = processedText;
        }

        public void AllOpsClick(object sender, RoutedEventArgs e)
        {
            if (AllOpList.SelectedItem != null)
            {
                this.FilterProgram.Text += AllOpList.SelectedItem.ToString().Replace(" ","") + Environment.NewLine;
            }
        }
    }
}
