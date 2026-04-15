using GCC_Canteen.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace GCC_Canteen.Models.Repository
{
    public class ImportUserRepository
    {
        public static ImportUserViewModel InsertnValidate(ImportUserViewModel _ImportUserViewModelList, DataTable dtImport, string fileName)
        {
            //ImportVerticalwiseMonthlyTargetViewModel _importPPProgramWiseTargetViewModel = new ImportVerticalwiseMonthlyTargetViewModel();
            UserInfo userinfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            using (var db = new MMModel())
            {

                db.Database.Initialize(force: false);

                // Create a SQL command to execute the sproc 
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandText = "[spInsertImportUser]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                // Adding the errormsg parameter as an output parameter
                SqlParameter paramErrorMsg = new SqlParameter("@errormsg", System.Data.SqlDbType.VarChar, -1);
                paramErrorMsg.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramErrorMsg);
                //SqlParameter paramDate = new SqlParameter("@ImportDate", System.Data.SqlDbType.Date);
                //paramDate.Value = DateTime.Parse(_ImportUserViewModelList._ImportUserHdr.ImportDate);
                //cmd.Parameters.Add(paramDate);

                ////SqlParameter paramFilePath = new SqlParameter("@ImportFilePath", System.Data.SqlDbType.VarChar, 100);
                ////paramFilePath.Value = ImportClientViewModel.ImportClient.Source;
                ////cmd.Parameters.Add(paramFilePath);
                //SqlParameter paramFilePath = new SqlParameter("@ImportFilePath", System.Data.SqlDbType.VarChar, 100);
                //if (HttpContext.Current.Session["ImportUserAttachment" + _ImportUserViewModelList._ImportUserHdr.AttachmentSessionKey] != null)
                //{
                //    paramFilePath.Value = /*ConfigurationManager.AppSettings["BrowseURL"].ToString() +*/ @"\ImportUser\" + fileName;
                //}
                //else
                //    paramFilePath.Value = "";
                //cmd.Parameters.Add(paramFilePath);


                //SqlParameter paramNoOfRecords = new SqlParameter("@NoOfRecords", System.Data.SqlDbType.Int);
                //paramNoOfRecords.Value = dtImport.Rows.Count;
                //cmd.Parameters.Add(paramNoOfRecords);

                //SqlParameter paramRemarks = new SqlParameter("@Remark", System.Data.SqlDbType.VarChar, -1);
                //paramRemarks.Value = _ImportUserViewModelList._ImportUserHdr.Remark == null ? "" : _ImportUserViewModelList._ImportUserHdr.Remark;
                //cmd.Parameters.Add(paramRemarks);

                //SqlParameter paramCreatedBy = new SqlParameter("@CreatedBy", System.Data.SqlDbType.Int);
                //paramCreatedBy.Value = userinfo.UserID;
                //cmd.Parameters.Add(paramCreatedBy);

                //SqlParameter paramCreatedUser = new SqlParameter("@CreatedUser", System.Data.SqlDbType.VarChar, 20);
                //paramCreatedUser.Value = userinfo.UserName;
                //cmd.Parameters.Add(paramCreatedUser);

                // List of column names to keep in the DataTable
                List<string> columnsToKeep = new List<string>
                //{"UserTypeID", "EmpCode", "Name", "RoleID", "Department", "Designation", "Director",
                // "DOJ", "DOL", "Education", "EmailAddress", "Gender", "Grade", "TeamLeader", "TeamName",
                // "Manager", "ReportingToUserID", "LocationID", "LoginID", "Password", "SyncDate"
                //};
                {"UserTypeID","Location","User Name","Emp Code","Card No","User ID","Emp Designation","Team Name","Reporting To","Reporting To Emp Code","Reporting To Designation" };

                // Remove extra columns from the DataTable
                List<DataColumn> columnsToRemove = new List<DataColumn>();
                foreach (DataColumn column in dtImport.Columns)
                {
                    if (!columnsToKeep.Contains(column.ColumnName))
                    {
                        columnsToRemove.Add(column);
                    }
                }

                foreach (DataColumn column in columnsToRemove)
                {
                    dtImport.Columns.Remove(column);
                }
                SqlParameter paramUDTImportDtl = new SqlParameter("@UDTImportDtl", System.Data.SqlDbType.Structured);
                paramUDTImportDtl.Value = dtImport;
                cmd.Parameters.Add(paramUDTImportDtl);

                //SqlParameter paramPageId = new SqlParameter("@PageID", System.Data.SqlDbType.Int);
                //paramPageId.Value = (int)GCC_Canteen.App_Code.CommonBase.Page.ImportUser;
                //cmd.Parameters.Add(paramPageId);

                try
                {

                    db.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();


                    //var _importPPProgramTargetDetails = ((IObjectContextAdapter)db)
                    //    .ObjectContext
                    //    .Translate<DataTable>(reader).AsEnumerable().ToList();

                    //DataTable _importUserDtl = new DataTable();
                    //_importUserDtl.Load(reader);
                    //_ImportUserViewModelList._ImportUserHdr.ImportID = _importUserDtl.Rows[0].Field<int>("ImportID");
                    ////_ImportVerticalwiseMonthlyTargetViewModelList._ImportVerticalwiseMonthlyTargetDtl = _importPPProgramTargetDtl;
                    //HttpContext.Current.Session["ImportUserDtl" + _ImportUserViewModelList._ImportUserHdr.SessionIdentity] = _importUserDtl;

                    ////reader.NextResult();
                    //var ImportFileStatisticsListValues = ((IObjectContextAdapter)db)
                    //    .ObjectContext
                    //    .Translate<ImportUserStatisticsListValues>(reader).AsEnumerable().ToList();

                    //List<ImportUserStatisticsListValues> ImportStatisticsData = new List<ImportUserStatisticsListValues>();
                    //ImportStatisticsData = ImportFileStatisticsListValues;
                    //_ImportUserViewModelList._ImportUserStatistics = ImportStatisticsData;

                    //reader.NextResult();
                    //var ImportDetails = ((IObjectContextAdapter)db)
                    //    .ObjectContext
                    //    .Translate<ImportUserDtl>(reader).AsEnumerable().ToList();

                    //List<ImportUserDtl> ImportValid = new List<ImportUserDtl>();
                    //ImportValid = ImportDetails;
                    //_ImportUserViewModelList._ImportUserValidRecords = ImportValid;

                    //reader.NextResult();
                    //var ImportInvalidDetails = ((IObjectContextAdapter)db)
                    //    .ObjectContext
                    //    .Translate<ImportUserDtl>(reader).AsEnumerable().ToList();

                    //List<ImportUserDtl> ImportInValid = new List<ImportUserDtl>();
                    //ImportInValid = ImportInvalidDetails;
                    //_ImportUserViewModelList._ImportUserInValidRecords = ImportInValid;

                    //reader.NextResult();
                    //var ImportDuplicateDetails = ((IObjectContextAdapter)db)
                    //    .ObjectContext
                    //    .Translate<ImportUserDtl>(reader).AsEnumerable().ToList();

                    //List<ImportUserDtl> ImportDuplicate = new List<ImportUserDtl>();
                    //ImportDuplicate = ImportDuplicateDetails;
                    //_ImportUserViewModelList._ImportUserDuplicateRecords = ImportDuplicate;
                }
                catch (Exception ex)
                {
                    //return false;
                    throw ex;
                }
                finally
                {
                    db.Database.Connection.Close();
                }
                // Retrieve the error message if any from the output parameter
                if (paramErrorMsg.Value != DBNull.Value)
                {
                    string errorMessage = paramErrorMsg.Value as string;
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        // Handle the error message appropriately
                        _ImportUserViewModelList.ErrorMessage = errorMessage;
                    }
                }
                else
                {
                    _ImportUserViewModelList.ErrorMessage = string.Empty;
                }
            }
            return _ImportUserViewModelList;

        }

        public static string Import(int ID, int PageID)
        {
            string process = "";
            string process1 = "";

            UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            SqlParameter pa;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MMModel"].ConnectionString);
            SqlCommand cmd = new SqlCommand();

            SqlCommand cmd1 = new SqlCommand();
            SqlParameter pa1;

            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                cmd = new SqlCommand("spProcessImportUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                pa = cmd.Parameters.Add("@ImportID", SqlDbType.Int);
                pa.Value = ID;

                pa = cmd.Parameters.Add("@CreatedBy", SqlDbType.Int);
                pa.Value = userInfo.UserID;

                pa = cmd.Parameters.Add("@CreatedUser", SqlDbType.VarChar, 20);
                pa.Value = userInfo.UserName;

                pa = cmd.Parameters.Add("@PageID", SqlDbType.Int);
                pa.Value = PageID;

                pa = cmd.Parameters.Add("@ErrMsg", SqlDbType.VarChar, 100);
                pa.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                process = pa.Value.ToString();


                //cmd1 = new SqlCommand("spUpdateImportReportingToUser", conn);
                //cmd1.CommandType = CommandType.StoredProcedure;
                //pa1 = cmd1.Parameters.Add("@ImportID", SqlDbType.Int);
                //pa1.Value = ID;
                //pa1 = cmd1.Parameters.Add("@ErrMsg", SqlDbType.VarChar, 100);
                //pa1.Direction = ParameterDirection.Output;

                //cmd1.ExecuteNonQuery();
                //process1 = pa1.Value.ToString();

            }
            catch (Exception e)
            {
                process = "Error";
                throw e;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return process;
        }

        public static ImportUserViewModel getData(int ID, string sessionKey, int PageID)
        {
            ImportUserViewModel _importUserViewModel = new ImportUserViewModel();

            UserInfo userinfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            using (var db = new MMModel())
            {

                db.Database.Initialize(force: false);

                // Create a SQL command to execute the sproc 
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandText = "[spGetImportUser]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter paramID = new SqlParameter("@ImportID", System.Data.SqlDbType.Int);
                paramID.Value = ID;
                cmd.Parameters.Add(paramID);

                SqlParameter paramPageID = new SqlParameter("@PageID", System.Data.SqlDbType.Int);
                paramPageID.Value = PageID;
                cmd.Parameters.Add(paramPageID);

                SqlParameter paramUserID = new SqlParameter("@UserID", System.Data.SqlDbType.Int);
                paramUserID.Value = userinfo.UserID;
                cmd.Parameters.Add(paramUserID);

                try
                {

                    db.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    var importUserHdr = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<ImportUserHdr>(reader).AsEnumerable().ToList();

                    List<ImportUserHdr> _importUserHdr = new List<ImportUserHdr>();
                    _importUserHdr = importUserHdr;
                    _importUserViewModel._ImportUserHdr = _importUserHdr[0];
                    _importUserViewModel._ImportUserHdr.SessionIdentity = sessionKey;

                    reader.NextResult();

                    //var _importPPProgramTargetDtl = ((IObjectContextAdapter)db)
                    //    .ObjectContext
                    //    .Translate<DataTable>(reader).AsEnumerable().ToList();

                    DataTable _ImportUserDtl = new DataTable();
                    _ImportUserDtl.Load(reader);
                    //_importPPProgramWiseTargetViewModel._ImportVerticalwiseMonthlyTargetDtl = _ImportVerticalwiseMonthlyTargetDtl;
                    HttpContext.Current.Session["ImportUserDtl" + _importUserViewModel._ImportUserHdr.SessionIdentity] = _ImportUserDtl;

                    //reader.NextResult();

                    var ImportFileStatisticsListValues = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<ImportUserStatisticsListValues>(reader).AsEnumerable().ToList();

                    List<ImportUserStatisticsListValues> ImportStatisticsData = new List<ImportUserStatisticsListValues>();
                    ImportStatisticsData = ImportFileStatisticsListValues;
                    _importUserViewModel._ImportUserStatistics = ImportStatisticsData;

                    reader.NextResult();
                    var ImportDetails = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<ImportUserDtl>(reader).AsEnumerable().ToList();

                    List<ImportUserDtl> ImportValid = new List<ImportUserDtl>();
                    ImportValid = ImportDetails;
                    _importUserViewModel._ImportUserValidRecords = ImportValid;

                    reader.NextResult();
                    var ImportInvalidDetails = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<ImportUserDtl>(reader).AsEnumerable().ToList();

                    List<ImportUserDtl> ImportInValid = new List<ImportUserDtl>();
                    ImportInValid = ImportInvalidDetails;
                    _importUserViewModel._ImportUserInValidRecords = ImportInValid;

                    reader.NextResult();
                    var ImportDuplicateDetails = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<ImportUserDtl>(reader).AsEnumerable().ToList();

                    List<ImportUserDtl> ImportDuplicate = new List<ImportUserDtl>();
                    ImportDuplicate = ImportDuplicateDetails;
                    _importUserViewModel._ImportUserDuplicateRecords = ImportDuplicate;
                }
                catch (Exception ex)
                {
                    throw ex;
                    //return false;
                }
                finally
                {
                    db.Database.Connection.Close();
                }
            }
            return _importUserViewModel;
        }

        public static ImportUserViewModel Update(ImportUserViewModel _ImportUserViewModelList, DataTable dtImport, string fileName)
        {
            UserInfo userinfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            using (var db = new MMModel())
            {

                db.Database.Initialize(force: false);

                // Create a SQL command to execute the sproc 
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandText = "[spUpdateImportUser]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter paramImportID = new SqlParameter("@ImportID", System.Data.SqlDbType.Int);
                paramImportID.Value = _ImportUserViewModelList._ImportUserHdr.ImportID;
                cmd.Parameters.Add(paramImportID);

                SqlParameter paramFilePath = new SqlParameter("@ImportFilePath", System.Data.SqlDbType.VarChar, 100);
                if (HttpContext.Current.Session["ImportUserAttachment" + _ImportUserViewModelList._ImportUserHdr.AttachmentSessionKey] != null)
                {
                    paramFilePath.Value = @"\ImportUser\" + fileName;
                }
                else
                    paramFilePath.Value = "";

                cmd.Parameters.Add(paramFilePath);

                SqlParameter paramCreatedBy = new SqlParameter("@UserID", System.Data.SqlDbType.Int);
                paramCreatedBy.Value = userinfo.UserID;
                cmd.Parameters.Add(paramCreatedBy);

                SqlParameter paramCreatedUser = new SqlParameter("@User", System.Data.SqlDbType.VarChar, 20);
                paramCreatedUser.Value = userinfo.UserName;
                cmd.Parameters.Add(paramCreatedUser);

                SqlParameter paramUDTImportDtl = new SqlParameter("@UDTImportDtl", System.Data.SqlDbType.Structured);
                paramUDTImportDtl.Value = dtImport;
                cmd.Parameters.Add(paramUDTImportDtl);

                SqlParameter paramPageId = new SqlParameter("@PageID", System.Data.SqlDbType.Int);
                paramPageId.Value = (int)GCC_Canteen.App_Code.CommonBase.Page.ImportUser;
                cmd.Parameters.Add(paramPageId);

                try
                {

                    db.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();
                    //var _ImportVerticalwiseMonthlyTargetDetail = ((IObjectContextAdapter)db)
                    //   .ObjectContext
                    //   .Translate<ImportVerticalwiseMonthlyTargetDtl>(reader).AsEnumerable().ToList();

                    DataTable _ImportUserDtl = new DataTable();
                    _ImportUserDtl.Load(reader);
                    //_ImportVerticalwiseMonthlyTargetViewModelList._ImportVerticalwiseMonthlyTargetDtl = _ImportVerticalwiseMonthlyTargetDtl;
                    HttpContext.Current.Session["ImportUserDtl" + _ImportUserViewModelList._ImportUserHdr.SessionIdentity] = _ImportUserDtl;

                    //reader.NextResult();

                    var ImportFileStatisticsListValues = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<ImportUserStatisticsListValues>(reader).AsEnumerable().ToList();

                    List<ImportUserStatisticsListValues> ImportStatisticsData = new List<ImportUserStatisticsListValues>();
                    ImportStatisticsData = ImportFileStatisticsListValues;
                    _ImportUserViewModelList._ImportUserStatistics = ImportStatisticsData;

                    reader.NextResult();
                    var ImportDetails = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<ImportUserDtl>(reader).AsEnumerable().ToList();

                    List<ImportUserDtl> ImportValid = new List<ImportUserDtl>();
                    ImportValid = ImportDetails;
                    _ImportUserViewModelList._ImportUserValidRecords = ImportValid;

                    reader.NextResult();
                    var ImportInvalidDetails = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<ImportUserDtl>(reader).AsEnumerable().ToList();

                    List<ImportUserDtl> ImportInValid = new List<ImportUserDtl>();
                    ImportInValid = ImportInvalidDetails;
                    _ImportUserViewModelList._ImportUserInValidRecords = ImportInValid;

                    reader.NextResult();
                    var ImportDuplicateDetails = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<ImportUserDtl>(reader).AsEnumerable().ToList();

                    List<ImportUserDtl> ImportDuplicate = new List<ImportUserDtl>();
                    ImportDuplicate = ImportDuplicateDetails;
                    _ImportUserViewModelList._ImportUserDuplicateRecords = ImportDuplicate;
                }
                catch (Exception ex)
                {
                    //return false;
                }
                finally
                {
                    db.Database.Connection.Close();
                }
            }
            return _ImportUserViewModelList;
        }
        public static bool Delete(int ID, int PageID)
        {
            bool process = false;
            UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            SqlParameter pa;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MMModel"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                cmd = new SqlCommand("spDeleteImportUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                pa = cmd.Parameters.Add("@ImportID", SqlDbType.Int);
                pa.Value = ID;

                pa = cmd.Parameters.Add("@CreatedBy", SqlDbType.Int);
                pa.Value = userInfo.UserID;

                pa = cmd.Parameters.Add("@CreatedUser", SqlDbType.VarChar, 20);
                pa.Value = userInfo.UserName;

                //pa = cmd.Parameters.Add("@PageID", SqlDbType.Int);
                //pa.Value = PageID;
                cmd.ExecuteNonQuery();
                process = true;
            }
            catch (Exception e)
            {
                process = false;
                throw e;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return process;
        }

        public static ImportUserViewModel getViewData()
        {
            ImportUserViewModel _importUserViewModel = new ImportUserViewModel();

            UserInfo userinfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            using (var db = new MMModel())
            {

                db.Database.Initialize(force: false);

                // Create a SQL command to execute the sproc 
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandText = "[spGetImportTempStaffData]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //SqlParameter paramID = new SqlParameter("@ImportID", System.Data.SqlDbType.Int);
                //paramID.Value = ID;
                //cmd.Parameters.Add(paramID);

                //SqlParameter paramPageID = new SqlParameter("@PageID", System.Data.SqlDbType.Int);
                //paramPageID.Value = PageID;
                //cmd.Parameters.Add(paramPageID);

                //SqlParameter paramUserID = new SqlParameter("@UserID", System.Data.SqlDbType.Int);
                //paramUserID.Value = userinfo.UserID;
                //cmd.Parameters.Add(paramUserID);

                try
                {

                    db.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();
                    List<ImportViewData> importUserHdr = ((IObjectContextAdapter)db)
            .ObjectContext
            .Translate<ImportViewData>(reader)
            .AsEnumerable()
            .ToList();

                    // Assign fetched data to your view model
                    _importUserViewModel._ImportTempViewData = importUserHdr;
                    //List<ImportViewData> importUserHdr = new List<ImportViewData>();
                    //importUserHdr = ((IObjectContextAdapter)db)
                    //    .ObjectContext
                    //    .Translate<ImportViewData>(reader).AsEnumerable().ToList();

                    ////_ImportTempViewData = _ImportTempViewData;
                    //_importUserViewModel._ImportTempViewData = importUserHdr;
                    ////_importUserViewModel._ImportUserHdr.SessionIdentity = sessionKey;

                }
                catch (Exception ex)
                {
                    throw ex;
                    //return false;
                }
                finally
                {
                    db.Database.Connection.Close();
                }
            }
            return _importUserViewModel;
        }

    }
}