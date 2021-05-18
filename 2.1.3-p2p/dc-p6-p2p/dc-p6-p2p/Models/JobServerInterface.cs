// Filename: JobServerInterface.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Interface for job servers - functions it must implement
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System.ServiceModel;

namespace dc_p6_p2p.Models
{
    [ServiceContract]
    public interface JobServerInterface
    {
        [OperationContract]
        JobModel GetJob();
        [OperationContract]
        void SubmitFinishedJob(int index, string result);        
        [OperationContract]
        int GetNumJobs();
    }
}
