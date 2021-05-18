// Filename: ValuesController.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Controller for API to query bank database 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using dc_api_data;
using dc_p1_web.Models;
using dc_web.Models;
using System.Web.Http;

namespace dc_web.Controllers
{
    public class ValuesController : ApiController
    {
        /// <summary>
        /// Gets number of entries in the bank database 
        /// </summary>
        /// <returns>Integer representing number of entries in bank database</returns>
        public int Get()
        {
            DataModel dm = new DataModel();

            Logger.Log("Number of entries requested");

            return dm.getNumEntries(); 
        }

        /// <summary>
        /// Gets a user based on the provided ID 
        /// </summary>
        /// <param name="id">Integer ID of user</param>
        /// <returns>UserModel object representing user</returns>
        public UserModel Get(int id)
        {
            UserModel user = new UserModel();
            DataModel dm = new DataModel();

            Logger.Log("User data request for ID: " + id.ToString());

            user.idx = id; 
            dm.getValuesForEntry(id, out user.acct, out user.pin, out user.bal, out user.fName, out user.lName, out user.imagePath);

            Logger.Log("User at index to be returned: " + user.ToString());

            return user;
        }

        /// <summary>
        /// Update an existing user in the bank database 
        /// </summary>
        /// <param name="user">UserModel representing user but with updated values</param>
        public void Post(UserModel user)
        {
            DataModel dm = new DataModel();

            Logger.Log("Update user: " + user.ToString());

            dm.updateEntry(user.idx, user.acct, user.pin, user.bal, user.fName, user.lName, user.imagePath);
        }
    }
}