using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GCC_Canteen.ViewModel
{
    public class ReportViewModel
    {
        public ReportParameter ReportParameterDetails { get; set; }
    }

    public class ReportParameter
    {
      

        [Required(ErrorMessage = "Report Type Required")]
        public int ReportID { get; set; }

        [Required(ErrorMessage = "From Date Required")]
        public string ReportFromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        public string ReportToDate { get; set; }

        public int? EventID { get; set; }
    }
}