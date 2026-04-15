using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GCC_Canteen.ViewModel
{
    public class SelectListModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Group 
        { 
            get; 
            set; 
        }
    }
}