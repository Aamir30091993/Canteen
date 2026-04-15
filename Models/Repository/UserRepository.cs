using GCC_Canteen.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GCC_Canteen.Models.Repository
{
    public class UserRepository
    {

        public static string SaveUser(UserViewModel obj, DataTable dtLocationAccess)
        {
            string flag = "";
            string outSR = "";
            string ErrMsg = "";
          
            UserInfo userinfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            var header = obj;

            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MMModel"].ToString());
            SqlTransaction sqlTran;
            if (Conn.State == ConnectionState.Closed)
                Conn.Open();
            sqlTran = Conn.BeginTransaction();
            try
            {
                using (SqlCommand cmd = new SqlCommand("spInsertUser", Conn, sqlTran))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;

                    var outErrMsg = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 50)
                    {
                        Value = 0,
                        Direction = ParameterDirection.InputOutput
                    };

                    cmd.Parameters.Add(outErrMsg);

                    //cmd.Parameters.Add(new SqlParameter("@LoggedInUserID", userinfo.UserID));
                    //cmd.Parameters.Add(new SqlParameter("@ID", header.ShipperDetails.ID));
                    cmd.Parameters.Add(new SqlParameter("@UserID", userinfo.UserID));
                    cmd.Parameters.Add(new SqlParameter("@LocationID", header.UserDetails.LocationID));
                    cmd.Parameters.Add(new SqlParameter("@Name", header.UserDetails.UserName));
                    cmd.Parameters.Add(new SqlParameter("@EmpCode", header.UserDetails.EmpCode));
                    cmd.Parameters.Add(new SqlParameter("@LoginID", header.UserDetails.LoginID));
                    cmd.Parameters.Add(new SqlParameter("@Password", header.UserDetails.Password));
                    cmd.Parameters.Add(new SqlParameter("@RoleID", header.UserDetails.RoleID));
                    cmd.Parameters.Add(new SqlParameter("@Designation", header.UserDetails.Designation));
                    cmd.Parameters.Add(new SqlParameter("@TeamName", header.UserDetails.TeamName));
                    cmd.Parameters.Add(new SqlParameter("@ReportingToUserID", header.UserDetails.ReportingToUserID));
                    cmd.Parameters.Add(new SqlParameter("@DOJ", header.UserDetails.DOJ == null ? "" : header.UserDetails.DOJ));
                    cmd.Parameters.Add(new SqlParameter("@DOL", header.UserDetails.DOL == null ? "" : header.UserDetails.DOL));
                    cmd.Parameters.Add(new SqlParameter("@IsActive", header.UserDetails.IsActive));


                    SqlParameter paramContact = new SqlParameter("@udtMstUserLocationAccess", SqlDbType.Structured);
                    paramContact.Value = dtLocationAccess;
                    cmd.Parameters.Add(paramContact);

                    cmd.ExecuteNonQuery();
                    ErrMsg = Convert.ToString(outErrMsg.Value);
                }
                flag = ErrMsg;
                sqlTran.Commit();
                return flag;
            }
            catch (Exception ex)
            {
                sqlTran.Rollback();
                return flag;
                throw ex;
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                    Conn.Close();
            }
        }

        public static UserViewModel GetUser(int ID)
        {
            UserViewModel obj = new UserViewModel();
            using (var db = new MMModel())
            {
                db.Database.Initialize(force: false);
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetUser";
                SqlParameter UserID = new SqlParameter();
                UserID.ParameterName = "@ID";
                UserID.SqlDbType = SqlDbType.Int;
                UserID.Value = ID;
                cmd.Parameters.Add(UserID);
                try
                {
                    if (db.Database.Connection.State == ConnectionState.Closed)
                    {
                        db.Database.Connection.Open();
                        var reader = cmd.ExecuteReader();
                        var UserDetails = ((IObjectContextAdapter)db).ObjectContext.Translate<User>(reader).AsEnumerable().ToList();
                        //reader.NextResult();
                        //var UserListValues = ((IObjectContextAdapter)db).ObjectContext.Translate<UserListValues>(reader).AsEnumerable().ToList();
                        obj.UserDetails = UserDetails[0];
                        //obj.UserListDetails = UserListValues;

                        reader.NextResult();

                        //var LocationDetails = ((IObjectContextAdapter)db).ObjectContext.Translate<User>(reader).AsEnumerable().ToList();

                        var objLocationInfo = ((IObjectContextAdapter)db)
                                .ObjectContext
                                .Translate<LocationInfo>(reader).AsEnumerable().ToList();

                        List<LocationInfo> Page = new List<LocationInfo>();
                        Page = objLocationInfo;
                        obj.LocationDetails = Page;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (db.Database.Connection.State == ConnectionState.Open)
                    {
                        db.Database.Connection.Close();
                    }
                }

            }
            return obj;
        }

        public static string UpdateUser(UserViewModel objUserViewModel, DataTable dtLocationAccess)
        {
            
            string flag = "";
            string outSR = "";
            string ErrMsg = "";

            UserInfo userinfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            var header = objUserViewModel;
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MMModel"].ToString());
            SqlTransaction sqlTran;
            if (Conn.State == ConnectionState.Closed)
                Conn.Open();
            sqlTran = Conn.BeginTransaction();

            try
            {
                using (SqlCommand cmd = new SqlCommand("spUpdateUser", Conn, sqlTran))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;

                    var outErrMsg = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 50)
                    {
                        Value = "",
                        Direction = ParameterDirection.InputOutput
                    };

                    //var OutUserID = new SqlParameter("@OutUserID", SqlDbType.VarChar, 50)
                    //{
                    //    Value = 0;
                    //    Direction = ParameterDirection.InputOutput
                    //};

                    cmd.Parameters.Add(outErrMsg);


                   //cmd.Parameters.Add(new SqlParameter("@LoggedInUserID", userinfo.UserID));
                    //cmd.Parameters.Add(new SqlParameter("@ID", header.ShipperDetails.ID));
                    cmd.Parameters.Add(new SqlParameter("@UserID", header.UserDetails.ID));
                    cmd.Parameters.Add(new SqlParameter("@LocationID", header.UserDetails.LocationID));
                    cmd.Parameters.Add(new SqlParameter("@Name", header.UserDetails.UserName));
                    cmd.Parameters.Add(new SqlParameter("@EmpCode", header.UserDetails.EmpCode));
                    cmd.Parameters.Add(new SqlParameter("@LoginID", header.UserDetails.LoginID));
                    cmd.Parameters.Add(new SqlParameter("@Password", header.UserDetails.Password));
                    cmd.Parameters.Add(new SqlParameter("@RoleID", header.UserDetails.RoleID));
                    cmd.Parameters.Add(new SqlParameter("@Designation", header.UserDetails.Designation));
                    cmd.Parameters.Add(new SqlParameter("@TeamName", header.UserDetails.TeamName));
                    cmd.Parameters.Add(new SqlParameter("@ReportingToUserID", header.UserDetails.ReportingToUserID));
                    cmd.Parameters.Add(new SqlParameter("@DOJ", header.UserDetails.DOJ==null?"": header.UserDetails.DOJ));
                    cmd.Parameters.Add(new SqlParameter("@DOL", header.UserDetails.DOL == null ? "" : header.UserDetails.DOL));

                    SqlParameter paramContact = new SqlParameter("@udtMstUserLocationAccess", SqlDbType.Structured);
                    paramContact.Value = dtLocationAccess;
                    cmd.Parameters.Add(paramContact);

                    cmd.ExecuteNonQuery();

                    ErrMsg = Convert.ToString(outErrMsg.Value);
                }

                flag = ErrMsg;
                sqlTran.Commit();
                return flag;
            }

            catch (Exception aa)
            {
                sqlTran.Rollback();
                throw aa;
            }
            finally
            {
                if (Conn.State == ConnectionState.Closed)
                    Conn.Close();
            }

        }
    }
}
