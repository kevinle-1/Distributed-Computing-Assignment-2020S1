// Filename: Task.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Object representation of a Task 
// Author:   Kevin Le (19472960)
//
// Date:     28/05/2020

using System;

namespace dc_p8_wallet.Task
{
    class Task
    {
        public string py; 

        /// <summary>
        /// Returns string representation of transaction 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return py; 
        }
    }
}
