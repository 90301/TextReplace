﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// This is an abstract class that holds all functional processes
/// </summary>
namespace ProgramableText.LogProcessor
{
	public abstract class ProgramNode : ProgramNodeInterface
	{
		public static readonly String[] NEWLINE = new string[] { Environment.NewLine };
        public static readonly String[] WORDS = new string[] { Environment.NewLine, " ", "=" , ">" ,"<","/","'","\"" };
        public static readonly String[] TRUE = new string[] { "true", "t", "1" };
        public static readonly String[] FALSE = new string[] { "false", "f", "0" };
        /// <summary>
        /// Calculates a given functional node
        /// Null or an Empty String outputs will be removed automatically.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public abstract String calculate(String input);

		public abstract String getOpName();

		public abstract void parseArgs(String[] args);

        public abstract ProgramNode createInstance();

		// Utilities:

		public static List<String> splitTextToLines(String text)
		{
			return text.Split(NEWLINE,StringSplitOptions.RemoveEmptyEntries).ToList();
		}

        public static List<String> splitTextToWords(String text)
        {
            return text.Split(WORDS, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// Utility to remove nulls
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<String> removeNullorEmptyLines(List<String> lines)
		{
			//TODO have a version of this that only takes
			return lines.Select(x => x).Where(x => x.Length >= 1).ToList();
		}

        public string createExample()
        {
            return this.ToString();
        }

        public static Boolean loadBoolean(String str)
        {
            String processed = str.Trim().ToLower();
            if (TRUE.Contains(processed))
            {
                return true;
            } else if (FALSE.Contains(processed))
            {
                return false;
            } else
            {
                return false;
            }
        }
    }

}
