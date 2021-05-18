﻿// Filename: BlockchainServerInterface.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Interface for blockchain server - functions it must implement
// Author:   Kevin Le (19472960)
//
// Date:     28/05/2020

using System.Collections.Generic;
using System.ServiceModel;

namespace dc_p8_blockchain.Models
{
    [ServiceContract]
    public interface BlockchainServerInterface
    {
        [OperationContract]
        List<Block> GetCurrentBlockchain();
        [OperationContract]
        Block GetCurrentBlock(); //End block
        [OperationContract]
        void RecieveNewTask(string py);
        [OperationContract]
        int GetNumBlocks();
    }
}