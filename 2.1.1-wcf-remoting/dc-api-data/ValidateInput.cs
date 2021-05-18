// Filename: ValidateInput.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Configuration for Web API 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

namespace dc_api_data
{
    public class ValidateInput
    {
        /// <summary>
        /// Checks if a string is a valid integer 
        /// </summary>
        /// 
        /// References: Based on algorithm at: 
        ///             https://stackoverflow.com/questions/7461080/fastest-way-to-check-if-string-contains-only-digits
        /// 
        /// <param name="str">String</param>
        /// <returns>Boolean if valid = true, else false</returns>
        public static bool IsValidInteger(string str)
        {
            if (str.Length > 10 || str.Equals("")) //Not more than 10 digits, or empty 
            {
                return false;
            }
            else 
            {
                foreach (char c in str)
                {
                    if (c < '0' || c > '9') //Numbers only 
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
