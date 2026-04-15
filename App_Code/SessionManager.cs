using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using GCC_Canteen.ViewModel;
/// <summary>
/// Summary description for SessionManager
/// </summary>
public class SessionManager
{
    UserInfo ObjUserInfo;
    public SessionManager()
    {
        //
        // TODO: Add constructor logic here
        ObjUserInfo = new UserInfo();
        //
    }

    public void SetUserSessionData(int userID, int RoleID, string loginName, string Username, List<UserAccessRights> UserAccessList, List<AttachmentConfig> AttachConfig/*, int ClientID, int UserTypeID*/)
    {
        ObjUserInfo.SetCurrentUserInfo(userID, RoleID, loginName, Username, UserAccessList,AttachConfig/*, ClientID, UserTypeID*/);
        userInfo = ObjUserInfo;
    }

    public UserInfo userInfo
    {
        get
        {
            if (HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session["UserInfo"] != null)
                    return HttpContext.Current.Session["UserInfo"] as UserInfo;

                else
                {
                    if (!HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains("login.aspx"))
                    {
                        HttpContext.Current.Response.Redirect("../Login.aspx?RedirectUrl=" + HttpContext.Current.Request.Url.AbsoluteUri, false);
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.End();
                    }
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        set
        {
            HttpContext.Current.Session["UserInfo"] = value;
            HttpContext.Current.Session["UserID"] = userInfo.UserID;
        }
    }
}

public class UserInfo
{
    public void SetCurrentUserInfo(int userID,int RoleID, string loginName, string Username, List<UserAccessRights> UserAccess, List<AttachmentConfig> AttachConfig/*, int ClientID,int UserTypeID*/)
    {
        this.UserID = userID;
        this.RoleID = RoleID;
        this.LoginName = loginName;
        this.UserName = Username;
        this.UserAccess = UserAccess;
        //this.UserRoles = UserRoles;
        this.AttachConfig = AttachConfig;
        //this.ClientID = ClientID;
        //this.UserTypeID = UserTypeID;

    }
    public int UserID { get; set; }
    public int RoleID{get;set;}
    public string LoginName { get; set; }
    public string UserName { get; set; }
    public List<UserAccessRights> UserAccess { get; set; }
   // public List<UserRoleRights> UserRoles { get; set; }
    public List<AttachmentConfig> AttachConfig { get; set; }  
    public int ClientID { get; set; }
    public int UserTypeID { get; set; }
}
public class UserAccessRights
{
    public int UserID { get; set; }
    public string UserName { get; set; }    
    public int PageID { get; set; }
    public int ParentPageID { get; set; }
    public string PageName { get; set; }
    public string ControllerName { get; set; }
    public bool Read { get; set; }
    public bool Write { get; set; }
    public bool Update { get; set; }
    public bool Delete { get; set; }
    public bool Approve { get; set; }
}

public class UserRoleRights
{
    public int UserID { get; set; }
    public int RoleID { get; set; }
    public string RoleName { get; set; }
    //public int SystemRoleID { get; set; }
    public string SystemRoleName { get; set; }
}

public class AttachmentConfig
{
    //ConfigID = 1 {Browse URL}, 2 {Physical Path}
    public int ConfigID { get; set; }
    public string FieldValue { get; set; }
}