using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using ProgramableText.Utils;

namespace ProgramableText
{
    /// <summary>
    /// Interaction logic for FlatFileReader.xaml
    /// </summary>
    public partial class FlatFileReader : Window
    {
        public string textLoaded { get; set; } = "";

        public string textFormat { get; set; } = "";

        public List<KeyValuePair<String,int>> formatList {get;set;} = new List<KeyValuePair<string, int>>();

        public string csvOutput { get; set; } = "";

        public FlatFileReader()
        {
            InitializeComponent();
        }

        private void loadFlatFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                textLoaded = File.ReadAllText(openFileDialog.FileName);
                this.previewTextbox.Text = textLoaded;//only if the format isn't loaded
            }
        }

        public void parseTextLoaded()
        {
            textFormat = formatPreviewTextbox.Text;
            //TODO add section Support (X lines of a particular format)
            if (textFormat.Contains(","))
            {
                //validation to ensure there's a comma
                formatList.Clear();
                String[] lines = TextUtils.splitOnNewLine(textFormat);
                foreach (string line in lines)
                {
                    String[] fields = line.Split(new [] {","},StringSplitOptions.None);
                    if (fields.Length >= 2)
                    {
                        String fieldTextSize = fields[1].Trim();
                        int fieldSize = Int32.Parse(fieldTextSize);
                        formatList.Add(new KeyValuePair<string, int>(fields[0],fieldSize));
                    }
                }
            }

            if (formatList.Count >= 1 && textLoaded.Length>=1)
            {
                /*
                foreach (var formatKeyValuePair in formatList)
                {
                    formatKeyValuePair.Key
                }
                */
                String topBar = formatList.Select(x => x.Key).Aggregate((i, j) => i + "," + j);

                String body = "";
                foreach (var line in TextUtils.splitOnNewLine(textLoaded))
                {
                    String outputLine = "";
                    int lastPosition = 0;
                    foreach (var formKeyValuePair in formatList)
                    {
                        outputLine += line.Substring(lastPosition, formKeyValuePair.Value) + ",";
                        lastPosition += formKeyValuePair.Value;
                    }
                    if (outputLine.Length >= 1)
                    {
                        outputLine = outputLine.Remove(outputLine.Length - 1, 1);
                    }
                    //TODO put extra text at the end of the row
                    body += outputLine + Environment.NewLine;
                }

                csvOutput = topBar + Environment.NewLine;
                csvOutput += body;
                this.previewTextbox.Text = csvOutput;

            }



        }

        private void processBtn_Click(object sender, RoutedEventArgs e)
        {
            parseTextLoaded();
        }

        private void saveCSVFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true ;
            saveFileDialog.DefaultExt = "csv";
            saveFileDialog.Filter = "Comma Seperated Value|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                parseTextLoaded();
                if (saveFileDialog.FileName != "")
                {
                    System.IO.FileStream fs =
                        (System.IO.FileStream)saveFileDialog.OpenFile();

                    byte[] info = new UTF8Encoding(true).GetBytes(csvOutput);
                    fs.Write(info, 0, info.Length);

                    fs.Close();

                }
            }
        }
    }
}
