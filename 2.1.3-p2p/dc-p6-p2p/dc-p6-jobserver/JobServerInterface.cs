using System.ServiceModel;

namespace dc_p6_jobserver
{
    [ServiceContract]
    public interface JobServerInterface
    {
        [OperationContract]
        JobModel GetJob();
        [OperationContract]
        void SubmitFinishedJob(int index, string result);
    }
}
