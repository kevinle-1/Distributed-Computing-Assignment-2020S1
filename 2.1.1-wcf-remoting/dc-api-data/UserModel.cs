// Filename: UserModel.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Object representing a user 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

namespace dc_api_data
{
    public class UserModel
    {
        public int bal; 
        public uint acct; 
        public uint pin; 
        public string fName; 
        public string lName;
        public string imagePath; 

        public int idx; 

        /// <summary>
        /// Returns string representation of user 
        /// </summary>
        /// <returns>^</returns>
        public override string ToString()
        {
            return idx.ToString() + ", " + bal.ToString() + ", " + acct.ToString() + ", " + pin.ToString() + ", " + fName + ", " + lName + ", " + imagePath;
        }
    }
}
