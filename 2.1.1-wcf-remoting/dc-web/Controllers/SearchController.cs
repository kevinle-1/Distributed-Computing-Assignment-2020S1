// Filename: SearchController.cs
// Project:  DC Assignment (COMP3008)
// Purpose   Post method for Searching through bank users
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System.Web.Http;
using dc_api_data;
using dc_p1_web.Models;
using dc_web.Models;

namespace dc_web.Controllers
{
    public class SearchController : ApiController
    {
        /// <summary>
        /// Calls the DataModel to query it for a user based on the SearchData provided. 
        /// .searchForEntry() returns the first user that matches the search. 
        /// </summary>
        /// <param name="sd">SearchData representing search query</param>
        /// <returns>UserModel object representing user - Account number will be 0 if no result</returns>
        public UserModel Post(SearchData sd)
        {
            UserModel user = new UserModel();
            DataModel dm = new DataModel();

            Logger.Log("Search performed for: " + sd.searchStr);

            dm.searchForEntry(sd.searchStr, out user.acct, out user.pin, out user.bal, out user.fName, out user.lName, out user.imagePath, out user.idx);

            Logger.Log("Search result to be returned: " + user.ToString());

            return user; 
        }
    }
}