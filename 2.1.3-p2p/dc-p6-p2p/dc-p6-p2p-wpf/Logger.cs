// Filename: Logger.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Logging  
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System;
using System.Diagnostics;
using System.Globalization;

namespace dc_p6_p2p_wpf
{
    class Logger
    {
        //Count for number of tasks completed 
        private static uint logNumber = 0;

        /// <summary>
        /// Logs tasks by outputting task string provided with timestamp and source of task. 
        /// </summary>
        /// <param name="logString">String to output and write to log file</param>
        public static void Log(string logString)
        {
            logNumber++;

            string outDate = DateTime.Now.ToString(new CultureInfo("en-GB"));

            Debug.WriteLine(outDate + " | Task: " + logString);
            Debug.WriteLine(outDate + " | Tasks performed so far: " + logNumber.ToString());

            //WriteLog(outDate + " | Task: " + logString);
            //WriteLog(outDate + " | Tasks performed so far: " + logNumber.ToString());
        }

        /// <summary>
        /// Logs errors by outputting error string provided with timestamp and source of error. 
        /// </summary>
        /// <param name="errString">String to output and write to log file</param>
        public static void Error(string errString)
        {
            logNumber++;

            string outDate = DateTime.Now.ToString(new CultureInfo("en-GB"));

            Debug.WriteLine(outDate + " | Error: " + errString);
            Debug.WriteLine(outDate + " | Tasks performed so far: " + logNumber.ToString());

            //WriteLog(outDate + " | Error: " + errString);
            //WriteLog(outDate + " | Tasks performed so far: " + logNumber.ToString());
        }

        /// <summary>
        /// Write log line to a file 
        /// </summary>
        /// 
        /// Note: Not used due to race condition. 
        ///       Multiple clients will attempt to write to the same location if they running on same machine. 
        ///       Not expected to be issue if running on multiple machines
        /// 
        /// <param name="logString">Log string provided when called</param>
        private static void WriteLog(string logString)
        {
            string path = "";

            path = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\log.txt";

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(logString);
            }
        }
    }
}
