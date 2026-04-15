using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using GCC_Canteen.App_Code;

namespace GCC_Canteen.App_Code
{
    public class HashGenerator
    {
        public string GetHashedString(string lpassword)
        {
            // SHA256 algorithm to generate the hash from this salted password
            SHA256Managed obj_shaAlgorithm = new SHA256Managed();
            byte[] dataBytes = Utility.GetBytes(lpassword);

            byte[] resultByte = obj_shaAlgorithm.ComputeHash(dataBytes);

            // return hash string to caller
            return Utility.GetString(resultByte);
        }
    }
}