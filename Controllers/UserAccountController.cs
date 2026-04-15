using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GCC_Canteen.ViewModel;
using System.Net;
using GCC_Canteen.App_Code;
using GCC_Canteen.Models;
using System.Data.SqlClient;
using System.Data;
using GCC_Canteen.Models.Repository;
using System.Web.Security;  
using GCC_Canteen.Models.BusinessLayer.UserAccount;
using static GCC_Canteen.ViewModel.Login;

namespace GCC_Canteen.Controllers
{
   // [NPSAuthorize((int)CommonBase.Page.ClientMaster)]
    public class UserAccountController : Controller
    {
        PasswordManager obj_PasswordMgr = new PasswordManager();
        MMModel db = new MMModel();


        // GET: UserAccount
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)

        {
            //Request.Headers["Cookie"] = "";
            Login login = new Login();
            HttpContext.Session.RemoveAll();
            HttpContext.Session.Abandon();
            return View(login);
        }

        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login, string ReturnUrl)
        {
            SessionManager obj_session = new SessionManager();
            //Request.Headers["Cookie"] = "";
            if (ModelState.IsValid)
            {
                bool userExists = UserAccountRepository.UserDetails(login);
                if (userExists == true)
                {


                    //bool verifyUser = true;//UserAccountRepository.VerifyUser(login);
                    var sysConfigData = CommonRepository.GetSysConfig("2");
                    bool verifyUser = false;
                    if(sysConfigData[0].Key == login.LoginName)
                    {
                        if(sysConfigData[0].Value == login.Password)
                        {
                            verifyUser = true;
                        }
                    }
                    //bool verifyUser = UserAccountRepository.VerifyUser(login);
                    if (verifyUser == true)
                    {
                        //UserInfo userInfo = Session["UserInfo"] as UserInfo;
                        //Session["UserName"] = userInfo.UserName;
                        if (this.Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return Redirect("~/Dashboard/Show");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Please enter a valid Login Name and Password.");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please enter a valid Login Name and Password.");
                    return View();
                }

            }
            else
            {

                return View(login);
            }
        }

        [AllowAnonymous]
        public ActionResult UpdatePassword()
        {
            UpdatePassword objUpdatePassword = new UpdatePassword();
            return View(objUpdatePassword);
        }

        
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ForgetPassword objUpdatePassword = new ForgetPassword();
            return View("ForgetPassword", objUpdatePassword);
        }

        [HttpPost]

        public ActionResult UpdatePassword(UpdatePassword objUpdatePassword)
        {

            if (ModelState.IsValid)
            {
                UserAccountBL objUserAccountBL = new UserAccountBL();

                string request = "Update";

                bool VerifyUserExists = UserAccountRepository.UpdatePasswordVerifyUser(objUpdatePassword, request);

                if (VerifyUserExists)
                {
                    //send an email

                   // bool UpdatePassword = objUserAccountBL.UpdatePassword(objUpdatePassword);

                    //if (UpdatePassword)
                    //{
                    //bool flag = Mail.SendMail(1, 2, "spGetEmailTemplateDetails", 0, "", 1);
                    return Json(true, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                   // {
                      //  return Json(false, JsonRequestBehavior.AllowGet);

                    //}
                }
                else
                {
                    return View(objUpdatePassword);
                }


            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ActionName("ForgetPassword")]
        public ActionResult ForgetPassword(ForgetPassword objUpdatePassword)
        {
            string ErrorMsg = "";
            string Url = "";
            if (ModelState.IsValid)
            {
                UserAccountBL objUserAccountBL = new UserAccountBL();

                string request = "Email";

                int Result = UserAccountRepository.ForgetLoginVerify(objUpdatePassword, request);

                //if (Result==2)
                //{
                //send an email

                // bool UpdatePassword = objUserAccountBL.UpdatePassword(objUpdatePassword);

                //if (UpdatePassword)
                //{

                if (Result == 0)
                {
                    ErrorMsg = "Please Enter Valid Login Name";
                    Url = "/UserAccount/ForgotPassword";
                }
                else if (Result == 1)
                {
                    ErrorMsg = "";
                    Url = "/UserAccount/ForgotPassword";
                }
                else if (Result == 2)
                {
                    ErrorMsg = "Your password has been reset and an email has been send to your registered email address. Please check your mail box and login again to access the portal";
                    Url = "/UserAccount/Login";
                }
                else if (Result == 3)
                {
                    ErrorMsg = "Error in Verifying Login Name.Try Again Later";
                    Url = "/UserAccount/ForgotPassword";
                }

                return Json(new { ErrorMsg, Url }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ErrorMsg = "Please Enter Valid Login Name";
                Url = "/UserAccount/ForgotPassword";
                return Json(new { ErrorMsg, Url }, JsonRequestBehavior.AllowGet);
            }
        }


        //  [HttpPost]
        //  public ActionResult ForgotPassword(ForgetPassword objUpdatePassword)
        // {
        //  string ErrorMsg = "";
        //  ErrorMsg = UserAccountRepository.ForgotPassword(objUpdatePassword);
        //return Json(ErrorMsg, JsonRequestBehavior.AllowGet);

        //string Url = "";
        //if (ModelState.IsValid)
        //{
        //    UserAccountBL objUserAccountBL = new UserAccountBL();

        //    string request = "Email";

        //    int Result = UserAccountRepository.ForgetLoginVerify(objUpdatePassword, request);

        //    if (Result == 0)
        //    {
        //        ErrorMsg = "Please Enter Valid Login Name";
        //        Url = "~/UserAccount/ForgotPassword";
        //    }
        //    else if (Result == 1)
        //    {
        //        ErrorMsg = "";
        //        Url = "~/UserAccount/ForgotPassword";
        //    }
        //    else if (Result == 2)
        //    {
        //        ErrorMsg = "";
        //        //ErrorMsg = "Your password has been reset and an email has been send to your registered email address. Please check your mail box and login again to access the portal";
        //        Url = "~/UserAccount/Login";
        //    }
        //    else if(Result==3)
        //    {
        //        ErrorMsg = "Error in Verifying Login Name.Try Again Later";
        //        Url = "~/UserAccount/ForgotPassword";
        //    }

        //    return Json(new { ErrorMsg,Url}, JsonRequestBehavior.AllowGet);
        //        //}
        //        //else
        //        // {
        //        //  return Json(false, JsonRequestBehavior.AllowGet);
        //    //
        //        //}
        //    //}
        //    //else
        //    //{
        //    //    return Json(new { Result });
        //    //}


        //}
        //else
        //{
        //    ErrorMsg = "Please Enter Valid Login Name";
        //    Url = "~/UserAccount/ForgotPassword";
        //    return Json(new { ErrorMsg, Url }, JsonRequestBehavior.AllowGet);
        //}
        // }

        public ActionResult ChangePassword()
        {
            SessionManager objSession = new SessionManager();           
            UpdatePassword objUpdatePassword = new UpdatePassword();
            UserInfo userInfo = Session["UserInfo"] as UserInfo;
            objUpdatePassword.LoginName = userInfo.LoginName;
            objUpdatePassword.UserID = Convert.ToInt32(userInfo.UserID);
            //objUpdatePassword.ClientID = userInfo.ClientID;
            objUpdatePassword.UserName = userInfo.UserName;
            return View(objUpdatePassword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(UpdatePassword objUpdatePassword)
        {
            UserAccountBL objUserAccountBL = new UserAccountBL();
            string request = "Change";

            if (ModelState.IsValid)
            {

                bool userExists = UserAccountRepository.UpdatePasswordVerifyUser(objUpdatePassword, request);
                if (userExists == true)
                {
                    bool ChangePassword = objUserAccountBL.UpdatePassword(objUpdatePassword);

                    if (ChangePassword == true)
                    {
                        return Json(ChangePassword, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(ChangePassword, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return View(objUpdatePassword);
                }
            }
            else
            {
                return View(objUpdatePassword);
            }
        }
        public ActionResult Logout()
        {
            HttpContext.Session.RemoveAll();
            HttpContext.Session.Abandon();
            return View("~/Views/UserAccount/Logout.cshtml");
        }

    }
}