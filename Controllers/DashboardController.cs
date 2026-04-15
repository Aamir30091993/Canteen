//using GCC_Canteen.ViewModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using GCC_Canteen.Models.Repository;
//using GCC_Canteen.App_Code;
//using System.Data;
//using System.Text;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
//using GCC_Canteen.Models.BusinessLayer;
//using System.IO;
//using DocumentFormat.OpenXml;
//using System.Web.UI;
//using Newtonsoft.Json;
//using OfficeOpenXml;
//using Excel = Microsoft.Office.Interop.Excel;
//using OfficeOpenXml.Style;
//using ClosedXML.Excel;

using GCC_Canteen.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GCC_Canteen.Models.Repository;
using GCC_Canteen.App_Code;
using System.Data;
using System.Text;
using GCC_Canteen.Models.BusinessLayer;
using System.IO;
using System.Web.UI;
using Newtonsoft.Json;
namespace GCC_Canteen.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        string SessionIdentifier = "";
        string[] headerColumns;

        Microsoft.Office.Interop.Excel.Application xlApp;
        Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
        Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
        Microsoft.Office.Interop.Excel.Sheets xlBigSheet;
        Microsoft.Office.Interop.Excel.Sheets xlSheet;
        object misValue;
        String newPath;

        public ActionResult Show()
        {

            if (Request.Headers["SessionIdentifier"] == null)
                SessionIdentifier = DateTime.UtcNow.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_');
            else
                SessionIdentifier = Request.Headers["SessionIdentifier"].ToString();
            TempData["SessionIdentifier"] = SessionIdentifier;
            UserInfo userInfo = Session["UserInfo"] as UserInfo;
            //ViewBag.RoleID = userInfo.RoleID;
            ViewBag.MealTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(1, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, 0, "", "");
            ViewBag.MealSubTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(2, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, 0, "", "");
            
            DashboardViewModel objDashboardViewModel=new DashboardViewModel();
            //objDashboardViewModel.DashboardDetails.FromDate = DateTime.Now.ToString("yyyy-MM-dd");
            //objDashboardViewModel.DashboardDetails.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
            objDashboardViewModel.DashboardDetails = new Dashboard
            {
                FromDate = DateTime.Now.ToString("dd-MM-yyyy"),
                ToDate = DateTime.Now.ToString("dd-MM-yyyy"),
                Employee = "",
                MealTypeID = 0,
                MealSubTypeID = 0,
                LoginID = ""
            };


            //Dashboard dashboardDetails = new Dashboard
            //{
            //    FromDate = "",
            //    ToDate = "",
            //    Employee = "",
            //    MealTypeID = 0,
            //    MealSubTypeID = 0
            //};

            //objDashboardViewModel = DashboardRepository.GetDashboard(userInfo.UserID, DateTime.Now.AddDays(-1).Date, DateTime.Now.AddDays(-1).Date,'',
            //    (userInfo.RoleID == 2 || userInfo.RoleID == 3 || userInfo.RoleID == 4 || userInfo.RoleID == 5 || userInfo.RoleID == 6) ? 999 /*My Team*/: 99, Convert.ToInt32(objDashboardViewModel?.DashboardDetails?.MealSubTypeID ?? 1));
            //int MealTypeID = objDashboardViewModel.DashboardDetails.MealTypeID;
            //int MealSubTypeID = objDashboardViewModel.DashboardDetails.MealSubTypeID;

            //objDashboardViewModel.SessionIdentifier = SessionIdentifier;
            //DashboardRepository.GenerateReport(objDashboardViewModel, SessionIdentifier);

            // Check if MealSubTypeID is not selected in the dropdown
            string ln = "";
            //if (MealTypeID != 0)
            //{
            //    ViewBag.lnlist = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(6, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, MealTypeID, "", "");
            //    ln = ViewBag.lnlist[0].Text;
            //}
            //else
            //{
            //    ln = "All";
            //}
            string etl = "";
            //if (MealSubTypeID != 0)
            //{
            //    ViewBag.etlList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(7, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, MealSubTypeID, "", "");
            //    etl = ViewBag.etlList[0].Text;
            //}
            //else
            //{
            //    etl = null;
            //}

            //string ParamValues = "";
            //ParamValues = "<b>From Date</b>: "
            //                + objDashboardViewModel.DashboardDetails.FromDate + ", <b>To Date</b>: "
            //                + objDashboardViewModel.DashboardDetails.ToDate + ", <b>Location</b>:" + ln + ",<b>Event Type</b>:" + etl;

            //Session["ParamValues_" + SessionIdentifier] = ParamValues;
            //if (userInfo.RoleID == 2 || userInfo.RoleID == 3 || userInfo.RoleID == 4 || userInfo.RoleID == 5 || userInfo.RoleID == 6)
            //{
            //    objDashboardViewModel.DashboardDetails.MealTypeID = 999;
            //    objDashboardViewModel.DashboardDetails.MealSubTypeID = 1;
            //}
            //else
            //{
            //    objDashboardViewModel.DashboardDetails.MealTypeID = 99;
            //    objDashboardViewModel.DashboardDetails.MealSubTypeID = 1;
            //}

            return View(objDashboardViewModel);
        }
        [HttpPost]
        public ActionResult Show(Dashboard objDashboard)
        {
            try
            {
                // Check if SessionIdentifier exists in headers, otherwise generate a new one
                string sessionIdentifier = Request.Headers["SessionIdentifier"];
                if (string.IsNullOrEmpty(sessionIdentifier))
                {
                    sessionIdentifier = objDashboard.FromDate + '&' + objDashboard.ToDate;//DateTime.UtcNow.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_');
                }

                // Store SessionIdentifier in TempData
                TempData["SessionIdentifier"] = sessionIdentifier;

                // Populate ViewBag with necessary data (assuming these are necessary for the view)
                ViewBag.MealTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(1, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, 0, "", "");
                ViewBag.MealSubTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(2, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, 0, "", "");

                // Example: Retrieve data from repository based on objDashboard parameters
                var getFilteredListData = DashboardRepository.GetDashboard(
                    Convert.ToDateTime(objDashboard.FromDate),
                    Convert.ToDateTime(objDashboard.ToDate),
                    objDashboard.Employee,
                    objDashboard.MealTypeID,
                    objDashboard.MealSubTypeID,
                    objDashboard.LoginID
                );

                //if (getFilteredListData == null || getFilteredListData.FilteredListDataDetails == null || getFilteredListData.FilteredListDataDetails.Count == 0)
                //{
                //    ViewBag.ErrorMessage = "No records found for the selected criteria."; // Set error message
                //    return View("ErrorView"); // Or handle no data scenario as needed
                //}
                // Check if filtered data is empty
                if (getFilteredListData == null || getFilteredListData.FilteredListDataDetails == null || getFilteredListData.FilteredListDataDetails.Count == 0)
                {
                    return Json(new { success = false, message = "No records found for the selected criteria." });
                }
                // If data is found, serialize and store it in TempData
                TempData["FilteredData"] = JsonConvert.SerializeObject(getFilteredListData.FilteredListDataDetails);

                // Redirect to Export action in Dashboard controller (change if Export is in a different controller)
                //return RedirectToAction("Export", "Dashboard");
                //return View(objDashboard);
                return Json(new { success = true, sessionIdentifier = sessionIdentifier });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine("Error in Show action:", ex.Message);
                return Json(new { success = false, message = "An error occurred while processing your request." });
                // Set error message for display or logging
                //ViewBag.ErrorMessage = "An error occurred while processing your request."; // Set error message
                //return View("ErrorView"); // Or handle error scenario as needed
            }
        }

        [HttpGet]
        public void Exportnew(Dashboard objDashboard)
        {
            // Example: Retrieve data from repository based on objDashboard parameters
            var getFilteredListData = DashboardRepository.GetDashboard(
                Convert.ToDateTime(objDashboard.FromDate),
                Convert.ToDateTime(objDashboard.ToDate),
                objDashboard.Employee,
                objDashboard.MealTypeID,
                objDashboard.MealSubTypeID,
                objDashboard.LoginID
            );

            if (objDashboard.MealTypeID == 1 || objDashboard.MealTypeID == 3 || objDashboard.MealTypeID == 5)
            {
                objDashboard.MealSubTypeID = 0;
                objDashboard.selectedMealSubTypeName = "";
            }

            //string ReportName = "Canteen_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ".xls"; // Change to .xlsx
            string ReportName = "Canteen_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".xls"; // Change to .xlsx
            ReportName = ReportName.Replace(',', '&');
            ReportName = ReportName.Replace("/", "");
            ReportName = ReportName.Replace("\\", "");
            ReportName = ReportName.Replace(":", "");
            ReportName = ReportName.Replace("*", "");
            ReportName = ReportName.Replace("?", "");
            ReportName = ReportName.Replace("\"", "");
            ReportName = ReportName.Replace("<", "");
            ReportName = ReportName.Replace(">", "");
            ReportName = ReportName.Replace("|", "");

            Session["PageID"] = (int)CommonBase.Page.Report;
            var filteredDataJson = TempData["FilteredData"] as string;
            //var filteredData = List<getFilteredListData>;//JsonConvert.DeserializeObject<List<FilteredListData>>(filteredDataJson);
            var filteredData = getFilteredListData.FilteredListDataDetails;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Location", typeof(string));
            dataTable.Columns.Add("UserName", typeof(string));
            dataTable.Columns.Add("EmployeeCode", typeof(string));
            dataTable.Columns.Add("CardNo", typeof(string));
            dataTable.Columns.Add("UserID", typeof(string));
            dataTable.Columns.Add("Date", typeof(string));
            dataTable.Columns.Add("Time", typeof(string));
            dataTable.Columns.Add("MealType", typeof(string));
            dataTable.Columns.Add("SubType", typeof(string));
            dataTable.Columns.Add("Gender", typeof(string));
            dataTable.Columns.Add("Designation", typeof(string));
            dataTable.Columns.Add("Department", typeof(string));
            dataTable.Columns.Add("AssignedShift", typeof(string));
            dataTable.Columns.Add("TeamName", typeof(string));
            dataTable.Columns.Add("Level", typeof(string));
            dataTable.Columns.Add("ReportingToUserName", typeof(string));
            dataTable.Columns.Add("ReportingToEmpCode", typeof(string));
            dataTable.Columns.Add("TeamLeaderName", typeof(string));
            dataTable.Columns.Add("ManagerName", typeof(string));
            dataTable.Columns.Add("DirectorName", typeof(string));

            foreach (var item in filteredData)
            {
                var row = dataTable.NewRow();
                row["Location"] = item.Location;
                row["UserName"] = item.UserName;
                row["EmployeeCode"] = item.EmployeeCode;
                row["CardNo"] = item.CardNo;
                row["UserID"] = item.UserID;
                row["Date"] = item.Date;
                row["Time"] = item.Time;
                row["MealType"] = item.MealType;
                row["SubType"] = item.SubType;
                row["Gender"] = item.Gender;
                row["Designation"] = item.Designation;
                row["Level"] = item.Level;
                row["Department"] = item.Department;
                row["AssignedShift"] = item.AssignedShift;
                row["TeamName"] = item.TeamName;
                row["ReportingToUserName"] = item.ReportingToUserName;
                row["ReportingToEmpCode"] = item.ReportingToEmpCode;
                row["TeamLeaderName"] = item.TeamLeaderName;
                row["ManagerName"] = item.ManagerName;
                row["DirectorName"] = item.DirectorName;
                dataTable.Rows.Add(row);
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dataTable);
            //DataSet ds = (DataSet)[filteredData];
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables.Count > 1)
                {
                    string ParamValues = Session["ParamValues_" + SessionIdentifier].ToString(); ;
                    ParamValues = ParamValues.Replace("<b>", "");
                    ParamValues = ParamValues.Replace("</b>", "");
                    DataSet ds1 = new DataSet();
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        DataTable dt = ds.Tables[i].Clone();
                        if (ds.Tables[i].Rows.Count > 0)
                        {
                            if (!dt.Columns.Contains("Style") && !dt.Columns.Contains("Grouping"))
                            {

                                dt = GetDataTable(dt, ReportName, ParamValues, i, ds.Tables[i]);
                            }
                            else
                            {
                                if (dt.Columns.Contains("Grouping"))
                                {
                                    dt.Columns["Grouping"].SetOrdinal(dt.Columns.Count - 1);
                                }
                                if (dt.Columns.Contains("Style"))
                                {
                                    dt.Columns["Style"].SetOrdinal(dt.Columns.Count - 1);
                                }
                                dt = GetDataTable(dt, ReportName, ParamValues, i, ds.Tables[i]);
                            }
                            ds1.Tables.Add(dt);
                        }
                    }
                    GCC_Canteen.Models.Report.ReportBL.CreateExcelDocumentAsStream(ds1, ReportName + ".xlsx", Response);
                }
                else
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null /*&& dt.Rows.Count > 0*/)
                    {
                        int colcount = 0;
                        if (dt.Columns.Contains("TableName"))
                        {
                            if (dt.Columns.Contains("TableSnapshot"))
                            {
                                if (dt.Columns.Contains("Style"))
                                {
                                    if (dt.Columns.Contains("Grouping"))
                                    {
                                        colcount = dt.Columns.Count - 4;
                                    }
                                    else
                                    {
                                        colcount = dt.Columns.Count - 3;
                                    }
                                }
                                else
                                {
                                    if (dt.Columns.Contains("Grouping"))
                                    {
                                        colcount = dt.Columns.Count - 3;
                                    }
                                    else
                                    {
                                        colcount = dt.Columns.Count - 2;
                                    }
                                }
                            }
                            else
                            {
                                if (dt.Columns.Contains("Style"))
                                {
                                    if (dt.Columns.Contains("Grouping"))
                                    {
                                        colcount = dt.Columns.Count - 3;
                                    }
                                    else
                                    {
                                        colcount = dt.Columns.Count - 2;
                                    }
                                }
                                else
                                {
                                    if (dt.Columns.Contains("Grouping"))
                                    {
                                        colcount = dt.Columns.Count - 2;
                                    }
                                    else
                                    {
                                        colcount = dt.Columns.Count - 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (dt.Columns.Contains("TableSnapshot"))
                            {
                                if (dt.Columns.Contains("Style"))
                                {
                                    if (dt.Columns.Contains("Grouping"))
                                    {
                                        colcount = dt.Columns.Count - 3;
                                    }
                                    else
                                    {
                                        colcount = dt.Columns.Count - 2;
                                    }
                                }
                                else
                                {
                                    if (dt.Columns.Contains("Grouping"))
                                    {
                                        colcount = dt.Columns.Count - 2;
                                    }
                                    else
                                    {
                                        colcount = dt.Columns.Count - 1;
                                    }
                                }
                            }
                            else
                            {
                                if (dt.Columns.Contains("Style"))
                                {
                                    if (dt.Columns.Contains("Grouping"))
                                    {
                                        colcount = dt.Columns.Count - 2;
                                    }
                                    else
                                    {
                                        colcount = dt.Columns.Count - 1;
                                    }
                                }
                                else
                                {
                                    if (dt.Columns.Contains("Grouping"))
                                    {
                                        colcount = dt.Columns.Count - 1;
                                    }
                                    else
                                    {
                                        colcount = dt.Columns.Count;
                                    }
                                }
                            }
                        }
                        //UserInfo userinfo = Session["UserInfo"] as UserInfo;
                        var configPath = CommonRepository.GetSysConfig("5");
                        string path = configPath[0].Value + "\\Report\\";
                        string file = ReportName; //.xls // Changed by Aamir - 06/03/2024
                                                  // Construct the report name
                        ViewBag.MealTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(1, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, 0, "", "");
                        ViewBag.MealSubTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(2, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, 0, "", "");

                        string headerReportName = "Report Name: Consolidated, From Date: " + objDashboard.FromDate + " To Date: " + objDashboard.ToDate;

                        headerReportName += !string.IsNullOrEmpty(objDashboard.Employee) ? ", Employee Code: " + objDashboard.Employee : "";
                        headerReportName += !string.IsNullOrEmpty(objDashboard.selectedMealTypeName) ? ", Meal Type: " + objDashboard.selectedMealTypeName : "";
                        headerReportName += !string.IsNullOrEmpty(objDashboard.selectedMealSubTypeName) ? ", Meal Sub Type: " + objDashboard.selectedMealSubTypeName : "";
                        Console.WriteLine(headerReportName);
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<html>");
                        sb.Append("<head>");
                        sb.Append("</head>");
                        sb.Append("<body>");

                        sb.Append("<table border='1' style='border:1px solid #4E5E6A;width:100%;' align='center'>"); 
                        sb.Append("<tr><td style='text-align:left;' colspan='" + colcount/*dt.Columns.Count*/.ToString() + "'>" + headerReportName + "</td></tr>");
                        //sb.Append("<tr><td style='text-align:left;' colspan='" + colcount/*dt.Columns.Count*/.ToString() + "'>" + Session["ParamValues_" + SessionIdentifier].ToString() + "</td></tr>");
                        if (dt.Columns.Contains("TableName"))
                        {
                            dt.Columns.Remove("TableName");
                            if (dt.Columns.Contains("TableSnapshot"))
                            {
                                sb.Append("<tr><td style='text-align:left;' colspan='" + colcount/*dt.Columns.Count*/.ToString() + "'>" + dt.Rows[0]["TableSnapshot"] + "</td></tr>");
                                dt.Columns.Remove("TableSnapshot");
                            }
                        }
                        if (dt.Columns.Contains("TableSnapshot"))
                        {
                            sb.Append("<tr><td style='text-align:left;' colspan='" + colcount/*dt.Columns.Count*/.ToString() + "'>" + dt.Rows[0]["TableSnapshot"] + "</td></tr>");
                            dt.Columns.Remove("TableSnapshot");
                        }
                        sb.Append("<tr>");
                        string cols = "";
                        string[] col = new string[dt.Columns.Count];
                        for (int i = (dt.Columns.Contains("Style") == true ? ((dt.Columns.Contains("Grouping") == true) ? 2 : 1) : ((dt.Columns.Contains("Grouping") == true) ? 1 : 0))
                            ; i < dt.Columns.Count; i++)
                        {

                            cols = cols == "" ? dt.Columns[i].ColumnName : cols
                                + "§" + dt.Columns[i].ColumnName;
                        }
                        col = cols.Split('§');

                        for (int i = (col.Contains("Style") == true ? ((col.Contains("Grouping") == true) ? 2 : 1) : ((col.Contains("Grouping") == true) ? 1 : 0)); i < col.Count(); i++)
                        {
                            sb.Append("<th>" + col[i] + "</th>");
                        }
                        sb.Append("</tr>");
                        if (dt.Rows.Count == 0)
                        {
                            sb.Append("<tr>");
                            sb.Append("<th colspan=" + colcount/*dt.Columns.Count*/.ToString() + "><label>No Records Found!!</label></th>");
                            sb.Append("</tr>");
                        }
                        else
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (dt.Columns.Contains("Style"))
                                {
                                    sb.Append("<tr style='" + dt.Rows[i]["Style"].ToString() + "'>");
                                }
                                else
                                {
                                    sb.Append("<tr>");
                                }
                                if (dt.Columns.Contains("Grouping") && dt.Rows[i]["Grouping"].ToString() != null && dt.Rows[i]["Grouping"].ToString() != "")
                                {
                                    sb.Append("<td colspan='" + colcount/*dt.Columns.Count*/.ToString() + "'>" + MvcHtmlString.Create(dt.Rows[i]["Grouping"].ToString()) + "</td>");
                                }
                                else
                                {
                                    for (int j = (dt.Columns.Contains("Style") == true ? ((dt.Columns.Contains("Grouping") == true) ? 2 : 1) : ((dt.Columns.Contains("Grouping") == true) ? 1 : 0)); j < dt.Columns.Count; j++)
                                    {
                                        if (dt.Rows[i][j].ToString().Contains("</td>"))
                                        {
                                            sb.Append(MvcHtmlString.Create(dt.Rows[i][j].ToString()));
                                        }
                                        else
                                        {
                                            sb.Append("<td>" + MvcHtmlString.Create(dt.Rows[i][j].ToString()) + "</td>");
                                        }
                                    }
                                }
                                sb.Append("</tr>");
                            }
                        }
                        sb.Append("</table>");
                        sb.Append("</body>");
                        sb.Append("</html>");
                        string Htmltext = sb.ToString();

                        System.IO.File.WriteAllText(path + file, Htmltext);
                        Response.ClearContent();
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                        Response.AddHeader("content-disposition", "attachment; filename=" + file);
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.BinaryWrite(System.IO.File.ReadAllBytes(path + file));
                        Response.Flush();
                        Response.End();
                    }
                }
            }

        }
        [HttpGet]
        public void Export5(string fromDate, string toDate, string employee, int selectedMealType, int selectedMealSubType, string LoginID)
        {
            // Example: Retrieve data from repository based on objDashboard parameters
            var getFilteredListData = DashboardRepository.GetDashboard(
                Convert.ToDateTime(fromDate),
                Convert.ToDateTime(toDate),
                employee,
                selectedMealType,
                selectedMealSubType,
                LoginID
            );
            Console.WriteLine("GetData : - " + getFilteredListData.FilteredListDataDetails);
            //CreateExcelFile.CreateExcelDocument(getFilteredListData.FilteredListDataDetails, "sample.xlsx", Response);

        }
        public ActionResult Show1(Dashboard objDashboard)
        {
            if (Request.Headers["SessionIdentifier"] == null)
                SessionIdentifier = DateTime.UtcNow.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_');
            else
                SessionIdentifier = Request.Headers["SessionIdentifier"].ToString();
            TempData["SessionIdentifier"] = SessionIdentifier;
            DashboardViewModel objDashboardViewModel = new DashboardViewModel();
            UserInfo userInfo = Session["UserInfo"] as UserInfo;
            //ViewBag.RoleID = userInfo.RoleID;
            ViewBag.MealTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(1, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, 0, "", "");
            ViewBag.MealSubTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(2, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, 0, "", "");

            // Handle the selected employees
            var selectedEmployees = objDashboard.SelectedEmployees;
            var getFilteredListData = DashboardRepository.GetDashboard(Convert.ToDateTime(objDashboard.FromDate), Convert.ToDateTime(objDashboard.ToDate), objDashboard.Employee, objDashboard.MealTypeID, objDashboard.MealSubTypeID, objDashboard.LoginID);
            objDashboardViewModel.SessionIdentifier = SessionIdentifier;
            string sessionIdentifier = Guid.NewGuid().ToString();
            //Session["FilteredData_" + sessionIdentifier] = getFilteredListData;
            //TempData["FilteredData"] = getFilteredListData.FilteredListDataDetails;
            TempData["FilteredData"] = JsonConvert.SerializeObject(getFilteredListData.FilteredListDataDetails);
            return RedirectToAction("Export", "Dashboard");
            //return Json(getFilteredListData, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Export4()
        //{
        //    try
        //    {
        //        var filteredDataJson = TempData["FilteredData"] as string;
        //        if (string.IsNullOrEmpty(filteredDataJson))
        //        {
        //            ViewBag.ErrorMessage = "No data available to export.";
        //            return View("ErrorView");
        //        }

        //        // Assuming filteredDataJson is stored in TempData and contains the filtered data
        //        //var filteredDataJson = TempData["FilteredData"] as string;
        //        var filteredData = JsonConvert.DeserializeObject<List<FilteredListData>>(filteredDataJson);

        //        // Create and populate the DataTable
        //        DataTable dataTable = new DataTable();
        //        dataTable.Columns.Add("Location", typeof(string));
        //        dataTable.Columns.Add("UserName", typeof(string));
        //        dataTable.Columns.Add("EmployeeCode", typeof(string));
        //        dataTable.Columns.Add("CardNo", typeof(string));
        //        dataTable.Columns.Add("UserID", typeof(string));
        //        dataTable.Columns.Add("Date", typeof(string));
        //        dataTable.Columns.Add("Time", typeof(string));
        //        dataTable.Columns.Add("MealType", typeof(string));
        //        dataTable.Columns.Add("SubType", typeof(string));
        //        dataTable.Columns.Add("Gender", typeof(string));
        //        dataTable.Columns.Add("Designation", typeof(string));
        //        dataTable.Columns.Add("Department", typeof(string));
        //        dataTable.Columns.Add("AssignedShift", typeof(string));
        //        dataTable.Columns.Add("TeamName", typeof(string));
        //        dataTable.Columns.Add("Level", typeof(string));
        //        dataTable.Columns.Add("ReportingToUserName", typeof(string));
        //        dataTable.Columns.Add("ReportingToEmpCode", typeof(string));
        //        dataTable.Columns.Add("TeamLeaderName", typeof(string));
        //        dataTable.Columns.Add("ManagerName", typeof(string));
        //        dataTable.Columns.Add("DirectorName", typeof(string));

        //        foreach (var item in filteredData)
        //        {
        //            var row = dataTable.NewRow();
        //            row["Location"] = item.Location;
        //            row["UserName"] = item.UserName;
        //            row["EmployeeCode"] = item.EmployeeCode;
        //            row["CardNo"] = item.CardNo;
        //            row["UserID"] = item.UserID;
        //            row["Date"] = item.Date;
        //            row["Time"] = item.Time;
        //            row["MealType"] = item.MealType;
        //            row["SubType"] = item.SubType;
        //            row["Gender"] = item.Gender;
        //            row["Designation"] = item.Designation;
        //            row["Department"] = item.Department;
        //            row["AssignedShift"] = item.AssignedShift;
        //            row["TeamName"] = item.TeamName;
        //            row["Level"] = item.Level;
        //            row["ReportingToUserName"] = item.ReportingToUserName;
        //            row["ReportingToEmpCode"] = item.ReportingToEmpCode;
        //            row["TeamLeaderName"] = item.TeamLeaderName;
        //            row["ManagerName"] = item.ManagerName;
        //            row["DirectorName"] = item.DirectorName;
        //            dataTable.Rows.Add(row);
        //        }
        //        // Set the license context for EPPlus
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //        // Export to Excel using EPPlus
        //        using (var package = new ExcelPackage())
        //        {
        //            var worksheet = package.Workbook.Worksheets.Add("Dashboard Data");

        //            // Load data from DataTable to worksheet
        //            worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);

        //            var stream = new MemoryStream();
        //            package.SaveAs(stream);
        //            stream.Position = 0;

        //            string excelName = $"DashboardData-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";

        //            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception using a logging framework
        //        ViewBag.ErrorMessage = "An error occurred while exporting data.";
        //        return View("ErrorView");
        //    }
        //}
        //public void Export3()
        //{
        //    try
        //    {
        //        string newFilename = "Canteen_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".xlsx";

        //        // Sanitize the file name
        //        newFilename = newFilename.Replace(",", "&").Replace("/", "").Replace("\\", "")
        //                                  .Replace(":", "").Replace("*", "").Replace("?", "")
        //                                  .Replace("\"", "").Replace("<", "").Replace(">", "")
        //                                  .Replace("|", "");

        //        // Assuming filteredDataJson is stored in TempData and contains the filtered data
        //        var filteredDataJson = TempData["FilteredData"] as string;
        //        var filteredData = JsonConvert.DeserializeObject<List<FilteredListData>>(filteredDataJson);

        //        DataTable dataTable = new DataTable();
        //        // Define your DataTable columns here
        //        dataTable.Columns.Add("Location", typeof(string));
        //        dataTable.Columns.Add("UserName", typeof(string));
        //        dataTable.Columns.Add("EmployeeCode", typeof(string));
        //        dataTable.Columns.Add("CardNo", typeof(string));
        //        dataTable.Columns.Add("UserID", typeof(string));
        //        dataTable.Columns.Add("Date", typeof(string));
        //        dataTable.Columns.Add("Time", typeof(string));
        //        dataTable.Columns.Add("MealType", typeof(string));
        //        dataTable.Columns.Add("SubType", typeof(string));
        //        dataTable.Columns.Add("Gender", typeof(string));
        //        dataTable.Columns.Add("Designation", typeof(string));
        //        dataTable.Columns.Add("Department", typeof(string));
        //        dataTable.Columns.Add("AssignedShift", typeof(string));
        //        dataTable.Columns.Add("TeamName", typeof(string));
        //        dataTable.Columns.Add("Level", typeof(string));
        //        dataTable.Columns.Add("ReportingToUserName", typeof(string));
        //        dataTable.Columns.Add("ReportingToEmpCode", typeof(string));
        //        dataTable.Columns.Add("TeamLeaderName", typeof(string));
        //        dataTable.Columns.Add("ManagerName", typeof(string));
        //        dataTable.Columns.Add("DirectorName", typeof(string));

        //        foreach (var item in filteredData)
        //        {
        //            var row = dataTable.NewRow();
        //            row["Location"] = item.Location;
        //            row["UserName"] = item.UserName;
        //            row["EmployeeCode"] = item.EmployeeCode;
        //            row["CardNo"] = item.CardNo;
        //            row["UserID"] = item.UserID;
        //            row["Date"] = item.Date;
        //            row["Time"] = item.Time;
        //            row["MealType"] = item.MealType;
        //            row["SubType"] = item.SubType;
        //            row["Gender"] = item.Gender;
        //            row["Designation"] = item.Designation;
        //            row["Level"] = item.Level;
        //            row["Department"] = item.Department;
        //            row["AssignedShift"] = item.AssignedShift;
        //            row["TeamName"] = item.TeamName;
        //            row["ReportingToUserName"] = item.ReportingToUserName;
        //            row["ReportingToEmpCode"] = item.ReportingToEmpCode;
        //            row["TeamLeaderName"] = item.TeamLeaderName;
        //            row["ManagerName"] = item.ManagerName;
        //            row["DirectorName"] = item.DirectorName;
        //            dataTable.Rows.Add(row);
        //        }

        //        DataSet ds = new DataSet();
        //        ds.Tables.Add(dataTable);
        //        // Save the DataTable as an Excel file
        //        using (XLWorkbook workbook = new XLWorkbook())
        //        {
        //            foreach (DataTable dt in ds.Tables)
        //            {
        //                workbook.Worksheets.Add(dt, dt.TableName);
        //            }

        //            using (var stream = new System.IO.MemoryStream())
        //            {
        //                workbook.SaveAs(stream);
        //                stream.Position = 0;
        //                File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SampleDataSet.xlsx");
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        WriteErrorLog(e.ToString());
        //    }
        //}
        //static void SaveDataSetToExcel(DataSet ds, string filePath)
        //{
        //    using (XLWorkbook workbook = new XLWorkbook())
        //    {
        //        // Add each DataTable in the DataSet as a worksheet
        //        foreach (DataTable dt in ds.Tables)
        //        {
        //            workbook.Worksheets.Add(dt, dt.TableName);
        //        }

        //        // Save the workbook to the specified path
        //        workbook.SaveAs(filePath);
        //    }
        //}
        public void Export(Dashboard objDashboard)
        {
            string newFilename = "Canteen_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".xlsx";

            //var filteredDataJson = TempData["FilteredData"] as string;
            //var filteredData = JsonConvert.DeserializeObject<List<FilteredListData>>(filteredDataJson);
            var getFilteredListData = DashboardRepository.GetDashboard(
               Convert.ToDateTime(objDashboard.FromDate),
               Convert.ToDateTime(objDashboard.ToDate),
               objDashboard.Employee,
               objDashboard.MealTypeID,
               objDashboard.MealSubTypeID,
               objDashboard.LoginID
           );
            var FilterDtl = new
            {
                FromDate = objDashboard.FromDate,
                ToDate = objDashboard.ToDate,
                Employee = objDashboard.Employee,
                MealType = objDashboard.selectedMealTypeName,
                MealSubType = objDashboard.selectedMealSubTypeName,
                LoginID = objDashboard.LoginID
            };
            //            var FilterDtl = [
            //    "From Date: " + objDashboard.FromDate,
            //    "To Date: " + objDashboard.ToDate,
            //    objDashboard.Employee ? "Employee Code: " + objDashboard.Employee : "",
            //    objDashboard.selectedMealTypeName ? "Meal Type: " + objDashboard.selectedMealTypeName : "",
            //    objDashboard.selectedMealSubTypeName ? "Meal Sub Type: " + objDashboard.selectedMealSubTypeName : ""
            //];
            //string sessionIdentifier = Request.Headers["SessionIdentifier"];
            //if (string.IsNullOrEmpty(sessionIdentifier))
            //{
            //    sessionIdentifier = objDashboard.FromDate + '&' + objDashboard.ToDate + '&' + objDashboard.Employee + '&' 
            //        + objDashboard.selectedMealTypeName + '&' + objDashboard.selectedMealSubTypeName + '&' + objDashboard.LoginID;
            //}
            //TempData["SessionIdentifier"] = sessionIdentifier;
            CreateExcelFile.CreateExcelDocument(getFilteredListData.FilteredListDataDetails, newFilename, Response, FilterDtl);
            //try
            //{
            //    UserInfo userinfo = Session["UserInfo"] as UserInfo;
            //    string newFilename = "Canteen_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".xlsx";

            //    var filteredDataJson = TempData["FilteredData"] as string;
            //    var filteredData = JsonConvert.DeserializeObject<List<FilteredListData>>(filteredDataJson);

            //    DataTable dataTable = new DataTable();
            //    dataTable.Columns.Add("Location", typeof(string));
            //    dataTable.Columns.Add("UserName", typeof(string));
            //    dataTable.Columns.Add("EmployeeCode", typeof(string));
            //    dataTable.Columns.Add("CardNo", typeof(string));
            //    dataTable.Columns.Add("UserID", typeof(string));
            //    dataTable.Columns.Add("Date", typeof(string));
            //    dataTable.Columns.Add("Time", typeof(string));
            //    dataTable.Columns.Add("MealType", typeof(string));
            //    dataTable.Columns.Add("SubType", typeof(string));
            //    dataTable.Columns.Add("Gender", typeof(string));
            //    dataTable.Columns.Add("Designation", typeof(string));
            //    dataTable.Columns.Add("Department", typeof(string));
            //    dataTable.Columns.Add("AssignedShift", typeof(string));
            //    dataTable.Columns.Add("TeamName", typeof(string));
            //    dataTable.Columns.Add("Level", typeof(string));
            //    dataTable.Columns.Add("ReportingToUserName", typeof(string));
            //    dataTable.Columns.Add("ReportingToEmpCode", typeof(string));
            //    dataTable.Columns.Add("TeamLeaderName", typeof(string));
            //    dataTable.Columns.Add("ManagerName", typeof(string));
            //    dataTable.Columns.Add("DirectorName", typeof(string));

            //    foreach (var item in filteredData)
            //    {
            //        var row = dataTable.NewRow();
            //        row["Location"] = item.Location;
            //        row["UserName"] = item.UserName;
            //        row["EmployeeCode"] = item.EmployeeCode;
            //        row["CardNo"] = item.CardNo;
            //        row["UserID"] = item.UserID;
            //        row["Date"] = item.Date;
            //        row["Time"] = item.Time;
            //        row["MealType"] = item.MealType;
            //        row["SubType"] = item.SubType;
            //        row["Gender"] = item.Gender;
            //        row["Designation"] = item.Designation;
            //        row["Level"] = item.Level;
            //        row["Department"] = item.Department;
            //        row["AssignedShift"] = item.AssignedShift;
            //        row["TeamName"] = item.TeamName;
            //        row["ReportingToUserName"] = item.ReportingToUserName;
            //        row["ReportingToEmpCode"] = item.ReportingToEmpCode;
            //        row["TeamLeaderName"] = item.TeamLeaderName;
            //        row["ManagerName"] = item.ManagerName;
            //        row["DirectorName"] = item.DirectorName;
            //        dataTable.Rows.Add(row);
            //    }

            //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //    using (ExcelPackage package = new ExcelPackage())
            //    {
            //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

            //        var sessionIdentifierObject = TempData["SessionIdentifier"];
            //        string sessionIdentifier = sessionIdentifierObject as string;
            //        var dateParts = sessionIdentifier.Split('&');
            //        string reportName = "Report Name: Consolidated, From Date: " + dateParts[0] + " To Date: " + dateParts[0];

            //        worksheet.Cells[1, 1].Value = reportName;
            //        worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Merge = true;
            //        worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //        worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Style.Font.Bold = true;

            //        for (int i = 0; i < dataTable.Columns.Count; i++)
            //        {
            //            worksheet.Cells[2, i + 1].Value = dataTable.Columns[i].ColumnName;
            //            worksheet.Cells[2, i + 1].Style.Font.Bold = true;
            //        }

            //        int rowIndex = 3;
            //        foreach (DataRow row in dataTable.Rows)
            //        {
            //            for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
            //            {
            //                worksheet.Cells[rowIndex, colIndex + 1].Value = row[colIndex].ToString();
            //                worksheet.Cells[rowIndex, colIndex + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //            }
            //            rowIndex++;
            //        }

            //        using (var memoryStream = new MemoryStream())
            //        {
            //            package.SaveAs(memoryStream);
            //            memoryStream.Position = 0;

            //            Response.Clear();
            //            Response.Buffer = true;
            //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //            Response.AddHeader("Content-Disposition", "attachment; filename=" + newFilename);
            //            Response.BinaryWrite(memoryStream.ToArray());
            //            Response.End();
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    WriteErrorLog(e.ToString());
            //}
        }
        //[HttpGet]
        //public void Export1()
        //{
        //    try
        //    {
        //        string newFilename = "Canteen_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".xlsx";

        //        // Sanitize the file name
        //        newFilename = newFilename.Replace(",", "&").Replace("/", "").Replace("\\", "")
        //                                  .Replace(":", "").Replace("*", "").Replace("?", "")
        //                                  .Replace("\"", "").Replace("<", "").Replace(">", "")
        //                                  .Replace("|", "");

        //        // Assuming filteredDataJson is stored in TempData and contains the filtered data
        //        var filteredDataJson = TempData["FilteredData"] as string;
        //        var filteredData = JsonConvert.DeserializeObject<List<FilteredListData>>(filteredDataJson);

        //        DataTable dataTable = new DataTable();
        //        // Define your DataTable columns here
        //        dataTable.Columns.Add("Location", typeof(string));
        //        dataTable.Columns.Add("UserName", typeof(string));
        //        dataTable.Columns.Add("EmployeeCode", typeof(string));
        //        dataTable.Columns.Add("CardNo", typeof(string));
        //        dataTable.Columns.Add("UserID", typeof(string));
        //        dataTable.Columns.Add("Date", typeof(string));
        //        dataTable.Columns.Add("Time", typeof(string));
        //        dataTable.Columns.Add("MealType", typeof(string));
        //        dataTable.Columns.Add("SubType", typeof(string));
        //        dataTable.Columns.Add("Gender", typeof(string));
        //        dataTable.Columns.Add("Designation", typeof(string));
        //        dataTable.Columns.Add("Department", typeof(string));
        //        dataTable.Columns.Add("AssignedShift", typeof(string));
        //        dataTable.Columns.Add("TeamName", typeof(string));
        //        dataTable.Columns.Add("Level", typeof(string));
        //        dataTable.Columns.Add("ReportingToUserName", typeof(string));
        //        dataTable.Columns.Add("ReportingToEmpCode", typeof(string));
        //        dataTable.Columns.Add("TeamLeaderName", typeof(string));
        //        dataTable.Columns.Add("ManagerName", typeof(string));
        //        dataTable.Columns.Add("DirectorName", typeof(string));

        //        foreach (var item in filteredData)
        //        {
        //            var row = dataTable.NewRow();
        //            row["Location"] = item.Location;
        //            row["UserName"] = item.UserName;
        //            row["EmployeeCode"] = item.EmployeeCode;
        //            row["CardNo"] = item.CardNo;
        //            row["UserID"] = item.UserID;
        //            row["Date"] = item.Date;
        //            row["Time"] = item.Time;
        //            row["MealType"] = item.MealType;
        //            row["SubType"] = item.SubType;
        //            row["Gender"] = item.Gender;
        //            row["Designation"] = item.Designation;
        //            row["Level"] = item.Level;
        //            row["Department"] = item.Department;
        //            row["AssignedShift"] = item.AssignedShift;
        //            row["TeamName"] = item.TeamName;
        //            row["ReportingToUserName"] = item.ReportingToUserName;
        //            row["ReportingToEmpCode"] = item.ReportingToEmpCode;
        //            row["TeamLeaderName"] = item.TeamLeaderName;
        //            row["ManagerName"] = item.ManagerName;
        //            row["DirectorName"] = item.DirectorName;
        //            dataTable.Rows.Add(row);
        //        }

        //        DataSet ds = new DataSet();
        //        ds.Tables.Add(dataTable);

        //        // Create the Excel application instance
        //        var xlApp = new Excel.Application();
        //        xlApp.DisplayAlerts = false; // Suppress any alerts from Excel
        //        var xlWorkBook = xlApp.Workbooks.Add();
        //        var xlWorkSheet = (Excel.Worksheet)xlWorkBook.Sheets[1];
        //        var sessionIdentifierObject = TempData["SessionIdentifier"];
        //        string sessionIdentifier = sessionIdentifierObject as string;
        //        var dateParts = sessionIdentifier.Split('&');
        //        // Add report name and date range in the first row
        //        string reportName = "Report Name: Consolidated, From Date: " + dateParts[0] + " To Date: " + dateParts[0];
        //        Excel.Range reportHeader = xlWorkSheet.Range[xlWorkSheet.Cells[1, 1], xlWorkSheet.Cells[1, dataTable.Columns.Count]];
        //        reportHeader.Merge();
        //        reportHeader.Value2 = reportName;
        //        reportHeader.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft; // Align left
        //        reportHeader.Font.Bold = true;

        //        // Make the header row bold and set column width
        //        for (int i = 0; i < dataTable.Columns.Count; i++)
        //        {
        //            xlWorkSheet.Cells[2, i + 1] = dataTable.Columns[i].ColumnName;
        //            xlWorkSheet.Cells[2, i + 1].Font.Bold = true;
        //        }

        //        // Populate data and set alignment
        //        int numberOfColumns = ds.Tables[0].Columns.Count;
        //        int numberOfRows = ds.Tables[0].Rows.Count;
        //        int lastUsedRow = 3; // Start from the third row for data
        //        foreach (DataRow r in ds.Tables[0].Rows)
        //        {
        //            for (int i = 0; i < numberOfColumns; i++)
        //            {
        //                var cell = (Excel.Range)xlWorkSheet.Cells[lastUsedRow, i + 1];
        //                cell.Value2 = r.ItemArray[i].ToString();
        //                cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft; // Left align horizontally
        //                cell.EntireColumn.ColumnWidth = 15; // Set width for all columns
        //            }
        //            lastUsedRow++;
        //        }

        //        // Define the full range of cells to apply the border, including empty cells if necessary
        //        Excel.Range fullRange = xlWorkSheet.Range[
        //            xlWorkSheet.Cells[1, 1],
        //            xlWorkSheet.Cells[numberOfRows + 2, numberOfColumns]
        //        ];

        //        // Apply thin border to all cells in the range
        //        Excel.Borders borders = fullRange.Borders;
        //        borders.LineStyle = Excel.XlLineStyle.xlContinuous;
        //        borders.Weight = Excel.XlBorderWeight.xlThin;

        //        // Save the workbook to a memory stream
        //        var memoryStream = new MemoryStream();
        //        xlWorkBook.SaveAs(memoryStream, Excel.XlFileFormat.xlOpenXMLWorkbook); // Save as .xlsx
        //        xlWorkBook.Close(false, Type.Missing, Type.Missing);
        //        xlApp.Quit();

        //        ReleaseObject(xlWorkSheet);
        //        ReleaseObject(xlWorkBook);
        //        ReleaseObject(xlApp);

        //        // Send the file to the browser
        //        Response.Clear();
        //        Response.Buffer = true;
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // Change to .xlsx MIME type
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + newFilename);
        //        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //        Response.BinaryWrite(memoryStream.ToArray());
        //        Response.End();
        //    }
        //    catch (Exception e)
        //    {
        //        WriteErrorLog(e.ToString());
        //    }
        //}

        [HttpGet]
        public void Export0()
        {
            try
            {
                UserInfo userinfo = Session["UserInfo"] as UserInfo;
                string newFilename = "Canteen_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ".xlsx"; // Change to .xlsx

                string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
                string targetPath = Path.Combine(directoryPath, newFilename);

                // Ensure the directory exists
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                WriteErrorLog("Directory Path : - "+directoryPath);
                // Sanitize the file name
                newFilename = newFilename.Replace(",", "&").Replace("/", "").Replace("\\", "")
                                          .Replace(":", "").Replace("*", "").Replace("?", "")
                                          .Replace("\"", "").Replace("<", "").Replace(">", "")
                                          .Replace("|", "");

                // Create the full path again with sanitized file name
                targetPath = Path.Combine(directoryPath, newFilename);

                Session["PageID"] = (int)CommonBase.Page.MainDashboard;
                var filteredDataJson = TempData["FilteredData"] as string;
                var filteredData = JsonConvert.DeserializeObject<List<FilteredListData>>(filteredDataJson);

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Location", typeof(string));
                dataTable.Columns.Add("UserName", typeof(string));
                dataTable.Columns.Add("EmployeeCode", typeof(string));
                dataTable.Columns.Add("CardNo", typeof(string));
                dataTable.Columns.Add("UserID", typeof(string));
                dataTable.Columns.Add("Date", typeof(string));
                dataTable.Columns.Add("Time", typeof(string));
                dataTable.Columns.Add("MealType", typeof(string));
                dataTable.Columns.Add("SubType", typeof(string));
                dataTable.Columns.Add("Gender", typeof(string));
                dataTable.Columns.Add("Designation", typeof(string));
                dataTable.Columns.Add("Department", typeof(string));
                dataTable.Columns.Add("AssignedShift", typeof(string));
                dataTable.Columns.Add("TeamName", typeof(string));
                dataTable.Columns.Add("Level", typeof(string));
                dataTable.Columns.Add("ReportingToUserName", typeof(string));
                dataTable.Columns.Add("ReportingToEmpCode", typeof(string));
                dataTable.Columns.Add("TeamLeaderName", typeof(string));
                dataTable.Columns.Add("ManagerName", typeof(string));
                dataTable.Columns.Add("DirectorName", typeof(string));

                foreach (var item in filteredData)
                {
                    var row = dataTable.NewRow();
                    row["Location"] = item.Location;
                    row["UserName"] = item.UserName;
                    row["EmployeeCode"] = item.EmployeeCode;
                    row["CardNo"] = item.CardNo;
                    row["UserID"] = item.UserID;
                    row["Date"] = item.Date;
                    row["Time"] = item.Time;
                    row["MealType"] = item.MealType;
                    row["SubType"] = item.SubType;
                    row["Gender"] = item.Gender;
                    row["Designation"] = item.Designation;
                    row["Level"] = item.Level;
                    row["Department"] = item.Department;
                    row["AssignedShift"] = item.AssignedShift;
                    row["TeamName"] = item.TeamName;
                    row["ReportingToUserName"] = item.ReportingToUserName;
                    row["ReportingToEmpCode"] = item.ReportingToEmpCode;
                    row["TeamLeaderName"] = item.TeamLeaderName;
                    row["ManagerName"] = item.ManagerName;
                    row["DirectorName"] = item.DirectorName;
                    dataTable.Rows.Add(row);
                }

                DataSet ds = new DataSet();
                ds.Tables.Add(dataTable);

                if (System.IO.File.Exists(targetPath))
                {
                    System.IO.File.Delete(targetPath);
                }

                var xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlApp.DisplayAlerts = false; // Suppress any alerts from Excel
                var xlWorkBook = xlApp.Workbooks.Add();
                var xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Sheets[1];
                var sessionIdentifierObject = TempData["SessionIdentifier"];
                string sessionIdentifier = sessionIdentifierObject as string;
                var dateParts = sessionIdentifier.Split('&');
                // Add report name and date range in the first row
                string reportName = "Report Name: Consolidated, From Date: "+ dateParts[0] +" To Date: " + dateParts[0];
                Microsoft.Office.Interop.Excel.Range reportHeader = xlWorkSheet.Range[xlWorkSheet.Cells[1, 1], xlWorkSheet.Cells[1, dataTable.Columns.Count]];
                reportHeader.Merge();
                reportHeader.Value2 = reportName;
                reportHeader.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft; // Align left
                reportHeader.Font.Bold = true;

                // Make the header row bold and set column width
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    xlWorkSheet.Cells[2, i + 1] = dataTable.Columns[i].ColumnName;
                    xlWorkSheet.Cells[2, i + 1].Font.Bold = true;
                }

                // Populate data and set alignment
                int numberOfColumns = ds.Tables[0].Columns.Count;
                int numberOfRows = ds.Tables[0].Rows.Count;
                int lastUsedRow = 3; // Start from the third row for data
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        var cell = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[lastUsedRow, i + 1];
                        cell.Value2 = r.ItemArray[i].ToString();
                        cell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft; // Left align horizontally
                        cell.EntireColumn.ColumnWidth = 15; // Set width for all columns
                    }
                    lastUsedRow++;
                }

                // Define the full range of cells to apply the border, including empty cells if necessary
                Microsoft.Office.Interop.Excel.Range fullRange = xlWorkSheet.Range[
                    xlWorkSheet.Cells[1, 1],
                    xlWorkSheet.Cells[numberOfRows + 2, numberOfColumns]
                ];

                // Apply thin border to all cells in the range
                Microsoft.Office.Interop.Excel.Borders borders = fullRange.Borders;
                borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
                WriteErrorLog("Target Path : - "+ targetPath);

                xlWorkBook.SaveAs(targetPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook); // Change to .xlsx format
                xlWorkBook.Close(false, Type.Missing, Type.Missing);
                xlApp.Quit();

                ReleaseObject(xlWorkSheet);
                ReleaseObject(xlWorkBook);
                ReleaseObject(xlApp);

                GC.WaitForPendingFinalizers();
                GC.Collect();

                // Send the file to the browser
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // Change to .xlsx MIME type
                Response.AddHeader("Content-Disposition", "attachment; filename=" + newFilename);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(System.IO.File.ReadAllBytes(targetPath));
                Response.End();
            }
            catch (Exception e)
            {
                WriteErrorLog(e.ToString());
            }
        }
        [HttpGet]

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                WriteErrorLog("Unable to release the object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private static string GetExcelColumnName(int columnIndex)
        {
            if (columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString();

            char firstChar = (char)('A' + (columnIndex / 26) - 1);
            char secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}", firstChar, secondChar);
        }
        private static void WriteErrorLog(string v)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(@"C:\GCC_Canteen_App\LogFile.txt", true); //D:\LogFile.txt  //Changed by Aamir - 06/03/2024
                sw.WriteLine(DateTime.Now.ToString() + ": " + v);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
        public ActionResult AutoCompleteList(string KeyValue, int ActionID)
        {
            List<KeyValuePair<string, string>> finalList = new List<KeyValuePair<string, string>>();
            finalList = CommonRepository.AutoCompleteList(KeyValue, ActionID);
            //ViewBag.EmployeeList = finalList.Select(e => new { Value = e.EmployeeID, Text = e.EmployeeName }).ToList();
            return Json(finalList, JsonRequestBehavior.AllowGet);
        }
        //private void getData(int lastRow_,DataTable dt)
        //{

        //    int numberOfColumns = dt.Columns.Count;
        //    double a, b, c, d, total = 0;
        //    //int lastRow_ = 4;

        //    foreach (DataRow r in dt.Rows)
        //    {
        //        for(int i=0;i<= numberOfColumns-1;i++)
        //        {
        //            xlWorkSheet.Cells[lastRow_, i] = r.ItemArray[i].ToString();
        //        }
        //        lastRow_++;
        //    }
        //    total = 0;
        //}
        //DataTable
        public DataTable GetDataTable(DataTable dt, string ReportName, string ParamValues, int i, DataTable dsdt)
        {
            if (dt.Columns[0].ColumnName.ToLower() == "tablename")
            {
                if (dt.Columns[1].ColumnName.ToLower() != "tablesnapshot")
                {
                    DataRow dr = dt.NewRow();
                    dr[1] = ReportName;
                    dt.Rows.Add(dr);
                    DataRow dr1 = dt.NewRow();
                    dr1[1] = ParamValues;
                    dt.Rows.Add(dr1);
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    dr[2] = ReportName;
                    dt.Rows.Add(dr);
                    DataRow dr1 = dt.NewRow();
                    dr1[2] = ParamValues;
                    dt.Rows.Add(dr1);

                    DataRow dr2 = dt.NewRow();
                    dr2[2] = dsdt.Rows[0][1].ToString().Replace("<br/>", " ");
                    dt.Rows.Add(dr2);
                }
            }
            else
            {
                if (dt.Columns[0].ColumnName.ToLower() != "tablesnapshot")
                {

                    DataRow dr = dt.NewRow();
                    dr[0] = ReportName;
                    dt.Rows.Add(dr);
                    DataRow dr1 = dt.NewRow();
                    dr1[0] = ParamValues;
                    dt.Rows.Add(dr1);
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    dr[1] = ReportName;
                    dt.Rows.Add(dr);
                    DataRow dr1 = dt.NewRow();
                    dr1[1] = ParamValues;
                    dt.Rows.Add(dr1);

                    DataRow dr2 = dt.NewRow();
                    dr2[1] = dsdt.Rows[0][0].ToString().Replace("<br/>", " ");
                    dt.Rows.Add(dr2);
                }
            }
            foreach (DataRow r in dsdt.Rows)
                dt.ImportRow(r);

            if (dt.Columns[0].ColumnName.ToLower() == "tablename")
            {
                if (dt.Columns[1].ColumnName.ToLower() == "tablesnapshot")
                {
                    dt.TableName = dt.Rows[3][0].ToString();
                    dt.Columns.RemoveAt(1);
                }
                else
                {
                    dt.TableName = dt.Rows[2][0].ToString();
                }
                dt.Columns.RemoveAt(0);
            }
            else if (dt.Columns[0].ColumnName.ToLower() == "tablesnapshot")
            {
                dt.Columns.RemoveAt(0);
            }
            else
                dt.TableName = "Sheet" + (i + 1).ToString();

            if (dt.Columns[0].ColumnName.ToLower() == "tablesnapshot")
            {
                //dt.TableName = dt.TableName.Contains("Sheet") == true ? "Sheet" + (i + 1).ToString() : dt.Rows[2][0].ToString();
                dt.Columns.RemoveAt(0);
            }
            return dt;
        }

    }
}