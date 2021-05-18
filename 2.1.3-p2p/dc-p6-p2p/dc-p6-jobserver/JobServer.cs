namespace dc_p6_jobserver
{
    public class JobServer : JobServerInterface
    {
        public JobModel GetJob() //Return the first non taken job  
        {
            JobModel job = new JobModel();
            job.jobNo = -1; 

            for(int idx = 0; idx < JobDB.GetNumJobs(); idx++)
            {
                JobModel j = JobDB.GetJobByIndex(idx);

                if (j.assigned == false)
                {
                    job = j;
                    break; 
                }
            }

            JobDB.SetJobAssignedByIndex(job.jobNo);

            return job;
        }

        public void SubmitFinishedJob(int index, string result)
        {
            JobDB.SetJobFinishByIndex(index, result);
        }
    }
}
