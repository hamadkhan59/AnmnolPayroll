using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class ActivityMarksViewModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string RollNumber { get; set; }
        public string Name { get; set; }
        public string ActitivtyName { get; set; }
        public string FatherName { get; set; }
        public string AdmissionNo { get; set; }
        public string Contact_1 { get; set; }
        public string Grade { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal ObtMarks { get; set; }
        public DateTime ActivityDate { get; set; }

    }

    public class ActivityMarksResponse
    {
        public string StatusCode { get; set; }
        public List<ActivityMarksViewModel> ActivityMarks { get; set; }
    }

}
