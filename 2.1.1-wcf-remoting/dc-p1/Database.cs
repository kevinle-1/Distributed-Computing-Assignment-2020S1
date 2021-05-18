// Filename: Database.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Stores all bank user information 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System.Collections.Generic;

namespace dc_p1
{
    public class Database
    {
        private List<DatabaseLib.DataStruct> dataStruct;
        
        /// <summary>
        /// Instantiate new database using random data. 
        /// </summary>
        public Database()
        {
            uint pin, acctNo;
            int balance; 
            string firstName, lastName, imagePath;

            dataStruct = new List<DatabaseLib.DataStruct>();

            DatabaseLib.RandomFieldGenerator rand = new DatabaseLib.RandomFieldGenerator(); //Instatiate new random field generator 

            for (int ii = 0; ii < 100000; ii++) //Create 100000 entries 
            {
                DatabaseLib.DataStruct ds = new DatabaseLib.DataStruct();

                rand.getAccount(out pin, out acctNo, out firstName, out lastName, out balance, out imagePath); //Create new random account 

                ds.pin = pin;
                ds.acctNo = acctNo;
                ds.firstName = firstName;
                ds.lastName = lastName;
                ds.balance = balance;
                ds.imagePath = imagePath; //Set new DataStruct values to the randomly generated values 

                dataStruct.Add(ds); //Add it to the list 
            }
        }

        //Getters
        public uint GetAcctNoByIndex(int index)
        {
            return dataStruct[index].acctNo;
        }
        public uint GetPINByIndex(int index)
        {
            return dataStruct[index].pin;
        }
        public string GetFirstNameByIndex(int index)
        {
            return dataStruct[index].firstName;
        }
        public string GetLastNameByIndex(int index)
        {
            return dataStruct[index].lastName;
        }
        public int GetBalanceByIndex(int index)
        {
            return dataStruct[index].balance;
        }
        public string GetImagePath(int index)
        {
            return dataStruct[index].imagePath;
        }

        public int GetNumRecords()
        {
            return dataStruct.Count;
        }

        //Setters
        public void SetAcctNoByIndex(int index, uint accNo)
        {
            dataStruct[index].acctNo = accNo;
        }
        public void SetPINByIndex(int index, uint pin)
        {
            dataStruct[index].pin = pin;
        }
        public void SetFirstNameByIndex(int index, string fName)
        {
            dataStruct[index].firstName = fName;
        }
        public void SetLastNameByIndex(int index, string lName)
        {
            dataStruct[index].lastName = lName;
        }
        public void SetBalanceByIndex(int index, int bal)
        {
            dataStruct[index].balance = bal;
        }
        public void SetImagePathByIndex(int index, string path)
        {
            dataStruct[index].imagePath = path; 
        }
    }
}