// Filename: MediaDB.cs
// Project:  DC Assignment 2020
// Purpose:  Mock database of media objects
// Author:   Kevin Le (19472960)
//
// Date:     28/05/2020

using System.Collections.Generic;
using System.Linq;

namespace dc_assignment_xss.Models
{
    public class MediaDB
    {
        private static List<Media> media = new List<Media>(); 

        /// <summary>
        /// Returns List of media 
        /// </summary>
        /// <returns></returns>
        public List<Media> GetMedia()
        {
            return media; 
        }

        /// <summary>
        /// Returns number of media in list 
        /// </summary>
        /// <returns></returns>
        public int GetNumMedia()
        {
            return media.Count();
        }

        /// <summary>
        /// Adds new media object to list 
        /// </summary>
        /// <param name="m"></param>
        public void AddMedia(Media m)
        {
            media.Add(m); 
        }
    }
}