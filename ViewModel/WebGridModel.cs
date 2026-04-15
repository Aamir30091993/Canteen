using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace GCC_Canteen.ViewModel
{    
        public class GridColumnSetting
        {
            public int PageID { get; set; }
            public string GridControl { get; set; }
            public string ColumnName { get; set; }
            public int Width { get; set; }
            public int SeqNo { get; set; }
            public string HeaderText { get; set; }
            public string Tag { get; set; }
            public string ColumnAliasValue { get; set; }
            public bool AllowSorting { get; set; }
            public bool AllowForFilter { get; set; }
            public bool AllowForClick { get; set; }
            public string URL { get; set; }
            public string DataType { get; set; }
            public bool IsIcon { get; set; }
            public string IconPath { get; set; }
            public string UrlColumnName { get; set; }
            public bool Visible { get; set; }
            public string Alignment { get; set; }
            public string LookUpQuery { get; set; }
            public string TableName { get; set; }
            public string ItemTextStyle { get; set; }

        }

        public class GridSettings
        {
            public int PageID { get; set; }
            public string GridControl { get; set; }
            public string GridCaption { get; set; }
            public int Mode { get; set; }
            public bool Visible { get; set; }
            public bool Readonly { get; set; }
            public bool Enabled { get; set; }
            public int DefaultRecCnt { get; set; }
            public int IncrementalRecCnt { get; set; }
            public bool AllowExportData { get; set; }
            public bool AllowSelect { get; set; }
            public bool AllowPaging { get; set; }
            public bool AllowGet { get; set; }
            public string GetControllerName { get; set; }
            public string GetActionMethod { get; set; }
            public bool AllowDelete { get; set; }
            public string DeleteControllerName { get; set; }
            public string DeleteActionMethod { get; set; }
            public string IsExportAllowed { get; set; }
        }

        public class GridSettingData
        {
            public GridSettings WebGridSetting { get; set; }
            public List<GridColumnSetting> WebGridColumnSetting { get; set; }
            //public IEnumerable<BranchListModel> BranchList { get; set; }
            public List<WebGridColumn> WebGridColumns { get; set; }

            public List<string> HeaderTextStyle { get; set; }
        }

     
}