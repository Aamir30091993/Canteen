using GCC_Canteen.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace GCC_Canteen.Models.Repository
{
    public class DashboardRepository
    {
        public static DashboardViewModel GetDashboard(DateTime FromDate, DateTime ToDate,string Employee, int MealTypeID,int MealSubTypeID, string LoginID)
        {
            DashboardViewModel objDashboardViewModel = new DashboardViewModel();
            using (var db = new MMModel())
            {
                db.Database.Initialize(force: false);
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetFilteredListData";
                //SqlParameter paramUserID = new SqlParameter();
                //paramUserID.ParameterName = "@UserId";
                //paramUserID.SqlDbType = SqlDbType.Int;
                //paramUserID.Value = UserID;
                //cmd.Parameters.Add(paramUserID);

                SqlParameter paramFromDate = new SqlParameter();
                paramFromDate.ParameterName = "@Fromdate";
                paramFromDate.SqlDbType = SqlDbType.Date;
                paramFromDate.Value = Convert.ToDateTime(FromDate);
                cmd.Parameters.Add(paramFromDate);

                SqlParameter paramToDate = new SqlParameter();
                paramToDate.ParameterName = "@Todate";
                paramToDate.SqlDbType = SqlDbType.Date;
                paramToDate.Value = Convert.ToDateTime(ToDate);
                cmd.Parameters.Add(paramToDate);

                SqlParameter paramEmployee = new SqlParameter();
                paramEmployee.ParameterName = "@Employee";
                paramEmployee.SqlDbType = SqlDbType.VarChar;
                paramEmployee.Value = Employee != null ? (object)Employee : DBNull.Value;
                cmd.Parameters.Add(paramEmployee);

                SqlParameter paramMealType = new SqlParameter();
                paramMealType.ParameterName = "@MealTypeID";
                paramMealType.SqlDbType = SqlDbType.Int;
                paramMealType.Value = MealTypeID != 0 ? (object)MealTypeID : DBNull.Value;
                cmd.Parameters.Add(paramMealType);

                SqlParameter paramMealSubType = new SqlParameter();
                paramMealSubType.ParameterName = "@MealSubTypeID";
                paramMealSubType.SqlDbType = SqlDbType.Int;
                paramMealSubType.Value = MealSubTypeID != 0 ? (object)MealSubTypeID : DBNull.Value;
                cmd.Parameters.Add(paramMealSubType);

                SqlParameter paramLoginID = new SqlParameter();
                paramLoginID.ParameterName = "@LoginID";
                paramLoginID.SqlDbType = SqlDbType.VarChar;
                paramLoginID.Value = LoginID != null ? (object)LoginID : DBNull.Value;
                cmd.Parameters.Add(paramLoginID);

                cmd.CommandTimeout=300;


                try
                {
                    if (db.Database.Connection.State == ConnectionState.Closed)
                    {
                        db.Database.Connection.Open();

                        
                        var reader = cmd.ExecuteReader();

                        //Filter Data List
                        var temp_FilteredDataList = ((IObjectContextAdapter)db).ObjectContext.Translate<FilteredListData>(reader).AsEnumerable().ToList();
                        objDashboardViewModel.FilteredListDataDetails = temp_FilteredDataList;
                        //Dashboard
                        //var DashboardDetails = ((IObjectContextAdapter)db).ObjectContext.Translate<Dashboard>(reader).AsEnumerable().ToList();
                        //objDashboardViewModel.DashboardDetails = DashboardDetails[0];

                        //reader.NextResult();



                        ////DashboardOverAllDetails
                        //var DashboardOverAllDetails = ((IObjectContextAdapter)db).ObjectContext.Translate<DashboardOverAllView>(reader).AsEnumerable().ToList();
                        //objDashboardViewModel.DashboardOverAllViewDetails = DashboardOverAllDetails[0];

                        //reader.NextResult();

                        ////DashboardSurveySummary_DirectorWise
                        //var DashboardSurveySummary_DirectorWise = ((IObjectContextAdapter)db).ObjectContext.Translate<DashboardSurveySummary>(reader).AsEnumerable().ToList();
                        //objDashboardViewModel.DashboardSurveySummary_DirectorWise = DashboardSurveySummary_DirectorWise;


                        //reader.NextResult();

                        ////DashboardSurveySummary_ManagerWise
                        //var DashboardSurveySummary_ManagerWise = ((IObjectContextAdapter)db).ObjectContext.Translate<DashboardSurveySummary>(reader).AsEnumerable().ToList();
                        //objDashboardViewModel.DashboardSurveySummary_ManagerWise = DashboardSurveySummary_ManagerWise;

                        //reader.NextResult();

                        ////DashboardSurveySummary_TeamLeaderWise
                        //var DashboardSurveySummary_TeamLeaderWise = ((IObjectContextAdapter)db).ObjectContext.Translate<DashboardSurveySummary>(reader).AsEnumerable().ToList();
                        //objDashboardViewModel.DashboardSurveySummary_TeamLeaderWise = DashboardSurveySummary_TeamLeaderWise;

                        //reader.NextResult();

                        ////DashboardSurveySummary_TeamMemberWise
                        //var DashboardSurveySummary_TeamMemberWise = ((IObjectContextAdapter)db).ObjectContext.Translate<DashboardSurveySummary>(reader).AsEnumerable().ToList();
                        //objDashboardViewModel.DashboardSurveySummary_TeamMemberWise = DashboardSurveySummary_TeamMemberWise;

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
            return objDashboardViewModel;
        }

    }
}