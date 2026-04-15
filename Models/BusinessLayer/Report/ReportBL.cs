using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GCC_Canteen.Models.Report
{
    public class ReportBL
    {
        public static void CreateExcelDocumentAsStream(DataSet ds, string filename, System.Web.HttpResponseBase Response)
        {
            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
                {
                    WriteExcelFile(ds, document);
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
                //  Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                //return false;
                throw ex;
            }

        }

        private static void WriteExcelFile(DataSet ds, SpreadsheetDocument spreadsheet)
        {
            //  Create the Excel file contents.  This function is used when creating an Excel file either writing 
            //  to a file, or writing to a MemoryStream.
            spreadsheet.AddWorkbookPart();
            spreadsheet.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

            //   The following line of code (which prevents crashes in Excel 2010)
            spreadsheet.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

            //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
            WorkbookStylesPart workbookStylesPart = ReportBL.AddStyleSheet(spreadsheet, ds);
            //spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
            //  Stylesheet stylesheet = new Stylesheet();
            //  workbookStylesPart.Stylesheet = stylesheet;

            //  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
            uint worksheetNumber = 1;
            foreach (DataTable dt in ds.Tables)
            {
                //  For each worksheet you want to create
                string workSheetID = "rId" + worksheetNumber.ToString();
                string worksheetName = dt.TableName;

                WorksheetPart newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
                newWorksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

                // create sheet data
                newWorksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

                // save worksheet
                WriteDataTableToExcelWorksheet(dt, newWorksheetPart);
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

        private static void WriteDataTableToExcelWorksheet(DataTable dt, WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();

            string cellValue = "";
            var ReportName = new Row { RowIndex = 1 };  // add a row at the top of spreadsheet
            sheetData.Append(ReportName);
            Cell cell = new Cell() { CellReference = "A1", DataType = CellValues.String };
            CellValue cellvalue = new CellValue();
            cellvalue.Text = dt.Rows[0][0].ToString();
            cell.Append(cellvalue);
            ReportName.Append(cell);

            var ReportParam = new Row { RowIndex = 2 };  // add a row at the top of spreadsheet
            sheetData.Append(ReportParam);
            Cell cell1 = new Cell() { CellReference = "A2", DataType = CellValues.String };
            CellValue cellvalues = new CellValue();
            cellvalues.Text = dt.Rows[1][0].ToString();
            cell1.Append(cellvalues);
            ReportParam.Append(cell1);
            if (dt.Rows[2][1].ToString() == "" && dt.Rows[2][dt.Columns.Count - 1].ToString() == "") //table snapshot
            {
                var ReportParam2 = new Row { RowIndex = 3 };  // add a row at the top of spreadsheet
                sheetData.Append(ReportParam2);
                Cell cell2 = new Cell() { CellReference = "A3", DataType = CellValues.String };
                CellValue cellvalues2 = new CellValue();
                cellvalues2.Text = dt.Rows[2][0].ToString();
                cell2.Append(cellvalues2);
                ReportParam2.Append(cell2);
            }
            //  Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
            //
            //  We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
            //  cells of data, we'll know if to write Text values or Numeric cell values.
            int numberOfColumns = dt.Columns.Contains("Style") == true ?
                                        (dt.Columns.Contains("Grouping") == true ?
                                            (dt.Columns.Contains("StyleIndex") == true ? dt.Columns.Count - 3 : dt.Columns.Count - 2)
                                        : (dt.Columns.Contains("StyleIndex") == true ? dt.Columns.Count - 2 : dt.Columns.Count - 1))
                                : (dt.Columns.Contains("Grouping") == true ?
                                    (dt.Columns.Contains("StyleIndex") == true ? dt.Columns.Count - 2 : dt.Columns.Count - 1)
                                    : (dt.Columns.Contains("StyleIndex") == true ? dt.Columns.Count - 1 : dt.Columns.Count));
            bool[] IsNumericColumn = new bool[numberOfColumns];

            string[] excelColumnNames = new string[numberOfColumns];
            for (int n = 0; n < numberOfColumns; n++)
                excelColumnNames[n] = GetExcelColumnName(n);

            //
            //  Create the Header row in our Excel Worksheet
            //
            uint rowIndex = 3;
            if (dt.Rows[2][1].ToString() == "" && dt.Rows[2][dt.Columns.Count - 1].ToString() == "")
            {
                rowIndex = 4;

            }
            var headerRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
            sheetData.Append(headerRow);

            for (int colInx = 0; colInx < numberOfColumns; colInx++)
            {
                DataColumn col = dt.Columns[colInx];
                Cell cell2 = new Cell() { CellReference = excelColumnNames[colInx] + (dt.Rows[2][1].ToString() == "" ? dt.Rows[2][dt.Columns.Count - 1].ToString() == "" ? "4" : "3" : "3"), DataType = CellValues.String, StyleIndex = Convert.ToUInt32(1) };
                CellValue cellValue2 = new CellValue();
                cellValue2.Text = col.ColumnName;
                cell2.Append(cellValue2);
                headerRow.Append(cell2);

                //AppendTextCell(excelColumnNames[colInx] + "3", col.ColumnName, headerRow);
                IsNumericColumn[colInx] = (col.DataType.FullName == "System.Decimal") || (col.DataType.FullName == "System.Int32");
            }

            //
            //  Now, step through each row of data in our DataTable...
            //
            double cellNumericValue = 0;
            dt.Rows.RemoveAt(0); dt.Rows.RemoveAt(0);
            if (dt.Rows[0][1].ToString() == "" && dt.Rows[0][dt.Columns.Count - 1].ToString() == "")
            {
                dt.Rows.RemoveAt(0);
            }
            //foreach (DataRow dr in dt.Rows)
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                DataRow dr = dt.Rows[r];
                // ...create a new row, and append a set of this row's data to it.
                ++rowIndex;
                var newExcelRow = new Row { RowIndex = rowIndex };  // add a row at the top of spreadsheet
                sheetData.Append(newExcelRow);

                for (int colInx = 0; colInx < numberOfColumns; colInx++)
                {
                    cellValue = dr.ItemArray[colInx].ToString();
                    if (dt.Columns.Contains("Grouping") && dt.Rows[r]["Grouping"].ToString() != null && dt.Rows[r]["Grouping"].ToString() != "")
                    {
                        colInx = dt.Columns["Grouping"].Ordinal;
                        cellValue = dr.ItemArray[colInx].ToString();
                        AppendTextCell(excelColumnNames[0] + rowIndex.ToString(), cellValue, newExcelRow, Convert.ToUInt32(dt.Columns.Contains("StyleIndex") == true ? dt.Rows[r]["StyleIndex"].ToString() == null ? "0" : dt.Rows[r]["StyleIndex"].ToString() == "" ? "0" : dt.Rows[r]["StyleIndex"].ToString() : "0"));

                        //MergeCells mergeCells = new MergeCells();
                        //mergeCells.Append(new MergeCell() { Reference = new StringValue(excelColumnNames[0] + rowIndex.ToString() + ":" + excelColumnNames[(excelColumnNames.Length - 1)] + rowIndex.ToString()) });
                        //worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());

                        MergeCells mergeCells;

                        if (worksheet.Elements<MergeCells>().Count() > 0)
                            mergeCells = worksheet.Elements<MergeCells>().First();
                        else
                        {
                            mergeCells = new MergeCells();

                            // Insert a MergeCells object into the specified position.
                            if (worksheet.Elements<CustomSheetView>().Count() > 0)
                                worksheet.InsertAfter(mergeCells, worksheet.Elements<CustomSheetView>().First());
                            else
                                worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
                        }

                        // Create the merged cell and append it to the MergeCells collection.
                        MergeCell mergeCell = new MergeCell()
                        {
                            Reference =
                                new StringValue(excelColumnNames[0] + rowIndex.ToString() + ":" + excelColumnNames[(excelColumnNames.Length - 1)] + rowIndex.ToString())
                        };
                        mergeCells.Append(mergeCell);
                        break;
                    }
                    else
                    {
                        // Create cell with data
                        if (IsNumericColumn[colInx])
                        {
                            //  For numeric cells, make sure our input data IS a number, then write it out to the Excel file.
                            //  If this numeric value is NULL, then don't write anything to the Excel file.
                            cellNumericValue = 0;
                            if (double.TryParse(cellValue, out cellNumericValue))
                            {
                                cellValue = cellNumericValue.ToString();
                                AppendNumericCell(excelColumnNames[colInx] + rowIndex.ToString(), cellValue, newExcelRow, Convert.ToUInt32(dt.Columns.Contains("StyleIndex") == true ? dt.Rows[r]["StyleIndex"].ToString() == null ? "0" : dt.Rows[r]["StyleIndex"].ToString() == "" ? "0" : dt.Rows[r]["StyleIndex"].ToString() : "0"));
                            }
                        }
                        else
                        {
                            //  For text cells, just write the input data straight out to the Excel file.
                            AppendTextCell(excelColumnNames[colInx] + rowIndex.ToString(), cellValue, newExcelRow, Convert.ToUInt32(dt.Columns.Contains("StyleIndex") == true ? dt.Rows[r]["StyleIndex"].ToString() == null ? "0" : dt.Rows[r]["StyleIndex"].ToString() == "" ? "0" : dt.Rows[r]["StyleIndex"].ToString() : "0"));
                        }
                    }
                }
            }
        }

        private static void AppendTextCell(string cellReference, string cellStringValue, Row excelRow, UInt32 styleindex)
        {
            Cell cell;
            //  Add a new Excel Cell to our Row 
            if (styleindex > 0)
            {
                cell = new Cell() { CellReference = cellReference, DataType = CellValues.String, StyleIndex = styleindex };
            }
            else
            {
                cell = new Cell() { CellReference = cellReference, DataType = CellValues.String };
            }
            CellValue cellValue = new CellValue();
            cellValue.Text = cellStringValue;
            cell.Append(cellValue);
            excelRow.Append(cell);
        }
        private static void AppendNumericCell(string cellReference, string cellStringValue, Row excelRow, UInt32 styleindex)
        {
            Cell cell;
            //  Add a new Excel Cell to our Row 
            if (styleindex > 0)
            {
                cell = new Cell() { CellReference = cellReference, StyleIndex = styleindex };
            }
            else
            {
                cell = new Cell() { CellReference = cellReference };
            }
            CellValue cellValue = new CellValue();
            cellValue.Text = cellStringValue;
            cell.Append(cellValue);
            excelRow.Append(cell);
        }
        private static string GetExcelColumnName(int columnIndex)
        {

            if (columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString();

            char firstChar = (char)('A' + (columnIndex / 26) - 1);
            char secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}", firstChar, secondChar);
        }

        private static WorkbookStylesPart AddStyleSheet(SpreadsheetDocument spreadsheet, DataSet ds)
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
            Fill fill1 = new Fill();        //Gray125 fill

            Fills fills = new Fills();      // <APENDING Fills>
            fills.Append(fill0);
            fills.Append(fill1);

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

            bool flag = false;
            if (ds.Tables.Count > 0)
            {
                foreach (DataTable dt in ds.Tables)
                {
                    //if (dt.Columns.Contains("Style"))
                    //{
                        flag = true;
                        break;
                   // }
                }
            }
            //else      Not needed as it is used for multiple sheets
            //{
            //    if (ds.Tables[0].Columns.Contains("Style"))
            //    {
            //        flag = true;
            //    }
            //}
            UInt32Value i = 2;
            if (flag)
            {
                //background
                if (ds.Tables.Count > 0)
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.Columns.Contains("Style"))
                        {
                            dt.Columns.Add("StyleIndex");
                            for (int row = 0; row < dt.Rows.Count; row++)
                            {
                                if (dt.Rows[row]["Style"].ToString() != null && dt.Rows[row]["Style"].ToString() != "")
                                {

                                    dt.Rows[row]["StyleIndex"] = i.ToString();
                                    Font font2 = new Font();
                                    Fill fill2 = new Fill();

                                    string[] styles = dt.Rows[row]["Style"].ToString().Split(';');
                                    for (int j = 0; j < styles.Length; j++)
                                    {
                                        if (styles[j].Split(':')[0].ToLower() == "color")
                                        {
                                            //System.Drawing.Color c = System.Drawing.Color.FromName(styles[j].Split(':')[1]);
                                            string hexcode = styles[j].Split(':')[1].Replace("#", ""); //c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
                                            font2.Color = new Color() { Rgb = hexcode };
                                            fonts.Append(font2);

                                        }
                                        else if (styles[j].Split(':')[0].ToLower() == "background")
                                        {
                                            //System.Drawing.Color c = System.Drawing.Color.FromName(styles[j].Split(':')[1]);
                                            string hexcode = styles[j].Split(':')[1].Replace("#", ""); // c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
                                            PatternFill _PatternFill;
                                            _PatternFill = new PatternFill() { PatternType = PatternValues.Solid };
                                            //_PatternFill = new PatternFill(new ForegroundColor() { Rgb = new HexBinaryValue() { Value = hexcode } });
                                            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = hexcode };
                                            BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)64U };
                                            _PatternFill.Append(foregroundColor1);
                                            _PatternFill.Append(backgroundColor1);

                                            fill2.Append(_PatternFill);

                                            fills.Append(fill2);
                                        }
                                    }
                                    CellFormat cellformat2 = new CellFormat() { FontId = i, FillId = i };
                                    cellformats.Append(cellformat2);
                                    i = i + 1;
                                }
                            }
                        }

                    }
                }
            }

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

    }
}