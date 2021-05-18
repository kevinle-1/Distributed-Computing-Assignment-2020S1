// Filename: MediaController.cs
// Project:  DC Assignment 2020
// Purpose:  API for MediaDB.
// Author:   Kevin Le (19472960)
//
// Date:     28/05/2020


using dc_assignment_xss.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace dc_assignment_xss.Controllers
{
    public class MediaController : ApiController
    {
        private MediaDB db = new MediaDB();

        /// <summary>
        /// Function to return list of media objects 
        /// </summary>
        /// <returns></returns>
        [Route("api/Media")]
        [HttpGet]
        public List<Media> GetMediaList()
        {
            return db.GetMedia();
        }

        /// <summary>
        /// Function to add Media object, as recieved from user input in Add page of Add.cshtml
        /// </summary>
        /// <param name="m"></param>
        [Route("api/Media/Add")]
        [HttpPost]
        public void AddMedia(Media m)
        {
            //Check if URL valid 
            Uri uriResult;
            bool validURL = Uri.TryCreate(m.url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            //Check if any fields are somehow empty 
            if (!(String.IsNullOrEmpty(m.title) || String.IsNullOrEmpty(m.description) || String.IsNullOrEmpty(m.url) || String.IsNullOrEmpty(m.type)))
            {
                if (validURL)
                {
                    db.AddMedia(m);
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid URL"));
                }
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Empty fields"));
            }
        }
    }
}