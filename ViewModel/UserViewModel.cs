using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GCC_Canteen.ViewModel
{
    public class UserViewModel
    {
        public User UserDetails { get; set; }
        public List<UserListValues> UserListDetails { get; set; }
        public List<LocationInfo> LocationDetails { get; set; }

    }

    public class User
    {
        public int ID { get; set; }
        //public int UserID { get; set; }

        [DisplayName("User")]
        [Required(ErrorMessage = "Please Enter Employee Code")]
        public string EmpCode { get; set; }

        [Required(ErrorMessage = "Please Enter Location")]
        public int LocationID { get; set; }
        [Required(ErrorMessage = "Please Enter Role")]
        public int RoleID { get; set; }
        [Required(ErrorMessage = "Please Enter Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Designation")]
        public string Designation { get; set; }


        [Required(ErrorMessage = "Please Enter Team Name")]
        public string TeamName { get; set; }

       
        public int? ReportingToUserID { get; set; }
        [Required(ErrorMessage = "Please Enter DOJ")]
        public string DOJ { get; set; }
       // [Required(ErrorMessage = "Please Enter DOL")]
        public string DOL { get; set; }
        [Required(ErrorMessage = "Please Enter LoginID")]
        public string LoginID { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }

      //  [Required(ErrorMessage = "Please Enter Location")]
        public string Location { get; set; }
     

        //[Required(ErrorMessage = "Please Enter Role Name")]
        //public string RoleName { get; set; }

        public bool IsActive { get; set; }
        [DisplayName("Created By")]
        public int CreatedBy { get; set; }
        [DisplayName("Created User")]
        public string CreatedUser { get; set; }
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string ModifiedUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? DeletedBy { get; set; }
        public string DeletedUser { get; set; }
        public DateTime? DeletedDate { get; set; }
    }

    public class UserListValues
    {
        public int ID { get; set; }
        public string Location { get; set; }
        public string UserName { get; set; }
        public string EmpCode { get; set; }
        public string Designation { get; set; }
        public string RoleName { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string IsActive { get; set; }
        public bool IsActiveStatus { get; set; }
    }

    public class UserListModel
    {
        public GridSettingData gridSettingData { get; set; }
        public List<UserListValues> UserList { get; set; }
    }

    public class LocationInfo
    {
        //public string DisplayLocationName { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public int UserID { get; set; }
        public bool Access { get; set; }
    }

    //public class CustomizableUserPageAccess
    //{
    //    public int UserPageAccessExclID { get; set; }
    //    //public int UserPageAccessID { get; set; }
    //    public int UserID { get; set; }
    //    public string User { get; set; }
    //    public int LocationID { get; set; }
    //}
}