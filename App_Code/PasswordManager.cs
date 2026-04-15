using GCC_Canteen.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GCC_Canteen.App_Code
{
    public class PasswordManager
    {
        // while entering password in DB
        HashGenerator obj_hashGenerator = new HashGenerator();

        public string GeneratePasswordHash(string lPassword, out string salt)
        {
            salt = SaltGenerator.GetSaltString();

            string finalString = lPassword + salt;

            return obj_hashGenerator.GetHashedString(finalString);
        }


        // while retriving password from DB
        public bool IsPasswordMatch(string lpassword, string salt, string hash)
        {
            //if (lpassword == (System.Configuration.ConfigurationManager.AppSettings["superhash"]).ToString())
            //{
            //    return true;
            //}
            if (lpassword == hash)
            {
                return true;
            }

            else
            {
                string finalString = lpassword + salt;
                string dbPass = obj_hashGenerator.GetHashedString(finalString);
                return hash == dbPass;
            }
        }

        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyz@#$&ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }


    }
}