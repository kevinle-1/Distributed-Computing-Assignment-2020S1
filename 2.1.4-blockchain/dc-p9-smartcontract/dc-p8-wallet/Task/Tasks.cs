// Filename: Tasks.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Manage Tasks  
// Author:   Kevin Le (19472960)
//
// Date:     28/05/2020

using System.Collections.Generic;

namespace dc_p8_wallet.Task
{
    class Tasks
    {
        private static Queue<Task> tasks = new Queue<Task>();

        /// <summary>
        /// Add a new task to the task list 
        /// </summary>
        public static void AddTask(string py)
        {
            Task t = new Task();
            t.py = py;

            tasks.Enqueue(t); 
        }

        /// <summary>
        /// Return list of tasks - Used by miner thread 
        /// </summary>
        /// <returns></returns>
        public static Queue<Task> GetTasks()
        {
            return tasks; 
        }

        /// <summary>
        /// Clears a queue of tasks if invalid blocks are recieved 
        /// </summary>
        public static void Reset()
        {
            tasks.Clear();
        }
    }
}
