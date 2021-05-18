using System;

namespace dc_p6_jobserver
{
    public class JobModel
    {
        public int jobNo;
        public Boolean assigned;
        public Boolean finished;

        public string src;
        public string result;

        public byte[] hash; 
    }
}
