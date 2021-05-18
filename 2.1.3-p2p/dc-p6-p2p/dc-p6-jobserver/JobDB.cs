using System.Collections.Generic;
using System.Linq;
namespace dc_p6_jobserver
{
    static class JobDB
    {
        private static List<JobModel> jobs = new List<JobModel>();
        private static int numJobs = 0; 

        public static void AddJob(string src, byte[] hash)
        {
            JobModel job = new JobModel();

            job.jobNo = numJobs;
            job.assigned = false;
            job.finished = false;

            job.src = src;
            job.hash = hash; 

            jobs.Add(job);

            numJobs++;
        }

        public static int GetNumJobs()
        {
            return jobs.Count(); 
        }

        public static JobModel GetJobByIndex(int index)
        {
            return jobs[index];
        }

        public static void SetJobAssignedByIndex(int idx)
        {
            jobs[idx].assigned = true; 
        }

        public static void SetJobFinishByIndex(int idx, string result)
        {
            jobs[idx].finished = true; 
            jobs[idx].result = result; 
        }
    }
}
