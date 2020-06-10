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
    /// Interaction logic for SpecialCharactersWindow.xaml
    /// </summary>
    public partial class SpecialCharactersWindow : Window
    {

        Dictionary<String, String> reverseLookup = new Dictionary<string, string>();
        public SpecialCharactersWindow()
        {
            InitializeComponent();
            SpListBox.ItemsSource = LogProcessor.LogProcessor
                .specialCharacters.Values;

            //create a reverse lookup allowing us to match values to keys
            foreach (String key in LogProcessor.LogProcessor
                .specialCharacters.Keys)
            {
                String value = LogProcessor.LogProcessor
                .specialCharacters[key];

                reverseLookup.Add(value, key);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpListBox.SelectedItem != null)
            {
                CodeOutput.Text =
                reverseLookup[SpListBox.SelectedItem.ToString()];
            }
        }

        private void TextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextOutput != null && TextInput.Text != "TextInput")
            {
                string input = TextInput.Text;
                foreach (String key in reverseLookup.Keys)
                {
                    input = input.Replace(key, reverseLookup[key]);
                }

                TextOutput.Text = input;
            }
        }


    }
}
