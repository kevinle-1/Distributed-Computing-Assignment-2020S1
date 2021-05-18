// Filename: JobServer.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Job server functions 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020


namespace dc_p6_p2p_wpf
{
    public class JobServer : JobServerInterface
    {
        /// <summary>
        /// Return the first non taken job 
        /// </summary>
        /// <returns>Job object</returns>
        public JobModel GetJob()  
        {
            JobModel job = new JobModel();
            job.jobNo = -1;

            if(JobDB.GetNumJobs() != 0) //Marks job as assigned in here even if client cant do it 
            {
                for (int idx = 0; idx < JobDB.GetNumJobs(); idx++)
                {
                    JobModel j = JobDB.GetJobByIndex(idx); 

                    if (j.assigned == false)
                    {
                        job = j;
                        JobDB.SetJobAssignedByIndex(job.jobNo);

                        break;
                    }
                }
            }

            return job;
        }

        /// <summary>
        /// Submit a finished job providing the index the job is at and the result 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="result"></param>
        public void SubmitFinishedJob(int index, string result)
        {
            JobDB.SetJobFinishByIndex(index, result);
        }

        /// <summary>
        /// Get the number of jobs 
        /// </summary>
        /// <returns></returns>
        public int GetNumJobs()
        {
            return JobDB.GetNumJobs(); 
        }
    }
}
