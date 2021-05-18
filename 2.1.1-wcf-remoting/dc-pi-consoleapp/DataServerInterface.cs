// Filename: DataServerInterface.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Interface for DataServer + Any connections to be made 
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

using System.ServiceModel;

namespace dc_p1_data
{
    [ServiceContract]
    public interface DataServerInterface
    {
        [OperationContract]
        int GetNumEntries();
        [OperationContract]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out string imagePath);
        [OperationContract]
        void UpdateEntry(int index, uint acctNo, uint pin, int bal, string fName, string lName, string imagePath);        
    }
}
