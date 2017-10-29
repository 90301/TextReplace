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


        public List<String> ilstMultiReplace = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            ilstOperation.Clear();
            ilstOperation.Add(INT_REPLACE);
            ilstOperation.Add(LIST_REPLACE);

            operationComboBox.Items.Clear();
            foreach (var lstrOp in ilstOperation)
            {
                operationComboBox.Items.Add(lstrOp);
            }
           
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void performActionBtn_Click(object sender, RoutedEventArgs e)
        {
            String inputText = textBox.Text;
            String replaceStr = replaceCharTextbox.Text;
            int dupTimes = (int)repeatSlider.Value;
            String outputText = "";

            String operation = ilstOperation[operationComboBox.SelectedIndex];

            if (operation.Equals(INT_REPLACE))
            {
                for (int i = 1; i < dupTimes; i++)
                {
                    outputText += inputText.Replace(replaceStr, "" + i);
                    outputText += Environment.NewLine;
                }

            }
            else if (operation.Equals(LIST_REPLACE))
            {
                foreach(string lstrItem in ilstMultiReplace)
                {
                    outputText += inputText.Replace(replaceStr, lstrItem);
                    outputText += Environment.NewLine;
                }
            }

            textBox_Output.Text = outputText;


        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //change avaliable textboxes?
        }

        private void addT_Click(object sender, RoutedEventArgs e)
        {
            ilstMultiReplace.Add(multReplaceTextbox.Text);
            multReplaceTextbox.Clear();
            updateListbox();

        }

        private void updateListbox()
        {
            listBox.Items.Clear();

            foreach (string lstrItem in ilstMultiReplace)
            {
                listBox.Items.Add(lstrItem);
            }
        }

        private void clearMultiReplaceBtn_Click(object sender, RoutedEventArgs e)
        {
            ilstMultiReplace.Clear();
            updateListbox();
        }
    }
}
