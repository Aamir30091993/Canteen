using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Routing;
using System.Web.Mvc;
using GCC_Canteen.ViewModel;
namespace GCC_Canteen.App_Code
{
    public class GridColumnGenerator
    {
        UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        public List<WebGridColumn> SetWebGridColumns(GridSettingData webgridData)
        {
            List<WebGridColumn> gridColumnlist = new List<WebGridColumn>();
            
                if (webgridData.WebGridSetting.AllowGet == true)
                {
                    gridColumnlist.Add(new WebGridColumn()
                    {
                        ColumnName = null,
                        Header = null,
                        CanSort = false,
                        //Style = "text-align: center",
                        Format = (item) =>
                        {
                            return new HtmlString(
                                string.Format("<a class='editRecord' href=" + urlHelper.Action(webgridData.WebGridSetting.GetActionMethod, webgridData.WebGridSetting.GetControllerName, new RouteValueDictionary(new { ID = item.ID })) + "> <i title=\"Edit\" class=\"fa fa-pencil\"></i></a>"));
                        }
                    });

                }
            //}

            foreach (var column in webgridData.WebGridColumnSetting)
            {
                if (column.Visible == false)
                {
                    gridColumnlist.Add(new WebGridColumn() { ColumnName = null, Header = null, CanSort = column.AllowSorting, Format = (item) => { return new HtmlString(string.Format("<input type=\"hidden\" id=\"IDHidden\" name=\"IDHidden\" value=" + item.ID + ">")); } });
                }

                if (column.Visible == true)
                {
                    //if (column.DataType == "VARCHAR")
                    //    gridColumnlist.Add(new WebGridColumn()
                    //    {
                    //        ColumnName = column.ColumnName,
                    //        Header = column.HeaderText,
                    //        CanSort = column.AllowSorting,
                    //        Format = (item) =>
                    //        {
                    //            string Name = column.ColumnName;
                    //            return new HtmlString(string.Format(Convert.ToString(item[Name])));
                    //        }
                    //    });
                    //gridColumnlist.Add(new WebGridColumn() { ColumnName = column.ColumnName, Header = column.HeaderText, CanSort = column.AllowSorting, Format = (item) => { return new HtmlString(string.Format("<a href=" + item.ProgramOutlinePath + " target=\"_blank\" ><img src=\"/assets/img/ar.png\"></a>")); } });//<img src=\"assets/img/ar.png\">
                    //else 
                    if (column.DataType == "RADIO")
                        gridColumnlist.Add(new WebGridColumn()
                        {
                            ColumnName = column.ColumnName,
                            Header = column.HeaderText,
                            CanSort = column.AllowSorting,
                            Style = column.ItemTextStyle,
                            Format = (item) =>
                            {
                                if (webgridData.WebGridSetting.PageID == 71)
                                {
                                    if (item.AllocationTypeID <= 2)
                                    {
                                        if (item.IsChecked == "1")
                                            return new HtmlString(string.Format("<input type=\"radio\" name=\"gridRd\" value=\"" + item.ID + "\" checked/>"));
                                        else
                                            return new HtmlString(string.Format("<input type=\"radio\" name=\"gridRd\"  value=\"" + item.ID + "\" />"));
                                    }
                                    else
                                    {
                                        return new HtmlString(string.Format(""));
                                    }
                                }
                                else
                                {
                                    if (item.IsChecked == "1")
                                        return new HtmlString(string.Format("<input type=\"radio\" name=\"gridRd\" value=\"" + item.ID + "\" checked/>"));
                                    else
                                        return new HtmlString(string.Format("<input type=\"radio\" name=\"gridRd\"  value=\"" + item.ID + "\" />"));
                                }
                            }
                        });
                    else if (column.DataType == "CHECKBOX")
                        gridColumnlist.Add(new WebGridColumn()
                        {
                            ColumnName = column.ColumnName,
                            Header = column.HeaderText == "" ? "{CheckBoxHeader}" : column.HeaderText,
                            CanSort = column.AllowSorting,
                            Style = column.ItemTextStyle,
                            Format = (item) =>
                            {
                                if (item.IsChecked == "1")
                                    return new HtmlString(string.Format("<input type=\"checkbox\" class=\"chkRow1\" id = \"chkChild\"  value=\"" + item.ID + "\"  checked/>"));
                                else
                                    return new HtmlString(string.Format("<input type=\"checkbox\" class=\"chkRow1\" id = \"chkChild\"  value=\"" + item.ID + "\" />"));
                            }
                        });
                    else if (column.DataType == "BUTTON")
                        gridColumnlist.Add(new WebGridColumn() { ColumnName = column.ColumnName, Header = column.HeaderText, CanSort = column.AllowSorting, Format = (item) => { return new HtmlString(string.Format("<input type=\"button\" onclick=\"ButtonClickGrid(this," + item.ID + ");\"")); } });//<img src=\"assets/img/ar.png\">
                    else if (column.DataType == "DATE")
                        gridColumnlist.Add(new WebGridColumn()
                        {
                            ColumnName = column.ColumnName,
                            Header = column.HeaderText,
                            CanSort = column.AllowSorting,
                            Style = column.ItemTextStyle,
                            Format = (item) =>
                            {
                                string Name = column.ColumnName;
                                // return new HtmlString(string.Format( item[Name] != "" ? DateTime.ParseExact(item[Name], "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"):""));
                                //return new HtmlString(string.Format(item[Name].ToString("dd/MM/yyyy") == "01/01/1900" ? "" : item[Name].ToString("dd MMM yyyy")));
                                return new HtmlString(string.Format(item[Name].ToString("dd/MM/yyyy") == "01/01/1900" ? "" : item[Name].ToString("dd/MM/yyyy")));
                            }
                        });

                    else
                    {
                        gridColumnlist.Add(new WebGridColumn()
                        {
                            ColumnName = column.ColumnName,
                            Header = column.HeaderText,
                            CanSort = column.AllowSorting,
                            Style = column.ItemTextStyle,
                            Format = (item) =>
                            {
                                string Name = column.ColumnName;
                                return new HtmlString(string.Format(Convert.ToString(item[Name] == null ? "" : item[Name])));
                            }
                        });
                    }

                }

            }

            if (webgridData.WebGridSetting.AllowDelete == true)
            {
                //gridColumnlist.Add(new WebGridColumn()
                //{
                //    ColumnName = null,
                //    Header = null,
                //    CanSort = false,
                //    Format = (item) =>
                //{
                //    UserInfo user = HttpContext.Current.Session["UserInfo"] as UserInfo;
                //    var userPageAccess = new List<UserAccessRights>();
                //    userPageAccess = user.UserAccess.Where(m => m.PageID == webgridData.WebGridSetting.PageID).ToList();
                //    if (userPageAccess.Count > 0)
                //    {
                //        if (userPageAccess[0].Delete == true && webgridData.WebGridSetting.PageID != (int)CommonBase.Page.Enquiry && item.IsActiveStatus == true)
                //        {
                //            return new HtmlString(
                //                //string.Format("<a href=" + urlHelper.Action(webgridData.WebGridSetting.DeleteActionMethod, webgridData.WebGridSetting.DeleteControllerName, new RouteValueDictionary(new { ID = item.ID })) + "> <i title=\"Delete\" class=\"fa fa-times\"></i></a>"));
                //            string.Format("<a href=\"#\" onclick=\"DeleteGridRecord(this);\" id=" + item.ID + "><i title=\"Delete\" class=\"fa fa-times\"></i></a>"));
                //        }
                //        else { return ""; }
                //    }
                //    else { return ""; }

                //}
                //});

            }
            return gridColumnlist;
        }
    }
}