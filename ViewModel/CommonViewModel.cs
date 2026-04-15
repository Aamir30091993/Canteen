using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCC_Canteen.ViewModel
{
    public class SelectOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
    public class GetSessionKey
    {
        public static string GetCurrentSessionkey(int PageID, Int64 RecordID, string Key)
        {
            return PageID.ToString() + "_" + RecordID.ToString() + "_" + Key;
        }
    }
}