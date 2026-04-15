using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GCC_Canteen.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;


namespace GCC_Canteen.Models.Repository
{
    public class SelectListRepository
    {
        public static IEnumerable<SelectListItem> FillSelectList(int ActionID = 0, int PageID = 0, int ControlPageID = 0, int RecordID = 0, string KeyValue = "", string SelectedValue = "", bool? Group = false, string spName = "", string _date = "")
        {
            UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;

            //SelectListGroup active = new SelectListGroup();
            //active.Name = "Active";
            //SelectListGroup inactive = new SelectListGroup();
            //inactive.Name = "In-Active";
            using (var ctx = new MMModel())
            {
                List<SelectListItem> selectlistitem = new List<SelectListItem>();
                List<SelectListGroup> selectlistgroup = new List<SelectListGroup>();
                if ((bool)Group)
                {
                    if (spName == "")
                    {
                        var result = from p in ctx.Database.SqlQuery<SelectListModel>("exec [dbo].[spGetFillList] @ActionID = {0},@PageID = {1},@ControlPageID = {2},@UserID = {3},@RecordID = {4}, @KeyValue = {5}, @Date = {6}", ActionID, PageID, ControlPageID, userInfo.UserID, RecordID, KeyValue, DateTime.Parse(_date == "" ? DateTime.Now.ToString() : _date))
                                     group p by p.Group;

                        foreach (var slgList in result)
                        {
                            if (slgList.Key != "" && slgList.Key != null)
                            {
                                SelectListGroup slg = new SelectListGroup();
                                slg.Name = slgList.Key;
                                selectlistgroup.Add(slg);
                            }
                        }
                        foreach (var sliList in result)
                        {
                            foreach (var list in sliList)
                            {
                                SelectListItem listitem = new SelectListItem();
                                listitem.Text = list.Description;
                                listitem.Value = list.Code;
                                listitem.Selected = list.Code == (SelectedValue != null ? SelectedValue : "");
                                foreach (var item in selectlistgroup)
                                {
                                    if (item.Name == list.Group)
                                    {
                                        listitem.Group = item;
                                        break;
                                    }
                                }
                                selectlistitem.Add(listitem);
                            }
                        }
                    }
                    else
                    {
                        var result = from p in ctx.Database.SqlQuery<SelectListModel>(spName)
                                     group p by p.Group;

                        foreach (var slgList in result)
                        {
                            if (slgList.Key != "" && slgList.Key != null)
                            {
                                SelectListGroup slg = new SelectListGroup();
                                slg.Name = slgList.Key;
                                selectlistgroup.Add(slg);
                            }
                        }
                        foreach (var sliList in result)
                        {
                            foreach (var list in sliList)
                            {
                                SelectListItem listitem = new SelectListItem();
                                listitem.Text = list.Description;
                                listitem.Value = list.Code;
                                listitem.Selected = list.Code == (SelectedValue != null ? SelectedValue : "");
                                foreach (var item in selectlistgroup)
                                {
                                    if (item.Name == list.Group)
                                    {
                                        listitem.Group = item;
                                        break;
                                    }
                                }
                                selectlistitem.Add(listitem);
                            }
                        }
                    }

                }
                else
                {
                    if (spName == "")
                    {
                        var result = from p in ctx.Database.SqlQuery<SelectListModel>("exec [dbo].[spGetFillList] @ActionID = {0},@PageID = {1},@ControlPageID = {2},@UserID = {3},@RecordID = {4}, @KeyValue = {5}, @Date = {6}", ActionID, PageID, ControlPageID, /*userInfo.UserID*/3208, RecordID, KeyValue, DateTime.Parse(_date == "" ? DateTime.Now.ToString() : _date))
                                     group p by p.Code;
                        foreach (var sliList in result)
                        {
                            foreach (var list in sliList)
                            {
                                SelectListItem listitem = new SelectListItem();
                                listitem.Text = list.Description;
                                listitem.Value = list.Code;
                                listitem.Selected = list.Code == (SelectedValue != null ? SelectedValue : "");
                                selectlistitem.Add(listitem);
                            }
                        }
                    }
                    else
                    {
                        var result = from p in ctx.Database.SqlQuery<SelectListModel>(spName)
                                     group p by p.Code;
                        foreach (var sliList in result)
                        {
                            foreach (var list in sliList)
                            {
                                SelectListItem listitem = new SelectListItem();
                                listitem.Text = list.Description;
                                listitem.Value = list.Code;
                                listitem.Selected = list.Code == (SelectedValue != null ? SelectedValue : "");
                                selectlistitem.Add(listitem);
                            }
                        }
                    }
                }
                return selectlistitem.AsEnumerable();
            }
        }

        public static IEnumerable<SelectListItem> FillCombo(int ActionID, string SelectedValue,bool ? Group = false)
        {
            UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            List<SelectListGroup> selectlistgroup = new List<SelectListGroup>();
            List<SelectListItem> selectlistitem = new List<SelectListItem>();
            using (var ctx = new MMModel())
            {
                if ((bool)Group)
                {
                    var List = from p in ctx.Database.SqlQuery<SelectListModel>("exec [dbo].[spGetFillCombo] @ActionID = {0},@UserID = {1}", ActionID, userInfo.UserID)
                                group p by p.Group;
                    foreach (var slgList in List)
                    {
                        if (slgList.Key != "" && slgList.Key != null)
                        {
                            SelectListGroup slg = new SelectListGroup();
                            slg.Name = slgList.Key;
                            selectlistgroup.Add(slg);
                        }
                    }
                    foreach (var sliList in List)
                    {
                        foreach (var list in sliList)
                        {
                            SelectListItem listitem = new SelectListItem();
                            listitem.Text = list.Description;
                            listitem.Value = list.Code;
                            listitem.Selected = list.Code == (SelectedValue != null ? SelectedValue : "");
                            foreach (var item in selectlistgroup)
                            {
                                if (item.Name == list.Group)
                                {
                                    listitem.Group = item;
                                    break;
                                }
                            }
                            selectlistitem.Add(listitem);
                        }
                    }
                    return selectlistitem;
                }
                else
                {
                    IEnumerable<SelectListItem> List = ctx.Database.SqlQuery<SelectListModel>("exec [dbo].[spGetFillCombo] @ActionID = {0},@UserID = {1}", ActionID, userInfo.UserID)
                       .Select(x => new SelectListItem { Text = x.Description.ToString(), Value = x.Code.ToString(), Selected = x.Code == (SelectedValue != null ? SelectedValue : "") })
                       .ToList();
                    return List;
                }
                
            }
        }
    }
}