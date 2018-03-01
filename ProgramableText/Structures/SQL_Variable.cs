﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.Structures
{
    /// <summary>
    /// A class that represents a SQL Variable
    /// </summary>
    public class SQL_Variable
    {
        /// <summary>
        /// The Name of 
        /// </summary>
        public String varName { get; set; }

        /// <summary>
        /// String representation of a variable type, such as int
        /// </summary>
        public String varType { get; set; }

        /// <summary>
        /// The value of a SQL variable such as 15 or '2015_5'
        /// </summary>
        public String varValue { get; set; }


        public void setDefaultValue()
        {
            if (this.varName.ToLower().Contains("year"))
            {
                //it's a year
                //TODO update it to use the current year
                this.varValue = "'2017'";
            }
            if (this.varType.ToLower().Equals("int"))
            {
                this.varValue = "0";
            }
        }

        public String getVarDeclare()
        {
            return "Declare " + this.varName + " " + this.varType + " = " + this.varValue + ";";
        }

    }
}
