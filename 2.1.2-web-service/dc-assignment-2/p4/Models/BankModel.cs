// Filename: BankModel.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Model representing BankDB - Holds static instance of BankDB (.dll provided in prac)
// Author:   Kevin Le (19472960)
//
// Date:     24/05/2020

namespace p4.Models
{
    static class BankModel
    {
        public static BankDB.BankDB db = new BankDB.BankDB();
    }
}