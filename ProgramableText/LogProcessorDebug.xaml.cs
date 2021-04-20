using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProgramableText
{
    /// <summary>
    /// Interaction logic for LogProcessorDebug.xaml
    /// </summary>
    public partial class LogProcessorDebug : Window
    {
        public LogProcessorDebug()
        {
            InitializeComponent();
            refresh();
        }

        public void refresh()
        {
            //TODO: name output steps
            //List<String> outputPreviews = LogProcessor.LogProcessor.outputSteps.Select(x => x.Split(LogProcessor.LogProcessor.LINE_SPLIT,StringSplitOptions.None)[0]).ToList();


            this.StepListBox.ItemsSource = LogProcessor.LogProcessor.opSteps;

            this.RegisterListBox.ItemsSource = LogProcessor.LogProcessor.registers;
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        private void StepListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StepListBox.SelectedItem != null)
            {
                this.StepOutputTextBox.Text = LogProcessor.LogProcessor.outputSteps[StepListBox.SelectedIndex];

                this.RegisterOutputTextBox.Text = LogProcessor.LogProcessor.opSteps[StepListBox.SelectedIndex];
            }
        }

        private void RegisterListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RegisterListBox.SelectedItem != null)
            {
                this.RegisterOutputTextBox.Text = RegisterListBox.SelectedItem.ToString();
                this.TopLabel.Content = "Register: " + RegisterListBox.SelectedIndex + " Size: " + RegisterListBox.SelectedItem.ToString().Length;
            }
        }

        private void DebugToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            LogProcessor.LogProcessor.debugMode = !LogProcessor.LogProcessor.debugMode;
            if (LogProcessor.LogProcessor.debugMode)
            {
                debugToggleBtn.Content = "Debug ON";
                debugToggleBtn.Foreground = Brushes.Green;
            }
            if (LogProcessor.LogProcessor.debugMode)
            {
                debugToggleBtn.Content = "Debug OFF";
                debugToggleBtn.Foreground = Brushes.Red;
            }
        }
    }
}
