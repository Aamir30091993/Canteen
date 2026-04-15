using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GCC_Canteen.Models;
using GCC_Canteen.ViewModel;
using GCC_Canteen.Controllers;
using GCC_Canteen.App_Code;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Text;
using static GCC_Canteen.ViewModel.Login;
using System.Configuration;

namespace GCC_Canteen.Models.Repository
{
    public class UserAccountRepository
    {
        public static bool UserDetails(Login login)
        {
            MMModel db = new MMModel();

            if (db.Database.SqlQuery<Login>("exec [spGetUserDetails] @LoginName = {0}", login.LoginName).Select(x => new Login { Password = x.Password.ToString(), UserID = x.UserID, LoginName = x.LoginName }).ToList().Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool VerifyUser(Login login)
        {
            MMModel db = new MMModel();
            PasswordManager obj_PasswordMgr = new PasswordManager();
            SessionManager obj_session = new SessionManager();

            var userDetails = db.Database.SqlQuery<Login>("exec [spGetUserDetails] @LoginName = {0}", login.LoginName)
                        .Select(x => new Login { Password = x.Password.ToString(), LoginPasswordSalt = x.LoginPasswordSalt, UserID = x.UserID, LoginName = x.LoginName, UserName = x.UserName, RoleID = x.RoleID }).ToList()[0];



            string existingPassword = userDetails.Password;
            string existingPasswordSalt = userDetails.LoginPasswordSalt;



            bool PasswordMatchResult = obj_PasswordMgr.IsPasswordMatch(login.Password, existingPasswordSalt, existingPassword);
            if (PasswordMatchResult == true)
            {
                return true;
              //  db.Database.ExecuteSqlCommand("EXEC [spInsertUserAccessLog] @UserID = {0}, @AccessedFrom = {1}", userDetails.UserID, HttpContext.Current.Request.UserHostAddress);
                //using (var db1 = new MMModel())
                //{
                //    db.Database.Initialize(force: false);
                //    //var cmd = db.Database.Connection.CreateCommand();
                //    SqlConnection SqlConn = new SqlConnection(db1.Database.Connection.ConnectionString);
                //    if (SqlConn.State == ConnectionState.Closed)
                //        SqlConn.Open();
                //    SqlParameter SqlParam;
                //    SqlCommand cmd = new SqlCommand();
                //    cmd = new SqlCommand("[spGetUserPageAccessRights]", SqlConn);
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    SqlParam = cmd.Parameters.Add("@UserID", SqlDbType.Int);
                //    SqlParam.Value = userDetails.UserID;

                //    try
                //    {
                //        //db.Database.Connection.Open();
                //        var reader = cmd.ExecuteReader();
                //        var FilterUserDetails = ((IObjectContextAdapter)db)
                //            .ObjectContext
                //            .Translate<UserAccessRights>(reader).AsEnumerable().ToList();

                //        //Move to second result set and read Posts User Roles
                //        //reader.NextResult();
                //        //var FilterUserRoleRightsDetails = ((IObjectContextAdapter)db)
                //        //    .ObjectContext
                //        //    .Translate<UserRoleRights>(reader).AsEnumerable().ToList();

                //        reader.NextResult();
                //        var AttachConfigDetails = ((IObjectContextAdapter)db)
                //            .ObjectContext
                //            .Translate<AttachmentConfig>(reader).AsEnumerable().ToList();

                //        obj_session.SetUserSessionData(userDetails.UserID, userDetails.RoleID, userDetails.LoginName, userDetails.UserName, FilterUserDetails, AttachConfigDetails);
                //    }
                //    catch (Exception e)
                //    {
                //        throw new Exception(e.ToString());
                //    }

                //    finally
                //    {
                //        db.Database.Connection.Close();
                //    }
                //    return true;
                //}
            }
            else
            {
                return false;
            }
        }

        public static bool UpdatePassword(UpdatePassword objUpdatePassword)
        {
            MMModel obj_db = new MMModel();
            UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;

            using (var sqlTan = obj_db.Database.BeginTransaction())
            {

                try
                {
                    var objInsertClient = obj_db.Database.ExecuteSqlCommand("spUpdatePassword @UserID,@LoginName,@LoginPassword",
                    new SqlParameter("UserID", Convert.ToInt32(userInfo.UserID)),
                    //new SqlParameter("ClientID", Convert.ToInt32(objUpdatePassword.ClientID)),
                    new SqlParameter("LoginName", objUpdatePassword.LoginName),
                    new SqlParameter("LoginPassword", objUpdatePassword.ConfirmNewPassword));

                    sqlTan.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    sqlTan.Rollback();
                    return false;
                    throw new Exception(ex.ToString());
                }
            }
        }

        public static bool UpdatePasswordVerifyUser(UpdatePassword objUpdatePassword, string request)
        {
            MMModel db = new MMModel();
            PasswordManager obj_PasswordMgr = new PasswordManager();
            SessionManager obj_session = new SessionManager();


            if (db.Database.SqlQuery<Login>("exec [spGetUserDetails] @LoginName = {0}", objUpdatePassword.LoginName).Select(x => new Login { Password = x.Password.ToString(), LoginPasswordSalt = x.LoginPasswordSalt, UserID = x.UserID, LoginName = x.LoginName }).ToList().Count > 0)
            {
                var userDetails = db.Database.SqlQuery<Login>("exec [spGetUserDetails] @LoginName = {0}", objUpdatePassword.LoginName)
                            .Select(x => new Login { Password = x.Password.ToString(), LoginPasswordSalt = x.LoginPasswordSalt, UserID = x.UserID, LoginName = x.LoginName, UserName = x.UserName }).ToList()[0];


                if (request == "Change")
                {

                    string existingPassword = userDetails.Password;
                    string existingPasswordSalt = userDetails.LoginPasswordSalt;

                    bool PasswordMatchResult = obj_PasswordMgr.IsPasswordMatch(objUpdatePassword.Password, existingPasswordSalt, existingPassword);
                    if (PasswordMatchResult == true)
                    {
                        //var userAccessRights = db.Database.SqlQuery<UserAccessRights>("exec [spGetUserAccessRights] @UserID = {0}", userDetails.UserID).ToList();
                        //List<UserAccessRights> obj = new List<UserAccessRights>();
                        //obj = userAccessRights;
                        //obj_session.SetUserSessionData(userDetails.UserID, userDetails.LoginName, userDetails.UserName, obj);
                        //return true;
                        using (var db1 = new MMModel())
                        {
                            db.Database.Initialize(force: false);
                            var cmd = db.Database.Connection.CreateCommand();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "[spGetUserPageAccessRights]";
                            SqlParameter param1 = new SqlParameter();
                            param1.ParameterName = "@UserID";
                            param1.SqlDbType = SqlDbType.Int;
                            param1.Value = userDetails.UserID;
                            cmd.Parameters.Add(param1);

                            try
                            {
                                db.Database.Connection.Open();

                                var reader = cmd.ExecuteReader();
                                var FilterUserDetails = ((IObjectContextAdapter)db)
                                    .ObjectContext
                                    .Translate<UserAccessRights>(reader).AsEnumerable().ToList();

                                //Move to second result set and read Posts User Roles
                                //reader.NextResult();
                                //var FilterUserRoleRightsDetails = ((IObjectContextAdapter)db)
                                //    .ObjectContext
                                //    .Translate<UserRoleRights>(reader).AsEnumerable().ToList();

                                reader.NextResult();
                                var AttachConfigDetails = ((IObjectContextAdapter)db)
                                    .ObjectContext
                                    .Translate<AttachmentConfig>(reader).AsEnumerable().ToList();

                                obj_session.SetUserSessionData(userDetails.UserID, userDetails.RoleID, userDetails.LoginName, userDetails.UserName, FilterUserDetails, AttachConfigDetails/*, userDetails.ClientID, userDetails.UserTypeID*/);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.ToString());
                            }

                            finally
                            {
                                db.Database.Connection.Close();
                            }
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //var userAccessRights = db.Database.SqlQuery<UserAccessRights>("exec [spGetUserAccessRights] @UserID = {0}", userDetails.UserID).ToList();
                    //List<UserAccessRights> obj = new List<UserAccessRights>();
                    //obj = userAccessRights;
                    //obj_session.SetUserSessionData(userDetails.UserID, userDetails.LoginName, userDetails.UserName, obj);
                    //return true;
                    using (var db1 = new MMModel())
                    {
                        db.Database.Initialize(force: false);
                        var cmd = db.Database.Connection.CreateCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[spGetUserPageAccessRights]";
                        SqlParameter param1 = new SqlParameter();
                        param1.ParameterName = "@UserID";
                        param1.SqlDbType = SqlDbType.Int;
                        param1.Value = userDetails.UserID;
                        cmd.Parameters.Add(param1);

                        try
                        {
                            db.Database.Connection.Open();

                            var reader = cmd.ExecuteReader();
                            var FilterUserDetails = ((IObjectContextAdapter)db)
                                .ObjectContext
                                .Translate<UserAccessRights>(reader).AsEnumerable().ToList();

                            //Move to second result set and read Posts User Roles
                            //reader.NextResult();
                            //var FilterUserRoleRightsDetails = ((IObjectContextAdapter)db)
                            //    .ObjectContext
                            //    .Translate<UserRoleRights>(reader).AsEnumerable().ToList();

                            reader.NextResult();
                            var AttachConfigDetails = ((IObjectContextAdapter)db)
                                .ObjectContext
                                .Translate<AttachmentConfig>(reader).AsEnumerable().ToList();

                            obj_session.SetUserSessionData(userDetails.UserID, userDetails.RoleID, userDetails.LoginName, userDetails.UserName, FilterUserDetails, AttachConfigDetails/*,userDetails.ClientID, userDetails.UserTypeID*/);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.ToString());
                        }

                        finally
                        {
                            db.Database.Connection.Close();
                        }
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        //public static int ForgetLoginVerify(ForgetPassword objForgetPassword, string request)
        //{
        //    MMModel db = new MMModel();
        //    PasswordManager obj_PasswordMgr = new PasswordManager();
        //    SessionManager obj_session = new SessionManager();
        //    int _returnvalue = 0;

        //    if (db.Database.SqlQuery<Login>("exec [spGetUserDetails] @LoginName = {0}", objForgetPassword.LoginName).Select(x => new Login { Password = x.Password.ToString(), LoginPasswordSalt = x.LoginPasswordSalt, UserID = x.UserID, LoginName = x.LoginName,RoleID = x.RoleID/*,UserTypeID=x.UserTypeID,ClientID=x.ClientID*/ }).ToList().Count > 0)
        //    {

        //        if (request == "Email")
        //        {
        //            UserLoginDetails ObjUser = new UserLoginDetails();
        //            using (var DBModel = new MMModel())
        //            {
        //                // If using Code First we need to make sure the model is built before we open the connection 
        //                // This isn't required for models created with the EF Designer 
        //                DBModel.Database.Initialize(force: false);

        //                // Create a SQL command to execute the sproc 
        //                var cmd = DBModel.Database.Connection.CreateCommand();
        //                cmd.CommandText = "[spGetLoginDetails]";
        //                cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //                SqlParameter loginName = new SqlParameter("@LoginName", System.Data.SqlDbType.VarChar, 100);
        //                loginName.Value = ObjUser.LoginName;
        //                cmd.Parameters.Add(loginName);
        //                try
        //                {
        //                    DBModel.Database.Connection.Open();
        //                    // Run the sproc  
        //                    var reader = cmd.ExecuteReader();

        //                    // Read Blogs from the first result set 
        //                    var _UserLoginDetails = ((IObjectContextAdapter)DBModel)
        //                        .ObjectContext
        //                        .Translate<UserLoginDetails>(reader).AsEnumerable().Single();


        //                    ObjUser = _UserLoginDetails;


        //                }
        //                finally
        //                {

        //                }
        //            }


        //            string salt = "";
        //            string NewPassword = obj_PasswordMgr.CreatePassword(8);
        //            string Path = HttpContext.Current.Request.PhysicalApplicationPath;
        //            //bool flag = Mail.SendMail(1, 2, "spGetEmailTemplateDetails", 0, "", ObjUser.UserID, NewPassword);
        //            //if (flag)
        //            //{

        //            //}
        //            //else
        //            //{
        //            //   return  _returnvalue = 1;
        //            //}
        //            var Result = obj_PasswordMgr.GeneratePasswordHash(NewPassword, out salt);
        //            objForgetPassword.NewPassword = Result;

        //            MMModel obj_db = new MMModel();
        //            //UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;

        //            using (var sqlTan = obj_db.Database.BeginTransaction())
        //            {

        //                try
        //                {
        //                    var objInsertClient = obj_db.Database.ExecuteSqlCommand("spUpdatePassword @UserID,@ClientID,@LoginName,@LoginPassword",
        //                    new SqlParameter("UserID", Convert.ToInt32(ObjUser.UserID)),
        //                 //   new SqlParameter("ClientID", Convert.ToInt32(ObjUser.ClientID)),
        //                    new SqlParameter("LoginName", ObjUser.LoginName),
        //                    new SqlParameter("LoginPassword", objForgetPassword.NewPassword));

        //                    sqlTan.Commit();
        //                    _returnvalue = 2;
        //                }
        //                catch (Exception ex)
        //                {
        //                    sqlTan.Rollback();
        //                    _returnvalue = 3;
        //                    throw new Exception(ex.ToString());
        //                }
        //            }

        //        }

        //    }
        //    return _returnvalue;
        //}

        public static int ForgetLoginVerify(ForgetPassword objForgetPassword, string request)
        {
            MMModel db = new MMModel();
            PasswordManager obj_PasswordMgr = new PasswordManager();
            SessionManager obj_session = new SessionManager();
            int _returnvalue = 0;

            if (db.Database.SqlQuery<Login>("exec [spGetUserDetails] @LoginName = {0}", objForgetPassword.LoginID).Select(x => new Login { Password = x.Password.ToString(), LoginPasswordSalt = x.LoginPasswordSalt, UserID = x.UserID, LoginName = x.LoginName }).ToList().Count > 0)
            {

                if (request == "Email")
                {
                    UserLoginDetails ObjUser = new UserLoginDetails();
                    using (var DBModel = new MMModel())
                    {
                        // If using Code First we need to make sure the model is built before we open the connection 
                        // This isn't required for models created with the EF Designer 
                        DBModel.Database.Initialize(force: false);

                        // Create a SQL command to execute the sproc 
                        var cmd = DBModel.Database.Connection.CreateCommand();
                        cmd.CommandText = "[spGetLoginDetails]";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter loginName = new SqlParameter("@LoginName", System.Data.SqlDbType.VarChar, 100);
                        loginName.Value = objForgetPassword.LoginID;
                        cmd.Parameters.Add(loginName);
                        try
                        {
                            DBModel.Database.Connection.Open();
                            // Run the sproc  
                            var reader = cmd.ExecuteReader();

                            // Read Blogs from the first result set 
                            var _UserLoginDetails = ((IObjectContextAdapter)DBModel)
                                .ObjectContext
                                .Translate<UserLoginDetails>(reader).AsEnumerable().Single();


                            ObjUser = _UserLoginDetails;


                        }
                        finally
                        {

                        }
                    }


                    string salt = "";
                    string NewPassword = obj_PasswordMgr.CreatePassword(8);
                    string Path = HttpContext.Current.Request.PhysicalApplicationPath;
                    bool flag = Mail.SendMail(1, 1, "spGetEmailTemplateDetails", 0, "", ObjUser.UserID, NewPassword);
                    if (flag)
                    {

                    }
                    else
                    {
                        return _returnvalue = 1;
                    }
                    var Result = NewPassword;
                    objForgetPassword.NewPassword = Result;

                    MMModel obj_db = new MMModel();
                    //UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;

                    using (var sqlTan = obj_db.Database.BeginTransaction())
                    {

                        try
                        {
                            var objInsertClient = obj_db.Database.ExecuteSqlCommand("spUpdatePassword @UserID,@LoginName,@LoginPassword",
                            new SqlParameter("UserID", Convert.ToInt32(ObjUser.UserID)),
                            new SqlParameter("LoginName", ObjUser.LoginName),
                            new SqlParameter("LoginPassword", objForgetPassword.NewPassword));
                            sqlTan.Commit();
                            _returnvalue = 2;
                        }
                        catch (Exception ex)
                        {
                            sqlTan.Rollback();
                            _returnvalue = 3;
                            throw new Exception(ex.ToString());
                        }
                    }

                }

            }
            return _returnvalue;
        }


        public static string ForgotPassword(ForgetPassword objForgetPassword)
        {
            string ErrMsg = "";
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MMModel"].ToString());
            SqlTransaction sqlTran;
            if (Conn.State == ConnectionState.Closed)
                Conn.Open();
            sqlTran = Conn.BeginTransaction();
            try
            {
                using (SqlCommand cmd = new SqlCommand("spForgotPassword", Conn, sqlTran))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;

                    var outErrMsg = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 50)
                    {
                        Value = 0,
                        Direction = ParameterDirection.InputOutput
                    };

                    cmd.Parameters.Add(outErrMsg);

                    cmd.Parameters.Add(new SqlParameter("@LoginName", objForgetPassword.LoginID));
                    cmd.Parameters.Add(new SqlParameter("@LoginPassword", objForgetPassword.NewPassword));

                    cmd.ExecuteNonQuery();
                    ErrMsg = Convert.ToString(outErrMsg.Value);
                }
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                sqlTran.Rollback();
                throw ex;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                    Conn.Close();
            }
            return ErrMsg;
        }
     
    }
}