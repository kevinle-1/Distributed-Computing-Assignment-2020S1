using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace dc_p6_jobserver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Job Server Starting...");

            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();

            host = new ServiceHost(typeof(JobServer));
            host.AddServiceEndpoint(typeof(JobServerInterface), tcp, "net.tcp://localhost:999/JobServer");

            host.Open(); //Start Job server 

            Console.WriteLine("Job Server Started");
            Console.ReadLine();
        }
    }
}
