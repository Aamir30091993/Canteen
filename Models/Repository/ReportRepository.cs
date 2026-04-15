using GCC_Canteen.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GCC_Canteen.Models.Repository
{
    public class ReportRepository
    {
        public static void GenerateReport(ReportViewModel objReportViewModel, string SessionIdentifier)
        {
            UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            SqlParameter pa, pa1; SqlTransaction tran = null;
            string sites = "";
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MMModel"].ConnectionString);
            SqlDataAdapter da = null;
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                tran = conn.BeginTransaction();

                cmd = new SqlCommand("SpRptGetReportData", conn, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userInfo.UserID);
                cmd.Parameters.AddWithValue("@ReportID", objReportViewModel.ReportParameterDetails.ReportID);
                cmd.Parameters.AddWithValue("@FromDate", objReportViewModel.ReportParameterDetails.ReportFromDate);
                cmd.Parameters.AddWithValue("@ToDate", objReportViewModel.ReportParameterDetails.ReportToDate);

                // Check if EventID is null, if so, add DBNull.Value as the parameter value
                if (objReportViewModel.ReportParameterDetails.EventID == null)
                {
                    cmd.Parameters.AddWithValue("@EventID", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@EventID", objReportViewModel.ReportParameterDetails.EventID);
                }

                //cmd.Parameters.AddWithValue("@Customer", objReportsViewModel.ReportParameterDetails.Customer == null ? "" : objReportsViewModel.ReportParameterDetails.Customer);
                //cmd.Parameters.AddWithValue("@StatusID", objReportsViewModel.ReportParameterDetails.Status == null ? "" : objReportsViewModel.ReportParameterDetails.Status);
                //cmd.Parameters.AddWithValue("@OrderNo", objReportsViewModel.ReportParameterDetails.OrderNo == null ? "" : objReportsViewModel.ReportParameterDetails.OrderNo);
                //cmd.Parameters.AddWithValue("@OrderDate", objReportsViewModel.ReportParameterDetails.OrderDate == null ? "" : objReportsViewModel.ReportParameterDetails.OrderDate);
                da = new SqlDataAdapter(cmd);
                cmd.CommandTimeout = 120;
                da.Fill(ds);
                //ToBeExported = "0";
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 5000)
                    {
                        //ToBeExported = "1";
                        break;
                    }
                }
                HttpContext.Current.Session["MISReportData_" + SessionIdentifier] = ds;
                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                conn.Close();
            }
        }

    }
}