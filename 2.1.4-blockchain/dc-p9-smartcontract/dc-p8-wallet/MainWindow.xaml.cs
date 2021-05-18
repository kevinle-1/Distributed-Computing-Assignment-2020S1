using dc_p8_wallet.Blockchain;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows;

namespace dc_p8_wallet
{
    public delegate void StartMinerDelegate();
    public delegate void StartBlockchainServerDelegate();

    public delegate void SetAddressDelegate(string address); //Shows IP:PORT the client is using 
    public delegate void SetAddOutputEntry(string output); //Allows another thread to add text to the output TextBox
    public delegate void UpdateJobOutputEntry(); //Updates jobs completed listbox 

    public delegate void UpdateBlockCount(); //Allows another thread to make call to update block count  

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string ClientListURL = "https://localhost:44322/"; 

        private Boolean IsClosed = false; //If the client is closed 
        private Boolean BlockchainServerStarted = false; //If the blockchain server has been started 

        private string BlockchainServerIP = "127.0.0.1"; //Default to 127.0.0.1 as it is only running on local machine right now 
        private uint BlockchainServerPort = 8100; //Start at port 8100, and check from here (incremented to check available ports) 

        private static List<Task.Task> submitted = new List<Task.Task>(); 

        public MainWindow()
        {
            StartBlockchainServerDelegate blockchainServer = BlockchainServer;
            StartMinerDelegate miner = Miner;

            blockchainServer.BeginInvoke(null, null);
            miner.BeginInvoke(null, null); 

            InitializeComponent();

            AddOutputEntry("Initializing Components...");
        }

        /// <summary>
        /// Logic for Miner thread TLDR: 
        ///     1. Checks if queue is of size 5
        ///         1.1. If it is, dequeue the tasks and sort, then for each run the task, and calculate the hash to make a block to append to blockchain 
        ///     2. Check if the clients current blockchain is the most popular hash
        ///         1.1. If it isn't, replace the current clients blockchain 
        /// </summary>
        public void Miner()
        {
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();

            NetTcpBinding tcp = new NetTcpBinding();

            //Process tasks 
            while (!IsClosed)
            {
                Boolean blockInvalidated = false;

                try
                {
                    Queue<Task.Task> tasks = Task.Tasks.GetTasks(); //Get task queue 
                    List<Task.Task> taskList = new List<Task.Task>(); //Create a list to hold tasks to sort 

                    if (tasks.Count == 5) 
                    {
                        Logger.Log("Queue has 5 Tasks. Building Block...");

                        Block appendBlock = new Block(); //Create a new block 
                        Block endBlock = Blockchain.Blockchain.GetEndBlock(); //Get current end block in blockchain 

                        for (int idx = 0; idx < 5; idx++)
                        {
                            taskList.Add(tasks.Dequeue()); //Dequeue tasks into a list 
                        }

                        List<Task.Task> taskListSorted = taskList.OrderBy(tEntry => tEntry.py).ToList(); //Sort list based on Task hash (LINQ) 

                        foreach (Task.Task t in taskListSorted) //Begin adding to block the tasks 
                        {
                            byte[] taskBytes = Convert.FromBase64String(t.py); //Convert base64 source string to byte array 
                            string taskSrc = Encoding.UTF8.GetString(taskBytes); //Convert byte array back to useable string. (The python source) 

                            Logger.Log("Performing job...");
                            Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Running job..." });

                            Logger.Log("Source: " + taskSrc);

                            //Run using IronPython 
                            engine.Execute(taskSrc, scope);
                            dynamic functionToRun = scope.GetVariable("func");
                            var result = functionToRun();

                            string resultStr = result.ToString(); //Get result as string 

                            Logger.Log("Job Result: " + resultStr);
                            Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Job Result: " + resultStr });

                            appendBlock.AddTask(new string[] { t.py, resultStr }); //Add it to new string[] in block where s[0] = the source and s[1] = the result 

                            Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Task: " + taskSrc + " Processed" });
                            Logger.Log("Task: " + taskSrc + " Processed");
                        } //Repeat for 5 tasks (Each block has 5 tasks) 

                        appendBlock.blockID = endBlock.blockID + 1; //Increment ID by +1 from end block 
                        appendBlock.offset = 0;

                        appendBlock.prevHash = endBlock.hash; //Set previous hash to the hash of the current end block in the chain 
                        appendBlock.hash = "";

                        appendBlock = GenerateHash(appendBlock); //Generate the hash based on the data so far. Set to the block hash

                        Blockchain.Blockchain.AddBlock(appendBlock); //Add block to blockchain + Queue should now be empty again 
                    }
                    else
                    {
                        Logger.Error("Queue empty");
                    }
                }
                catch(EndpointNotFoundException)
                {
                    Logger.Error("Error: Connection could not be made to client.");
                    Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Error: Connection could not be made to client." });
                }
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    Logger.Error("Function has no return");
                    Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Function has no return" });

                    blockInvalidated = true; 
                }
                catch (Microsoft.Scripting.SyntaxErrorException)
                {
                    Logger.Error("Python Source Syntax Invalid");
                    Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Python Source Syntax Invalid" });

                    blockInvalidated = true;
                }
                catch (IronPython.Runtime.UnboundNameException une)
                {
                    Logger.Error("NameError: " + une.Message);
                    Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "NameError: " + une.Message });

                    blockInvalidated = true;
                }
                catch (MissingMemberException)
                {
                    Logger.Error("Function has incorrent name. Must be func");
                    Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Function has incorrent name. Must be func" });

                    blockInvalidated = true;
                }                
                catch (InvalidOperationException)
                {
                    Logger.Error("Could not dequeue");
                    Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Could not dequeue" });
                }
                catch (Exception)
                {
                    Logger.Error("Error: Could not process tasks.");
                    Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Error: Could not process tasks." });

                    blockInvalidated = true;
                }

                if(blockInvalidated) //On error, entire queue of jobs is cleared
                {
                    Task.Tasks.Reset();
                    Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Queue cleared." });
                }

                Dispatcher.Invoke(new UpdateBlockCount(UpdateBlocks));
                Dispatcher.Invoke(new UpdateJobOutputEntry(UpdateJobCompleted));

                //Check if have popular hash 

                Logger.Log("Getting clients");

                RestClient client = new RestClient(ClientListURL);
                RestRequest request = new RestRequest("api/Client/Clients");
                IRestResponse list = client.Get(request); //GET request for list of clients 

                if (list.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<Client> clients = JsonConvert.DeserializeObject<List<Client>>(list.Content); //Deserialize JSON to List<Client>
                    Dictionary<string, int> endblockDictionary = new Dictionary<string, int>(); //Dictionary to hold count of popular hash 

                    if (clients != null) //Clients list not empty 
                    {
                        foreach (Client c in clients)
                        {
                            try
                            {
                                Logger.Log("Connecting to client " + c.ToString());

                                string clientURL = "net.tcp://" + c.ToString() + "/BlockchainServer"; //Build client URL 

                                ChannelFactory<BlockchainServerInterface> blockchainFactory = new ChannelFactory<BlockchainServerInterface>(tcp, clientURL);
                                BlockchainServerInterface clientBlockchainServer = blockchainFactory.CreateChannel(); //Create channel 

                                String hash = clientBlockchainServer.GetCurrentBlock().hash; //Get the clients endblock hash 
                                Logger.Log("Client current end block hash " + hash);

                                if (endblockDictionary.ContainsKey(hash)) //Count the hashes for all clients 
                                {
                                    endblockDictionary[hash] += 1; //Hash already exists + 1 count 
                                }
                                else
                                {
                                    endblockDictionary.Add(hash, 1); //Hash doesn't exist, new entry in dictionary with beginning count 1 
                                }
                            }
                            catch (EndpointNotFoundException)
                            {
                                Logger.Error("Error: Connection could not be made to client.");
                            }
                        }

                        int blockCount = 0;
                        string popularHash = "";

                        foreach (KeyValuePair<string, int> entry in endblockDictionary) //Check dictionary to see which hash was most popular 
                        {
                            Logger.Log("End block hash frequencies: " + entry.Key + " > " + entry.Value);

                            if (entry.Value > blockCount)
                            {
                                blockCount = entry.Value;
                                popularHash = entry.Key;
                            }
                        }

                        if (!(Blockchain.Blockchain.GetEndBlock().hash == popularHash)) //If Current Blockchain doesn't have the most popular end block
                        {
                            Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Current Blockchain doesn't have popular hash." });
                            Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Replacing Blockchain..." });

                            //Replace the chain with chain that has most popular block 
                            foreach (Client c in clients)
                            {
                                string clientURL = "net.tcp://" + c.ToString() + "/BlockchainServer"; //Build client URL 

                                ChannelFactory<BlockchainServerInterface> blockchainFactory = new ChannelFactory<BlockchainServerInterface>(tcp, clientURL);
                                BlockchainServerInterface clientBlockchainServer = blockchainFactory.CreateChannel(); //Create channel to client 

                                if (clientBlockchainServer.GetCurrentBlock().hash == popularHash) //If the client has the popular blockchain 
                                {
                                    Blockchain.Blockchain.UpdateChain(clientBlockchainServer.GetCurrentBlockchain()); //Get that clients blockchain and replace own clients chain 
                                    break; //No need to continue loop as popular chain found 
                                }
                            }
                        }
                    }
                }
                else
                {
                    Logger.Error("Could not retrieve client list");
                }
            }
        }

        /// <summary>
        /// Blockchain server thread. Starts the blockchain server for other clients to connect to 
        /// </summary>
        public void BlockchainServer()
        {
            Logger.Log("Blockchain Server Starting...");
            Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Blockchain Server Starting..." });

            ServiceHost host = new ServiceHost(typeof(BlockchainServer));

            while(!BlockchainServerStarted) //Keep trying to start it till it has started. 
            {
                string port = BlockchainServerPort.ToString(); //Get the port to attempt to start the server from 

                try
                {
                    NetTcpBinding tcp = new NetTcpBinding();

                    Logger.Log("Using Address: net.tcp://" + BlockchainServerIP + ":" + port + "/BlockchainServer");

                    host.AddServiceEndpoint(typeof(BlockchainServerInterface), tcp, "net.tcp://" + BlockchainServerIP + ":" + port + "/BlockchainServer"); //Build URI 

                    host.Open(); //Start Blockchain server - AddressAlreadyInUseException exception thrown here if another client is already using 
                    BlockchainServerStarted = true; //Stop the while !start loop 

                    RegisterBlockchain(); //Register with the Client web service it's Blockchain server
                    Dispatcher.Invoke(new SetAddOutputEntry(AddOutputEntry), new Object[] { "Blockchain Server Started" });
                }
                catch (AddressAlreadyInUseException) //Client already using the port 
                {
                    BlockchainServerPort++; //Increment port 

                    Logger.Error("Address already in use, attempting with next port: " + BlockchainServerPort.ToString());
                    Logger.Error("Using Address: net.tcp://" + BlockchainServerIP + ":" + port + "/BlockchainServer");

                    host = new ServiceHost(typeof(BlockchainServer)); //Reset host and try again until has started
                }
            }

            Logger.Log("Blockchain Server Started");

            while (!IsClosed) { } //Keep Blockchain server running 

            Logger.Log("Blockchain Server Closing");
            host.Close();
        }

        /// <summary>
        /// Monitor when the UI has been closed 
        /// Based on answer at: https://stackoverflow.com/a/36400810/11378789
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Logger.Log("[Closing Program]");
            IsClosed = true;
        }

        /// <summary>
        /// Add the client to the Clients list in the client web service 
        /// </summary>
        private void RegisterBlockchain()
        {
            Logger.Log("Registering with Client Web Service: " + BlockchainServerIP + ":" + BlockchainServerPort);

            RestClient client = new RestClient(ClientListURL);
            RestRequest request = new RestRequest("api/Client/Register/" + BlockchainServerIP + "/" + BlockchainServerPort);
            client.Post(request); //POST request to add

            Dispatcher.Invoke(new SetAddressDelegate(SetAddress), new Object[] { BlockchainServerIP + ":" + BlockchainServerPort });
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
            Output.Items.Add(output);
        }

        /// <summary>
        /// Update the field showing number of blocks in the blockchain by calling Blockchain web service. 
        /// </summary>
        private void UpdateBlocks()
        {
            int numBlocks = Blockchain.Blockchain.GetNumBlocks();
            NumBlocksText.Text = numBlocks.ToString(); //Set number of blocks 
        }

        /// <summary>
        /// Adds a completed job to job list 
        /// </summary>
        private void UpdateJobCompleted()
        {
            List<Block> blockchain = Blockchain.Blockchain.GetChain(); 

            //Oh no O(n^3)
            foreach(Block b in blockchain)
            {
                List<String[]> tasks = JsonConvert.DeserializeObject<List<string[]>>(b.tasks); 

                foreach(string[] s in tasks)
                {
                    foreach(Task.Task t in submitted)
                    {
                        if(s[0] == t.py && s[1] != "")
                        {
                            string src = Encoding.UTF8.GetString(Convert.FromBase64String(s[0])); //Convert byte array back to useable string. 

                            string entry = src + " -> " + s[1]; 

                            if (!ResultBox.Items.Contains(entry))
                            {
                                ResultBox.Items.Add(entry);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calls hash function in blockchain 
        /// Generates a new hash by brute force. If no hash begins with 12345, increment offset by 1 and check again. 
        /// </summary>
        /// <param name="b"></param>
        /// <returns>Block with hash</returns>
        public Block GenerateHash(Block b)
        {
            return Blockchain.Blockchain.GenerateHash(b);
        }

        /// <summary>
        /// "Send" button click handler, gets the list of clients and validates the input 
        /// If valid input creates connection to each client and submits the task. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendClick(object sender, RoutedEventArgs e)
        {
            Logger.Log("Getting clients");

            string task = TaskText.Text;

            SHA256 sha256 = SHA256.Create(); 

            NetTcpBinding tcp = new NetTcpBinding();

            RestClient client = new RestClient(ClientListURL);
            RestRequest request = new RestRequest("api/Client/Clients");
            IRestResponse list = client.Get(request); //GET request for list of clients 

            List<Client> clients = JsonConvert.DeserializeObject<List<Client>>(list.Content); //Deserialize JSON to List<Client>

            if(!string.IsNullOrEmpty(task))
            {
                foreach (Client c in clients) //Send new task to all clients 
                {
                    try
                    {
                        string clientURL = "net.tcp://" + c.ToString() + "/BlockchainServer"; //Build client URL 

                        ChannelFactory<BlockchainServerInterface> blockchainFactory = new ChannelFactory<BlockchainServerInterface>(tcp, clientURL);
                        BlockchainServerInterface clientBlockchainServer = blockchainFactory.CreateChannel(); //Create channel 

                        string task64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(task)); //Convert to base 64 

                        Logger.Log("Encoded Task: " + task64);

                        clientBlockchainServer.RecieveNewTask(task64);

                        Task.Task t = new Task.Task();
                        t.py = task64;

                        submitted.Add(t);

                        TaskText.Text = ""; //Clear TaskBox
                    }
                    catch(EndpointNotFoundException)
                    {
                        Logger.Error("Error: Connection could not be made to client.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Error: Invalid Task");
            }
        }
    }
}
