// Filename: JobDB.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Database to hold all Jobs available for each job server instance 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System;
using System.Collections.Generic;
using System.Linq;
namespace dc_p6_p2p_wpf
{
    static class JobDB
    {
        private static List<JobModel> jobs = new List<JobModel>(); 
        private static int numJobs = 0; 

        /// <summary>
        /// Function to add a new job given source and hash of job 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="hash"></param>
        public static void AddJob(string src, byte[] hash)
        {
            JobModel job = new JobModel(); //Create a new JobModel to hold job details 

            job.jobNo = numJobs; //Package information into job - Jobs are identified by the index they are at. 
            job.assigned = false;
            job.finished = false;

            job.src = src;
            job.hash = hash; 

            jobs.Add(job);

            numJobs++;
        }

        /// <summary>
        /// Get the number of jobs currently on the job server (completed & incomplete) 
        /// </summary>
        /// <returns></returns>
        public static int GetNumJobs()
        {
            return jobs.Count(); 
        }

        /// <summary>
        /// Get a job based on it's index in the list 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>JobModel object representing the job</returns>
        public static JobModel GetJobByIndex(int idx)
        {
            JobModel job = new JobModel();

            try
            {
                job = jobs[idx];
            }
            catch(IndexOutOfRangeException)
            {
                Logger.Error("Attempted to access job index out of bounds: " + idx);
            }

            return job; 
        }

        /// <summary>
        /// Mark a jobs field assigned as true given an index containing a job 
        /// </summary>
        /// <param name="idx"></param>
        public static void SetJobAssignedByIndex(int idx)
        {
            try
            {
                jobs[idx].assigned = true;
            }
            catch (IndexOutOfRangeException)
            {
                Logger.Error("Attempted to mark job assigned that is out of bounds: " + idx);
            }
        }

        /// <summary>
        /// Mark a job as finished and set the result 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="result"></param>
        public static void SetJobFinishByIndex(int idx, string result)
        {
            try
            {
                jobs[idx].finished = true;
                jobs[idx].result = result;
            }
            catch (IndexOutOfRangeException)
            {
                Logger.Error("Attempted to mark job as finished that is out of bounds: " + idx);
            }
        }
    }
}
