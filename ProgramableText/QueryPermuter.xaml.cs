using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for QueryPermuter.xaml
    /// </summary>
    public partial class QueryPermuter : Window
    {
        public String JOIN_KEYWORD = "JOIN";
        public String ON_KEYWORD = "ON";
        List<String> tables = new List<string>();
        List<String> conditions = new List<string>();
        private String previewQuery = "";
        private SqlConnection sqlConnection;

        public QueryPermuter()
        {
            InitializeComponent();
        }

        private void addTableBtn_Click(object sender, RoutedEventArgs e)
        {
            String tableToAdd = "";
            if (tables.Count >= 1)
            {
                //add join condition
                tableToAdd += " " + JOIN_KEYWORD + " ";
            }
            //table includes join condition for now
            tableToAdd += addTableTextBox.Text;

            if (tables.Count >= 1)
            {
                //add join condition
                tableToAdd += " " + ON_KEYWORD + " " + addTableJoinConditionTextBox.Text;
            }


            tables.Add(tableToAdd);

            updatePreviewQuery();

            addTableJoinConditionTextBox.Clear();

        }

        private void addConditionBtn_Click(object sender, RoutedEventArgs e)
        {
            String conditionToAdd = addConditionTextBox.Text;
            conditions.Add(conditionToAdd);
            
            updatePreviewQuery();
            addTableTextBox.Clear();
            addConditionTextBox.Clear();

        }

        public void updatePreviewQuery()
        {
            previewQuery = "Select count(1) from " + Environment.NewLine;
            previewQuery += tables.Select(x => x).Aggregate((i, j) => i + Environment.NewLine + j);
            if (conditions.Count >= 1)
            {
                previewQuery += Environment.NewLine + " where " + Environment.NewLine;
                previewQuery += conditions.Select(x => x).Aggregate((i, j) => i + Environment.NewLine + j);
            }

            QueryPreviewTextBox.Text = previewQuery;
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {

            String connStr = "";
            if (sqlServerConString.Text.Length > 5)
            {
                connStr = sqlServerConString.Text;
            }
            else
            {
                connStr = "Data Source="+ sqlserverTextbox.Text+
                    ";Initial Catalog="+ sqlserverDBTextbox.Text +
                    ";User ID=" + sqlserverUserTextbox.Text +
                    ";Password=" + sqlserverPassTextbox.Text;
                sqlServerConString.Text = connStr;
            }

            sqlConnection = new SqlConnection(connStr);
            //sqlConnection.ConnectionString = connStr;
            sqlConnection.Open();
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            //currently cycle through (IN ORDER) the table joins (with different styles)
        }
    }
}
