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
            this.StepListBox.ItemsSource = LogProcessor.LogProcessor.outputSteps;

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
                this.StepOutputTextBox.Text = StepListBox.SelectedItem.ToString();

                this.RegisterOutputTextBox.Text = LogProcessor.LogProcessor.opSteps[StepListBox.SelectedIndex];
            }
        }

        private void RegisterListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RegisterListBox.SelectedItem != null)
            {
                this.RegisterOutputTextBox.Text = RegisterListBox.SelectedItem.ToString();
            }
        }
    }
}
