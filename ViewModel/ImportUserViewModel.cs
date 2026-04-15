using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GCC_Canteen.ViewModel
{
    public class ImportUserViewModel
    {
        [Required(ErrorMessage = "File Required")]

        public HttpPostedFileBase Attachment { get; set; }
        public List<ImportViewData> _ImportTempViewData { get; set; }
        public ImportUserHdr _ImportUserHdr { get; set; }
        public List<ImportUserDtl> _ImportUserDtl { get; set; }
        public ImportUserAttach _ImportUserAttach { get; set; }
        public List<ImportUserDtl> _ImportUserValidRecords { get; set; }
        public List<ImportUserDtl> _ImportUserInValidRecords { get; set; }
        public List<ImportUserDtl> _ImportUserDuplicateRecords { get; set; }
        public List<ImportUserStatisticsListValues> _ImportUserStatistics { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ImportUserListValues
    {
        public int ID { get; set; }
        public DateTime ImportDate { get; set; }
        public int ImportedBy { get; set; }
        public string ImportedUser { get; set; }
        public int NoOfRecords { get; set; }
        public int ValidRecords { get; set; }
        //public int LOBID { get; set; }
        //public string LOBName { get; set; }
        //public string FinYear { get; set; }
        public int StatusID { get; set; }
        public string Status { get; set; }
    }
    public class ImportUserListModel
    {
        public GridSettingData gridSettingData { get; set; }
        public IEnumerable<ImportUserListValues> ImportUserList { get; set; }
    }
    public class ImportViewData
    {
        public string Location { get; set; }
        public string UserName { get; set; }
        public string EmpCode { get; set; }
        public string CardNo { get; set; }
        public string UserID { get; set; }
        public string EmpDesignation { get; set; }
        public string TeamName { get; set; }
        public string ReportingTo { get; set; }
        public string ReportingToEmpCode { get; set; }
        public string ReportingToDesignation { get; set; }

    }
    public class ImportUserHdr
    {
        public int ImportID { get; set; }
        //[DisplayName("Financial Year")]
        //[Required(ErrorMessage = "Financial Year Required")]
        //public string FinYear { get; set; }
        //public string Model { get; set; }
        //[DisplayName("LOB")]
        //[Required(ErrorMessage = "LOB Required")]
        //public string LOBID { get; set; }
        //public string LOB { get; set; }
        [DisplayName("Import Date")]
        [Required(ErrorMessage = "Import Date Required")]
        public string ImportDate { get; set; } 
        public string ImportFilePath { get; set; }
        public string Remark { get; set; }
        public int NoOfRecords { get; set; }
        public int? ValidRecords { get; set; }
        public int? InvalidRecords { get; set; }
        public int? SelfDuplicateRecords { get; set; }
        public int? ImportedBy { get; set; }
        public string ImportedUser { get; set; }
        public string ImportedDate { get; set; }
        public int StatusID { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedUser { get; set; }
        public string CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string ModifiedUser { get; set; }
        public string ModifiedDate { get; set; }
        public int? DeletedBy { get; set; }
        public string DeletedUser { get; set; }
        public string DeletedDate { get; set; }
        public string IsEdit { get; set; }
        public string SessionIdentity { get; set; }
        public string AttachmentSessionKey { get; set; }
    }
    public class ImportUserDtl
    {
        public int ImportDtlID { get; set; }
        public int ImportID { get; set; }
        //public string Vertical { get; set; }
        //public string AccountManagerName { get; set; }
        //public string Month { get; set; }
        //public string BranchName { get; set; }
        //public string TargetAmount { get; set; }

        public string Location { get; set; }
        public string UserName { get; set; }
        public string EmpCode  { get; set; }
        public string LoginID  { get; set; }
        public string DOJ { get; set; }  //string
        public string DOL { get; set; }  //string
        public string EmpDesignation { get; set; }
        public string SystemRole { get; set; }
        public string TeamName { get; set; }
        public string ReportingTo { get; set; }
        public string ReportingToEmpCode { get; set; }
        public string ReportingToDesignation { get; set; }
        public string RemarkStatus { get; set; }

        public string EmailAddress { get; set; }
    }
    public class ImportUserAttach
    {
        public int ImportAttachID { get; set; }
        public int ImportID { get; set; }
        public string Path { get; set; }
        public bool Status { get; set; }

    }
    public class ImportUserStatisticsListValues
    {
        public string StatisticsDetails { get; set; }
        public int StatisticsValue { get; set; }
    }
}