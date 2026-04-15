using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GCC_Canteen.Models.Repository;
using GCC_Canteen.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace GCC_Canteen.Models.BusinessLayer
{
    public class Common
    {
        internal List<T> GetListPageData<T>(int PageID, string WhereClouse, string Control, int page, string sort, string SessionIdentifier = "0", GridSettingData _GridSettingData = null)
        {
            if ((sort == "" && page == 0 && WhereClouse != "") || (sort == "" && page == 0 && WhereClouse == ""))
            {
                HttpContext.Current.Session["whereClouse_" + SessionIdentifier] = null;
            }
            List<T> data = new List<T>();
            if (HttpContext.Current.Session["whereClouse_" + SessionIdentifier] == null && sort == "" && page == 0 && WhereClouse == "")
                data = CommonRepository.getList<T>(PageID, "", "ctrlGrid1", page, sort, SessionIdentifier);
            else if ((HttpContext.Current.Session["whereClouse_" + SessionIdentifier] != null && sort != "") || (HttpContext.Current.Session["whereClouse_" + SessionIdentifier] != null && page != 0))
                data = CommonRepository.getList<T>(PageID, HttpContext.Current.Session["whereClouse_" + SessionIdentifier].ToString(), "ctrlGrid1", page, sort, SessionIdentifier);
            else if ((HttpContext.Current.Session["whereClouse_" + SessionIdentifier] == null && sort != "") || (HttpContext.Current.Session["whereClouse_" + SessionIdentifier] == null && page != 0))
                data = CommonRepository.getList<T>(PageID, "", "ctrlGrid1", page, sort, SessionIdentifier);
            else if ((HttpContext.Current.Session["whereClouse_" + SessionIdentifier] == null && sort == "" && page == 0 && WhereClouse != "") || (HttpContext.Current.Session["whereClouse_" + SessionIdentifier] != null && sort == "" && page == 0 && WhereClouse != ""))
                data = CommonRepository.getList<T>(PageID, WhereClouse, "ctrlGrid1", page, sort, SessionIdentifier);
            else
                data = CommonRepository.getList<T>(PageID, "", "ctrlGrid1", page, sort, SessionIdentifier);

            //if (_GridSettingData != null && _GridSettingData.WebGridSetting.IsExportAllowed == "1")
            //    GetExportData(PageID, Sessi onIdentifier, _GridSettingData.WebGridColumnSetting, data);

            return data;
        }

        public static string GetColumnName(string cellReference)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }
        public static DataTable getImportStdColumn(int PageID, int? ClientType = null)
        {
            UserInfo userinfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MMModel"].ConnectionString);
            SqlCommand cmd = new SqlCommand("[spGetImportStdColumn]", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter paramPageID = new SqlParameter("@PageID", System.Data.SqlDbType.Int);
            paramPageID.Value = PageID;
            cmd.Parameters.Add(paramPageID);

            //if (ClientType != null)
            //{
            //    SqlParameter paramControl = new SqlParameter("@ClientType", System.Data.SqlDbType.Int);
            //    paramControl.Value = ClientType;
            //    cmd.Parameters.Add(paramControl);
            //}
            SqlParameter paramUser = new SqlParameter("@UserID", System.Data.SqlDbType.Int);
            paramUser.Value = userinfo.UserID;
            cmd.Parameters.Add(paramUser);
            try
            {

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                da.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        public static int? GetColumnIndexFromName(string columnName)
        {

            //return columnIndex;
            string name = columnName;
            int number = 0;
            int pow = 1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                number += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }
            return number;
        }
        //public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        //{
        //    SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
        //    if (cell.CellValue == null)
        //    {
        //        return "";
        //    }
        //    string value = cell.CellValue.InnerXml;
        //    if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        //    {
        //        return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
        //    }
        //    else
        //    {
        //        return value;
        //    }
        //}
        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            if (cell.CellValue == null)
            {
                return "";
            }
            string value = cell.CellValue.InnerXml;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else if (cell.StyleIndex != null && cell.StyleIndex.Value > 0) // Check if the cell has a style index
            {
                // Check if the style is a date style
                CellFormat cf = (CellFormat)document.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ChildElements[(int)cell.StyleIndex.Value];
                if (cf.NumberFormatId > 0 && cf.NumberFormatId <= 14) // Assuming date formats 1-14 are date formats
                {
                    double numericValue;
                    if (Double.TryParse(value, out numericValue))
                    {
                        DateTime dateValue = DateTime.FromOADate(numericValue);
                        return dateValue.ToString("MM/dd/yyyy"); // Adjust the date format as needed
                    }
                }
            }
            return value;
        }
        public class AutoComplete
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }
    }
}