// Filename: Server.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Starts Data Server 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System;
using System.ServiceModel;

namespace dc_p1_data
{
    class Server
    {
        /// <summary>
        /// Launches console to start dataserver on localhost on port 8100 
        /// </summary>
        /// <param name="args">Unused</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Data server starting...");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();

            host = new ServiceHost(typeof(DataServer));
            host.AddServiceEndpoint(typeof(DataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");

            host.Open(); //Start

            Console.WriteLine("Data server started.");
            Console.ReadLine();

            host.Close(); 
        }
    }
}
