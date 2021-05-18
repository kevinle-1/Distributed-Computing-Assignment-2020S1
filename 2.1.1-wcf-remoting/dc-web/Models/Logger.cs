// Filename: Logger.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Function to handle all task logging 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace dc_web.Models
{
    public class Logger
    {
        private static uint logNumber = 0;

        /// <summary>
        /// Simply outputs Log strings when called like: Logger.Log("Hello World"); 
        /// Used to debug and keep track of whats happening 
        /// </summary>
        /// <param name="logString">String to output</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Log(string logString)
        {
            DateTime now = DateTime.Now; 
            logNumber++;

            string outDate = now.ToString(new CultureInfo("en-GB"));

            string logOutputString = outDate + " | [Business] Task: " + logString;
            string logCountString = outDate + " | [Business] Tasks performed so far: " + logNumber.ToString();

            System.Diagnostics.Debug.WriteLine(logOutputString);
            System.Diagnostics.Debug.WriteLine(logCountString);
        }
    }
}