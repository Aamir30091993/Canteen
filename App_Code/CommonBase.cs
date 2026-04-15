using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCC_Canteen.App_Code
{
    public class CommonBase
    {
        public enum Page
        {
             MainDashboard = 1 
            ,Report=2
            ,ImportUser = 3
            ,User = 4         
            ,Security = 5
            ,ChangePassword = 6
            ,LogOut = 7
            ,MealTimeConfig
        }

        public class PageName
        {
            public const string MainDashboard = "Dashboard";        
            public const string User = "User";
            public const string ImportUser = "User Upload";
            public const string Report = "Report";
            public const string ChangePassword = "Change Password";
            public const string LogOut = "LogOut";
            public const string MealTimeConfig = "Meal Time Config"; 
        }

        public static List<UserAccessRights> UserPageAccess(int PageID)
        {
            UserInfo user = HttpContext.Current.Session["UserInfo"] as UserInfo;
            var userPageAccess = new List<UserAccessRights>();
            userPageAccess = user.UserAccess.Where(m => m.PageID == PageID).ToList();
            return userPageAccess;
        }
    }
}