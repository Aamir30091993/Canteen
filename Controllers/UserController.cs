using GCC_Canteen.App_Code;
using GCC_Canteen.Models;
using GCC_Canteen.Models.BusinessLayer;
using GCC_Canteen.Models.Repository;
using GCC_Canteen.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GCC_Canteen.Controllers
{
    public class UserController : Controller
    {
        string SessionIdentifier = "";

        public ActionResult List(int page = 0, string sort = "", string FilterSession = "")

        {
            if (Request.Headers["SessionIdentifier"] == null)
                SessionIdentifier = DateTime.UtcNow.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_');
            else
                SessionIdentifier = Request.Headers["SessionIdentifier"].ToString();
            TempData["SessionIdentifier"] = SessionIdentifier;
            Session["PageID"] = (int)CommonBase.Page.User;
            ViewBag.WriteAccess = (CommonBase.UserPageAccess((int)CommonBase.Page.User)).Count > 0
                ? (CommonBase.UserPageAccess((int)CommonBase.Page.User))[0].Write : true;

            GridSettingData gridSettingModel = new GridSettingData();
            WebGridDataLayer webgridData = new WebGridDataLayer();
            gridSettingModel = webgridData.GridData((int)CommonBase.Page.User, "ctrlGrid1");
            GridColumnGenerator columnGenerator = new GridColumnGenerator();
            gridSettingModel.WebGridColumns = columnGenerator.SetWebGridColumns(gridSettingModel);

            UserListModel UserList = new UserListModel();
            UserList.gridSettingData = gridSettingModel;
            List<UserListValues> Data = new List<UserListValues>();
            Common _Common = new Common();
            string Where = "";
            if (Session["WhereClause" + FilterSession] != null)
            {
                Where = Session["WhereClause" + FilterSession].ToString();
                ViewBag.SearchCriteria = Session["FilterCriteria" + FilterSession].ToString();
                ViewBag.FilterSession = FilterSession;
            }
            Data = _Common.GetListPageData<UserListValues>((int)CommonBase.Page.User, Where, "ctrlGrid1", page, sort, SessionIdentifier, gridSettingModel);
            UserList.UserList = Data;
            return View(UserList);
        }

        // GET: User
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult New()
        {
            Session["PageID"] = (int)CommonBase.Page.User;
            UserViewModel ViewModelObj = new UserViewModel();
            UserInfo userInfo = Session["UserInfo"] as UserInfo;
            MMModel dbConn = new MMModel();
            ViewBag.RoleList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(2, Convert.ToInt32(CommonBase.Page.User), 0, 0, "", "");
            ViewBag.LocationList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(3, Convert.ToInt32(CommonBase.Page.User), 0, 0, "", "");
            ViewBag.ReportingList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(1, Convert.ToInt32(CommonBase.Page.User), 0, 0, "", "");
            List<LocationInfo> objLocationList = dbConn.Database.SqlQuery<LocationInfo>("exec spGetLocationList")
                 .Select(x => new LocationInfo {/* DisplayLocationName = x.DisplayLocationName, */LocationID = x.LocationID,LocationName = x.LocationName,UserID=x.UserID,Access=x.Access }).ToList();
            Session[GetSessionKey.GetCurrentSessionkey((int)CommonBase.Page.User, 0, "LocationInfoList")] = objLocationList;

            ViewModelObj.LocationDetails = objLocationList;

            return View(ViewModelObj);
        }

        //[HttpPost]
        //public ActionResult New(UserViewModel obj)
        //{
        //    //List<PagesInfo> objPagesInfo = Session[GetSessionKey.GetCurrentSessionkey((int)CommonBase.Page.User, 0, "PagesInfoList")] as List<PagesInfo>;
        //    //Session[GetSessionKey.GetCurrentSessionkey((int)CommonBase.Page.User, 0, "PagesInfoList")] = null;
        //    Session["PageID"] = (int)CommonBase.Page.User;
        //    string Save = GCC_Canteen.Models.Repository.UserRepository.SaveUser(obj);
        //    return Json(Save, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public ActionResult New(UserViewModel obj)
        {
            List<LocationInfo> objLocationList = Session[GetSessionKey.GetCurrentSessionkey((int)CommonBase.Page.User, 0, "LocationInfoList")] as List<LocationInfo>;
            obj.LocationDetails = objLocationList;
            //Session[GetSessionKey.GetCurrentSessionkey((int)CommonBase.Page.User, 0, "LocationInfoList")] = null;
            Session["PageID"] = (int)CommonBase.Page.User;
            string Save = GCC_Canteen.Models.BusinessLayer.User.UserBL.SaveUser(obj);
            return Json(Save, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int ID, string FilterSession)
        {
            Session["PageID"] = (int)CommonBase.Page.User;
            UserViewModel obj = new UserViewModel();
            ViewData["UpdateAccess"] = (CommonBase.UserPageAccess((int)CommonBase.Page.User)).Count > 0
               ? ((CommonBase.UserPageAccess((int)CommonBase.Page.User))[0].Update == true ? false : true) : true;
            ViewData["DeleteAccess"] = (CommonBase.UserPageAccess((int)CommonBase.Page.User)).Count > 0
                ? ((CommonBase.UserPageAccess((int)CommonBase.Page.User))[0].Delete == true ? false : true) : true;
            obj = GCC_Canteen.Models.Repository.UserRepository.GetUser(ID);
            ViewBag.FilterSession = FilterSession;
            ViewBag.RoleList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(2, Convert.ToInt32(CommonBase.Page.User), 0, 0, "", "");
            ViewBag.LocationList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(3, Convert.ToInt32(CommonBase.Page.User), 0, 0, "", "");
            ViewBag.ReportingList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(1, Convert.ToInt32(CommonBase.Page.User), 0, 0, "", "");

            Session[GetSessionKey.GetCurrentSessionkey((int)CommonBase.Page.User, ID, "LocationInfoList")] = obj.LocationDetails;

            return View(obj);


            //Session["ContactCount"] = 0;
            //UserViewModel objUsertViewModel = new UserViewModel();
            //UserInfo userinfo = Session["UserInfo"] as UserInfo;
            //Session["ContactCount"] = 2;
            //ViewBag.UserTypeID = Session["UserTypeIDForCreation"];
            //ViewBag.LocationIDList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(4, Convert.ToInt32(CommonBase.Page.User), 0, 0, "", "");
            //objUsertViewModel = GCC_Canteen.Models.Repository.UserRepository.GetUser(ID);



            // return View(objUsertViewModel);


        }


        //bool flag = false;
        //flag = ImportShipmentTracker.Models.Repository.ShipperRepository.DeleteShipper(ID);
        //    return Json(flag, JsonRequestBehavior.AllowGet);

        [HttpPost]
        public ActionResult Edit(UserViewModel objUserViewModel)
         {
            string flag = "";
            List<LocationInfo> objLocationInfo = Session[GetSessionKey.GetCurrentSessionkey((int)CommonBase.Page.User, objUserViewModel.UserDetails.ID, "LocationInfoList")] as List<LocationInfo>;

            Session["PageID"] = (int)CommonBase.Page.User;
            flag = GCC_Canteen.Models.BusinessLayer.User.UserBL.UpdateUser(objUserViewModel);
            //string Update = GCC_Canteen.Models.Repository.UserRepository.UpdateUser(objUserViewModel);
            //Session[GetSessionKey.GetCurrentSessionkey((int)CommonBase.Page.User, objUserViewModel.UserDetails.ID, "LocationInfoList")] = null;

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateLocationAccess(int UserID, string strLocationAccess)
        {
            UserViewModel objUserRightsViewModel = new UserViewModel();

            if (strLocationAccess != "")
            {
                List<LocationInfo> objLocationInfo = Session[GetSessionKey.GetCurrentSessionkey((int)CommonBase.Page.User, UserID, "LocationInfoList")] as List<LocationInfo>;
                string[] strLocationAccessDetails = strLocationAccess.Split('|');
                for (int item = 0; item < strLocationAccessDetails.Length; item++)
                {
                    int strLocationID = Convert.ToInt32(strLocationAccessDetails[item].Split(',')[0]);
                    bool strAccess = Convert.ToBoolean(Convert.ToInt32(strLocationAccessDetails[item].Split(',')[1]));
                    objLocationInfo.Find(m => (m.LocationID == strLocationID)).Access = strAccess;
                }

                Session[GetSessionKey.GetCurrentSessionkey((int)CommonBase.Page.User, UserID, "LocationInfoList")] = objLocationInfo;
                objUserRightsViewModel.LocationDetails = objLocationInfo;
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}