using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using ProgramableText.Annotations;

namespace ProgramableText
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ObservableCollection<String> ilstOperation = new ObservableCollection<string>();
        public const string INT_REPLACE = "Integer Replace";
        public const string LIST_REPLACE = "List Replace";
        public const string MULTI_REPLACE = "Multi Replace";

        public Boolean devOutput = true;

        public ObservableCollection<String> ilstMultiReplace = new ObservableCollection<string>();

        public List<replacement> ilstMultiReplacement = new List<replacement>();
        public MainWindow()
        {
            InitializeComponent();
            ilstOperation.Clear();
            ilstOperation.Add(INT_REPLACE);
            ilstOperation.Add(LIST_REPLACE);
            ilstOperation.Add(MULTI_REPLACE);

            operationComboBox.Items.Clear();
            foreach (var lstrOp in ilstOperation)
            {
                operationComboBox.Items.Add(lstrOp);
            }
           // replacementIteration.ItemsSource = ilstMultiReplacement;
           // replacementIteration.DisplayMemberPath = "replaceText";
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void performActionBtn_Click(object sender, RoutedEventArgs e)
        {
            String inputText = textBox.Text;
            String replaceStr = replaceCharTextbox.Text;
            String inverseReplaceStr = replaceCharInverseTextbox.Text;
            int dupTimes = (int)repeatSlider.Value;
            String outputText = "";

            String operation = INT_REPLACE;
            if (operationComboBox.SelectedIndex >= 0)
            {
                operation = ilstOperation[operationComboBox.SelectedIndex];
            }

            if (operation.Equals(INT_REPLACE))
            {
                for (int i = 1; i <= dupTimes; i++)
                {
                    outputText += inputText.Replace(replaceStr, "" + i).Replace(inverseReplaceStr,""+(dupTimes-i+1));
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
            else if (operation.Equals(MULTI_REPLACE))
            {
                for (int i = 0; i < ilstMultiReplacement[0].ilstMultiReplace.Count; i++)
                {
                    string lstrLine = inputText;
                    foreach (var lobjReplace in ilstMultiReplacement)
                    {
                        //copy line over

                        string replaceWith = lobjReplace.ilstMultiReplace[i];

                        lstrLine = lstrLine.Replace(lobjReplace.replaceText, replaceWith);


                    }
                    outputText += lstrLine;
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

        private void addReplace_Click(object sender, RoutedEventArgs e)
        {
        ilstMultiReplacement.Add(new replacement());
            //replacementIteration.DisplayMemberPath = "replaceText";
            replacementIteration.Items.Add(ilstMultiReplacement.Last().replaceText);
        }

        private void removeReplaceClick(object sender, RoutedEventArgs e)
        {
            if (ilstMultiReplacement.Count > 1)
            {
                ilstMultiReplacement.Remove(ilstMultiReplacement[replacementIteration.SelectedIndex]);
            }
            replacementIteration.Items.Remove(replacementIteration.SelectedIndex);

        }

        private void updateReplacement() {
    replacementIteration.Items.Clear();
    foreach (var lstrOp in ilstMultiReplacement)
    {
        operationComboBox.Items.Add(lstrOp);
    }
    }

        private void replacementIteration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lobjReplacement = this.ilstMultiReplacement[replacementIteration.SelectedIndex];

        }

        private void updateMultiReplaceBtn_Click(object sender, RoutedEventArgs e)
        {
            var lobjReplacement = this.ilstMultiReplacement[replacementIteration.SelectedIndex];
            //lobjReplacement.ilstMultiReplace

        }

        private void toSqlStringBtn_Click(object sender, RoutedEventArgs e)
        {
            textBox_Output.Text = CyberiaPreProcessor.StringToSqlString(textBox.Text);
        }

        private void OpenQueryPremuterButton_Click(object sender, RoutedEventArgs e)
        {
            QueryPermuter qp = new QueryPermuter();
            qp.Show();
        }

        private void addFeatureButton_Click(object sender, RoutedEventArgs e)
        {
            //load in full text (input textbox)
            String inputText = textBox.Text;
            String textToAdd = featureTextBoxReplace.Text;
            String replaceText = replaceCharTextbox.Text;

            Boolean replaceFunctionality = replaceFeatureCheckBox.IsChecked.Value;

            String varType = featureTextBox1.Text;//Decimal
            String varMatch = featureTextBox2.Text;//idec
            //(Decimal idec[^\s]+)
            String regexEndChar = featureTextBox3.Text;
            String regexFindVarLines = "(" +varType + varMatch + "[^"+ regexEndChar+"]+)";
            //String regexFindLine = "(" + varType + varMatch + "[^\\n]+)";//(Decimal idec[^\n]+)
            String regexFindVarName = "(" + varMatch + "[^"+ regexEndChar+"]+)";

            Regex findVarLinesRegex = new Regex(regexFindVarLines);
            Regex findVarNameRegex = new Regex(regexFindVarName);

            String outputText = "";
            //split input into lines
            String[] splitText = inputText.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);

            foreach (String splitLine in splitText)
            {
                //TODO add replace functionality
                if (!replaceFunctionality)
                {
                    outputText += splitLine + Environment.NewLine;
                }
                //find any matches, do stuff
                if (findVarLinesRegex.IsMatch(splitLine))
                {
                    Match m = findVarNameRegex.Match(splitLine);
                    Group g = m.Groups[0];
                    String varName = g.ToString();
                    Match m2 = findVarLinesRegex.Match(splitLine);
                    Group g2 = m2.Groups[0];
                    String varName2 = g2.ToString();
                    if (varMatch.Length == 0)
                    {
                        varName = varName2.Replace(varType, "");
                    }

                    String textToAddLocal = textToAdd.Replace(replaceText, varName);

                    if (!replaceFunctionality)
                    {
                        outputText += textToAddLocal;
                    }
                    else
                    {
                        
                        //should turn prop.idecX -> prop.istrX
                        outputText += splitLine.Replace(varName2, textToAddLocal) + Environment.NewLine;
                    }

                }
                else
                {
                    if (replaceFunctionality)
                    {
                        outputText += splitLine + Environment.NewLine;
                    }
                }
                
                //don't find matches, just add that to the output
            }
            this.textBox_Output.Text = outputText;

        }

        string devOutputText, prodOutputText;

        private void cyberiaPreProcessBtn_Click(object sender, RoutedEventArgs e)
        {
            CyberiaPreProcessor cyberia = new CyberiaPreProcessor();
            cyberia.LanguageUsing = CyberiaPreProcessor.LANGUAGE_SQL;
            String input = textBox.Text;

            
            cyberia.processText(input,out devOutputText,out prodOutputText);

            if (devOutput)
            {
                textBox_Output.Text = devOutputText;
            }
            else
            {
                textBox_Output.Text = prodOutputText;
            }

        }

        private void cyberiaPreProcessDevToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            devOutput = !devOutput;
            if (devOutput)
            {
                cyberiaPreProcessDevToggleBtn.Content = "Dev";
                textBox_Output.Text = devOutputText;
            }
            else
            {
                cyberiaPreProcessDevToggleBtn.Content = "Prod";
                textBox_Output.Text = prodOutputText;
            }
        }
    }




    #region SupportClasses

    public class replacement
    {
        private static int counter = 0;
        public string replaceText { get; set; } = "[R" + (counter++) + "]";
        public ObservableCollection<String> ilstMultiReplace = new ObservableCollection<string>();
    }


#endregion
}
