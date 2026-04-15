using GCC_Canteen.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GCC_Canteen.Models.BusinessLayer.User
{
    public class UserBL
    {
        public static string SaveUser(UserViewModel objUserViewModel)
        {
            DataTable dtLocationAccess = new DataTable();
            dtLocationAccess.Columns.Add("UserID");
            dtLocationAccess.Columns.Add("LocationID");
            dtLocationAccess.Columns.Add("Access");
            foreach (var item in objUserViewModel.LocationDetails)
            {
                DataRow drLocationAccess = dtLocationAccess.NewRow();
                drLocationAccess["UserID"] = 0;
                drLocationAccess["LocationID"] = item.LocationID;
                drLocationAccess["Access"] = Convert.ToInt32(item.Access);
                dtLocationAccess.Rows.Add(drLocationAccess);
            }
            return GCC_Canteen.Models.Repository.UserRepository.SaveUser(objUserViewModel, dtLocationAccess);
        }

        public static string UpdateUser(UserViewModel objUserViewModel)
        {

            DataTable dtLocationAccess = new DataTable();
            dtLocationAccess.Columns.Add("UserID");
            dtLocationAccess.Columns.Add("LocationID");
            dtLocationAccess.Columns.Add("Access");
            foreach (var item in objUserViewModel.LocationDetails)
            {
                DataRow drLocationAccess = dtLocationAccess.NewRow();
                drLocationAccess["UserID"] = objUserViewModel.UserDetails.ID;
                drLocationAccess["LocationID"] = item.LocationID;
                drLocationAccess["Access"] = Convert.ToInt32(item.Access);
                dtLocationAccess.Rows.Add(drLocationAccess);
            }
            
            return GCC_Canteen.Models.Repository.UserRepository.UpdateUser(objUserViewModel, dtLocationAccess);
        }
        }
}