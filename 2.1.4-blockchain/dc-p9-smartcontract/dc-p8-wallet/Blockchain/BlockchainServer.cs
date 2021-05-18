// Filename: BlockchainServer.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Implementation of BlockchainServerInterface. Calls relevant functions in Tasks or Blockchain 
// Author:   Kevin Le (19472960)
//
// Date:     28/05/2020

using dc_p8_wallet.Task;
using System.Collections.Generic;

namespace dc_p8_wallet.Blockchain
{
    class BlockchainServer : BlockchainServerInterface
    {
        public List<Block> GetCurrentBlockchain()
        {
            return Blockchain.GetChain(); 
        }

        public Block GetCurrentBlock()
        {
            return Blockchain.GetEndBlock(); 
        }

        public void RecieveNewTask(string py)
        {
            Tasks.AddTask(py); 
        }

        public int GetNumBlocks()
        {
            return Blockchain.GetNumBlocks(); 
        }
    }
}
