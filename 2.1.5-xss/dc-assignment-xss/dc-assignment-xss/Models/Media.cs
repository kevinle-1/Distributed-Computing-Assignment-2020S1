// Filename: Media.cs
// Project:  DC Assignment 2020
// Purpose:  Object representing media
// Author:   Kevin Le (19472960)
//
// Date:     28/05/2020

namespace dc_assignment_xss.Models
{
    public class Media
    {
        public string title;
        public string description;
        public string url;
        public string type;

        /// <summary>
        /// Returns string representation of media object 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return title + ", " + description + ", " + url + ", " + type;
        }
    }
}