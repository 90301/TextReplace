
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
        
        public LogProcessorWindow()
		{
			InitializeComponent();

            InnerReadNode.setupEscapeChars();

            this.AllOpList.ItemsSource = LogProcessor.LogProcessor.allProgramNodeInterfaces;

        }

        private void ProcessBtn_Click(object sender, RoutedEventArgs e)
        {
            LogProcessor.LogProcessor.loadProgram(this.FilterProgram.Text);

            this.ProgramReader.Text = LogProcessor.LogProcessor.nodeToString;

            LogProcessor.LogProcessor.loadInputText(this.LogText.Text);

            LogProcessor.LogProcessor.process();
            
            if (LogProcessor.LogProcessor.errors.Length > 1)
            {
                this.OutputBox.Text = LogProcessor.LogProcessor.errors;
                return;
            }

            this.OutputBox.Text = LogProcessor.LogProcessor.output;
        }

        public void AllOpsClick(object sender, RoutedEventArgs e)
        {
            if (AllOpList.SelectedItem != null)
            {
                this.FilterProgram.Text += ((ProgramNodeInterface)AllOpList.SelectedItem).createExample() + Environment.NewLine;
            }
        }
    }
}
