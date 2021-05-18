// Filename: DataServer.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Mock database for banking system 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System;
using System.ServiceModel;
using dc_p1;

namespace dc_p1_data
{
    //Ensure multiple databases aren't made. 
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    internal class DataServer : DataServerInterface
    {
        private Database db = new Database(); //Create new database 

        /// <summary>
        /// Get number of entries in database 
        /// </summary>
        /// <returns>Integer representing number of entries</returns>
        public int GetNumEntries()
        {
            return db.GetNumRecords();
        }

        /// <summary>
        /// Returns user information based on index provided
        /// </summary>
        /// 
        /// <param name="index"></param>
        /// <param name="acctNo"></param>
        /// <param name="pin"></param>
        /// <param name="bal"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <param name="iPath"></param>
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out string iPath)
        {
            acctNo = 0;
            pin = 0;
            bal = 0;
            fName = "";
            lName = "";
            iPath = ""; 

            if (index >= 0 && index < db.GetNumRecords()) //Bounds check
            {
                acctNo = db.GetAcctNoByIndex(index);
                pin = db.GetPINByIndex(index);
                bal = db.GetBalanceByIndex(index);
                fName = db.GetFirstNameByIndex(index);
                lName = db.GetLastNameByIndex(index);

                iPath = db.GetImagePath(index); 
            }
            else
            {
                Console.WriteLine("[Data] Error: Index out of bounds."); 
            }
        }

        /// <summary>
        /// Updates user entry based on index and fields provided.
        /// </summary>
        /// 
        /// <param name="index"></param>
        /// <param name="acctNo"></param>
        /// <param name="pin"></param>
        /// <param name="bal"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <param name="imagePath"></param>
        public void UpdateEntry(int index, uint acctNo, uint pin, int bal, string fName, string lName, string imagePath)
        {
            if (index >= 0 && index < db.GetNumRecords()) //Bounds check
            {
                db.SetAcctNoByIndex(index, acctNo);
                db.SetPINByIndex(index, pin);
                db.SetBalanceByIndex(index, bal);
                db.SetFirstNameByIndex(index, fName);
                db.SetLastNameByIndex(index, lName);
                db.SetImagePathByIndex(index, imagePath);
            }
            else
            {
                Console.WriteLine("[Data] Error: Index out of bounds.");
            }
        }
    }
}
