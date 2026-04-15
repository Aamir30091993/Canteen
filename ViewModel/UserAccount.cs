using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GCC_Canteen.ViewModel
{
    public class Login
    {
        [Required(ErrorMessage = "Login Name Required")]
        public string LoginName { get; set; }
        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }
        public string LoginPasswordSalt { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string UserName
        {
            get; set;
        }
    }

        public class UpdatePassword
        {
            [Required(ErrorMessage = "Login Name Is Required")]
            public string LoginName { get; set; }

            [Required(ErrorMessage = "Current Password Is Required")]
            public string Password { get; set; }
            public string LoginPasswordSalt { get; set; }

            [Required(ErrorMessage = "New password Is Required")]
            public string NewPassword { get; set; }
            [System.Web.Mvc.CompareAttribute("NewPassword", ErrorMessage = "New Password and Confirm Password Must Be Same")]
            [Required(ErrorMessage = "New password Is Required")]
            public string ConfirmNewPassword { get; set; }

            public string NewPasswordSalt { get; set; }
            public int UserID { get; set; }
            public string UserName { get; set; }

            //[Required(ErrorMessage = "New Password Required")]
            //[System.ComponentModel.DataAnnotations.Compare("ConfirmPassword")]
            //public string NewPassword { get; set; }

            //[Required(ErrorMessage = "Confirm Password Required")]
            //public string ConfirmPassword { get; set; }
        }

    //public class ForgetPassword
    //{
    //[Required(ErrorMessage = "Login Name is required")]
    //public string LoginID { get; set; }
    //[Required(ErrorMessage = "New Password is required")]
    //public string NewPassword { get; set; }
    //[Required(ErrorMessage = "Confirm New Password is required")]
    //public string ConfirmNewPassword { get; set; }

    //}

    public class ForgetPassword
    {
        [Required(ErrorMessage = "Login Name Is Required")]
        public string LoginID { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public string EmailID { get; set; }

        public string NewPassword { get; set; }

    }

    public class UserAccessRights
        {
        List<UserAccessDetails> UserAccessList { get; set; }
        }
        public class UserAccessDetails
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
            public int RoleID { get; set; }
            public string RoleName { get; set; }
            public int SystemRoleID { get; set; }
            public string SystemRoleName { get; set; }
            public int PageID { get; set; }
            public int ParentPageID { get; set; }
            public string PageName { get; set; }
            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public bool Read { get; set; }
            public bool Write { get; set; }
            public bool Update { get; set; }
            public bool Delete { get; set; }
            public string IsActive { get; set; }
        }

        public class UserLoginDetails
        {
            public int UserID { get; set; }
            public string LoginName { get; set; }
            public string UserName { get; set; }
            public string EmailID { get; set; }
        }
}