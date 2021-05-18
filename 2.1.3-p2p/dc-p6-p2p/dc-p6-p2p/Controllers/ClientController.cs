// Filename: ClientController.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Controller client API 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using dc_p6_p2p.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace dc_p6_p2p.Controllers
{
    public class ClientController : ApiController
    {
        //Determine if it is the start is the first start of the client 
        private Boolean firstStart = true; 

        /// <summary>
        /// Allow a client to register by providing their IP and PORT 
        /// </summary>
        /// <param name="ip">String IP</param>
        /// <param name="port">String Port</param>
        [Route("api/Client/Register/{ip}/{port}")]
        [HttpPost]
        public void Register(string ip, string port)
        {
            if(Database.GetClients().Count == 0 && firstStart) //If this is the first client, start monitoring 
            {
                Monitor.StartMonitor(); //StartMonitor()'s job is to monitor and remove clients if they go offline -> Only want to call it once to start the loop 
                firstStart = false; 
            }

            Database.AddClient(ip, port); 
        }
        
        /// <summary>
        /// Get a list of clients that are registered
        /// </summary>
        /// <returns>Returns list of clients represented by client objects</returns>
        [Route("api/Client/Clients")]
        [HttpGet]
        public List<Client> Get()
        {
            return Database.GetClients(); 
        }       
        
        /// <summary>
        /// Clients call this when they've finished a job to increase their tally of jobs completed.
        /// Clients are identified by their job server IP and PORT which are assumed to be unique. 
        /// </summary>
        /// <param name="ip">String IP</param>
        /// <param name="port">String Port</param>
        [Route("api/Client/CompletedJob/{ip}/{port}")]
        [HttpPost]
        public void CompletedJob(string ip, string port)
        {
            Database.CompletedJob(ip, port);
        }
    }
}