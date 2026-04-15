using GCC_Canteen.App_Code;
using GCC_Canteen.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GCC_Canteen.Models.BusinessLayer;
using Newtonsoft.Json;

namespace GCC_Canteen.Controllers
{
    public class ReportController : Controller
    {
        string SessionIdentifier = "";

        // GET: Report
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult Show()
        {
            if (Request.Headers["SessionIdentifier"] == null)
                SessionIdentifier = DateTime.UtcNow.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_');
            else
                SessionIdentifier = Request.Headers["SessionIdentifier"].ToString();
            TempData["SessionIdentifier"] = SessionIdentifier;
            UserInfo userinfo = Session["UserInfo"] as UserInfo;
            ViewBag.ReportTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(4, Convert.ToInt32(CommonBase.Page.Report), 0, 0, "", "");
            ViewBag.EventTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(7, Convert.ToInt32(CommonBase.Page.Report), 0, userinfo.UserID, "", "");
            string etl = "";

            foreach (var eventType in ViewBag.EventTypeList)
            {
                etl += eventType.Text;
            }
            //ViewBag.ReportTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillCombo(4, "", false);
            // ViewBag.StatusList = GCC_Canteen.Models.Repository.SelectListRepository.FillCombo(4, "", false);
            return View();
        }

        [HttpPost]
        public ActionResult Show(ReportViewModel objReportViewModel)
        {
            UserInfo userinfo = Session["UserInfo"] as UserInfo;
            ViewBag.ReportTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(4, Convert.ToInt32(CommonBase.Page.Report), 0, 0, "", "");

            //eventType List

            // ViewBag.ReportTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillCombo(4, "", false);
            //ViewBag.StatusList = GCC_Canteen.Models.Repository.SelectListRepository.FillCombo(4, "", false);
            if (Request.Headers["SessionIdentifier"] == null)
                SessionIdentifier = DateTime.UtcNow.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_');
            else
                SessionIdentifier = Request.Headers["SessionIdentifier"].ToString();
            string ParamValues = "";
            TempData["SessionIdentifier"] = SessionIdentifier;

            GCC_Canteen.Models.Repository.ReportRepository.GenerateReport(objReportViewModel, SessionIdentifier);
            ViewBag.ReportID = objReportViewModel.ReportParameterDetails.ReportID;
            ViewBag.EventID = objReportViewModel.ReportParameterDetails.EventID;

            // ViewBag.StatusList = objReportViewModel.ReportParameterDetails.Status;
            var eventNameList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(7, (int)CommonBase.Page.Report, 0, userinfo.UserID, "", "");

            var selectedEventID = objReportViewModel.ReportParameterDetails.EventID.ToString();

            var selectedEvent = eventNameList.FirstOrDefault(item => item.Value == selectedEventID);

            var etl = selectedEvent != null ? selectedEvent.Text : "";


            var ReportName = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(5, (int)CommonBase.Page.Report, 0, userinfo.UserID, objReportViewModel.ReportParameterDetails.ReportID.ToString(), "");
            string rn = "";
            foreach (var reportname in ReportName)
            {
                rn += reportname.Text;
            }
           
            ParamValues = "<b>Report Name</b>: " + rn + ", <b>From Date</b>: "
                            + objReportViewModel.ReportParameterDetails.ReportFromDate + ", <b>To Date</b>: "
                            + objReportViewModel.ReportParameterDetails.ReportToDate+",<b>Event Type</b>:" + etl;

            Session["ParamValues_" + SessionIdentifier] = ParamValues;
            return PartialView("_GridReport");
        }

        //Export
        //[HttpGet]
        //public void Export(string ReportName,/* string ReportID,*/ string SessionIdentifier)
        //{
        //    ReportName = "FilteredData";
        //    ReportName = ReportName.Replace(',', '&');
        //    ReportName = ReportName.Replace("/", "");
        //    ReportName = ReportName.Replace("\\", "");
        //    ReportName = ReportName.Replace(":", "");
        //    ReportName = ReportName.Replace("*", "");
        //    ReportName = ReportName.Replace("?", "");
        //    ReportName = ReportName.Replace("\"", "");
        //    ReportName = ReportName.Replace("<", "");
        //    ReportName = ReportName.Replace(">", "");
        //    ReportName = ReportName.Replace("|", "");

        //    Session["PageID"] = (int)CommonBase.Page.Report;
        //    //DataSet ds = (DataSet)Session["MISReportData_" + SessionIdentifier];
        //    //var ds = (DataSet)Session["FilteredData_" + SessionIdentifier];
        //    var filteredDataJson = TempData["FilteredData"] as string;
        //    var filteredData = JsonConvert.DeserializeObject<List<FilteredListData>>(filteredDataJson);

        //    DataTable dataTable = new DataTable();
        //    dataTable.Columns.Add("EntryDate", typeof(string));
        //    dataTable.Columns.Add("EmployeeName", typeof(string));
        //    dataTable.Columns.Add("MealType", typeof(string));
        //    dataTable.Columns.Add("MealSubType", typeof(string));

        //    foreach (var item in filteredData)
        //    {
        //        var row = dataTable.NewRow();
        //        row["EntryDate"] = item.EntryDate;
        //        row["EmployeeName"] = item.EmployeeName;
        //        row["MealType"] = item.MealType;
        //        row["MealSubType"] = item.MealSubType;
        //        dataTable.Rows.Add(row);
        //    }

        //    DataSet ds = new DataSet();
        //    ds.Tables.Add(dataTable);
        //    if (ds.Tables.Count > 0)
        //    {
        //        if (ds.Tables.Count > 0)
        //        {
        //            //string ParamValues = Session["ParamValues_" + SessionIdentifier].ToString();
        //            //ParamValues = ParamValues.Replace("<b>", "");
        //            string ParamValues = "filteredData and " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");//ParamValues.Replace("</b>", "");
        //            DataSet ds1 = new DataSet();
        //            for (int i = 0; i < ds.Tables.Count; i++)
        //            {
        //                DataTable dt = ds.Tables[i].Clone();
        //                if (ds.Tables[i].Rows.Count > 0)
        //                {
        //                    if (!dt.Columns.Contains("Style") && !dt.Columns.Contains("Grouping"))
        //                    {

        //                        dt = GetDataTable(dt, ReportName, ParamValues, i, ds.Tables[i]);
        //                    }
        //                    else
        //                    {
        //                        if (dt.Columns.Contains("Grouping"))
        //                        {
        //                            dt.Columns["Grouping"].SetOrdinal(dt.Columns.Count - 1);
        //                        }
        //                        if (dt.Columns.Contains("Style"))
        //                        {
        //                            dt.Columns["Style"].SetOrdinal(dt.Columns.Count - 1);
        //                        }
        //                        dt = GetDataTable(dt, ReportName, ParamValues, i, ds.Tables[i]);
        //                    }
        //                    ds1.Tables.Add(dt);
        //                }
        //            }
        //            GCC_Canteen.Models.Report.ReportBL.CreateExcelDocumentAsStream(ds1, ReportName + ".xlsx", Response);
        //        }
        //        else
        //        {
        //            DataTable dt = ds.Tables[0];
        //            if (dt != null /*&& dt.Rows.Count > 0*/)
        //            {
        //                int colcount = 0;
        //                if (dt.Columns.Contains("TableName"))
        //                {
        //                    if (dt.Columns.Contains("TableSnapshot"))
        //                    {
        //                        if (dt.Columns.Contains("Style"))
        //                        {
        //                            if (dt.Columns.Contains("Grouping"))
        //                            {
        //                                colcount = dt.Columns.Count - 4;
        //                            }
        //                            else
        //                            {
        //                                colcount = dt.Columns.Count - 3;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (dt.Columns.Contains("Grouping"))
        //                            {
        //                                colcount = dt.Columns.Count - 3;
        //                            }
        //                            else
        //                            {
        //                                colcount = dt.Columns.Count - 2;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (dt.Columns.Contains("Style"))
        //                        {
        //                            if (dt.Columns.Contains("Grouping"))
        //                            {
        //                                colcount = dt.Columns.Count - 3;
        //                            }
        //                            else
        //                            {
        //                                colcount = dt.Columns.Count - 2;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (dt.Columns.Contains("Grouping"))
        //                            {
        //                                colcount = dt.Columns.Count - 2;
        //                            }
        //                            else
        //                            {
        //                                colcount = dt.Columns.Count - 1;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (dt.Columns.Contains("TableSnapshot"))
        //                    {
        //                        if (dt.Columns.Contains("Style"))
        //                        {
        //                            if (dt.Columns.Contains("Grouping"))
        //                            {
        //                                colcount = dt.Columns.Count - 3;
        //                            }
        //                            else
        //                            {
        //                                colcount = dt.Columns.Count - 2;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (dt.Columns.Contains("Grouping"))
        //                            {
        //                                colcount = dt.Columns.Count - 2;
        //                            }
        //                            else
        //                            {
        //                                colcount = dt.Columns.Count - 1;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (dt.Columns.Contains("Style"))
        //                        {
        //                            if (dt.Columns.Contains("Grouping"))
        //                            {
        //                                colcount = dt.Columns.Count - 2;
        //                            }
        //                            else
        //                            {
        //                                colcount = dt.Columns.Count - 1;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (dt.Columns.Contains("Grouping"))
        //                            {
        //                                colcount = dt.Columns.Count - 1;
        //                            }
        //                            else
        //                            {
        //                                colcount = dt.Columns.Count;
        //                            }
        //                        }
        //                    }
        //                }
        //                UserInfo userinfo = Session["UserInfo"] as UserInfo;
        //                string path = "D:\\AmnnGCC_Canteen\\MOM_WebApp\\MoodMeter_oldSVN\\Documents" + ReportName + ".xls";//userinfo.AttachConfig[0].FieldValue + "\\Report\\";
        //                string file = ReportName + "_" /*+ ReportID.ToString()*/ + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".xls"; //.xls // Changed by Aamir - 06/03/2024
        //                StringBuilder sb = new StringBuilder();
        //                sb.Append("<html>");
        //                sb.Append("<head>");
        //                sb.Append("</head>");
        //                sb.Append("<body>");

        //                sb.Append("<table border='1' style='border:1px solid #4E5E6A;width:100%;' align='center'>");
        //                sb.Append("<tr><td style='text-align:left;' colspan='" + colcount/*dt.Columns.Count*/.ToString() + "'>" + ReportName + "</td></tr>");
        //                //sb.Append("<tr><td style='text-align:left;' colspan='" + colcount/*dt.Columns.Count*/.ToString() + "'>" + Session["ParamValues_" + SessionIdentifier].ToString() + "</td></tr>");
        //                if (dt.Columns.Contains("TableName"))
        //                {
        //                    dt.Columns.Remove("TableName");
        //                    if (dt.Columns.Contains("TableSnapshot"))
        //                    {
        //                        sb.Append("<tr><td style='text-align:left;' colspan='" + colcount/*dt.Columns.Count*/.ToString() + "'>" + dt.Rows[0]["TableSnapshot"] + "</td></tr>");
        //                        dt.Columns.Remove("TableSnapshot");
        //                    }
        //                }
        //                if (dt.Columns.Contains("TableSnapshot"))
        //                {
        //                    sb.Append("<tr><td style='text-align:left;' colspan='" + colcount/*dt.Columns.Count*/.ToString() + "'>" + dt.Rows[0]["TableSnapshot"] + "</td></tr>");
        //                    dt.Columns.Remove("TableSnapshot");
        //                }
        //                sb.Append("<tr>");
        //                string cols = "";
        //                string[] col = new string[dt.Columns.Count];
        //                for (int i = (dt.Columns.Contains("Style") == true ? ((dt.Columns.Contains("Grouping") == true) ? 2 : 1) : ((dt.Columns.Contains("Grouping") == true) ? 1 : 0))
        //                    ; i < dt.Columns.Count; i++)
        //                {

        //                    cols = cols == "" ? dt.Columns[i].ColumnName : cols
        //                        + "§" + dt.Columns[i].ColumnName;
        //                }
        //                col = cols.Split('§');
        //                //if (ReportID != "442")
        //                //{
        //                //    for (int i = (dt.Columns.Contains("Style") == true ? ((dt.Columns.Contains("Grouping") == true) ? 2 : 1) : ((dt.Columns.Contains("Grouping") == true) ? 1 : 0))
        //                //   ; i < dt.Columns.Count; i++)
        //                //    {
        //                //        for (int c = i + 1; c < col.Length; c++)
        //                //        {
        //                //            if (col[c].IndexOf(dt.Columns[i].ColumnName.ToString()) > -1)
        //                //            {
        //                //                int lenD = dt.Columns[i].ColumnName.Length;
        //                //                int lenc = col[c].Length;
        //                //                if (lenc - lenD <= 2 && lenc - lenD > 0)
        //                //                {
        //                //                    col[c] = dt.Columns[i].ColumnName.ToString();
        //                //                }
        //                //            }
        //                //        }
        //                //    }
        //                //}

        //                for (int i = (col.Contains("Style") == true ? ((col.Contains("Grouping") == true) ? 2 : 1) : ((col.Contains("Grouping") == true) ? 1 : 0)); i < col.Count(); i++)
        //                {
        //                    sb.Append("<th>" + col[i] + "</th>");
        //                }
        //                sb.Append("</tr>");
        //                if (dt.Rows.Count == 0)
        //                {
        //                    sb.Append("<tr>");
        //                    sb.Append("<th colspan=" + colcount/*dt.Columns.Count*/.ToString() + "><label>No Records Found!!</label></th>");
        //                    sb.Append("</tr>");
        //                }
        //                else
        //                {
        //                    for (int i = 0; i < dt.Rows.Count; i++)
        //                    {
        //                        if (dt.Columns.Contains("Style"))
        //                        {
        //                            sb.Append("<tr style='" + dt.Rows[i]["Style"].ToString() + "'>");
        //                        }
        //                        else
        //                        {
        //                            sb.Append("<tr>");
        //                        }
        //                        if (dt.Columns.Contains("Grouping") && dt.Rows[i]["Grouping"].ToString() != null && dt.Rows[i]["Grouping"].ToString() != "")
        //                        {
        //                            sb.Append("<td colspan='" + colcount/*dt.Columns.Count*/.ToString() + "'>" + MvcHtmlString.Create(dt.Rows[i]["Grouping"].ToString()) + "</td>");
        //                        }
        //                        else
        //                        {
        //                            for (int j = (dt.Columns.Contains("Style") == true ? ((dt.Columns.Contains("Grouping") == true) ? 2 : 1) : ((dt.Columns.Contains("Grouping") == true) ? 1 : 0)); j < dt.Columns.Count; j++)
        //                            {
        //                                if (dt.Rows[i][j].ToString().Contains("</td>"))
        //                                {
        //                                    sb.Append(MvcHtmlString.Create(dt.Rows[i][j].ToString()));
        //                                }
        //                                else
        //                                {
        //                                    sb.Append("<td>" + MvcHtmlString.Create(dt.Rows[i][j].ToString()) + "</td>");
        //                                }
        //                            }
        //                        }
        //                        sb.Append("</tr>");
        //                    }
        //                }
        //                sb.Append("</table>");
        //                sb.Append("</body>");
        //                sb.Append("</html>");
        //                string Htmltext = sb.ToString();
        //                System.IO.File.WriteAllText(path + file, Htmltext);
        //                Response.ClearContent();
        //                Response.Clear();
        //                Response.Buffer = true;
        //                Response.Charset = "";

        //                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
        //                Response.AddHeader("content-disposition", "attachment; filename=" + file);
        //                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //                Response.BinaryWrite(System.IO.File.ReadAllBytes(path + file));
        //                Response.Flush();
        //                Response.End();
        //            }
        //        }
        //    }

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