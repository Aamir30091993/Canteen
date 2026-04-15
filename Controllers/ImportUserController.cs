using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GCC_Canteen.Models;
using GCC_Canteen.App_Code;
using GCC_Canteen.ViewModel;
using GCC_Canteen.Models.Repository;
using GCC_Canteen.Models.BusinessLayer;
using System.IO;
using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;

namespace GCC_Canteen.Controllers
{
    public class ImportUserController : Controller
    {
        string SessionIdentifier = "";
        MMModel obj_db = new MMModel();
        // GET: UserUpload
        public ActionResult List(string sort = "", int page = 0)
        {
            if (Request.Headers["SessionIdentifier"] == null)
                SessionIdentifier = DateTime.UtcNow.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_');
            else
                SessionIdentifier = Request.Headers["SessionIdentifier"].ToString();

            TempData["SessionIdentifier"] = SessionIdentifier;
            Session["PageID"] = (int)CommonBase.Page.ImportUser;
            Session["ImportUserTargetDtl" + SessionIdentifier] = null;
            ViewBag.WriteAccess = (CommonBase.UserPageAccess((int)CommonBase.Page.ImportUser)).Count > 0
                ? (CommonBase.UserPageAccess((int)CommonBase.Page.ImportUser))[0].Write : true;
            GridSettingData gridSettingModel = new GridSettingData();


            WebGridDataLayer webgridData = new WebGridDataLayer();
            gridSettingModel = webgridData.GridData((int)CommonBase.Page.ImportUser, "ctrlGrid1");
            GridColumnGenerator columnGenerator = new GridColumnGenerator();
            gridSettingModel.WebGridColumns = columnGenerator.SetWebGridColumns(gridSettingModel);

            ImportUserListModel _ImportUserList = new ImportUserListModel();
            _ImportUserList.gridSettingData = gridSettingModel;
            List<ImportUserListValues> Data = new List<ImportUserListValues>();
            Common _Common = new Common();

            Data = _Common.GetListPageData<ImportUserListValues>((int)CommonBase.Page.ImportUser, "", "ctrlGrid1", page, sort, SessionIdentifier, gridSettingModel);

            _ImportUserList.ImportUserList = Data;
            return View(_ImportUserList);
        
        }

        public ActionResult New()
        {
            ImportUserViewModel _ImportUserViewModel = new ImportUserViewModel();
            ImportUserHdr _importUserHdr = new ImportUserHdr();
            _importUserHdr.ImportDate = DateTime.Now.ToString("dd/MM/yyyy");
            _importUserHdr.SessionIdentity = DateTime.UtcNow.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_');
            _ImportUserViewModel._ImportUserHdr = _importUserHdr;
            Session["ImportUserDtl" + _ImportUserViewModel._ImportUserHdr.SessionIdentity] = null;
            UserInfo userInfo = Session["UserInfo"] as UserInfo;
            var path = CommonRepository.GetSysConfig("4");
            var sampleFormatPath = path[0].Value;
            ViewBag.SampleFormat = sampleFormatPath + @"/ImportUser/TempUserImport.xlsx"; //userInfo.AttachConfig[1].FieldValue + @"/ImportUser/ImportUser.xlsx";
            //@"C:\Users\admin\Documents\Mood Meter Upload File\ImportUser.xlsx"; //userInfo.AttachConfig[1].FieldValue + "ImportUser/ImportUser.xlsx"; //TODO
           
            return View("New", _ImportUserViewModel);
        }

        [HttpPost]
        public ActionResult New(ImportUserViewModel _ImportUserViewModel)
        {
            //if (ModelState.IsValid)
            //{
            Session["ImportUserDtl" + _ImportUserViewModel._ImportUserHdr.SessionIdentity] = null;
            bool Validate = true; string Error = "";
            UserInfo userInfo = Session["UserInfo"] as UserInfo;
            //To get ExcelPath/FilePath
            string ExcelPath = ""; //"D:\\AmnnGCC_Canteen\\Temp User Import.xlsx";
            HttpPostedFileBase Attachement = null;
            if (System.Web.HttpContext.Current.Session["ImportUserAttachment" + _ImportUserViewModel._ImportUserHdr.AttachmentSessionKey] != null)
                Attachement = (HttpPostedFileBase)System.Web.HttpContext.Current.Session["ImportUserAttachment" + _ImportUserViewModel._ImportUserHdr.AttachmentSessionKey];
            var myPath = @"\ImportUser\";
            string file = "";
            if (Attachement.ContentLength > 0)
            {
                var fileName = "ImportUser_" + DateTime.Now.ToString("ddMMyyyy_HHmmssfff");
                string extn = Attachement.FileName.Substring(Attachement.FileName.LastIndexOf('.'), Attachement.FileName.Length - Attachement.FileName.LastIndexOf('.'));//Path.GetFileName(Attachement.FileName);
                var path = CommonRepository.GetSysConfig("5");
                var physicalPath = path[0].Value/*"C:\\WebApp\\GCC_Canteen"*/ + fileName + extn; //userInfo.AttachConfig[0].FieldValue + myPath + fileName + extn; //@"C:\Users\admin\Downloads\Mood Meter\" + fileName + extn; //TODO
                ExcelPath = physicalPath;
                Attachement.SaveAs(physicalPath);
                file = fileName + extn;
            }

            #region Validation
            //  string StdFilePath = @"C:\Users\admin\Documents\Mood Meter Upload File\ImportUser.xlsx"; //Path.Combine(userInfo.AttachConfig[3].FieldValue + myPath, "ImportUser.xlsx"); //TODO
            //DataTable dtStdFormat = Common.getImportStdColumn((int)CommonBase.Page.ImportUser);
            DataTable dtUpload = new DataTable();

            int header = 0;
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(ExcelPath, false))
            {
                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();
                foreach (Cell cell in rows.ElementAt(0))
                {
                    dtUpload.Columns.Add(Common.GetCellValue(spreadSheetDocument, cell));
                }
                foreach (Row row in rows) //Upload File
                {
                    DataRow tempRow = dtUpload.NewRow();
                    int columnIndex = 0;
                    if (!string.IsNullOrEmpty(row.FirstChild.InnerText))
                    {
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            // Gets the column index of the cell with data
                            int cellColumnIndex = (int)Common.GetColumnIndexFromName(Common.GetColumnName(cell.CellReference));
                            cellColumnIndex--; //zero based index
                            if (columnIndex < cellColumnIndex)
                            {
                                do
                                {
                                    tempRow[columnIndex] = ""; //Insert blank data here;
                                    columnIndex++;
                                }
                                while (columnIndex < cellColumnIndex);
                            }
                            tempRow[columnIndex] = Common.GetCellValue(spreadSheetDocument, cell);

                            columnIndex++;
                        }
                        dtUpload.Rows.Add(tempRow);
                    }
                    header++;
                    if (header > 1)
                        break;
                }
            }
            //if (dtStdFormat.Rows.Count == dtUpload.Columns.Count && dtUpload.Rows.Count > 1)
            //{
            //    for (int i = 0; i < dtStdFormat.Rows.Count; i++)
            //    {
            //        if (dtStdFormat.Rows[i]["FieldName"].ToString() != dtUpload.Rows[0][i].ToString())
            //        {
            //            Validate = false;
            //            Error = "Column name doesnt match the Sample format column name";
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    Validate = false;
            //    if (dtUpload.Rows.Count <= 1)
            //    {
            //        Error = "No Data in File";
            //    }
            //    else
            //    {
            //        Error = "File doesnt match the Sample format";
            //    }
            //}
            #endregion
            Validate = true;
            if (Validate)
            {
                DataTable dt = new DataTable();
                if (ExcelPath != "")
                {
                    using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(ExcelPath, false))
                    {
                        WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                        IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                        string relationshipId = sheets.First().Id.Value;
                        WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                        Worksheet workSheet = worksheetPart.Worksheet;
                        SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                        IEnumerable<Row> rows = sheetData.Descendants<Row>();
                        foreach (Cell cell in rows.ElementAt(0))
                        {
                            dt.Columns.Add(Common.GetCellValue(spreadSheetDocument, cell));
                        }
                        foreach (Row row in rows) //this will also include your header row...
                        {
                            DataRow tempRow = dt.NewRow();
                            int columnIndex = 0;
                            if (!string.IsNullOrEmpty(row.FirstChild.InnerText))
                            {
                                foreach (Cell cell in row.Descendants<Cell>())
                                {
                                    // Gets the column index of the cell with data
                                    int cellColumnIndex = (int)Common.GetColumnIndexFromName(Common.GetColumnName(cell.CellReference));
                                    cellColumnIndex--; //zero based index
                                    if (columnIndex < cellColumnIndex)
                                    {
                                        do
                                        {
                                            tempRow[columnIndex] = ""; //Insert blank data here;
                                            columnIndex++;
                                        }
                                        while (columnIndex < cellColumnIndex);
                                    }
                                    tempRow[columnIndex] = Common.GetCellValue(spreadSheetDocument, cell);

                                    columnIndex++;
                                }
                                dt.Rows.Add(tempRow);
                            }
                        }
                    }
                    dt.Rows.RemoveAt(0);
                }
                _ImportUserViewModel = ImportUserRepository.InsertnValidate(_ImportUserViewModel, dt, file);

                //return Json(_ImportUserViewModel._ImportUserHdr.ImportID, _ImportUserViewModel.ErrorMessage, JsonRequestBehavior.AllowGet);
                return Json(_ImportUserViewModel.ErrorMessage, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Error, JsonRequestBehavior.AllowGet);
            }
            //return Json(importClientViewModel, JsonRequestBehavior.AllowGet);
            //}
            //else
            //    return View();

        }

        #region Attachement
        public ActionResult UploadAttachment(HttpPostedFileBase AttachedFile)
        {
            string sessionkey = DateTime.UtcNow.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_');
            System.Web.HttpContext.Current.Session["ImportUserAttachment" + sessionkey] = AttachedFile;
            return Json(sessionkey, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveAttachment(string SessionKey)
        {
            System.Web.HttpContext.Current.Session["ImportUserAttachment" + SessionKey] = null;
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        #endregion

        public ActionResult Import(int ID)
        {
            string process = "";
            process = ImportUserRepository.Import(ID, (int)CommonBase.Page.ImportUser);
            return Json(process, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int ID)
        {
            Session["ImportUserDtl"] = null;
            UserInfo userInfo = Session["UserInfo"] as UserInfo;
            ViewBag.SampleFormat = userInfo.AttachConfig[1].FieldValue + @"/ImportUser/ImportUser.xlsx";//@"C:\Users\admin\Documents\Mood Meter Upload File\ImportUser.xlsx"; //userInfo.AttachConfig[2].FieldValue + "ImportVerticalwiseMonthlyTarget/ImportVerticalwiseMonthlyTarget.xlsx"; //TODO
            string Sessionkey = DateTime.UtcNow.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_');
            ImportUserViewModel _ImportUserViewModel = new ImportUserViewModel();
            _ImportUserViewModel = ImportUserRepository.getData(ID, Sessionkey, (int)CommonBase.Page.ImportUser);
            _ImportUserViewModel._ImportUserHdr.IsEdit = "1";
            
            if (_ImportUserViewModel._ImportUserHdr.ImportFilePath != "")
            {
                ImportUserAttach _attach = new ImportUserAttach();
                _attach.Status = true;
                _attach.ImportID = ID;
                _attach.Path = _ImportUserViewModel._ImportUserHdr.ImportFilePath;
                _ImportUserViewModel._ImportUserAttach = _attach;
            }

            return View("Edit", _ImportUserViewModel);
        }

        [HttpPost]
        public ActionResult Edit(ImportUserViewModel _ImportUserViewModel)
        {
            //if (ModelState.IsValid)
            //{
            UserInfo userInfo = Session["UserInfo"] as UserInfo;
            bool Validate = true; string Error = "";
            string ExcelPath = "";
            HttpPostedFileBase Attachement = null;
            if (System.Web.HttpContext.Current.Session["ImportUserAttachment" + _ImportUserViewModel._ImportUserHdr.AttachmentSessionKey] != null)
            {
                Attachement = (HttpPostedFileBase)System.Web.HttpContext.Current.Session["ImportUserAttachment" + _ImportUserViewModel._ImportUserHdr.AttachmentSessionKey];
                var myPath = @"\ImportUser\";
                string file = "";
                if (Attachement.ContentLength > 0)
                {
                    var fileName = "ImportUser_" + DateTime.Now.ToString("ddMMyyyy_HHmmssfff");
                    string extn = Attachement.FileName.Substring(Attachement.FileName.LastIndexOf('.'), Attachement.FileName.Length - Attachement.FileName.LastIndexOf('.'));//Path.GetFileName(Attachement.FileName);
                    var physicalPath = userInfo.AttachConfig[0].FieldValue + myPath + fileName + extn;//@"C:\Users\admin\Downloads\Mood Meter\" + fileName + extn; //userInfo.AttachConfig[1].FieldValue + myPath + fileName + extn; //TODO
                    ExcelPath = physicalPath;
                    Attachement.SaveAs(physicalPath);
                    file = fileName + extn;
                }

                #region Validation
                myPath = @"ImportUser\";
              //  string StdFilePath = @"C:\Users\admin\Documents\Mood Meter Upload File\ImportUser.xlsx"; //Path.Combine(userInfo.AttachConfig[3].FieldValue + myPath, "ImportUser.xlsx"); //TODO
                DataTable dtStdFormat = Common.getImportStdColumn((int)CommonBase.Page.ImportUser);
                DataTable dtUpload = new DataTable();

                int header = 0;
                using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(ExcelPath, false))
                {
                    WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                    IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                    string relationshipId = sheets.First().Id.Value;
                    WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                    Worksheet workSheet = worksheetPart.Worksheet;
                    SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                    IEnumerable<Row> rows = sheetData.Descendants<Row>();
                    foreach (Cell cell in rows.ElementAt(0))
                    {
                        dtUpload.Columns.Add(Common.GetCellValue(spreadSheetDocument, cell));
                    }
                    foreach (Row row in rows) //Upload File
                    {
                        DataRow tempRow = dtUpload.NewRow();
                        int columnIndex = 0;
                        if (!string.IsNullOrEmpty(row.FirstChild.InnerText))
                        {
                            foreach (Cell cell in row.Descendants<Cell>())
                            {
                                // Gets the column index of the cell with data
                                int cellColumnIndex = (int)Common.GetColumnIndexFromName(Common.GetColumnName(cell.CellReference));
                                cellColumnIndex--; //zero based index
                                if (columnIndex < cellColumnIndex)
                                {
                                    do
                                    {
                                        tempRow[columnIndex] = ""; //Insert blank data here;
                                        columnIndex++;
                                    }
                                    while (columnIndex < cellColumnIndex);
                                }
                                tempRow[columnIndex] = Common.GetCellValue(spreadSheetDocument, cell);

                                columnIndex++;
                            }
                            dtUpload.Rows.Add(tempRow);
                        }
                        header++;
                        if (header > 1)
                            break;
                    }
                }
                if (dtStdFormat.Rows.Count == dtUpload.Columns.Count && dtUpload.Rows.Count > 1)
                {
                    for (int i = 0; i < dtStdFormat.Rows.Count; i++)
                    {
                        if (dtStdFormat.Rows[i]["FieldName"].ToString() != dtUpload.Rows[0][i].ToString())
                        {
                            Validate = false;
                            Error = "Column name doesnt match the Sample format column name";
                            break;
                        }
                    }
                }
                else
                {
                    Validate = false;
                    if (dtUpload.Rows.Count <= 1)
                    {
                        Error = "No Data in File";
                    }
                    else
                    {
                        Error = "File doesnt match the Sample format";
                    }
                }
                #endregion

                if (Validate)
                {
                    DataTable dt = new DataTable();
                    if (ExcelPath != "")
                    {
                        using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(ExcelPath, false))
                        {
                            WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                            IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                            string relationshipId = sheets.First().Id.Value;
                            WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                            Worksheet workSheet = worksheetPart.Worksheet;
                            SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                            IEnumerable<Row> rows = sheetData.Descendants<Row>();
                            foreach (Cell cell in rows.ElementAt(0))
                            {
                                dt.Columns.Add(Common.GetCellValue(spreadSheetDocument, cell));
                            }
                            foreach (Row row in rows) //this will also include your header row...
                            {
                                DataRow tempRow = dt.NewRow();
                                int columnIndex = 0;
                                if (!string.IsNullOrEmpty(row.FirstChild.InnerText))
                                {
                                    foreach (Cell cell in row.Descendants<Cell>())
                                    {
                                        // Gets the column index of the cell with data
                                        int cellColumnIndex = (int)Common.GetColumnIndexFromName(Common.GetColumnName(cell.CellReference));
                                        cellColumnIndex--; //zero based index
                                        if (columnIndex < cellColumnIndex)
                                        {
                                            do
                                            {
                                                tempRow[columnIndex] = ""; //Insert blank data here;
                                                columnIndex++;
                                            }
                                            while (columnIndex < cellColumnIndex);
                                        }
                                        tempRow[columnIndex] = Common.GetCellValue(spreadSheetDocument, cell);

                                        columnIndex++;
                                    }
                                    dt.Rows.Add(tempRow);
                                }
                            }
                        }
                        dt.Rows.RemoveAt(0);
                    }
                    Session["ImportUserDtl" + _ImportUserViewModel._ImportUserHdr.SessionIdentity] = null;
                    _ImportUserViewModel = ImportUserRepository.Update(_ImportUserViewModel, dt, file);
                    return PartialView("_ParentDiv", _ImportUserViewModel);
                }
                else
                {
                    return Json(Error, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

            //return Json(importClientViewModel, JsonRequestBehavior.AllowGet);
            //}
            //else
            //    return View();

        }

        [HttpGet]
        public void Export(string SessionIdentity)
        {
            DataTable _import = (DataTable)Session["ImportUserDtl" + SessionIdentity];
            if (_import != null && _import.Rows.Count > 0)
            {

                string file = "ImportUser_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".xlsx";
                //DataTable dt = new DataTable();
                //dt = CreateExcelFile.ListToDataTable(_import);
                if (_import.Columns.Contains("ImportDtlID"))
                {
                    _import.Columns.Remove("ImportDtlID");
                }
                if (_import.Columns.Contains("ImportID"))
                {
                    _import.Columns.Remove("ImportID");
                }

                CreateExcelFile.CreateExcelDocument(_import, file, Response);
            }
        }

        public ActionResult Delete(int ID)
        {
            bool delete = ImportUserRepository.Delete(ID, (int)CommonBase.Page.ImportUser);
            return Json(delete, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ViewData1()
        {
            ImportUserViewModel _ImportUserViewModel = new ImportUserViewModel();
            _ImportUserViewModel = ImportUserRepository.getViewData();

            var tableData = _ImportUserViewModel._ImportTempViewData; // Adjust as per your actual model structure
            return View("New", _ImportUserViewModel);
            //return PartialView("_ParentDiv", tableData);

            //return View("New", _ImportUserViewModel);
        }
        [HttpGet]
        public ActionResult ViewData()
        {
            try
            {
                ImportUserViewModel _importUserViewModel = ImportUserRepository.getViewData();
                return Json(new { success = true, data = _importUserViewModel }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}