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
    public class CommonRepository
    {
        public static List<KeyValuePair<string, string>> GetCities(string term)
        {
            MMModel obj_db = new MMModel();
            UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            var obj = obj_db.Database.SqlQuery<AutoComplete>("exec spGetFillList @ActionID={0},@PageID={1},@ControlPageID={2},@UserID={3},@RecordID={4},@KeyValue={5}", 24, 0, 0, userInfo.UserID, 0, term).
                Select(x => new AutoComplete { Code = x.Code, Description = x.Description }).ToList();
            var listResult = new List<KeyValuePair<string, string>>();

            foreach (var item in obj)
            {
                listResult.Add(new KeyValuePair<string, string>(item.Code.ToString(), item.Description));
            }
            return listResult;
        }

        public static List<KeyValuePair<string, string>> AutoCompleteList(string KeyValue, int ActionID)
        {
            MMModel obj_db = new MMModel();
            UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            var obj = obj_db.Database.SqlQuery<AutoComplete>("exec spGetFillList @KeyValue={0},@ActionID={1}", KeyValue, ActionID).
             Select(x => new AutoComplete { Code = x.Code, Description = x.Description }).ToList();
            var listResult = new List<KeyValuePair<string, string>>();

            foreach (var item in obj)
            {
                listResult.Add(new KeyValuePair<string, string>(item.Code.ToString(), item.Description));
            }
            return listResult;
        }


        public static List<T> getList<T>(int PageID, string whereClouse, string Control, int page, string sort, string SessionIdentifier = "0")
        {
            UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;

            using (var db = new MMModel())
            {

                db.Database.Initialize(force: false);

                // Create a SQL command to execute the sproc 
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandText = "[spGetSearchResults]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 360000000;
                SqlParameter paramPageID = new SqlParameter("@PageID", System.Data.SqlDbType.Int);
                paramPageID.Value = Convert.ToInt32(PageID);
                cmd.Parameters.Add(paramPageID);

                if (whereClouse != "")
                {
                    SqlParameter paramWhereClouse = new SqlParameter("@WhereClause", System.Data.SqlDbType.NVarChar, -1);
                    paramWhereClouse.Value = whereClouse;
                    cmd.Parameters.Add(paramWhereClouse);
                }

                SqlParameter paramControl = new SqlParameter("@Control", System.Data.SqlDbType.VarChar, 50);
                paramControl.Value = Control.ToString();
                cmd.Parameters.Add(paramControl);

                SqlParameter paramUserID = new SqlParameter("@LoggedInUserID", System.Data.SqlDbType.Int);
                paramUserID.Value = userInfo.UserID;
                cmd.Parameters.Add(paramUserID);

                try
                {
                    db.Database.Connection.Open();
                    // Run the sproc  
                    var reader = cmd.ExecuteReader();

                    var ListValues = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<T>(reader).AsEnumerable().ToList();

                    //List<T> ListData = new List<T>();
                    //if (whereClouse == "")
                    //{
                    //    ListData = ListValues.Find(m => m.IsActiveStatus == true);
                    //}
                    //else
                    //{
                    //    ListData = ListValues;
                    //}

                    if (whereClouse != "")
                        HttpContext.Current.Session["whereClouse_" + SessionIdentifier] = whereClouse;
                    return ListValues;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    db.Database.Connection.Close();
                }
            }

        }

        public static void InsertLogException(Exception ex, ref int ExceptionId, ref string Exception, string ExceptionOnAction, string Severity, int ModuleId, int UserId)
        {
            string exMethod = ex.TargetSite.ToString();
            Exception = ex.Message;
            string exDetails = ex.ToString();

            InsertException(ref ExceptionId, null, exMethod, ExceptionOnAction, ref Exception, exDetails, ModuleId, UserId, Severity);
        }

        private static void InsertException(ref int ExceptionId, string ExceptionCode, string ExceptionInMethod,
                            string ExceptionOnAction, ref string Exception, string ExceptionDetails, int ModuleId, int UserId, string Severity)
        {
            MMModel db = new MMModel();
            SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[spInsertExceptionLog]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //Adding parameters 
                cmd.Parameters.Add("@ExceptionId", SqlDbType.Int).Value = ExceptionId;
                cmd.Parameters.Add("@ExceptionCode", SqlDbType.VarChar).Value = ExceptionCode;
                cmd.Parameters.Add("@ExceptionInMethod", SqlDbType.VarChar).Value = ExceptionInMethod;
                cmd.Parameters.Add("@ExceptionOnAction", SqlDbType.VarChar).Value = ExceptionOnAction;
                cmd.Parameters.Add("@Exception", SqlDbType.VarChar).Value = Exception;
                cmd.Parameters.Add("@ExceptionDetails", SqlDbType.VarChar).Value = ExceptionDetails;
                cmd.Parameters.Add("@Severity", SqlDbType.VarChar).Value = Severity;
                cmd.Parameters.Add("@ModuleId", SqlDbType.Int).Value = ModuleId;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                //Input/Ouptput setting
                cmd.Parameters["@ExceptionId"].Direction = ParameterDirection.InputOutput;
                cmd.Parameters["@Exception"].Direction = ParameterDirection.InputOutput;

                cmd.ExecuteNonQuery();
                ExceptionId = Convert.ToInt32(cmd.Parameters["@ExceptionId"].Value);
                Exception = Convert.ToString(cmd.Parameters["@Exception"].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
    //    public static MealTimeConfigViewModel GetSysConfig()
    //    {
    //        MealTimeConfigViewModel model = new MealTimeConfigViewModel();
    //        using (var db = new MMModel())
    //        {
    //            db.Database.Initialize(force: false);
    //            var cmd = db.Database.Connection.CreateCommand();
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.CommandText = "spGetSysConfig";

    //            cmd.CommandTimeout = 300;


    //            try
    //            {
    //                if (db.Database.Connection.State == ConnectionState.Closed)
    //                {
    //                    db.Database.Connection.Open();


    //                    var reader = cmd.ExecuteReader();

    //                    //Filter Data List
    //                    var temp_SysConfigDetails = ((IObjectContextAdapter)db).ObjectContext.Translate<SysConfig>(reader).AsEnumerable().ToList();
    //                    model.SysConfigDetails = temp_SysConfigDetails[0];

    //                }
    //            }
    //            catch (Exception e)
    //            {
    //                throw e;
    //            }
    //            finally
    //            {
    //                if (db.Database.Connection.State == ConnectionState.Open)
    //                {
    //                    db.Database.Connection.Close();
    //                }
    //            }
    //        }
    //        return model;
    //    }
    //}

    public static List<SysConfig> GetSysConfig(string SysConfigID)
    {
        List<SysConfig> sysConfigList = new List<SysConfig>();
            using (var db = new MMModel())
            {
                db.Database.Initialize(force: false);
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetSysConfig";
                cmd.CommandTimeout = 300;

                SqlParameter paramSysConfigID = new SqlParameter();
                paramSysConfigID.ParameterName = "@SysConfigID";
                paramSysConfigID.SqlDbType = SqlDbType.VarChar;
                paramSysConfigID.Value = SysConfigID != null ? (object)SysConfigID : DBNull.Value;
                cmd.Parameters.Add(paramSysConfigID);
                try
                {
                    db.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();
                    sysConfigList = ((IObjectContextAdapter)db).ObjectContext.Translate<SysConfig>(reader).AsEnumerable().ToList();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    db.Database.Connection.Close();
                }
            }
        return sysConfigList;
    }
}

public class AutoComplete
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class SysConfig
    {
        public int SysConfigID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Datatype { get; set; }
    }
}