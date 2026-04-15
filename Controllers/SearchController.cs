using GCC_Canteen.App_Code;
using GCC_Canteen.App_Code;
using GCC_Canteen.Models;
using GCC_Canteen.Models.BusinessLayer;
using GCC_Canteen.Models.Repository;
using GCC_Canteen.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GCC_Canteen.Controllers
{
    public class SearchController : Controller
    {
        string SessionIdentifier = "";
        string Ticks = DateTime.Now.Ticks.ToString();
        [HttpPost]
        public ActionResult GetSearchResult(FormCollection SearchValues)
        {
            UserInfo userInfo = Session["UserInfo"] as UserInfo;
            if (Request.Headers["SessionIdentifier"] == null)
                SessionIdentifier = "0";
            else
                SessionIdentifier = Request.Headers["SessionIdentifier"].ToString();



            TempData["SessionIdentifier"] = SessionIdentifier;

            string whereClause = buildWhereClouse(SearchValues);
            ViewBag.FilterSession = Ticks;
            Session["WhereClause" + Ticks] = whereClause;
            //if (Convert.ToInt32(SearchValues["PageID"]) == 65 || Convert.ToInt32(SearchValues["PageID"]) == 151)
            //{
            //    whereClause = whereClause.Replace("[IsApproved] IN('2')", "[IsApproved] IS NULL ");
            //}
            PartialViewResult result = GetSearchResult(Convert.ToInt32(SearchValues["PageID"]), whereClause, SessionIdentifier);
            return result;
        }
        [NonAction]
        internal PartialViewResult GetSearchResult(int PageID, string whereClause, string SessionIdentifier = "0")
        {
            UserInfo userInfo = System.Web.HttpContext.Current.Session["UserInfo"] as UserInfo;
            PartialViewResult result = new PartialViewResult();
            ViewBag.WriteAccess = (CommonBase.UserPageAccess(PageID)).Count > 0
                ? (CommonBase.UserPageAccess(PageID))[0].Write : true;
            Common _common = new Common();

            if (PageID == (int)CommonBase.Page.User)
            {
                var BindingData = _common.GetListPageData<UserListValues>(PageID, whereClause, "ctrlGrid1", 0, "", SessionIdentifier);
                result = PartialView("_GridView", BindingData);
            }

            if (PageID == (int)CommonBase.Page.ImportUser)
            {
                var BindingData = _common.GetListPageData<ImportUserListValues>(PageID, whereClause, "ctrlGrid1", 0, "", SessionIdentifier);
                result = PartialView("_GridView", BindingData);
            }

            return result;
        }
        [NonAction]
        internal string buildWhereClouse(FormCollection SearchValues)
        {
            WebGridDataLayer webgridData = new WebGridDataLayer();
            GridSettingData gridSettingModel = webgridData.GridData(Convert.ToInt32(SearchValues["PageID"]), "ctrlGrid1");
            List<GridColumnSetting> WebGridColumnSetting = gridSettingModel.WebGridColumnSetting;

            ViewData["WriteAccess"] = (CommonBase.UserPageAccess(Convert.ToInt32(SearchValues["PageID"]))).Count > 0
                ? (CommonBase.UserPageAccess(Convert.ToInt32(SearchValues["PageID"])))[0].Write : true;
            GridColumnGenerator columnGenerator = new GridColumnGenerator();

            ViewData["ColumnsSetting"] = columnGenerator.SetWebGridColumns(gridSettingModel);
            ViewData["WebGridSettings"] = gridSettingModel.WebGridSetting;
            StringBuilder sb = new StringBuilder();
            StringBuilder filter = new StringBuilder();
            int count = 0;
            foreach (var attr in SearchValues.AllKeys)
            {
                if (SearchValues.Count - 1 != count)
                {
                    if (attr != "PageID")
                    {
                        string searchValues = SearchValues[attr].ToString().Split(',')[0];
                        //if ((count == 0 || sb.ToString() !="") && (SearchValues[attr] != "") && (SearchValues[attr] != "XMLHttpRequest"))
                        if ((count == 0 || sb.ToString() == "") && (searchValues != "") && (searchValues != "XMLHttpRequest"))
                        {
                            count++;
                            sb.Append(" WHERE");
                            foreach (var colmn in WebGridColumnSetting)
                            {
                                //if (attr == colmn.ColumnName) 
                                //{
                                //    filter.Append(colmn.HeaderText + " : " + searchValues);
                                //}
                                if (attr == colmn.ColumnName && colmn.DataType.ToUpper() == "VARCHAR")
                                {
                                    //sb.Append(" " + "[" + attr + "]" + " Like " + "'%" + SearchValues[attr] + "%'");
                                    sb.Append(" " + "[" + attr + "]" + " Like " + "N'%" + searchValues + "%'");
                                    filter.Append(colmn.HeaderText + " : " + searchValues);
                                }
                                //if (attr == colmn.ColumnName && colmn.DataType.ToUpper() == "BIT")
                                //{

                                //}
                                if (attr == colmn.ColumnName && colmn.DataType.ToUpper() == "DROPDOWN")
                                {
                                    sb.Append(" " + "[" + attr + "]" + " IN('" + searchValues + "')");
                                    using (var ctx = new MMModel())
                                    {
                                        var result = ctx.Database.SqlQuery<SelectListModel>(colmn.LookUpQuery);
                                        filter.Append(colmn.HeaderText + " : " + result.Where(x => x.Code == searchValues).ToList()[0].Description);
                                    }

                                }
                                if (attr == colmn.ColumnName && colmn.AllowForFilter == true/*colmn.ColumnName*/ && colmn.DataType.ToUpper() == "DATE")
                                {
                                    sb.Append(" " + "CONVERT(VARCHAR(10),[" + colmn.ColumnName + "],106) = " + " CONVERT(VARCHAR(10),'" + searchValues + "',106)");
                                    filter.Append(colmn.HeaderText + " : " + searchValues);
                                }
                                if (attr == colmn.ColumnName && colmn.AllowForFilter == true/*colmn.ColumnName*/ && colmn.DataType.ToUpper() == "FROMDATE")
                                {
                                    sb.Append(" " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) >= " + " CONVERT(datetime,'" + searchValues + "',106)");
                                    filter.Append(colmn.HeaderText + " : " + searchValues);
                                }
                                if (attr == colmn.ColumnName && colmn.AllowForFilter == true/*colmn.ColumnName*/ && colmn.DataType.ToUpper() == "TODATE")
                                {
                                    sb.Append(" " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) <= " + " CONVERT(datetime,'" + searchValues + "',106)");
                                    filter.Append(colmn.HeaderText + " : " + searchValues);
                                }
                                if (attr == colmn.ColumnName && colmn.AllowForFilter == true/*colmn.ColumnName*/ && colmn.DataType.ToUpper() == "MONTHDATE" && colmn.ColumnName.ToLower() == "surveyperiod")
                                {
                                    sb.Append(" " + colmn.ColumnName + " = " + Convert.ToDateTime("01 " + searchValues).ToString("yyyyMM"));
                                    filter.Append(colmn.HeaderText + " : " + searchValues);
                                    //if(datevalues)
                                }
                                if (attr == colmn.ColumnName && colmn.AllowForFilter == true/*colmn.ColumnName*/ && colmn.DataType.ToUpper() == "MONTHDATE" && colmn.ColumnName.ToLower() != "surveyperiod")
                                {
                                    string searchValues1 = Convert.ToDateTime(searchValues).ToString("MM/yyyy");
                                    if (Convert.ToInt32(searchValues1.Split('/')[0]) == 1
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 3
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 5
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 7
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 8
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 10
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 12)
                                    {
                                        sb.Append(" " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) >= " + " CONVERT(datetime,'01/" + searchValues1 + "',106) AND CONVERT(datetime,[" + colmn.ColumnName + "],106) <= " + " CONVERT(datetime,'31/" + searchValues1 + "',106)");
                                        filter.Append(colmn.HeaderText + " : " + searchValues);
                                    }
                                    else if (Convert.ToInt32(searchValues1.Split('/')[0]) == 4
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 6
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 9
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 11)
                                    {
                                        sb.Append(" " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) >= " + " CONVERT(datetime,'01/" + searchValues1 + "',106) AND CONVERT(datetime,[" + colmn.ColumnName + "],106) <= " + " CONVERT(datetime,'30/" + searchValues1 + "',106)");
                                        filter.Append(colmn.HeaderText + " : " + searchValues);
                                    }
                                    else if (Convert.ToInt32(searchValues1.Split('/')[0]) == 2
                                        && (Convert.ToInt32(searchValues1.Split('/')[1]) % 4) == 0)
                                    {
                                        sb.Append(" " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) >= " + " CONVERT(datetime,'01/" + searchValues1 + "',106) AND CONVERT(datetime,[" + colmn.ColumnName + "],106) <= " + " CONVERT(datetime,'29/" + searchValues1 + "',106)");
                                        filter.Append(colmn.HeaderText + " : " + searchValues);
                                    }
                                    else if (Convert.ToInt32(searchValues1.Split('/')[0]) == 2
                                        && (Convert.ToInt32(searchValues1.Split('/')[1]) % 4) != 0)
                                    {
                                        sb.Append(" " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) >= " + " CONVERT(datetime,'01/" + searchValues1 + "',106) AND CONVERT(datetime,[" + colmn.ColumnName + "],106) <= " + " CONVERT(datetime,'28?" + searchValues1 + "',106)");
                                        filter.Append(colmn.HeaderText + " : " + searchValues);
                                    }
                                }
                            }
                        }
                        else if (count != 0 && SearchValues[attr] != "")
                        {
                            foreach (var colmn in WebGridColumnSetting)
                            {
                                //if (attr == colmn.ColumnName)
                                //{
                                //    filter.Append(", " + colmn.HeaderText + " : " + searchValues);
                                //}
                                if (attr == colmn.ColumnName && colmn.DataType.ToUpper() == "VARCHAR")
                                {
                                    //sb.Append(" AND " + "[" + attr + "]" + " Like " + "'%" + SearchValues[attr] + "%'");
                                    sb.Append(" AND " + "[" + attr + "]" + " Like " + "N'%" + searchValues + "%'");
                                    filter.Append(", " + colmn.HeaderText + " : " + searchValues);
                                }
                                //if (attr == colmn.ColumnName && colmn.DataType.ToUpper() == "BIT")
                                //{

                                //}
                                if (attr == colmn.ColumnName && colmn.DataType.ToUpper() == "DROPDOWN")
                                {
                                    sb.Append(" AND " + "[" + attr + "]" + " IN('" + searchValues + "')");
                                    using (var ctx = new MMModel())
                                    {
                                        var result = ctx.Database.SqlQuery<SelectListModel>(colmn.LookUpQuery);
                                        filter.Append(", " + colmn.HeaderText + " : " + result.Where(x => x.Code == searchValues).ToList()[0].Description);
                                    }
                                }
                                if (attr == colmn.ColumnName && colmn.AllowForFilter == true/*colmn.ColumnName*/ && colmn.DataType.ToUpper() == "DATE")
                                {
                                    sb.Append(" AND " + "CONVERT(VARCHAR(10),[" + colmn.ColumnName + "],106) = " + " CONVERT(VARCHAR(10),'" + searchValues + "',106)");
                                    filter.Append(", " + colmn.HeaderText + " : " + searchValues);
                                }
                                if (attr == colmn.ColumnName && colmn.AllowForFilter == true/*colmn.ColumnName*/ && colmn.DataType.ToUpper() == "FROMDATE")
                                {
                                    sb.Append(" AND " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) = " + " CONVERT(datetime,'" + searchValues + "',106)");
                                    filter.Append(", " + colmn.HeaderText + " : " + searchValues);
                                }
                                if (attr == colmn.ColumnName && colmn.AllowForFilter == true/*colmn.ColumnName*/ && colmn.DataType.ToUpper() == "TODATE")
                                {
                                    sb.Append(" AND " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) <= " + " CONVERT(datetime,'" + searchValues + "',106)");
                                    filter.Append(", " + colmn.HeaderText + " : " + searchValues);
                                }
                                if (attr == colmn.ColumnName && colmn.AllowForFilter == true/*colmn.ColumnName*/ && colmn.DataType.ToUpper() == "MONTHDATE" && colmn.ColumnName.ToLower() == "surveyperiod")
                                {
                                    sb.Append(" " + colmn.ColumnName + " = " + Convert.ToDateTime("01 " + searchValues).ToString("yyyyMM"));
                                    filter.Append(", " + colmn.HeaderText + " : " + searchValues);
                                }
                                if (attr == colmn.ColumnName && colmn.AllowForFilter == true/*colmn.ColumnName*/ && colmn.DataType.ToUpper() == "MONTHDATE" && colmn.ColumnName.ToLower() != "surveyperiod")
                                {
                                    string searchValues1 = Convert.ToDateTime(searchValues).ToString("MM/yyyy");
                                    if (Convert.ToInt32(searchValues1.Split('/')[0]) == 1
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 3
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 5
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 7
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 8
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 10
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 12)
                                    {
                                        sb.Append(" AND " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) >= " + " CONVERT(datetime,'01/" + searchValues1 + "',106) AND CONVERT(datetime,[" + colmn.ColumnName + "],106) <= " + " CONVERT(datetime,'31/" + searchValues1 + "',106)");
                                        filter.Append(", " + colmn.HeaderText + " : " + searchValues);
                                    }
                                    else if (Convert.ToInt32(searchValues1.Split('/')[0]) == 4
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 6
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 9
                                        || Convert.ToInt32(searchValues1.Split('/')[0]) == 11)
                                    {
                                        sb.Append(" AND " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) >= " + " CONVERT(datetime,'01/" + searchValues1 + "',106) AND CONVERT(datetime,[" + colmn.ColumnName + "],106) <= " + " CONVERT(datetime,'30/" + searchValues1 + "',106)");
                                        filter.Append(", " + colmn.HeaderText + " : " + searchValues);
                                    }
                                    else if (Convert.ToInt32(searchValues1.Split('/')[0]) == 2
                                        && (Convert.ToInt32(searchValues1.Split('/')[1]) % 4) == 0)
                                    {
                                        sb.Append(" AND " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) >= " + " CONVERT(datetime,'01/" + searchValues1 + "',106) AND CONVERT(datetime,[" + colmn.ColumnName + "],106) <= " + " CONVERT(datetime,'29/" + searchValues1 + "',106)");
                                        filter.Append(", " + colmn.HeaderText + " : " + searchValues);
                                    }
                                    else if (Convert.ToInt32(searchValues1.Split('/')[0]) == 2
                                        && (Convert.ToInt32(searchValues1.Split('/')[1]) % 4) != 0)
                                    {
                                        sb.Append(" AND " + "CONVERT(datetime,[" + colmn.ColumnName + "],106) >= " + " CONVERT(datetime,'01/" + searchValues1 + "',106) AND CONVERT(datetime,[" + colmn.ColumnName + "],106) <= " + " CONVERT(datetime,'28/" + searchValues1 + "',106)");
                                        filter.Append(", " + colmn.HeaderText + " : " + searchValues);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Session["FilterCriteria" + Ticks] = filter;
            return sb.ToString();
        }
    }
}