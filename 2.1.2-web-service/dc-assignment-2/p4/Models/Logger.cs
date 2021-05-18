// Filename: Logger.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Called by functions when task is performed. Logs task to console with timestamp and source + writes to file. 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System;
using System.Diagnostics;
using System.Globalization;

namespace p4.Models
{
    public class Logger
    {
        //Number of tasks performed
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

            Debug.WriteLine(outDate + "Data | Task: " + logString);
            Debug.WriteLine(outDate + "Data | Tasks performed so far: " + logNumber.ToString());

            WriteLog(outDate + "Data | Task: " + logString);
            WriteLog(outDate + "Data | Tasks performed so far: " + logNumber.ToString());
        }

        /// <summary>
        /// Logs error by outputting error string provided with timestamp and source of error. 
        /// </summary>
        /// <param name="errString"></param>
        public static void Error(string errString)
        {
            DateTime now = DateTime.Now;
            logNumber++;

            string outDate = now.ToString(new CultureInfo("en-GB"));

            Debug.WriteLine(outDate + " Data | Error: " + errString);
            Debug.WriteLine(outDate + " Data | Tasks performed so far: " + logNumber.ToString());

            WriteLog(outDate + " Data | Error: " + errString);
            WriteLog(outDate + " Data | Tasks performed so far: " + logNumber.ToString());
        }

        /// <summary>
        /// Write log/ error line to a file 
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