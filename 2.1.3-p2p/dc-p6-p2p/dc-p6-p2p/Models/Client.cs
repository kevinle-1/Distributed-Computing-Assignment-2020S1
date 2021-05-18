// Filename: Client.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Object representing a Client (their job server) 
//           It is assumed a client is uniquely identifiable by their IP and Port
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

namespace dc_p6_p2p.Models
{
    public class Client
    {
        public string ip;
        public string port;

        public int jobsCompleted;

        /// <summary>
        /// Returns string representation of client 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ip + ":" + port;
        }
    }
}