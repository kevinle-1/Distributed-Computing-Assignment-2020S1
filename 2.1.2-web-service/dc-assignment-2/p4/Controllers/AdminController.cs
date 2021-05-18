// Filename: AdminController.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Handle administrative functionality that only certain things can call 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using p4.Models;
using System.Web.Http;

namespace p4.Controllers
{
    public class AdminController : ApiController
    {
        //Get reference to BankDB 
        private BankDB.BankDB db = BankModel.db;

        /// <summary>
        /// Saves current bank state to a file 
        /// </summary>
        [Route("api/Admin/Save")]
        [HttpPost]
        public void Save()
        {
            Logger.Log("[AdminController] Saving to disk");

            db.SaveToDisk(); 
        }        
        
        /// <summary>
        /// Process all transactions in queue 
        /// </summary>
        [Route("api/Admin/ProcessTransactions")]
        [HttpPost]
        public void ProcessTransactions()
        {
            Logger.Log("[AdminController] Processing transactions");

            db.ProcessAllTransactions(); 
        }
    }
}