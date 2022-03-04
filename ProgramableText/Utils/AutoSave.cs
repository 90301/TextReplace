using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProgramableText.Utils
{
    public static class AutoSave
    {
        public static void saveLogProcessor(String programText,String inputText)
        {
            try
            {
                if (ConfigFile.AutosaveDirectory != null)
                {
                    String timeOfDay = System.DateTime.Now.TimeOfDay.ToString();
                    timeOfDay = timeOfDay.Replace(".", "_").Replace(":", "_");
                    String fileName = ConfigFile.AutosaveDirectory + "\\" + "ProgramText_" + System.DateTime.Now.DayOfYear+"_"+ timeOfDay + ".txt";
                    StreamWriter file = new StreamWriter(fileName);

                    file.Write(programText);
                    file.Flush();
                    file.Close();

                    fileName = ConfigFile.AutosaveDirectory + "\\" + "InputText_" + System.DateTime.Now.DayOfYear + "_" + timeOfDay + ".txt";
                    StreamWriter file2 = new StreamWriter(fileName);

                    file2.Write(inputText);
                    file2.Flush();
                    file2.Close();
                }

            } catch (Exception e)
            {
                MessageBox.Show(e.Message, "Exception Occured with AutoSave");
            }
        }
    }
}
