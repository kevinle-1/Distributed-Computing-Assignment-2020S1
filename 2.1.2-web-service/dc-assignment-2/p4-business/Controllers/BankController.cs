// Filename: BankController.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Provides API endpoint for Web view 
// Contacts the various APIs in Data tier to perform operations 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using p4_business.Models;
using RestSharp;
using System.Web.Http;


namespace p4_business.Controllers
{
    public class BankController : ApiController
    {
        private string URL = "https://localhost:44396/"; //Data API 
        private static uint transactionsInQueue = 0;

        //Manage Users

        /// <summary>
        /// Create new user with First name and Last name 
        /// </summary>
        /// 
        /// <param name="fname">String first name</param>
        /// <param name="lname">String last name</param>
        [Route("api/Bank/CreateUser/{fname}/{lname}")]
        [HttpPost]
        public void CreateUser(string fname, string lname)
        {
            Logger.Log("Creating user with details: " + fname + ", " + lname);

            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("/api/User/Create/" + fname + "/" + lname);

            client.Post(request);

            Save(); //Save to file on create
        }        
        
        /// <summary>
        /// Update user first and last name based on their user ID 
        /// </summary>
        /// 
        /// <param name="uid">Unsigned integer ID</param>
        /// <param name="fname"></param>
        /// <param name="lname"></param>
        /// <returns>Returns </returns>
        [Route("api/Bank/UpdateUser/{uid}/{fname}/{lname}")]
        [HttpPost]
        public string UpdateUser(uint uid, string fname, string lname)
        {
            Logger.Log("Changing user name for ID: " + uid + " to: " + fname + ", " + lname);
         
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("/api/User/SetUserName/" + uid + "/" + fname + "/" + lname);

            IRestResponse response = client.Post(request);

            Save(); //Save to file on update 
            return response.Content; //Boolean true/ false if success in response 
        }

        /// <summary>
        /// Get user details based on user ID 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>User details in JSON format</returns>
        [Route("api/Bank/UserDetails/{uid}")]
        [HttpGet]
        public string UserDetails(uint uid)
        {
            Logger.Log("Getting user details for ID: " + uid);

            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("/api/User/Details/" + uid);

            IRestResponse response = client.Get(request);
            return response.Content; 
        }

        //Manage Accounts 

        /// <summary>
        /// Create an account given a User ID 
        /// Note: Will still work if user doesn't exist 
        /// </summary>
        /// <param name="uid">Unsigned integer User ID </param>
        [Route("api/Bank/CreateAccount/{uid}")]
        [HttpPost]
        public void CreateAccount(uint uid)
        {
            Logger.Log("Opening account for user ID: " + uid);

            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("/api/Account/Create/" + uid);

            client.Post(request);

            Save();
        }

        /// <summary>
        /// Get account details given account ID 
        /// </summary>
        /// <param name="aid"></param>
        /// <returns>JSON response containing account details</returns>
        [Route("api/Bank/AccountDetails/{aid}")]
        [HttpGet]
        public string AccountDetails(uint aid)
        {
            Logger.Log("Getting account details for ID: " + aid);

            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("/api/Account/Details/" + aid);

            IRestResponse response = client.Get(request);
            return response.Content;
        }

        /// <summary>
        /// Perform deposit for account ID 
        /// Adds amount to account 
        /// </summary>
        /// 
        /// <param name="aid"></param>
        /// <param name="amt"></param>
        /// <returns>String boolean if success or not</returns>
        [Route("api/Bank/Deposit/{aid}/{amt}")]
        [HttpPost]
        public string Deposit(uint aid, uint amt)
        {
            Logger.Log("Depositing $" + amt + " for: " + aid);

            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("/api/Account/Deposit/" + aid + "/" + amt);

            IRestResponse response = client.Post(request);

            Save();
            return response.Content; 
        }

        /// <summary>
        /// Perform withdrawal for account ID 
        /// Deducts amount from account 
        /// </summary>
        /// 
        /// <param name="aid"></param>
        /// <param name="amt"></param>
        /// <returns>String boolean if success or not</returns>
        [Route("api/Bank/Withdraw/{aid}/{amt}")]
        [HttpPost]
        public string Withdraw(uint aid, uint amt)
        {
            Logger.Log("Withdrawing $" + amt + " from: " + aid);

            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("/api/Account/Withdraw/" + aid + "/" + amt);

            IRestResponse response = client.Post(request);

            Save();
            return response.Content; 
        }

        //Manage Transactions

        /// <summary>
        /// Send money between accounts 
        /// </summary>
        /// 
        /// <param name="from">Account ID from</param>
        /// <param name="to">Account ID to</param>
        /// <param name="amt">Amount</param>
        /// <returns>String boolean if success or not</returns>
        [Route("api/Bank/Send/{from}/{to}/{amt}")]
        [HttpPost]
        public string Send(uint from, uint to, uint amt)
        {
            Logger.Log("Sending $" + amt + " from user account ID: " + from + " to: " + to);

            RestClient client = new RestClient(URL);

            RestRequest request = new RestRequest("/api/Transaction/Send/" + from + "/" + to + "/" + amt);
            IRestResponse response = client.Post(request);

            if (transactionsInQueue == 5) //Process transactions everytime there is 5 in the queue 
            {
                ProcessTransactionsInQueue();
                transactionsInQueue = 0; 
            }
            else
            {
                transactionsInQueue++; 
            }

            Save();

            return response.Content; 
        }

        /// <summary>
        /// Get details of transaction in queue 
        /// Note: If transaction not in queue, will return empty. 
        /// </summary>
        /// <param name="tid">Transaction ID</param>
        /// <returns>TransactionDetailsStruct as JSON</returns>
        [Route("api/Bank/Transaction/{tid}")]
        [HttpGet]
        public string Transaction(uint tid)
        {
            Logger.Log("Getting transaction details for transaction ID: " + tid);

            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("/api/Transaction/Details/" + tid);

            IRestResponse response = client.Get(request);

            return response.Content;
        }

        /// <summary>
        /// Submit request to Data tier admin controller to save to disk 
        /// Note: Not directly callable 
        /// </summary>
        private void Save()
        {
            Logger.Log("Submitting request to save state to disk...");

            RestClient client = new RestClient(URL);

            RestRequest requestSave = new RestRequest("/api/Admin/Save");
            client.Post(requestSave);
        }

        /// <summary>
        /// Submit request to Data tier admin controller to process all transactions 
        /// Note: Not directly callable 
        /// </summary>
        private void ProcessTransactionsInQueue()
        {
            Logger.Log("Submitting request to process transactions...");

            RestClient client = new RestClient(URL);

            RestRequest requestTransaction = new RestRequest("/api/Admin/ProcessTransactions");
            client.Post(requestTransaction);
        }
    }
}