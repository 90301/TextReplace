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
        List<String> tables = new List<string>();//Tables and join conditions
        List<String> conditions = new List<string>();//WHERE CONDITIONS
        List<String> joinList = new List<string>();
        private String query = "";
        private SqlConnection sqlConnection;

        

        public QueryPermuter()
        {
            InitializeComponent();

            joinList.Clear();
            joinList.Add("JOIN");
            joinList.Add("FULL OUTER JOIN");
            joinList.Add("INNER JOIN");
            joinList.Add("LEFT JOIN");
            joinList.Add("RIGHT JOIN");
        }

        private void addTableBtn_Click(object sender, RoutedEventArgs e)
        {
            /*
            String tableToAdd = "";
            if (tableToAdd.Length > 1)
            {
                tableToAdd += " ";
            }
            if (tables.Count >= 1 && !addTableTextBox.Text.Contains(JOIN_KEYWORD))
            {
                //add join condition (if it doesn't exist)
                
                tableToAdd += JOIN_KEYWORD + " ";
            }
            //table includes join condition for now
            tableToAdd += addTableTextBox.Text;
            */
            /*
            if (tables.Count >= 1 && addTableJoinConditionTextBox.Text.Length>1)
            {
                //add join "ON" condition (if using the 2nd textbox)
                tableToAdd += " " + ON_KEYWORD + " " + addTableJoinConditionTextBox.Text;
            }
            */

            //tables.Add(tableToAdd);

            addTable(addTableTextBox.Text);

            updatePreviewQuery();

            addTableJoinConditionTextBox.Clear();

        }

        public void addTable(String text)
        {
            String tableToAdd = "";
            if (tableToAdd.Length > 1)
            {
                tableToAdd += " ";
            }
            if (tables.Count >= 1 && !text.Contains(JOIN_KEYWORD))
            {
                //add join condition (if it doesn't exist)

                tableToAdd += JOIN_KEYWORD + " ";
            }
            //table includes join condition for now
            tableToAdd += text;

            if (tables.Count >= 1 && addTableJoinConditionTextBox.Text.Length > 1)
            {
                //add join "ON" condition (if using the 2nd textbox)
                tableToAdd += " " + ON_KEYWORD + " " + addTableJoinConditionTextBox.Text;
            }


            tables.Add(tableToAdd);
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
            query = "Select count(1) from " + Environment.NewLine;
            query += tables.Select(x => x).Aggregate((i, j) => i + Environment.NewLine + j);
            if (conditions.Count >= 1)
            {
                query += Environment.NewLine + " where " + Environment.NewLine;
                query += conditions.Select(x => x).Aggregate((i, j) => i + Environment.NewLine +"AND " + j);
            }

            QueryPreviewTextBox.Text = query;
        }

        public void tableJoinPermute()
        {
            

            String startSQL = "Select count(1) from " + tables[0];
            String firstJoin = tables[1];
            List<String> tablesToJoin = new List<string>();
            tablesToJoin.AddRange(tables.Skip(2));

            tableJoinRecursion(startSQL,firstJoin,tablesToJoin);



        }

        public void tableJoinRecursion(String queryWithoutJoin,String tableToJoin,List<String> remainingTablesToJoin)
        {
            String nextJoin = "";
            List<String> nextRemainingTables = new List<string>();
            if (remainingTablesToJoin.Count >= 1)
            {
                nextJoin = remainingTablesToJoin[0];
                for (int i = 1; i < remainingTablesToJoin.Count; i++)
                {
                    nextRemainingTables.Add(remainingTablesToJoin[i]);
                }
            }

            foreach (String join in joinList)
            {
                string queryToRun = queryWithoutJoin + " "+ tableToJoin.Replace(JOIN_KEYWORD, join);
                SqlCommand sql = new SqlCommand(queryToRun,sqlConnection);
                
                SqlDataReader reader = sql.ExecuteReader();
                int count = 0;
                if (reader.HasRows)
                {
                    reader.Read();
                    count = reader.GetInt32(0);
                }
                reader.Close();
                Console.WriteLine(count + queryToRun);

                if (remainingTablesToJoin.Count >= 1)
                {
                    tableJoinRecursion(queryToRun, nextJoin,nextRemainingTables);
                }


            }
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
            tableJoinPermute();
        }

        private void clearQueryButton_Click(object sender, RoutedEventArgs e)
        {
            tables.Clear();
            conditions.Clear();
        }

        private void partialQueryAddButton_Click(object sender, RoutedEventArgs e)
        {
            String partialQuery = QueryPreviewTextBox.Text;

            partialQuery = partialQuery.Replace("from", "").Replace("FROM","").Replace(Environment.NewLine," ");

            foreach (String join in joinList)
            {
                partialQuery = partialQuery.Replace(join,JOIN_KEYWORD);
            }

            String[] splitQuery = partialQuery.Split(new string[] {JOIN_KEYWORD},StringSplitOptions.None);

            foreach (string table in splitQuery)
            {
                addTable(table);
            }

            updatePreviewQuery();


        }

        private void partialQueryAddConditionButton_Click(object sender, RoutedEventArgs e)
        {
            String partialQuery = QueryPreviewTextBox.Text;
            partialQuery = partialQuery.Replace("WHERE", "").Replace("where", "").Replace("and","AND");

            String[] splitCondtions = partialQuery.Split(new String[] {Environment.NewLine + "AND"},StringSplitOptions.None);

            foreach (String condition in splitCondtions)
            {
                this.conditions.Add(condition);
            }

            updatePreviewQuery();

        }

        private void runConditionsButton_Click(object sender, RoutedEventArgs e)
        {
            //For now just use standard joins
            //later permute the joins (but due to condition testing, it must have all tables)
            string queryWithoutConditions = "Select count(1) from " + Environment.NewLine;
            queryWithoutConditions += tables.Select(x => x).Aggregate((i, j) => i + Environment.NewLine + j);

            String conditionsSoFar = "";
            for (int i = -1; i < conditions.Count; i++)
            {
                //first run doesn't have conditions
                String queryToRun = queryWithoutConditions;


                if (i >= 0)
                {

                    queryToRun += Environment.NewLine + " where " + Environment.NewLine;
                    //query += conditions.Select(x => x).Aggregate((i, j) => i + Environment.NewLine + "AND " + j);


                }
                if (i >= 0)
                {
                    if (i >= 1)
                    {
                        conditionsSoFar += " And ";
                    }
                    conditionsSoFar += conditions[i];
                }
                queryToRun += conditionsSoFar;

                SqlCommand sql = new SqlCommand(queryToRun, sqlConnection);

                SqlDataReader reader = sql.ExecuteReader();
                int count = 0;
                if (reader.HasRows)
                {
                    reader.Read();
                    count = reader.GetInt32(0);
                }
                reader.Close();

                Console.WriteLine(count +"\t" + conditions[i].Replace(Environment.NewLine, " ") + "\t"+ conditionsSoFar.Replace(Environment.NewLine, " ") + "\t"+ queryToRun.Replace(Environment.NewLine," "));
            }

            //condition testing
        }
    }
}
