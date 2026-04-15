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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GCC_Canteen.Models.BusinessLayer;
using System.IO;
using DocumentFormat.OpenXml;
using System.Web.UI;
using GCC_Canteen.ViewModel;

namespace GCC_Canteen.Controllers
{
    public class MealTimeConfigController : Controller
    {
        public ActionResult Show()
        {
            ViewBag.TimeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(3, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, 0, "", "");
            //ViewBag.MealSubTypeList = GCC_Canteen.Models.Repository.SelectListRepository.FillSelectList(2, Convert.ToInt32(CommonBase.Page.MainDashboard), 0, 0, "", "");
            MealTimeConfigViewModel objMealTimeConfigViewModel = new MealTimeConfigViewModel();
            var getFilteredListData = MealTimeConfigRepository.GetMealTimeConfigList();
            ViewBag.sysConfigData = CommonRepository.GetSysConfig("1");
            
            return View(getFilteredListData);
        }   

        [HttpPost]
        public ActionResult Show(List<MealTimeConfigList> mealTimeConfigurations, SysConfigDtl sysConfigDetails)
        {
            MealTimeConfigRepository.UpdateSysConfig(sysConfigDetails);
            // Update the meal time configurations in your database or perform any other necessary operations
            MealTimeConfigRepository.UpdateMealTimeConfigList(mealTimeConfigurations);

            return Json(new { success = true, message = "Meal time configurations updated successfully" });
        }
    }
}