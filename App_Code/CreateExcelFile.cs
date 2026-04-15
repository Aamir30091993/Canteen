//#define INCLUDE_WEB_FUNCTIONS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.Web;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
namespace GCC_Canteen.App_Code
{



    public class CreateExcelFile
    {

        public static bool CreateExcelDocument<T>(List<T> list, string xlsxFilePath,object FilterDtl)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(ListToDataTable(list));

            return CreateExcelDocument(ds, xlsxFilePath, FilterDtl);
        }
        #region HELPER_FUNCTIONS
        //  This function is adapated from: http://www.codeguru.com/forum/showthread.php?t=450171
        //  My thanks to Carl Quirion, for making it "nullable-friendly".
        public static DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, GetNullableType(info.PropertyType)));
            }
            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    if (!IsNullableType(info.PropertyType))
                        row[info.Name] = info.GetValue(t, null);
                    else
                        row[info.Name] = (info.GetValue(t, null) ?? DBNull.Value);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        private static Type GetNullableType(Type t)
        {
            Type returnType = t;
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                returnType = Nullable.GetUnderlyingType(t);
            }
            return returnType;
        }
        private static bool IsNullableType(Type type)
        {
            return (type == typeof(string) ||
                    type.IsArray ||
                    (type.IsGenericType &&
                     type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))));
        }

        public static bool CreateExcelDocument(DataTable dt, string xlsxFilePath,object FilterDtl)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            bool result = CreateExcelDocument(ds, xlsxFilePath, FilterDtl);
            ds.Tables.Remove(dt);
            return result;
        }
        #endregion


        /// <summary>
        /// Create an Excel file, and write it out to a MemoryStream (rather than directly to a file)
        /// </summary>
        /// <param name="dt">DataTable containing the data to be written to the Excel.</param>
        /// <param name="filename">The filename (without a path) to call the new Excel file.</param>
        /// <param name="Response">HttpResponse of the current page.</param>
        /// <returns>True if it was created succesfully, otherwise false.</returns>
        public static bool CreateExcelDocument(DataTable dt, string filename, System.Web.HttpResponseBase Response,object FilterDtl)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable coydt = dt.Copy();
                ds.Tables.Add(coydt);
                CreateExcelDocumentAsStream(ds, filename, Response, FilterDtl);
                //ds.Tables.Remove(coydt);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                //return false;
                throw ex;
            }
        }

        public static bool CreateExcelDocument<T>(List<T> list, string filename, System.Web.HttpResponseBase Response,object FilterDtl)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(ListToDataTable(list));
                CreateExcelDocumentAsStream(ds, filename, Response, FilterDtl);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                //return false;
                throw ex;
            }
        }

        /// <summary>
        /// Create an Excel file, and write it out to a MemoryStream (rather than directly to a file)
        /// </summary>
        /// <param name="ds">DataSet containing the data to be written to the Excel.</param>
        /// <param name="filename">The filename (without a path) to call the new Excel file.</param>
        /// <param name="Response">HttpResponse of the current page.</param>
        /// <returns>Either a MemoryStream, or NULL if something goes wrong.</returns>
        public static void CreateExcelDocumentAsStream(DataSet ds, string filename, System.Web.HttpResponseBase Response,object FilterDtl)
        {
            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
                {
                    WriteExcelFile(ds, document, FilterDtl);
                }
                stream.Flush();
                stream.Position = 0;

                Response.ClearContent();
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";

                //  NOTE: If you get an "HttpCacheability does not exist" error on the following line, make sure you have
                //  manually added System.Web to this project's References.

                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                Response.AddHeader("content-disposition", "attachment; filename=" + filename);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                byte[] data1 = new byte[stream.Length];
                stream.Read(data1, 0, data1.Length);
                stream.Close();
                Response.BinaryWrite(data1);
                Response.Flush();
                Response.End();


            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                //return false;
                throw ex;
            }


        }
        //  End of "INCLUDE_WEB_FUNCTIONS" section

        /// <summary>
        /// Create an Excel file, and write it to a file.
        /// </summary>
        /// <param name="ds">DataSet containing the data to be written to the Excel.</param>
        /// <param name="excelFilename">Name of file to be written.</param>
        /// <returns>True if successful, false if something went wrong.</returns>
        public static bool CreateExcelDocument(DataSet ds, string excelFilename,object FilterDtl)
        {
            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(excelFilename, SpreadsheetDocumentType.Workbook))
                {
                    WriteExcelFile(ds, document, FilterDtl);
                }
                Trace.WriteLine("Successfully created: " + excelFilename);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                // return false;
                throw ex;
            }
        }

        private static void WriteExcelFile(DataSet ds, SpreadsheetDocument spreadsheet,object FilterDtl)
        {
            //  Create the Excel file contents.  This function is used when creating an Excel file either writing 
            //  to a file, or writing to a MemoryStream.
            spreadsheet.AddWorkbookPart();
            spreadsheet.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

            //   The following line of code (which prevents crashes in Excel 2010)
            spreadsheet.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

            //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
            WorkbookStylesPart workbookStylesPart = AddStyleSheet(spreadsheet);
            //WorkbookStylesPart workbookStylesPart = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
            //Stylesheet stylesheet = new Stylesheet();
            //workbookStylesPart.Stylesheet = stylesheet;

            //  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
            uint worksheetNumber = 1;
            foreach (DataTable dt in ds.Tables)
            {
                //  For each worksheet you want to create
                string workSheetID = "rId" + worksheetNumber.ToString();
                string worksheetName = dt.TableName;

                WorksheetPart newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
                newWorksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();
                newWorksheetPart.Worksheet.Append(AutoFit_Columns(dt));
                // create sheet data
                newWorksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

                // save worksheet
                WriteDataTableToExcelWorksheet(dt, newWorksheetPart, FilterDtl);
                newWorksheetPart.Worksheet.Save();

                // create the worksheet to workbook relation
                if (worksheetNumber == 1)
                    spreadsheet.WorkbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());

                spreadsheet.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                {
                    Id = spreadsheet.WorkbookPart.GetIdOfPart(newWorksheetPart),
                    SheetId = (uint)worksheetNumber,
                    Name = dt.TableName
                });

                worksheetNumber++;
            }

            spreadsheet.WorkbookPart.Workbook.Save();
        }
        private static void WriteDataTableToExcelWorksheet(DataTable dt, WorksheetPart worksheetPart, object FilterDtl)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();

            // Retrieve filter details
            dynamic filterDtl = FilterDtl;
            string reportName = "Consolidated";
            string fromDate = filterDtl.FromDate;
            string toDate = filterDtl.ToDate;
            string mealType = filterDtl.MealType; // You can set this based on your MealTypeID
            string mealSubType = filterDtl.MealSubType; // You can set this based on your MealSubTypeID
            string UserID = filterDtl.LoginID;

            // Create report details string
            string reportDetails = $"Report Name: {reportName}, From Date: {fromDate}, To Date: {toDate}, Meal Type: {mealType}, Meal Sub Type: {mealSubType}, User ID: {UserID}";

            // Append report details to cell A1
            uint rowIndex = 1;
            var headingRow = new Row { RowIndex = rowIndex };
            sheetData.Append(headingRow);
            AppendTextCell("A1", reportDetails, headingRow);
            MergeCellsInRow(worksheet, "A1", GetExcelColumnName(dt.Columns.Count - 1) + "1");

            // Add header row
            rowIndex++;
            var headerRow = new Row { RowIndex = rowIndex };
            sheetData.Append(headerRow);

            // Write header columns
            string[] excelColumnNames = new string[dt.Columns.Count];
            for (int colInx = 0; colInx < dt.Columns.Count; colInx++)
            {
                DataColumn col = dt.Columns[colInx];
                AppendTextCell(GetExcelColumnName(colInx) + rowIndex.ToString(), col.ColumnName, headerRow);
            }

            // Write data rows
            foreach (DataRow dr in dt.Rows)
            {
                rowIndex++;
                var newExcelRow = new Row { RowIndex = rowIndex };
                sheetData.Append(newExcelRow);

                for (int colInx = 0; colInx < dt.Columns.Count; colInx++)
                {
                    string cellValue = dr[colInx].ToString();

                    // Append cells based on data type
                    AppendTextCell(GetExcelColumnName(colInx) + rowIndex.ToString(), cellValue, newExcelRow);
                }
            }
        }

        private static void AppendTextCell(string cellReference, string cellValue, Row row)
        {
            if (row.RowIndex == 1 || row.RowIndex == 2)
            {
                var cell = new Cell() { CellReference = cellReference, DataType = CellValues.String, StyleIndex = Convert.ToUInt32(1) };
                var cellValueObj = new CellValue(cellValue);
                cell.Append(cellValueObj);
                row.Append(cell);
            }
            else
            {
                var cell = new Cell() { CellReference = cellReference, DataType = CellValues.String };
                var cellValueObj = new CellValue(cellValue);
                cell.Append(cellValueObj);
                row.Append(cell);
            }
        }

        private static void MergeCellsInRow(Worksheet worksheet, string startCell, string endCell)
        {
            MergeCells mergeCells;
            if (worksheet.Elements<MergeCells>().Any())
            {
                mergeCells = worksheet.Elements<MergeCells>().First();
            }
            else
            {
                mergeCells = new MergeCells();
                worksheet.Append(mergeCells);
            }

            // Create the merged cell and append it to the mergeCells collection.
            MergeCell mergeCell = new MergeCell() { Reference = new StringValue(startCell + ":" + endCell) };
            mergeCells.Append(mergeCell);
        }

        private static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber + 1;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
        //private static void WriteDataTableToExcelWorksheet(DataTable dt, WorksheetPart worksheetPart,object FilterDtl)
        //{
        //    var worksheet = worksheetPart.Worksheet;
        //    var sheetData = worksheet.GetFirstChild<SheetData>();

        //    string cellValue = "";

        //    // Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
        //    // We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
        //    // cells of data, we'll know if to write Text values or Numeric cell values.
        //    int numberOfColumns = dt.Columns.Count;
        //    bool[] IsNumericColumn = new bool[numberOfColumns];

        //    string[] excelColumnNames = new string[numberOfColumns];
        //    for (int n = 0; n < numberOfColumns; n++)
        //        excelColumnNames[n] = GetExcelColumnName(n);

        //    // Add the heading row
        //    uint rowIndex = 1;
        //    var headingRow = new Row { RowIndex = rowIndex };
        //    sheetData.Append(headingRow);
        //    dynamic filterDtl = FilterDtl;
        //    string reportName = "Consolidated";
        //    string fromDate = filterDtl.FromDate;
        //    string toDate = filterDtl.ToDate;
        //    string mealType = filterDtl.MealType; // You can set this based on your MealTypeID
        //    string mealSubType = filterDtl.MealSubType; // You can set this based on your MealSubTypeID
        //    string UserID = filterDtl.LoginID;
        //    string reportDetails = $"Report Name: {reportName}, From Date: {fromDate}, To Date: {toDate}, Meal Type: {mealType}, Meal Sub Type: {mealSubType},User ID :{UserID}";
        //    AppendTextCell("A1", reportDetails, headingRow);
        //    MergeCellsInRow(worksheet, "A1", GetExcelColumnName(numberOfColumns - 1) + "1");

        //    // Create the Header row in our Excel Worksheet
        //    rowIndex++;
        //    var headerRow = new Row { RowIndex = rowIndex }; // add a row at the top of spreadsheet
        //    sheetData.Append(headerRow);

        //    for (int colInx = 0; colInx < numberOfColumns; colInx++)
        //    {
        //        DataColumn col = dt.Columns[colInx];
        //        AppendTextCell(excelColumnNames[colInx] + rowIndex.ToString(), col.ColumnName, headerRow);
        //        IsNumericColumn[colInx] = (col.DataType.FullName == "System.Decimal") || (col.DataType.FullName == "System.Int32");
        //    }

        //    // Now, step through each row of data in our DataTable...
        //    double cellNumericValue = 0;
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        // Create a new row, and append a set of this row's data to it.
        //        ++rowIndex;
        //        var newExcelRow = new Row { RowIndex = rowIndex }; // add a row at the top of spreadsheet
        //        sheetData.Append(newExcelRow);

        //        for (int colInx = 0; colInx < numberOfColumns; colInx++)
        //        {
        //            cellValue = dr.ItemArray[colInx].ToString();

        //            // Create cell with data
        //            if (IsNumericColumn[colInx])
        //            {
        //                // For numeric cells, make sure our input data IS a number, then write it out to the Excel file.
        //                // If this numeric value is NULL, then don't write anything to the Excel file.
        //                cellNumericValue = 0;
        //                if (double.TryParse(cellValue, out cellNumericValue))
        //                {
        //                    cellValue = cellNumericValue.ToString();
        //                    AppendNumericCell(excelColumnNames[colInx] + rowIndex.ToString(), cellValue, newExcelRow);
        //                }
        //            }
        //            else
        //            {
        //                // For text cells, just write the input data straight out to the Excel file.
        //                AppendTextCell(excelColumnNames[colInx] + rowIndex.ToString(), cellValue, newExcelRow);
        //            }
        //        }
        //    }

        //    // Apply thin borders to all cells
        //    //ApplyThinBorderToAllCells(worksheet);
        //}
        //private static void MergeCellsInRow(Worksheet worksheet, string cell1Name, string cell2Name)
        //{
        //    MergeCells mergeCells;

        //    if (worksheet.Elements<MergeCells>().Count() > 0)
        //    {
        //        mergeCells = worksheet.Elements<MergeCells>().First();
        //    }
        //    else
        //    {
        //        mergeCells = new MergeCells();

        //        if (worksheet.Elements<CustomSheetView>().Count() > 0)
        //        {
        //            worksheet.InsertAfter(mergeCells, worksheet.Elements<CustomSheetView>().First());
        //        }
        //        else
        //        {
        //            worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
        //        }
        //    }

        //    MergeCell mergeCell = new MergeCell()
        //    {
        //        Reference = new StringValue(cell1Name + ":" + cell2Name)
        //    };
        //    mergeCells.Append(mergeCell);
        //}
        private static void WriteDataTableToExcelWorksheet0(DataTable dt, WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();

            string cellValue = "";

            //  Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
            //
            //  We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
            //  cells of data, we'll know if to write Text values or Numeric cell values.
            int numberOfColumns = dt.Columns.Count;
            bool[] IsNumericColumn = new bool[numberOfColumns];

            string[] excelColumnNames = new string[numberOfColumns];
            for (int n = 0; n < numberOfColumns; n++)
                excelColumnNames[n] = GetExcelColumnName(n);

            //
            //  Create the Header row in our Excel Worksheet
            //
            uint rowIndex = 1;

            var headerRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
            sheetData.Append(headerRow);

            for (int colInx = 0; colInx < numberOfColumns; colInx++)
            {
                DataColumn col = dt.Columns[colInx];
                AppendTextCell(excelColumnNames[colInx] + "1", col.ColumnName, headerRow);
                IsNumericColumn[colInx] = (col.DataType.FullName == "System.Decimal") || (col.DataType.FullName == "System.Int32");
            }

            //
            //  Now, step through each row of data in our DataTable...
            //
            double cellNumericValue = 0;
            foreach (DataRow dr in dt.Rows)
            {
                // ...create a new row, and append a set of this row's data to it.
                ++rowIndex;
                var newExcelRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
                sheetData.Append(newExcelRow);

                for (int colInx = 0; colInx < numberOfColumns; colInx++)
                {
                    cellValue = dr.ItemArray[colInx].ToString();

                    // Create cell with data
                    if (IsNumericColumn[colInx])
                    {
                        //  For numeric cells, make sure our input data IS a number, then write it out to the Excel file.
                        //  If this numeric value is NULL, then don't write anything to the Excel file.
                        cellNumericValue = 0;
                        if (double.TryParse(cellValue, out cellNumericValue))
                        {
                            cellValue = cellNumericValue.ToString();
                            AppendNumericCell(excelColumnNames[colInx] + rowIndex.ToString(), cellValue, newExcelRow);
                        }
                    }
                    else
                    {
                        //  For text cells, just write the input data straight out to the Excel file.
                        AppendTextCell(excelColumnNames[colInx] + rowIndex.ToString(), cellValue, newExcelRow);
                    }
                }
            }
        }

        //private static void AppendTextCell(string cellReference, string cellStringValue, Row excelRow)
        //{
        //    if (excelRow.RowIndex == 1)
        //    {
        //        Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.String, StyleIndex = Convert.ToUInt32(1) };
        //        CellValue cellValue = new CellValue();
        //        cellValue.Text = cellStringValue;
        //        cell.Append(cellValue);
        //        excelRow.Append(cell);
        //    }
        //    if (excelRow.RowIndex == 2)
        //    {
        //        Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.String, StyleIndex = Convert.ToUInt32(1) };
        //        CellValue cellValue = new CellValue();
        //        cellValue.Text = cellStringValue;
        //        cell.Append(cellValue);
        //        excelRow.Append(cell);
        //    }
        //    else
        //    {
        //        Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.String, StyleIndex = Convert.ToUInt32(2) };
        //        CellValue cellValue = new CellValue();
        //        cellValue.Text = cellStringValue;
        //        cell.Append(cellValue);
        //        excelRow.Append(cell);
        //    }
        //    //  Add a new Excel Cell to our Row 
            
        //}

        private static void AppendNumericCell(string cellReference, string cellStringValue, Row excelRow)
        {
            //  Add a new Excel Cell to our Row 
            Cell cell = new Cell() { CellReference = cellReference };
            CellValue cellValue = new CellValue();
            cellValue.Text = cellStringValue;
            cell.Append(cellValue);
            excelRow.Append(cell);
        }

        //private static string GetExcelColumnName(int columnIndex)
        //{
        //    //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
        //    //
        //    //  eg  GetExcelColumnName(0) should return "A"
        //    //      GetExcelColumnName(1) should return "B"
        //    //      GetExcelColumnName(25) should return "Z"
        //    //      GetExcelColumnName(26) should return "AA"
        //    //      GetExcelColumnName(27) should return "AB"
        //    //      ..etc..
        //    //
        //    if (columnIndex < 26)
        //        return ((char)('A' + columnIndex)).ToString();

        //    char firstChar = (char)('A' + (columnIndex / 26) - 1);
        //    char secondChar = (char)('A' + (columnIndex % 26));

        //    return string.Format("{0}{1}", firstChar, secondChar);
        //}
        private static WorkbookStylesPart AddStyleSheet0(SpreadsheetDocument workbookPart)
        {
            WorkbookStylesPart stylesheet = workbookPart.AddNewPart<WorkbookStylesPart>();
            stylesheet.Stylesheet = new Stylesheet();

            // Create Fonts
            Fonts fonts = new Fonts(
                new Font( // Index 0 - Default font
                    new FontSize { Val = 11 },
                    new Color { Rgb = new HexBinaryValue { Value = "000000" } },
                    new FontName { Val = "Calibri" }
                ),
                new Font( // Index 1 - Bold font
                    new Bold(),
                    new FontSize { Val = 11 },
                    new Color { Rgb = new HexBinaryValue { Value = "000000" } },
                    new FontName { Val = "Calibri" }
                )
            );

            // Create Fills
            Fills fills = new Fills(
                new Fill( // Index 0 - No fill
                    new PatternFill { PatternType = PatternValues.None }
                ),
                new Fill( // Index 1 - Gray fill
                    new PatternFill { PatternType = PatternValues.Gray125 }
                )
            );

            // Create Borders
            Borders borders = new Borders(
                new Border( // Index 0 - Default border
                    new LeftBorder(),
                    new RightBorder(),
                    new TopBorder(),
                    new BottomBorder(),
                    new DiagonalBorder()
                ),
                new Border( // Index 1 - Thin border
                    new LeftBorder(new Color { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color { Auto = true }) { Style = BorderStyleValues.Thin },
                    new BottomBorder(new Color { Auto = true }) { Style = BorderStyleValues.Thin },
                    new DiagonalBorder()
                ),// Index 2 - All side border
                 new Border(new LeftBorder(new Color() { Rgb = HexBinaryValue.FromString("000000") }) { Style = BorderStyleValues.Thin }, new RightBorder(new Color() { Rgb = HexBinaryValue.FromString("000000") }) { Style = BorderStyleValues.Thin },
            new TopBorder(new Color() { Rgb = HexBinaryValue.FromString("000000") }) { Style = BorderStyleValues.Thin }, new BottomBorder(new Color() { Rgb = HexBinaryValue.FromString("000000") }) { Style = BorderStyleValues.Thin }
            ));

            // Create Cell Style Formats
            CellStyleFormats cellStyleFormats = new CellStyleFormats(
                new CellFormat() // Index 0 - Default cell style
            );

            // Create Cell Formats
            CellFormats cellFormats = new CellFormats(
                new CellFormat { FontId = 0, FillId = 0, BorderId = 0, ApplyFont = true, ApplyFill = true, ApplyBorder = true }, // Index 0 - Default cell format
                new CellFormat { FontId = 1, FillId = 0, BorderId = 1, ApplyFont = true, ApplyFill = true, ApplyBorder = true }, // Index 1 - Bold text with border
                new CellFormat { FontId = 1, FillId = 0, BorderId = 2, ApplyFont = true, ApplyFill = true, ApplyBorder = true } // Index 2 - Bold text with border
            );

            // Create Columns for Default Widths
            Columns columns = new Columns();
            for (uint col = 1; col <= 100; col++) // Adjust the loop based on the number of columns needed
            {
                Column column = new Column();
                column.Min = col;
                column.Max = col;
                column.Width = 15; // Default width (adjust as needed)
                column.CustomWidth = true;
                columns.Append(column);
            }

            // Append elements to stylesheet
            stylesheet.Stylesheet.Append(fonts);
            stylesheet.Stylesheet.Append(fills);
            stylesheet.Stylesheet.Append(borders);
            stylesheet.Stylesheet.Append(cellStyleFormats);
            stylesheet.Stylesheet.Append(cellFormats);
            stylesheet.Stylesheet.Append(columns);

            // Save the stylesheet
            stylesheet.Stylesheet.Save();
            return stylesheet;
        }
        private static WorkbookStylesPart AddStyleSheet(SpreadsheetDocument spreadsheet)
        {
            WorkbookStylesPart stylesheet = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();

            Stylesheet workbookstylesheet = new Stylesheet();

            Font font0 = new Font();         // Default font

            Font font1 = new Font();         // Bold font
            Bold bold = new Bold();
            font1.Append(bold);

            Fonts fonts = new Fonts();      // <APENDING Fonts>
            fonts.Append(font0);
            fonts.Append(font1);

            // <Fills>
            Fill fill0 = new Fill();        // Default fill

            Fills fills = new Fills();      // <APENDING Fills>
            fills.Append(fill0);

            // <Borders>
            Border border0 = new Border();     // Defualt border

            Borders borders = new Borders();    // <APENDING Borders>
            borders.Append(border0);

            // <CellFormats>
            CellFormat cellformat0 = new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 }; // Default style : Mandatory | Style ID =0

            CellFormat cellformat1 = new CellFormat() { FontId = 1 };  // Style with Bold text ; Style ID = 1


            // <APENDING CellFormats>
            CellFormats cellformats = new CellFormats();
            cellformats.Append(cellformat0);
            cellformats.Append(cellformat1);


            // Append FONTS, FILLS , BORDERS & CellFormats to stylesheet <Preserve the ORDER>
            workbookstylesheet.Append(fonts);
            workbookstylesheet.Append(fills);
            workbookstylesheet.Append(borders);
            workbookstylesheet.Append(cellformats);

            // Finalize
            stylesheet.Stylesheet = workbookstylesheet;
            stylesheet.Stylesheet.Save();

            return stylesheet;
        }
        private static WorkbookStylesPart AddStyleSheet1(SpreadsheetDocument spreadsheet)
        {
            WorkbookStylesPart stylesheet = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();

            Stylesheet workbookstylesheet = new Stylesheet();

            Font font0 = new Font();         // Default font

            Font font1 = new Font();         // Bold font
            Bold bold = new Bold();
            font1.Append(bold);
            DocumentFormat.OpenXml.Spreadsheet.Font ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
            FontName ftn = new FontName();
            FontSize ftsz = new FontSize();
            //ft 1
            //Cols Header
            ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
            ftn = new FontName();
            ftsz = new FontSize();
            ftn.Val = StringValue.FromString("Calibri");
            ftsz.Val = DoubleValue.FromDouble(16);
            ft.FontName = ftn;
            ft.FontSize = ftsz;
            //ft.Color = new Color() { Rgb = new HexBinaryValue("1F497D") };
            ft.Bold = new Bold();


            Fonts fonts = new Fonts();      // <APENDING Fonts>
            fonts.Append(font0);
            fonts.Append(font1);
            fonts.Append(ft);

            Fills fills = new Fills();
            Fill fill;
            PatternFill patternFill;
            //0
            fill = new Fill();
            patternFill = new PatternFill { PatternType = PatternValues.None };
            fill.PatternFill = patternFill;
            fills.Append(fill);
            //1
            fill = new Fill();
            patternFill = new PatternFill { PatternType = PatternValues.Gray125 };
            fill.PatternFill = patternFill;
            fills.Append(fill);
            //for header
            //2
            fill = new Fill();
            patternFill = new PatternFill
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor()
            };
            patternFill.ForegroundColor.Rgb = HexBinaryValue.FromString("c0c0c0");
            fill.PatternFill = patternFill;
            fills.Append(fill);
            fills.Count = UInt32Value.FromUInt32((uint)fills.ChildElements.Count);

            // <Borders>
            //Border border0 = new Border();     // Defualt border

            //Borders borders = new Borders();    // <APENDING Borders>
            //borders.Append(border0);
            // <Borders>
            Border border0 = new Border();     // Default border
            Border border1 = new Border(new LeftBorder(new Color() { Rgb = HexBinaryValue.FromString("000000") }) { Style = BorderStyleValues.Thin }); // Custom border 1
            Border border2 = new Border(new LeftBorder(new Color() { Rgb = HexBinaryValue.FromString("000000") }) { Style = BorderStyleValues.Thin }, new RightBorder(new Color() { Rgb = HexBinaryValue.FromString("000000") }) { Style = BorderStyleValues.Thin },
            new TopBorder(new Color() { Rgb = HexBinaryValue.FromString("000000") }) { Style = BorderStyleValues.Thin }, new BottomBorder(new Color() { Rgb = HexBinaryValue.FromString("000000") }) { Style = BorderStyleValues.Thin });


            Borders borders = new Borders();    // <APPENDING Borders>
            borders.Append(border0);
            borders.Append(border1);
            borders.Append(border2);

            // <CellFormats>
            CellFormat cellformat0 = new CellFormat() { FontId = 1 }; // Default style : Mandatory | Style ID =0

            //CellFormat cellformat1 = new CellFormat() { FontId = 1 };  // Style with Bold text ; Style ID = 1
            CellFormat cellformat1 = new CellFormat(new Alignment() { WrapText = true, Vertical = VerticalAlignmentValues.Top }) { BorderId = 2 }; // Style with textwrap set (style ID = 1)
            CellFormat Number = new CellFormat() { FillId = 0, FontId = 0, NumberFormatId = 9, FormatId = 0, ApplyNumberFormat = true }; //(style ID = 2)
            CellFormat headerColor = new CellFormat() { FontId = 1, FillId = 2, BorderId = 2, Alignment = new Alignment() { WrapText = true, Vertical = VerticalAlignmentValues.Top, Horizontal = HorizontalAlignmentValues.Center } }; // To Color Header Rows (Style ID = 3)
            CellFormat headerText = new CellFormat() { FontId = 1 }; // Style with textwrap set (style ID = 4)
            // Add a new CellFormat for centered style (style ID = 5)
            CellFormat centeredStyle = new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Top }) { BorderId = 2 }; //(style ID = 5)
            CellFormat rightAlignStyle = new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Right, Vertical = VerticalAlignmentValues.Top }) { BorderId = 2 }; //(style ID = 6)
            CellFormat cellFormatWithFontSize16 = new CellFormat() { FontId = 2 };//(style id=7)
            // <APENDING CellFormats>
            CellFormats cellformats = new CellFormats();
            cellformats.Append(cellformat0);
            cellformats.Append(cellformat1);
            cellformats.Append(Number);
            cellformats.Append(headerColor);
            cellformats.Append(headerText);
            cellformats.Append(centeredStyle);
            cellformats.Append(rightAlignStyle);
            cellformats.Append(cellFormatWithFontSize16);





            // <Columns> for custom column width
            Columns columns = new Columns();
            Column customColumn = new Column() { Min = 1, Max = 1, Width = 1000, CustomWidth = true }; // Customize as needed
            columns.Append(customColumn);


            // Append FONTS, FILLS , BORDERS & CellFormats to stylesheet <Preserve the ORDER>
            workbookstylesheet.Append(fonts);
            workbookstylesheet.Append(fills);
            workbookstylesheet.Append(borders);
            workbookstylesheet.Append(cellformats);
            workbookstylesheet.Append(columns);

            // Finalize
            stylesheet.Stylesheet = workbookstylesheet;
            stylesheet.Stylesheet.Save();

            return stylesheet;

        }
        private static Columns AutoFit_Columns(DataTable dt)
        {
            Columns cols = new Columns();
            int Excel_column = 0;

            //DataTable dt = new DataTable();
            //dt = (DataTable)dgv.DataSource;
            DataTable dtClone = dt.Clone();
            foreach (DataRow dr in dt.Rows)
            {
                dtClone.ImportRow(dr);
            }
            dtClone.AcceptChanges();
            int colCount = dtClone.Columns.Count;
            for (int col = 0; col < colCount; col++)
            {
                //double max_width = 14.5f; // something like default Excel width, I'm not sure about this


                //We search for longest string in each column and convert that into double to get desired width 
                dtClone.Columns.Add("len" + dtClone.Columns[col].ColumnName.Replace(" ", ""), typeof(int));
                foreach (DataRow dr in dtClone.Rows)
                {
                    dr["len" + dtClone.Columns[col].ColumnName.Replace(" ", "")] = dr[col].ToString().Length;
                }
                //dtClone.DefaultView.Sort = "len" + dtClone.Columns[col].ColumnName + " desc";

                DataView dv = new DataView(dtClone);
                dv.Sort = "len" + dtClone.Columns[col].ColumnName.Replace(" ", "") + " desc";

                //string longest_string = dtClone.AsEnumerable()
                //     .Select(row => row[col].ToString())
                //     .OrderByDescending(st => st.Length).FirstOrDefault();

                //string longest_string1 = dv.ToTable().Rows[0][col].ToString();
                //int longestStringLength = longest_string1.Length;

                //if (dtClone.Columns[col].ColumnName.Length > longestStringLength)
                //    longest_string1 = dtClone.Columns[col].ColumnName;


                //     dtClone.Columns.RemoveAt(dtClone.Columns.Count - 1);
                double cell_width = 15;//GetWidth(new System.Drawing.Font("Calibri", 12), longest_string1);


                if (col == 0) //first column of Datagridview is index 0, but there is no 0 index of column in Excel, careful with that !!!
                {
                    Excel_column = 1;
                }

                //now append column to worksheet, calculations done
                Column c = new Column() { Min = Convert.ToUInt32(Excel_column), Max = Convert.ToUInt32(Excel_column), Width = cell_width, CustomWidth = true };
                cols.Append(c);

                Excel_column++;
            }
            return cols;
        }
    }
}
