// Filename: DataModel.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Connection to Data tier 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System.ServiceModel;
using dc_p1_data;

namespace dc_p1_web.Models
{
    public class DataModel
    {
        private string URL = "net.tcp://localhost:8100/DataService";
        private NetTcpBinding tcp;
        private ChannelFactory<DataServerInterface> accFactory;
        private DataServerInterface acc;

        /// <summary>
        /// Instantiate new DataModel 
        /// Create a new ChannelFactory and connects to the data tier
        /// </summary>
        public DataModel()
        {
            tcp = new NetTcpBinding();
            accFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            acc = accFactory.CreateChannel();
        }

        /// <summary>
        /// Gets number of entries from Data tier 
        /// </summary>
        /// <returns>Integer representing number of entries</returns>
        public int getNumEntries()
        {
            return acc.GetNumEntries();
        }

        /// <summary>
        /// Returns user fields based on index.
        /// </summary>
        /// <param name="index">Index of user requested</param>
        /// 
        /// User fields returned as out mode parameters: 
        /// <param name="acctNo"></param>
        /// <param name="pin"></param>
        /// <param name="bal"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <param name="imagePath"></param>
        public void getValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out string imagePath)
        {
            acc.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out imagePath);
        }

        /// <summary>
        /// Logic to search for existing user based on Last name 
        /// </summary>
        /// 
        /// <param name="lName">Last name of user to search by </param>
        /// <param name="acctNo"></param>
        /// <param name="pin"></param>
        /// <param name="bal"></param>
        /// <param name="fName"></param>
        /// <param name="lNameO"></param>
        /// <param name="imagePath"></param>
        /// <param name="idx"></param>
        public void searchForEntry(string lName, out uint acctNo, out uint pin, out int bal, out string fName, out string lNameO, out string imagePath, out int idx)
        {
            int numRecords = getNumEntries();
            string lNameResult = "";

            acctNo = 0;
            pin = 0;
            bal = 0;
            fName = "";
            lNameO = "";
            imagePath = "";

            idx = 0;

            for (int index = 0; index < numRecords; index++) 
            {
                getValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lNameResult, out imagePath);

                if (string.Compare(lName, lNameResult) == 0) //Returns first result in database, ignores multiple matches. 
                {
                    lNameO = lNameResult;
                    idx = index;
                    break;
                }
            }
        }

        /// <summary>
        /// Updates an existing user based on user index 
        /// </summary>
        /// 
        /// <param name="index">User index to update</param>
        /// 
        /// Values to be updated: (Can also remain unchanged)
        /// <param name="acctNo"></param>
        /// <param name="pin"></param>
        /// <param name="bal"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <param name="imagePath"></param>
        public void updateEntry(int index, uint acctNo, uint pin, int bal, string fName, string lName, string imagePath)
        {
            acc.UpdateEntry(index, acctNo, pin, bal, fName, lName, imagePath);
        }
    }
}