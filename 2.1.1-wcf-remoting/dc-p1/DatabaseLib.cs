// Filename: DatabaseLib.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Database miscellaneous 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System;
using System.IO;

public class DatabaseLib
{
    /// <summary>
    /// DataStruct representing entries in the database 
    /// </summary>
    internal class DataStruct
    {
        public uint acctNo;
        public uint pin;
        public int balance;
        public string firstName;
        public string lastName;
        public string imagePath; 

        /// <summary>
        /// Instatiate new DataStruct with 0 or "" fields
        /// </summary>
        public DataStruct()
        {
            acctNo = 0;
            pin = 0;
            balance = 0;
            firstName = "";
            lastName = "";
            imagePath = ""; 
        }
    }


    internal class RandomFieldGenerator
    {
        private Random r; 

        /// <summary>
        /// Instantiate a new RandomFieldGenerator, ensuring Random is only created once 
        /// If created on every function - it will not be truly random. 
        /// </summary>
        public RandomFieldGenerator()
        {
            r = new Random();
        }

        /// <summary>
        /// Returns randomly generated data for Database to use to fill DataStructs  
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="acctNo"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="balance"></param>
        /// <param name="imagePath"></param>
        public void getAccount(out uint pin, out uint acctNo, out string firstName, out string lastName, out int balance, out string imagePath)
        {
            acctNo = getAcctNo();
            firstName = getFirstName();
            lastName = getLastName();
            balance = getBalance();
            pin = getPin();
            imagePath = getImage(); 
        }

        /// <summary>
        /// Returns a random first name based on random array of names index.
        /// </summary>
        /// <returns>String first name</returns>
        private string getFirstName()
        {
            string[] fNames = { "William", "Justin", "Adam", "Alex", "Aaron", "Ben", "Carl", "Dan", "David", "Edward", "Fred", "Frank", "George", "Hal", "Hank", "Ike", "John", "Jack", "Joe", "Larry", "Monte", "Matthew", "Mark", "Nathan", "Otto", "Paul", "Peter", "Roger", "Roger", "Steve", "Thomas", "Kevin", "Tim", "Ty", "Victor", "Walter" };
            string name = fNames[r.Next(0, fNames.Length - 1)];
            return name;
        }

        /// <summary>
        /// Returns a random last name based on random array of names index.
        /// </summary>
        /// <returns>String last name</returns>
        private string getLastName()
        {
            string[] lNames = { "Anderson", "Ashwoon", "Aikin", "Bateman", "Bongard", "Bowers", "Boyd", "Cannon", "Cast", "Deitz", "Dewalt", "Ebner", "Frick", "Hancock", "Haworth", "Hesch", "Hoffman", "Kassing", "Knutson", "Lawless", "Lawicki", "Mccord", "McCormack", "Miller", "Myers", "Nugent", "Ortiz", "Orwig", "Ory", "Paiser", "Pak", "Pettigrew", "Quinn", "Quizoz", "Ramachandran", "Resnick", "Sagar", "Schickowski", "Schiebel", "Sellon", "Severson", "Shaffer", "Solberg", "Soloman", "Sonderling", "Soukup", "Soulis", "Stahl", "Sweeney", "Tandy", "Trebil", "Trusela", "Trussel", "Turco" };
            return lNames[r.Next(0, lNames.Length - 1)];
        }

        /// <summary>
        /// Returns a random pin 
        /// </summary>
        /// <returns>4 digit integer</returns>
        private uint getPin() 
        {
            return (uint)r.Next(1000, 9999);
        }

        /// <summary>
        /// Returns random account number 
        /// </summary>
        /// <returns>8 digit integer</returns>
        private uint getAcctNo() 
        {
            return (uint)r.Next(10000000, 99999999);
        }

        /// <summary>
        /// Returns user balance - Assumed it can be negative (in debt) 
        /// </summary>
        /// <returns>Rand number from -10000 to 100000</returns>
        private int getBalance() 
        {
            return r.Next(-10000, 1000000);
        }

        /// <summary>
        /// Get a random placeholder image from the /images/ folder. 
        /// Filenames are 1.png, 2.png, ... 5.png 
        /// </summary>
        /// <returns>String representing path of image</returns>
        private string getImage()
        {
            string image = r.Next(1, 5).ToString() + ".png";
            return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "images", image)); //Go up 3 folders and get random image from /images/ folder. 
        }
    }
}
