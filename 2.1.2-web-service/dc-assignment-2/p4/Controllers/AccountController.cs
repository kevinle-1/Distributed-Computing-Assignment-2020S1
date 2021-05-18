// Filename: AccountController.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Web API controller for Accounts 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using p4.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace p4.Controllers
{
    public class AccountController : ApiController
    {
        //Get reference to BankDB (Static)
        private BankDB.BankDB db = BankModel.db;

        /// <summary>
        /// Get account details based on provided Account ID 
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns>AccountDetailsStruct representing data</returns>
        [Route("api/Account/Details/{accountID}")]
        [HttpGet]
        public AccountDetailsStruct GetAccountDetails(uint accountID) //Account must exist 
        {
            AccountDetailsStruct ad = new AccountDetailsStruct(); //AccountDetailsStruct to hold account data 

            Logger.Log("[AccountController] Getting account details for ID: " + accountID);

            BankDB.AccountAccessInterface acc = db.GetAccountInterface();

            try
            {
                acc.SelectAccount(accountID); //Select account and populate struct with account data
                ad.bal = acc.GetBalance(); //NoAccountException if account id does not exist
                ad.accId = accountID;
                ad.userId = acc.GetOwner();
            }
            catch(Exception e)
            {
                Logger.Error("[AccountController] Account does not exist" + e.Message); 
            }

            return ad;
        }
        
        /// <summary>
        /// Create a new account based on User ID 
        /// </summary>
        /// <param name="userID">Unsigned integer user ID</param>
        [Route("api/Account/Create/{userID}")]
        [HttpPost]
        public void CreateAccount(uint userID)
        {
            BankDB.AccountAccessInterface acc = db.GetAccountInterface();
            uint accId = 0; 

            Logger.Log("[AccountController] Creating account for user ID: " + userID); //Creates account regardless if user exists or not 

            try
            {
                accId = acc.CreateAccount(userID);
            }
            catch(Exception e)
            {
                Logger.Error("[AccountController] " + e.Message);
            }
        }

        /// <summary>
        /// Deposit amount into account ID 
        /// </summary>
        /// 
        /// <param name="accountID">Unsigned integer Account ID</param>
        /// <param name="amt">Unsigned integer amount to deposit</param>
        /// <returns></returns>
        [Route("api/Account/Deposit/{accountID}/{amt}")]
        [HttpPost]
        public Boolean Deposit(uint accountID, uint amt)
        {
            BankDB.AccountAccessInterface acc = db.GetAccountInterface();

            Logger.Log("[AccountController] Depositing $" + amt + " into account with ID: " + accountID);

            try
            {
                acc.SelectAccount(accountID);
                acc.Deposit(amt);
            }
            catch(Exception e)
            {
                Logger.Error("[AccountController] " + e.Message);
                return false; 
            }

            return true; 
        }

        /// <summary>
        /// Withdraw amount from Account ID 
        /// </summary>
        /// 
        /// <param name="accountID"></param>
        /// <param name="amt"></param>
        /// <returns>Boolean if withdrawl successful</returns>
        [Route("api/Account/Withdraw/{accountID}/{amt}")]
        [HttpPost]
        public Boolean Withdraw(uint accountID, uint amt)
        {
            BankDB.AccountAccessInterface acc = db.GetAccountInterface();

            Logger.Log("[AccountController] Withdrawing $" + amt + " from account with ID: " + accountID);

            try
            {
                acc.SelectAccount(accountID);
                acc.Withdraw(amt);
            }
            catch(Exception e)
            {
                Logger.Error("[AccountController] " + e.Message);
                return false; 
            }

            return true; 
        }

        /// <summary>
        /// Get balance for given Account ID 
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns>Unsigned Integer Balance</returns>
        [Route("api/Account/Balance/{accountID}")]
        [HttpGet]
        public uint GetBalance(uint accountID)
        {
            BankDB.AccountAccessInterface acc = db.GetAccountInterface();

            uint bal = 0; 

            try
            {
                acc.SelectAccount(accountID);
                bal = acc.GetBalance();
            }
            catch(Exception e)
            {
                Logger.Error("[AccountController] " + e.Message);
            }

            return bal;
        }

        /// <summary>
        /// Get all accounts that a given user has 
        /// </summary>
        /// <param name="uid">Unsigned integer User ID</param>
        /// <returns></returns>
        [Route("api/Account/{uid}")]
        [HttpGet]
        public List<uint> GetAccounts(uint uid)
        {
            Logger.Log("[AccountController] Getting list of accounts for user ID: " + uid);

            List<uint> accountIDs = new List<uint>(); 

            try
            {
                BankDB.AccountAccessInterface acc = db.GetAccountInterface();
                accountIDs = acc.GetAccountIDsByUser(uid);
            }
            catch(Exception e)
            {
                Logger.Error("[AccountController] " + e.Message);
            }

            return accountIDs;
        }
    }
}