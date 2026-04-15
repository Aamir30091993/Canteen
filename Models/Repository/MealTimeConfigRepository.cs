using GCC_Canteen.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace GCC_Canteen.Models.Repository
{
    public class MealTimeConfigRepository
    {
        public static MealTimeConfigViewModel GetMealTimeConfigList()
        {
            MealTimeConfigViewModel objMealTimeConfigListDetails = new MealTimeConfigViewModel();
            using (var db = new MMModel())
            {
                db.Database.Initialize(force: false);
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetMealTime";

                cmd.CommandTimeout = 300;


                try
                {
                    if (db.Database.Connection.State == ConnectionState.Closed)
                    {
                        db.Database.Connection.Open();


                        var reader = cmd.ExecuteReader();

                        //Filter Data List
                        var temp_MealTimeConfigList = ((IObjectContextAdapter)db).ObjectContext.Translate<MealTimeConfigList>(reader).AsEnumerable().ToList();
                        objMealTimeConfigListDetails.MealTimeConfigListDetails = temp_MealTimeConfigList;
                        
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
            return objMealTimeConfigListDetails;
        }
        
        //public static MealTimeConfigViewModel GetSysConfig()
        //{
        //    MealTimeConfigViewModel model = new MealTimeConfigViewModel();
        //    using (var db = new MMModel())
        //    {
        //        db.Database.Initialize(force: false);
        //        var cmd = db.Database.Connection.CreateCommand();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = "spGetSysConfig";

        //        cmd.CommandTimeout = 300;


        //        try
        //        {
        //            if (db.Database.Connection.State == ConnectionState.Closed)
        //            {
        //                db.Database.Connection.Open();


        //                var reader = cmd.ExecuteReader();

        //                //Filter Data List
        //                var temp_SysConfigDetails = ((IObjectContextAdapter)db).ObjectContext.Translate<SysConfig>(reader).AsEnumerable().ToList();
        //                model.SysConfigDetails = temp_SysConfigDetails[0];

        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            throw e;
        //        }
        //        finally
        //        {
        //            if (db.Database.Connection.State == ConnectionState.Open)
        //            {
        //                db.Database.Connection.Close();
        //            }
        //        }
        //    }
        //    return model;
        //}

        public static string UpdateMealTimeConfigList(List<MealTimeConfigList> mealTimeConfigurations)
        {
            string flag = "";
            string outSR = "";
            string ErrMsg = "";
            DataTable dtMealTimeConfig = new DataTable();
            dtMealTimeConfig.Columns.Add("MealTimeID");
            //dtMealTimeConfig.Columns.Add("MealTypeID");
            dtMealTimeConfig.Columns.Add("StartTime");
            dtMealTimeConfig.Columns.Add("EndTime");
            dtMealTimeConfig.Columns.Add("BufferTime");
            //int mealTimeIDCounter = 1;
            foreach (var item in mealTimeConfigurations)
            {
                DataRow tempMealTime = dtMealTimeConfig.NewRow();
                tempMealTime["MealTimeID"] = item.MealTimeID; //mealTimeIDCounter++;
                //tempMealTime["MealTypeID"] = item.MealTypeID;
                tempMealTime["StartTime"] = item.StartTime;
                tempMealTime["EndTime"] = item.EndTime;
                tempMealTime["BufferTime"] = item.BufferTime;
                dtMealTimeConfig.Rows.Add(tempMealTime);
            }

            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MMModel"].ToString());
            SqlTransaction sqlTran;
            if (Conn.State == ConnectionState.Closed)
                Conn.Open();
            sqlTran = Conn.BeginTransaction();
            try
            {
                using (SqlCommand cmd = new SqlCommand("spUpdateMealTime", Conn, sqlTran))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;

                    //var outErrMsg = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 50)
                    //{
                    //    Value = 0,
                    //    Direction = ParameterDirection.InputOutput
                    //};

                    //cmd.Parameters.Add(outErrMsg);

                    SqlParameter paramDt = new SqlParameter("@dt_MealTime", SqlDbType.Structured);
                    paramDt.Value = dtMealTimeConfig;
                    cmd.Parameters.Add(paramDt);

                    cmd.ExecuteNonQuery();
                    //ErrMsg = Convert.ToString(outErrMsg.Value);
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
        public static string UpdateSysConfig(SysConfigDtl sysConfigDetails)
        {
            string flag = "";
            string outSR = "";
            string ErrMsg = "";

            UserInfo userinfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            //var header = obj;

            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MMModel"].ToString());
            SqlTransaction sqlTran;
            if (Conn.State == ConnectionState.Closed)
                Conn.Open();
            sqlTran = Conn.BeginTransaction();
            try
            {
                using (SqlCommand cmd = new SqlCommand("UpdateSysConfigDetail", Conn, sqlTran))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@SysConfigID", sysConfigDetails.SysConfigID));
                    cmd.Parameters.Add(new SqlParameter("@Key", sysConfigDetails.Key));
                    cmd.Parameters.Add(new SqlParameter("@Value", sysConfigDetails.Value));
                    cmd.Parameters.Add(new SqlParameter("@Datatype", sysConfigDetails.Datatype));
                    cmd.ExecuteNonQuery();
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

    }
}