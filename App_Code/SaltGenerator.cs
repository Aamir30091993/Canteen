using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;

namespace GCC_Canteen.App_Code
{
    public class SaltGenerator
    {
        private static RNGCryptoServiceProvider obj_cryptoserviceprovider = null;
        private const int salt_size = 20;

        static SaltGenerator()
        {
            obj_cryptoserviceprovider = new RNGCryptoServiceProvider();
        }

        public static string GetSaltString()
        {
            // create a byte array to store the salt bytes
            byte[] saltBytes = new byte[salt_size];

            // generate the salt in the byte array
            obj_cryptoserviceprovider.GetNonZeroBytes(saltBytes);

            // get some string representation for this salt
            string saltString = Utility.GetString(saltBytes);

            // now salt string generated, return it to caller
            return saltString;
        }
    }
}