// Filename: MainWindow.xaml.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Client Program   
// Author:   Kevin Le (19472960)
//
// Date:     25/05/2020

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using RestSharp;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Linq;

namespace dc_p6_p2p_wpf
{
    //Delegates for Networking and Job Server thread 
    public delegate void StartServerDelegate(); 
    public delegate void StartWorkerDelegate(); 

    //Delegates for Networking and Job Server thread to call to update UI
    public delegate void SetWorkingDelegate(); //Shows progress bar at 100% to indicate working 
    public delegate void SetFinishedDelegate(); //Shows progress bar at 0% to indicate not working  
    public delegate void SetAddressDelegate(string address); //Shows IP:PORT the client is using 

    public delegate void SetAddOutputEntry(string output); //Allows another thread to add text to the output TextBox

    public partial class MainWindow : Window
    {
        private static int JobsCompleted = 0; //Number of jobs the clients networking thread has completed 

        private Boolean IsClosed = false; //If the client is closed 
        private Boolean JobServerStarted = false; //If the job server has been started 

        private string ClientListURL = "https://localhost:44353/"; //The URL for the Client Web Service 

        private string JobServerIP = "127.0.0.1"; //Default to 127.0.0.1 as it is only running on local machine right now 
        private uint JobServerPort = 8100; //Start at port 8100, and check from here (incremented to check available ports) 

        public MainWindow()
        {
            StartServerDelegate serverDelegate = Server; //Set Delegates to corresponding functions 
            StartWorkerDelegate workerDelegate = Worker;

            serverDelegate.BeginInvoke(null, null); //Start server (server thread) 
            workerDelegate.BeginInvoke(null, null); //Start worker (networking thread) 

            InitializeComponent(); //Start window
        }

        /// <summary>
        /// Networking thread, gets list of clients and checks each clients for jobs, 
        /// validates and does jobs as it is assigned them 
        /// </summary>
        public void Worker()
        {
            NetTcpBinding tcp = new NetTcpBinding();

            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope(); 

            //Run while window isn't closed 
            while (!IsClosed)
            {
                Logger.Log("Getting clients");

                Boolean success = true; //Used to determine if a job has been performed successfully. Set to false if not 
                string resultStr = ""; //Result of job (if any) 

                RestClient client = new RestClient(ClientListURL);
                RestRequest request = new RestRequest("api/Client/Clients");
                IRestResponse list = client.Get(request); //GET request for list of clients 

                if(list.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<Client> clients = JsonConvert.DeserializeObject<List<Client>>(list.Content); //Deserialize JSON to List<Client>

                    Logger.Log("Number of active clients: " + clients.Count.ToString());

                    try
                    {
                        //For each client, connect and check for jobs 
                        foreach (Client c in clients)
                        {
                            if (c.port != JobServerPort.ToString()) //If the client it is connecting to has the same port, don't connect. (Don't do own jobs). -> If used on multiple machines can check for IP too 
                            {
                                SHA256 sha256Hash = SHA256.Create();
                                Logger.Log("Connecting to client at: " + c.ToString());

                                string clientURL = "net.tcp://" + c.ToString() + "/JobServer"; //Build client URL 

                                ChannelFactory<JobServerInterface> jobFactory = new ChannelFactory<JobServerInterface>(tcp, clientURL);
                                JobServerInterface clientJobServer = jobFactory.CreateChannel(); //Create channel 

                                JobModel job = clientJobServer.GetJob(); //Get job 

                                if (job.jobNo == -1) //If -1 there is no job available from that client 
                                {
                                    Logger.Log("Client at: " + c.ToString() + " currently has no jobs");
                                }
                                else //There is a job available 
                                {
                                    if (!String.IsNullOrEmpty(job.src)) //If the source isn't empty 
                                    {
                                        Logger.Log("Client at: " + c.ToString() + " has a job"); //Job has been recieved 
                                        Logger.Log("Getting Job & validating Hash...");

                                        Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Got job from: " + c.ToString() }); //Update the UI status to show it is working 
                                        Dispatcher.Invoke(new SetWorkingDelegate(SetWorking));

                                        byte[] recievedBytes = Convert.FromBase64String(job.src); //Convert base64 source string to byte array 
                                        string srcOfJob = Encoding.UTF8.GetString(recievedBytes); //Convert byte array back to useable string. 

                                        byte[] originHash = job.hash; //Get the hash that was sent with the job 
                                        byte[] recievedHash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(job.src)); //Compute own hash based on the job source base64 string. 

                                        Logger.Log(Encoding.UTF8.GetString(recievedBytes));

                                        if (originHash.SequenceEqual(recievedHash)) //If both hashes match, it was recieved successfully 
                                        {
                                            Logger.Log("Hash validated. Performing job...");
                                            Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Job validated." });
                                            Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Running job..." });

                                            Logger.Log("Source: " + srcOfJob);

                                            //Run using IronPython 
                                            engine.Execute(srcOfJob, scope);
                                            dynamic functionToRun = scope.GetVariable("func");
                                            var result = functionToRun();

                                            resultStr = result.ToString(); //Perhaps risky with a var type? 

                                            JobsCompleted++;

                                            clientJobServer.SubmitFinishedJob(job.jobNo, resultStr); //Mark the job as finished and send it with the result 

                                            Logger.Log("Result: " + resultStr);
                                            Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Result: " + resultStr });
                                            Dispatcher.Invoke(new SetFinishedDelegate(SetFinished));

                                            request = new RestRequest("api/Client/CompletedJob/" + JobServerIP + "/" + JobServerPort);
                                            client.Post(request); //Send POST request to client list to increase it's tally of jobs completed 
                                        }
                                        else //Hashes didn't match 
                                        {
                                            success = false;
                                            Logger.Error("Hash validatation failed.");
                                        }
                                    }
                                    else //Empty source 
                                    {
                                        success = false;
                                        Logger.Error("Client at: " + c.ToString() + " has empty job source");
                                    }
                                }

                                if (!success) //Output error to UI 
                                {
                                    Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Could not perform job." });
                                    Dispatcher.Invoke(new SetFinishedDelegate(SetFinished));
                                }
                            }
                            else
                            {
                                Logger.Error("Cannot connect to itself.");
                            }
                        }
                    }
                    catch (EndpointNotFoundException)
                    {
                        Logger.Error("Could not connect to client");
                    }
                    catch(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                    {
                        Logger.Error("Function has no return");
                    }
                    catch(Microsoft.Scripting.SyntaxErrorException)
                    {
                        Logger.Error("Python Source Syntax Invalid");
                    }
                    catch(IronPython.Runtime.UnboundNameException une)
                    {
                        Logger.Error("NameError: " + une.Message);
                    }
                    catch(MissingMemberException)
                    {
                        Logger.Error("Function has incorrent name. Must be func");
                    }
                    catch(Exception e)
                    {
                        Logger.Error("Could not process Python job due to: " + e.Message);
                    }

                    Dispatcher.Invoke(new SetFinishedDelegate(SetFinished));
                }
                else
                {
                    Logger.Error("Could not get Client list.");
                }

                
                Thread.Sleep(new Random().Next(5000, 10000)); //Checks for jobs every 5-10 seconds as to not unfairly disadvantage other clients. Now its just luck! 
            }
        }

        /// <summary>
        /// Job server thread. Starts a job server for other clients to connect to and check if it has any jobs. 
        /// </summary>
        public void Server()
        {
            Logger.Log("Job Server Starting...");

            ServiceHost host = new ServiceHost(typeof(JobServer));

            while (!JobServerStarted) //Keep trying to start it till it has started. 
            {
                string port = JobServerPort.ToString(); //Get the port to attempt to start the server from 

                try
                {
                    NetTcpBinding tcp = new NetTcpBinding();

                    Logger.Log("Using Address: net.tcp://" + JobServerIP + ":" + port + "/JobServer");

                    host.AddServiceEndpoint(typeof(JobServerInterface), tcp, "net.tcp://" + JobServerIP + ":" + port + "/JobServer"); //Build URI 

                    host.Open(); //Start Job server - AddressAlreadyInUseException exception thrown here if another client is already using 
                    JobServerStarted = true; //Stop the while !start loop 

                    RegisterForWork(); //Register with the Client web service it's job server
                }
                catch (AddressAlreadyInUseException) //Client already using the port 
                {
                    JobServerPort++; //Increment port 

                    Logger.Error("Address already in use, attempting with next port: " + JobServerPort.ToString());
                    Logger.Error("Using Address: net.tcp://" + JobServerIP + ":" + port + "/JobServer");

                    host = new ServiceHost(typeof(JobServer)); //Reset host and try again until has started
                }
            }

            Logger.Log("Job Server Started");

            while (!IsClosed) { } //Keep Job server running 

            Logger.Log("Job Server Closing");
            host.Close();
        }

        /// <summary>
        /// "Submit" Button click handler, when a new job is to be submitted 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewJobButton_Click(object sender, RoutedEventArgs e)
        {
            string src = Src.Text.ToString(); //Get text in TextBox 

            if (!String.IsNullOrEmpty(src)) //If box is not empty 
            {
                SHA256 sha256Hash = SHA256.Create();

                byte[] textBytes = Encoding.UTF8.GetBytes(src); //Encode UTF-8 
                string base64Str = Convert.ToBase64String(textBytes); //Convert to base 64 

                byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(base64Str)); //Hash base64Str string 

                Logger.Log("Encoded String: " + base64Str); 
                
                JobDB.AddJob(base64Str, hash); //Add job to the Job server 

                AddOutputEntry("Job submitted."); 
                NumJobsAvail.Text = JobDB.GetNumJobs().ToString(); //Update tally of number of jobs available 
                Src.Text = ""; //Clear box 
            }
            else
            {
                Logger.Error("Invalid Job");
                MessageBox.Show("Invalid job! Must not be empty.");
            }
        }

        /// <summary>
        /// Add the client to the Clients list in the client web service 
        /// </summary>
        private void RegisterForWork()
        {
            Logger.Log("Registering with Client Web Service: " + JobServerIP + ":" + JobServerPort);

            RestClient client = new RestClient(ClientListURL);
            RestRequest request = new RestRequest("api/Client/Register/" + JobServerIP + "/" + JobServerPort);
            client.Post(request); //POST request to add

            Dispatcher.Invoke(new SetAddressDelegate(SetAddress), new Object[] { JobServerIP + ":" + JobServerPort });
        }

        /// <summary>
        /// Monitor when the UI has been closed 
        /// 
        /// Based on answer at: https://stackoverflow.com/a/36400810/11378789
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Logger.Log("[Closing Program]");
            IsClosed = true; 
        }

        /// <summary>
        /// Set the progress bar to 100% to indicate a job is being worked on 
        /// </summary>
        private void SetWorking()
        {
            WorkingStatus.Value = 100.00;
        }

        /// <summary>
        /// Set the progress bar to 0% to indicate no job is being worked on 
        /// </summary>
        private void SetFinished()
        {
            WorkingStatus.Value = 0.00;
            NumJobsDone.Text = JobsCompleted.ToString(); 
        }

        /// <summary>
        /// Display the IP:PORT the client is currently using
        /// </summary>
        /// <param name="address"></param>
        private void SetAddress(string address)
        {
            Connection.Text = address;
        }        
        
        /// <summary>
        /// Append text to the output TextBox in client - Adds on new line 
        /// Used as a log to show the user. 
        /// </summary>
        /// <param name="output">Text to add </param>
        private void AddOutputEntry(string output)
        {
            Output.AppendText(output);
            Output.AppendText(Environment.NewLine); 
        }
    }
}
