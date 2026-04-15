using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GCC_Canteen.ViewModel
{
    public class DashboardViewModel
    {
        public Dashboard DashboardDetails { get; set; }
        public List<FilteredListData> FilteredListDataDetails { get; set; }
        public string SessionIdentifier { get; set; }
    }
    public class FilteredListData
    {
         //public string EntryDate { get; set; }
         //public string EmployeeName { get; set; }
        //public string MealSubType { get; set; }

        public string Location { get; set; }
        //public string User { get; set; }
        public string UserName { get; set; }
        public string EmployeeCode { get; set; }
        public string CardNo { get; set; }
        public string UserID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
         public string MealType { get; set; }
        public string SubType { get; set; }
        public string Gender { get; set; }
        public string Designation { get; set; }
        public string Level { get; set; }
        public string Department { get; set; }
        public string AssignedShift { get; set; }
        public string TeamName { get; set; }
        public string ReportingToUserName { get; set; }
        public string ReportingToEmpCode { get; set; }
        public string TeamLeaderName { get; set; }
        public string ManagerName { get; set; }
        public string DirectorName { get; set; }

    }
    public class Dashboard
    {
        [Required(ErrorMessage = "From Date is mandatory")]
        public string FromDate { get; set; }
        [Required(ErrorMessage = "To Date is mandatory")]
        public string ToDate { get; set; }
        public string Employee { get; set; }
        public int MealTypeID { get; set; }
        public int MealSubTypeID { get; set; }
        //public string EmployeeName { get; set; }
        public List<int> SelectedEmployees { get; set; } = new List<int>();
        public List<int> SelectedUserID { get; set; } = new List<int>();

        public string selectedMealTypeName { get; set; }
        public string selectedMealSubTypeName { get; set; }
        public string LoginID { get; set; }
        
    }
    public class DashboardOverAllView
    {
        public string TotalUser { get; set; }
        public string ResponseUser { get; set; }
        public string ResponsePercent { get; set; }
        public string SkippedUser { get; set; }
        public string SkippedPercent { get; set; }
        public string SadOverallUser { get; set; }
        public string SadOverallPercent { get; set; }
        public string SadPersonalUser { get; set; }
        public string SadPersonalPercent { get; set; }
        public string SadProfessionalUser { get; set; }
        public string SadProfessionalPercent { get; set; }
        //public string OkUser { get; set; }
        //public string OkPercent { get; set; }
        public string HappyOverallUser { get; set; }
        public string HappyOverallPercent { get; set; }
        public string HappyProfessionalUser { get; set; }
        public string HappyProfessionalPercent { get; set; }
        public string HappyPersonalUser { get; set; }
        public string HappyPersonalPercent { get; set; }
    }
    public class DashboardSurveySummary
    {
        public string Name { get; set; }
        public string TotalUser { get; set; }
        public string ResponseUser { get; set; }
        public string ResponsePercent { get; set; }
        public string SkippedUser { get; set; }
        public string SkippedPercent { get; set; }
        //Over All
        public string OverallSadPersonalUser { get; set; }
        public string OverallSadPersonalPercent { get; set; }
        public string OverallSadProfessionalUser { get; set; }
        public string OverallSadProfessionalPercent { get; set; }
        //public string OverallOkUser { get; set; }
        //public string OverallOkPercent { get; set; }
        public string OverallHappyPersonalUser { get; set; }
        public string OverallHappyPersonalPercent { get; set; }
        public string OverallHappyProfessionalUser { get; set; }
        public string OverallHappyProfessionalPercent { get; set; }
        //Day
        public string DaySadPersonalUser { get; set; }
        public string DaySadPersonalPercent { get; set; }
        public string DaySadProfessionalUser { get; set; }
        public string DaySadProfessionalPercent { get; set; }
        //public string DayOkUser { get; set; }
        //public string DayOkPercent { get; set; }
        public string DayHappyPersonalUser { get; set; }
        public string DayHappyPersonalPercent { get; set; }  
        public string DayHappyProfessionalUser { get; set; }
        public string DayHappyProfessionalPercent { get; set; }
        //Nite
        public string NightSadPersonalUser { get; set; }
        public string NightSadPersonalPercent { get; set; }
        public string NightSadProfessionalUser { get; set; }
        public string NightSadProfessionalPercent { get; set; }
        //public string NightOkUser { get; set; }
        //public string NightOkPercent { get; set; }
        public string NightHappyPersonalUser { get; set; }
        public string NightHappyPersonalPercent { get; set; }
        public string NightHappyProfessionalUser { get; set; }
        public string NightHappyProfessionalPercent { get; set; }

    }
}