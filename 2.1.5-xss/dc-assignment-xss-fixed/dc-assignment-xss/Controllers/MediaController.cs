// Filename: MediaController.cs
// Project:  DC Assignment 2020
// Purpose:  API for MediaDB.
// Author:   Kevin Le (19472960)
//
// Date:     28/05/2020

using dc_assignment_xss.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
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
        /// 
        /// Based on approach detailed at: https://docs.microsoft.com/en-us/aspnet/core/security/cross-site-scripting 
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
                if(validURL)
                {
                    Media tempMedia = new Media();

                    //HTML encode URLs to prevent special characters being rendered 
                    tempMedia.title = HtmlEncoder.Default.Encode(m.title);
                    tempMedia.description = HtmlEncoder.Default.Encode(m.description);
                    tempMedia.url = HtmlEncoder.Default.Encode(m.url);
                    tempMedia.type = HtmlEncoder.Default.Encode(m.type);

                    db.AddMedia(tempMedia);
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