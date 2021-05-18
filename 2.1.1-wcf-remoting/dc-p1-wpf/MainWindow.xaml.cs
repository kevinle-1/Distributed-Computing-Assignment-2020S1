// Filename: MainWindow.xaml.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Configuration for Web API 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using dc_api_data;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;

namespace dc_p1_wpf
{
    /*/// <summary>
    /// Delegate function prototype for asynchronous background search 
    /// </summary>
    /// <param name="lName"></param>
    /// <param name="acctNo"></param>
    /// <param name="pin"></param>
    /// <param name="bal"></param>
    /// <param name="fName"></param>
    /// <param name="lNameO"></param>
    public delegate void BackgroundSearch(string lName, out uint acctNo, out uint pin, out int bal, out string fName, out string lNameO);*/
    
    public partial class MainWindow : Window
    {
        private RestClient client;
        private string selectedImage; 

        private string url = "https://localhost:44382/"; //Business Tier API URL 

        /// <summary>
        /// Initialize the main window 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            UrlField.Text = url; 
            client = new RestClient(url);

            selectedImage = "";

            RestRequest request = new RestRequest("api/Values");

            IRestResponse count = client.Get(request);
            Index.Text = count.Content;
        }

        /// <summary>
        /// "Get" Button event handler for when user wants to get user by index 
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetBtn_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("[MainWindow] Task: Finding by index");

            ProgressBar.Value = 20; //Show progress bar as 20% -> Indicating retrieval is underway 

            if (ValidateInput.IsValidInteger(Index.Text)) //Check if valid number 
            {
                int idx = Int32.Parse(Index.Text) - 1; //Decrement index (array)
                RestRequest request = new RestRequest("api/Values/" + idx.ToString()); //Prepare new Get request 

                IRestResponse resp = await client.ExecuteAsync(request);

                if (resp.StatusCode == System.Net.HttpStatusCode.OK) //If status 200 
                {
                    UserModel details = JsonConvert.DeserializeObject<UserModel>(resp.Content); //Convert JSON response to UserModel object 

                    if (details.fName != "") //If the fields aren't empty (Indicates no result) -> Set the fields to the returned user 
                    {
                        selectedImage = "";

                        FNameBox.Text = details.fName;
                        LNameBox.Text = details.lName;
                        BalanceBox.Text = details.bal.ToString("C");
                        AcctNoBox.Text = details.acct.ToString();
                        PinBox.Text = details.pin.ToString("D4");

                        if (details.imagePath != "")
                        {
                            Photo.Source = new BitmapImage(new System.Uri(details.imagePath));
                        }
                        else
                        {
                            Photo.Source = null;
                        }
                    }
                    else //No user was found -> Display error 
                    {
                        Console.WriteLine("[MainWindow] Error: Index " + idx.ToString() + " does not exist");
                        MessageBox.Show("Error: Index not found.");
                    }
                }
                else //Non OK status 
                {
                    Console.WriteLine("[MainWindow] Error: Could not get details for index " + idx.ToString());
                    MessageBox.Show("Error: Could not get details for index.");
                }
            }
            else //Invalid index was provided -> Disply + Log error 
            {
                Console.WriteLine("[MainWindow] Error: Invalid number provided ");
                MessageBox.Show("Error: Please input numbers only (max 10 digits).");
            }

            ProgressBar.Value = 100; //Show retrieval as finished by setting progress bar to 100% 
        }

        /// <summary>
        /// "Search" Button click handler - Gets the Last Name input and performs search 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("[MainWindow] Task: Finding by Last name");

            SearchData search = new SearchData(); //Package query into SearchData object 
            search.searchStr = SearchField.Text;

            RestRequest request = new RestRequest("api/Search/"); //Prepare search 
            request.AddJsonBody(search);

            ProgressBar.Value = 20;

            IRestResponse resp = await client.ExecutePostAsync(request);

            if (resp.StatusCode == System.Net.HttpStatusCode.OK) //If 200 OK 
            {
                UserModel details = JsonConvert.DeserializeObject<UserModel>(resp.Content); //Convert JSON response to UserModel object 
                    
                if (details.lName != "") //If field not empty (Indicates no result) -> Update fields 
                {
                    selectedImage = ""; 

                    FNameBox.Text = details.fName;
                    LNameBox.Text = details.lName;
                    BalanceBox.Text = details.bal.ToString("C");
                    AcctNoBox.Text = details.acct.ToString();
                    PinBox.Text = details.pin.ToString("D4");

                    Index.Text = (details.idx + 1).ToString();

                    if (details.imagePath != "") 
                    {
                        Photo.Source = new BitmapImage(new System.Uri(details.imagePath)); //Set the image to the new ImagePath if exists 
                    }
                    else
                    {
                        Photo.Source = null; //If image field empty then set to no image 
                    }
                }
                else
                {
                    Console.WriteLine("[MainWindow] Error: No results found for: " + search.searchStr);
                    MessageBox.Show("Error: No results found for: " + search.searchStr);
                }
            }
            else
            {
                Console.WriteLine("[MainWindow] Error: Could not perform search for last name");
                MessageBox.Show("Error: Could not perform search for last name.");
            }

            ProgressBar.Value = 100;
        }

        /// <summary>
        /// "Set" Button click handler, when user wishes to set a new base URL. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UrlBtn_Click(object sender, RoutedEventArgs e)
        {
            string newUrl = UrlField.Text.ToString(); 
            
            //Validate URL - HTTP/ HTTPS accepted 
            //Note: Valid URL may not have a compatiable business tier at the end. 
            //Will result in all requests returning an error. E.g. can be set to https://google.com and will be accepted. 
            if(Uri.IsWellFormedUriString(newUrl, UriKind.Absolute))
            {
                url = newUrl;
                client = new RestClient(newUrl); //Create new RestClient with provided URL 
            }
            else
            {
                Console.WriteLine("[MainWindow] Error: Invalid URL provided");
                MessageBox.Show("Error: Invalid URL");
            }
        }

        /// <summary>
        /// Update user information based on fields entered. 
        /// Will update user based on current index in index field 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("[MainWindow] Task: Updating user");

            ProgressBar.Value = 20;

            RestRequest request = new RestRequest("api/Values/"); //Post request to update user 

            try
            {
                int bal = int.Parse(BalanceBox.Text, System.Globalization.NumberStyles.Currency);

                if (ValidateInput.IsValidInteger(AcctNoBox.Text) && //Validate all text boxes that must be integers 
                    ValidateInput.IsValidInteger(PinBox.Text) &&
                    ValidateInput.IsValidInteger(Index.Text))
                {
                    UserModel user = new UserModel(); //Create new user model to send 

                    user.fName = FNameBox.Text.ToString(); //Set fields to TextBox 
                    user.lName = LNameBox.Text.ToString();

                    user.bal = bal;
                    user.acct = UInt32.Parse(AcctNoBox.Text); //Convert textbox to integer 
                    user.pin = UInt32.Parse(PinBox.Text);

                    user.idx = Int32.Parse(Index.Text) - 1;

                    if(selectedImage != "") //Set new image 
                    {
                        user.imagePath = selectedImage; 
                    }
                    else
                    {
                        user.imagePath = Photo.Source.ToString();
                    }

                    request.AddJsonBody(user);
                    await client.ExecutePostAsync(request); //Post
                }
                else 
                {
                    Console.WriteLine("[MainWindow] Error: Invalid values used");
                    MessageBox.Show("Error: Invalid values.");
                }
            }
            catch (FormatException) //Invalid format -> Show error + log 
            {
                Console.WriteLine("[MainWindow] Error: Fields contain invalid formats (Balance not numerical and .00 cents only)");
                MessageBox.Show("Error: Fields contain invalid formats (Balance must be numerical and .00 cents only)");
            }

            ProgressBar.Value = 100;
        }

        /// <summary>
        /// "Select Image" Button, loads file explorer for user to select image 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowBtn_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("[MainWindow] Task: Show selection window");

            OpenFileDialog file = new OpenFileDialog();
            file.Title = "Select a picture";
            file.Filter = "Supported Image Formats|*.jpg;*.jpeg;*.png";

            if(file.ShowDialog() == true) //Result ok - Selected 
            {
                Console.WriteLine(file.FileName); 

                BitmapImage image = new BitmapImage(new Uri(file.FileName)); //Get BitmapImage based on filepath 

                Photo.Source = image; //Set new image to be shown to BitmapImage
                selectedImage = file.FileName; //Set new filepath 
            }
        }
    }
}
