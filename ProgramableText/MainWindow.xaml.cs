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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProgramableText
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<String> ilstOperation = new List<string>();
        public const string INT_REPLACE = "Integer Replace";
        public const string LIST_REPLACE = "List Replace";

        public MainWindow()
        {
            InitializeComponent();
            ilstOperation.Clear();
            ilstOperation.Add(INT_REPLACE);
            ilstOperation.Add(LIST_REPLACE);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void performActionBtn_Click(object sender, RoutedEventArgs e)
        {
            String inputText = textBox.Text;
            String replaceStr = replaceCharTextbox.Text;
            int dupTimes = 7;
            String outputText = "";

            for (int i = 1; i < dupTimes; i++)
            {
                outputText += inputText.Replace(replaceStr, ""+i);
                outputText += Environment.NewLine;
            }

            textBox_Output.Text = outputText;


        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //change avaliable textboxes?
        }
    }
}
