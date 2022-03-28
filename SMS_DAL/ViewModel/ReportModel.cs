using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class ReportModel
    {
        public int reportId { get; set; }
        public string reportName { get; set; }
        public string ledgerAccount { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }

        public string staffIdsRange { get; set; }
        public string year { get; set; }
        public int classId { get; set; }
        public int staffId { get; set; }
        public int feeHeadId { get; set; }

        public int sectionId { get; set; }
        public int financeLevelId { get; set; }
        public bool isActiveAccounts { get; set; }
        public bool includeCashWithdrawal { get; set; }
        public bool isCashWithdrawalDetailReport { get; set; }
        public string rollNo { get; set; }
        public string Name { get; set; }
        public string BankName { get; set; }
        public string CashName { get; set; }
        public string fatherName { get; set; }
        public string inquiryNumber { get; set; }
        public int paid { get; set; }
        public string month { get; set; }
        public int categoryId { get; set; }
        public int designationId { get; set; }
        public int activeAccounts { get; set; }
        public int firstLevel { get; set; }
        public int secondLevel { get; set; }
        public int thirdLevel { get; set; }
        public int fourthLevel { get; set; }
        public int fifthLevel { get; set; }
        public int bankFifthLevel { get; set; }
        public int cashFifthLevel { get; set; }
        public int entryId { get; set; }
        public int genderId { get; set; }
        public int directoryOptionId { get; set; }
        public int attendanceStatusId { get; set; }
        public int mode { get; set; }
        public int jvEntryMode { get; set; }
        public int detailedReport { get; set; }
        public int reportLevel { get; set; }
        public int cashWihdrawReport { get; set; }
        public int orderId { get; set; }
        public int itemId { get; set; }


    }
}
