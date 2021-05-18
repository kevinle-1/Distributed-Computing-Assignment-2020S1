/**
* Filename: media.js
* Project:  DC Assignment 2020
* Purpose:  Handle all Javascript functionality of Index.cshtml and Add.cshtml
*          Vulnerable to DOM, Persistent and Reflective XSS    
*            
* Author:   Kevin Le (19472960)
*
* Date:     25/05/2020
*/

/**
 * Purpose: Sends a Post request to API to add a new media object if fields not empty 
 *          Used by Add.cshtml. Triggered when "Add" button clicked. 
 * */
function addEntry() {
    var title = $("#title").val(); //Get value of Title <input> 
    var description = $("#description").val(); //Get value of Description <input> 
    var url = $("#url").val(); //Get value of URL <input> 

    if (title && description && url) { //If string not empty 

        var type = "Movie"; //Defaults to movie 

        //Check which Radio button is selected 
        if (document.getElementById("tv").checked) {
            type = "TV";
        }

        //Create a new Media object to send 
        var media = new Object();
        media.title = title;
        media.description = description;
        media.url = url;
        media.type = type;

        $.ajax({
            url: "/api/Media/Add",
            type: "post",
            data: JSON.stringify(media),
            contentType: "application/json; charset=utf-8",
            dataType: "json", //Send as JSON 
            processData: false,
            success: function (data, textStatus, jQxhr) {
                window.location.href = "/"; //Return user back to home page (Index.cshtml)
            },
            error: function (jqXhr, textStatus, errorThrown) {
                var response = JSON.parse(jqXhr.responseText); //Get response from HtmlResponseException
                alert("Couldn't add media. " + response.Message); //Show Alert box with error message 
            }
        });
    }
    else {
        alert("Fields must not be empty!");
    }
}

/**
 * Purpose: Uses JQuery to add user comment to table of comments if fields not empty. 
 *          Used in Index.cshtml. Triggered when "Comment" is clicked. 
 *          
 * Note: Vulnerable to DOM XSS using the .innerHTML method. Adds new row to table without proper encoding. 
 * */
function addComment() {
    var name = document.getElementById("commenterName").value; //Get values in <input> for commenter name and the comment 
    var text = document.getElementById("commenterText").value;

    var table = document.getElementById("commenttable");

    if (name && text) { //If values aren't empty - Add it to the comment table 
        var row = table.insertRow(-1); 
        var cell = row.insertCell(-1); 

        cell.innerHTML = "<tr><td><p><strong>" + name + ": </strong>" + text + "</p></tr></td>"
    }
    else {
        alert("Comment must not be empty!"); 
    }
    
}

/**
 * Purpose: Determine if URL contains query for "title" and calls getEntries with corresponding 
 *          imports. Triggered everytime window is loaded to load list of media from the Media DB
 *          via API calls. 
 *          
 * Note: Vulnerable to Reflective XSS - Shows the search query without proper encoding. 
 * */
window.onload = function loadEntries() {

    var currentURL = new URL(window.location.href); //Get current window address 

    if (currentURL.searchParams.get("title")) { //If the current window has the parameter "title" with non null/ empty query 

        var queryView = document.getElementById("queryString"); //Get reference to the element that shows the query. "Searching for: .." 
        var query = currentURL.searchParams.get("title"); //Get the search query 

        queryView.innerHTML = "Searching for: " + query; //Set the query the user has input 
        queryView.style.visibility = "visible"; //Show the query 

        getEntries(query); //Populate table with results 
    }
    else { //If current window doesn't have parameter "title" then show all 
        document.getElementById("queryString").style.visibility = "hidden"; //Don't show the query input (as none)
        getEntries(""); //Populate table with all media in list 
    }
}

/**
 * Purpose: Populate list of media with entries from List (retrieved from API) based on the filter "query".
 *          Called by loadEntries() 
 * 
 * Note: Part of the persistent XSS vulnerability, shows stored user input without proper encoding/ filtering.
 * @param {any} query
 */
function getEntries(query) {
$.ajax(
    {
        url: "/api/Media",
        type: "get",
        contentType: "application/json",
        dataType: "json",
        processData: false,
        success: function (data, textStatus, jQxhr) {

            data.forEach(function (media) { //For each media entry 
                if (query == "" || media.title.includes(query)) { //If there is a query, match the title 

                    //Persistent XSS
                    $(".media .table").find("tbody").append("<tr><td><img class=\"poster\" src=" + media.url + "></td><td><h3>" + media.title + "<br></h3><p> " + media.description + "</p></td><td>" + media.type + "</td></tr>");
                }
            });
        },
        error: function (jqXhr, textStatus, errorThrown) {
            console.log(errorThrown);
            alert("Error: Could not retrieve media")
        }
    });
}

/**
 * Purpose: Search for media by title. Triggered when "Search" clicked. 
 * */
function search()
{
    var searchQuery = document.getElementById("query").value; //Get the value of the search query 

    if (searchQuery) { //If search query not empty/ null 
        window.location.href = "/?title=" + searchQuery; //Load page with "title" query parameter 
    }
    else { 
        alert("Search must not be empty!"); 
    }
}