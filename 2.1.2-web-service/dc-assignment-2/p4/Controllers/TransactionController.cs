// Filename: TransactionController.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Handle transaction functionality 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using p4.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace p4.Controllers
{
    public class TransactionController : ApiController
    {
        //Get reference to BankDB 
        private BankDB.BankDB db = BankModel.db;

        /// <summary>
        /// Get list of transactions enqueued 
        /// </summary>
        /// <returns>List of transactions represented by their IDs</returns>
        [Route("api/Transaction/")]
        [HttpGet]
        public List<uint> ShowTransactions()
        {
            Logger.Log("[TransactionController] Getting transactions");

            BankDB.TransactionAccessInterface tr = db.GetTransactionInterface();
            return tr.GetTransactions();
        }

        /// <summary>
        /// Send funds between 2 accounts given sender account ID, reciever account ID and amount 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciever"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Route("api/Transaction/Send/{sender}/{reciever}/{amount}")]
        [HttpPost]
        public Boolean Send(uint sender, uint reciever, uint amount)
        {
            Logger.Log("[TransactionController] Sending $" + amount + " from user account ID: " + sender + " to: " + reciever);

            BankDB.TransactionAccessInterface tr = db.GetTransactionInterface();
            TransactionDetailsStruct trans = new TransactionDetailsStruct();

            try
            {
                trans.id = tr.CreateTransaction();
                trans.senderId = sender;
                trans.recieverId = reciever;
                trans.amount = amount;

                tr.SelectTransaction(trans.id);
                tr.SetAmount(amount);
                tr.SetSendr(sender);
                tr.SetRecvr(reciever);
            }
            catch(Exception e)
            {
                Logger.Error("[TransactionController] " + e.Message);
                return false; 
            }

            return true; 
        }

        /// <summary>
        /// Get details of a transaction given transaction ID 
        /// </summary>
        /// <param name="trid"></param>
        /// <returns>TransactionDetailsStruct object representing transaction</returns>
        [Route("api/Transaction/Details/{trid}")]
        [HttpGet]
        public TransactionDetailsStruct GetTransactionDetails(uint trid)
        {
            Logger.Log("[TransactionController] Retrieving transaction details for transaction ID: " + trid);

            BankDB.TransactionAccessInterface tr = db.GetTransactionInterface();
            TransactionDetailsStruct trans = new TransactionDetailsStruct();

            try
            {
                tr.SelectTransaction(trid);

                trans.id = trid;
                trans.senderId = tr.GetSendrAcct();
                trans.recieverId = tr.GetRecvrAcct();
                trans.amount = tr.GetAmount();
            }
            catch(Exception e)
            {
                Logger.Error("[TransactionController] " + e.Message); 
            }

            return trans; 
        }
    }
}