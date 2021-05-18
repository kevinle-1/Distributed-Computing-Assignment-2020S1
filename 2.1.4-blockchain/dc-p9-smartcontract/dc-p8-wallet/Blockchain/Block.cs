// Filename: Block.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Object representation of a Block 
// Author:   Kevin Le (19472960)
//
// Date:     28/05/2020

using Newtonsoft.Json;
using System.Collections.Generic;

namespace dc_p8_wallet.Blockchain
{
    public class Block
    {
        public uint blockID;

        public string tasks;

        public uint offset;
        public string prevHash;
        public string hash;

        /// <summary>
        /// Create a new List of String[] but serializes to JSON string 
        /// </summary>
        public Block()
        {
            tasks = JsonConvert.SerializeObject(new List<string[]>()); 
        }

        /// <summary>
        /// Deserializes the JSON list and adds a new task, serializing it again. 
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(string[] task)
        {
            List<string[]> tasksList = JsonConvert.DeserializeObject<List<string[]>>(tasks);
            tasksList.Add(task);

            tasks = JsonConvert.SerializeObject(tasksList);
        }

        /// <summary>
        /// String that concatenates all fields of the block except the hash of the block.
        /// This is used by functions that generate the hash of the block to store in the block. 
        /// </summary>
        /// <returns>String with all fields concatenated</returns>
        public string ToHashString()
        {
            return blockID.ToString() + tasks + offset.ToString() + prevHash.ToString();
        }
    }
}