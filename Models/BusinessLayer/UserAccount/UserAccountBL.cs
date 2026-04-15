using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GCC_Canteen.ViewModel;
using GCC_Canteen.App_Code;
using static GCC_Canteen.ViewModel.Login;

namespace GCC_Canteen.Models.BusinessLayer.UserAccount
{
    public class UserAccountBL
    {


        internal bool UpdatePassword(UpdatePassword objUpdatePassword)
        {

            PasswordManager obj_passManager = new PasswordManager();
            string salt = null;
            string passwordHash = obj_passManager.GeneratePasswordHash(objUpdatePassword.NewPassword, out salt);
            string SaltedValue = salt;
            objUpdatePassword.Password = passwordHash;
            objUpdatePassword.LoginPasswordSalt = SaltedValue;

            return GCC_Canteen.Models.Repository.UserAccountRepository.UpdatePassword(objUpdatePassword);

        }
    }
}