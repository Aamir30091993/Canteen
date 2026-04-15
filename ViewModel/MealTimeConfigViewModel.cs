using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GCC_Canteen.ViewModel;
namespace GCC_Canteen.ViewModel
{
    public class MealTimeConfigViewModel
    {
        public List<MealTimeConfigList> MealTimeConfigListDetails { get; set; }
        public SysConfigDtl SysConfigDetails { get; set; }
    }
    public class MealTimeConfigList
    {
        public int MealTimeID { get; set; }
        public int MealTypeID { get; set; }
        public string MealType { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string BufferTime { get; set; }

    }
    public class SysConfigDtl
    {
        public int SysConfigID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Datatype { get; set; }

    }
}