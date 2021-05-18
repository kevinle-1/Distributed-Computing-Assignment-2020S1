// Filename: UserController.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Handle user related functionality  
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using p4.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace p4.Controllers
{
    public class UserController : ApiController
    {
        //Get reference to Bank DB
        private BankDB.BankDB db = BankModel.db;

        /// <summary>
        /// Get user details given user ID 
        /// </summary>
        /// <param name="userID">Integer User ID</param>
        /// <returns>UserDetailsStruct object representing user</returns>
        [Route("api/User/Details/{userID}")]
        [HttpGet]
        public UserDetailsStruct GetUserDetails(uint userID) //User must exist 
        {
            Logger.Log("[UserController] Getting user details for ID: " + userID);

            UserDetailsStruct usr = new UserDetailsStruct();
            BankDB.UserAccessInterface user = db.GetUserAccess();

            try
            {
                user.SelectUser(userID);
                user.GetUserName(out usr.fName, out usr.lName);
                usr.id = userID;
            }
            catch (Exception e)
            {
                Logger.Error("[UserController] " + e.Message); //If BankDB.NoUserException "No user is selected" - Often due to user not existing
            }

            return usr;
        }

        /// <summary>
        /// Create new user given First and Last name 
        /// </summary>
        /// <param name="fName">String first name</param>
        /// <param name="lName">String last name</param>
        [Route("api/User/Create/{fname}/{lname}")]
        [HttpPost]
        public void CreateUser(string fName, string lName)
        {
            string sanitizedFname = SanitizeInput(fName); 
            string sanitizedLname = SanitizeInput(lName); 

            Logger.Log("[UserController] Creating user with first and last name: " + sanitizedFname + ", " + sanitizedLname);

            BankDB.UserAccessInterface user = db.GetUserAccess();

            uint id = user.CreateUser();
            user.SelectUser(id);
            user.SetUserName(sanitizedFname, sanitizedLname); 
        }          
        
        /// <summary>
        /// Update user first and last name given user ID to update 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <returns>Boolean if successful</returns>
        [Route("api/User/SetUserName/{userID}/{fname}/{lname}")]
        [HttpPost]
        public Boolean SetUserName(uint userID, string fName, string lName)
        {
            string sanitizedFname = SanitizeInput(fName);
            string sanitizedLname = SanitizeInput(lName);

            Logger.Log("[UserController] Setting user with ID: " + userID + " first and last name to: " + sanitizedFname + ", " + sanitizedLname);

            BankDB.UserAccessInterface user = db.GetUserAccess();

            try
            {
                user.SelectUser(userID);
                user.SetUserName(sanitizedFname, sanitizedLname);
            }
            catch (Exception e) 
            {
                Logger.Error("[UserController] " + e.Message);
                return false; 
            }

            return true; 
        }        
        
        /// <summary>
        /// Get List of users 
        /// </summary>
        /// <returns>List of Users represented by their IDs</returns>
        [Route("api/User/")]
        [HttpGet]
        public List<uint> GetUsers()
        {
            Logger.Log("[UserController] Getting list of users");

            BankDB.UserAccessInterface user = db.GetUserAccess();
            return user.GetUsers(); 
        }

        /// <summary>
        /// Sanitize input string as an extra measure, cannot trust the client. 
        /// Removes all invalid characters 
        /// </summary>
        /// 
        /// References: https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-strip-invalid-characters-from-a-string
        /// "The regular expression pattern [^\w\.@-] matches any character that is not a word character, a period, an @ symbol, or a hyphen. "
        /// 
        /// <param name="input"></param>
        /// <returns>String with any invalid characters removed</returns>
        private string SanitizeInput(string input)
        {
            string retString;

            try
            {
                retString = Regex.Replace(input, @"[^\w\.@-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
                Logger.Log("[UserController] Input sanitized to: " + retString);
            }
            catch (RegexMatchTimeoutException)
            {
                retString = String.Empty;
                Logger.Error("[UserController] Input could not be sanitized (Timed out)");
            }

            return retString; 
        }
    }
}