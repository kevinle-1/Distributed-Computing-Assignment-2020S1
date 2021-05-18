// Filename: Logger.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Logging  
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System;
using System.Diagnostics;
using System.Globalization;

namespace p4_business.Models
{
    public class Logger
    {
        private static uint logNumber = 0;

        /// <summary>
        /// Logs tasks by outputting task string provided with timestamp and source of task. 
        /// </summary>
        /// <param name="logString">String to output and write to log file</param>
        public static void Log(string logString)
        {
            DateTime now = DateTime.Now;
            logNumber++;

            string outDate = now.ToString(new CultureInfo("en-GB"));

            Debug.WriteLine(outDate + " Business | Task: " + logString);
            Debug.WriteLine(outDate + " Business | Tasks performed so far: " + logNumber.ToString());

            WriteLog(outDate + " Business | Task: " + logString);
            WriteLog(outDate + " Business | Tasks performed so far: " + logNumber.ToString());
        }

        /// <summary>
        /// Write log line to a file 
        /// </summary>
        /// <param name="logString">Log string provided when called</param>
        private static void WriteLog(string logString)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\log.txt";

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(logString);
            }
        }
    }
}