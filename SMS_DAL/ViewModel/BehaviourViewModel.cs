using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class StaffBehaviourViewModel
    {
        public int StaffID { get; set; }

        public int EvaluatingStaffID { get; set; }
        public string StaffName { get; set; }
        public string FatherName { get; set; }
        public string Designation { get; set; }
        public string CNIC { get; set; }
        //public string BForm { get; set; }
        //public string Class { get; set; }
        //public string Section { get; set; }
        public byte[] ImageSource { get; set; }

        public List<BehaviourCategoryViewModel> Categories { get; set; }
    }

    public class StudentBehaviourViewModel
    {        
        public int StaffID { get; set; }

        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string FatherName { get; set; }
        public string RollNumber { get; set; }
        public string BForm { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public byte[] ImageSource { get; set; }

        public List<BehaviourCategoryViewModel> Categories { get; set; }
    }

    public class BehaviourCategoryViewModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public double CategoryRating { get; set; }

        public List<BehaviourParameterViewModel> Parameters { get; set; }
    }

    public class BehaviourParameterViewModel
    {
        public int ParameterID { get; set; }
        public string ParameterName { get; set; }
        public string ParameterDescription { get; set; }
        public double ParameterRating { get; set; }
        public List<BehaviourDetails> BehaviourDetails { get; set; }
        public List<BehaviourDetailsByStaff> BehaviourDetailsByStaff { get; set; }
    }

    public class BehaviourDetailsByStaff
    {
        public long StaffId { get; set; }
        public string StaffName { get; set; }
        public List<BehaviourDetails> BehaviourDetails { get; set; }
    }

    public class BehaviourDetails
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string StaffComment { get; set; }
        public double StaffRating { get; set; }
    }

    public class PerformanceChartData
    {
        public List<string> Parameters { get; set; }
        public List<RatingsByStaff> RatingsByStaff { get; set; }
    }

    public class RatingsByStaff
    {
        public string StaffName { get; set; }
        public List<float> Ratings { get; set; }        
    }
}
