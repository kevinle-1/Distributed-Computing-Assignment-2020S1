// Filename: JobModel.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Object representation of a Job 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System;

namespace dc_p6_p2p_wpf
{
    public class JobModel
    {
        public int jobNo;
        public Boolean assigned;
        public Boolean finished;

        public string src; //Python
        public string result;

        public byte[] hash; 
    }
}
