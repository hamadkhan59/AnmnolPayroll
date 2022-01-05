using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class IssuedChallanViewModel
    {
        public int Id { get; set; }
        public int ChallanId { get; set; }
        public int studentId { get; set; }
        public int StudentChallanId { get; set; }
        public int ? FeeBalance { get; set; }
        public string RollNumber { get; set; }
        public string ForMonth { get; set; }
        public string Name { get; set; }
        public string Fathername { get; set; }
        public string Contact_1 { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PaidDate { get; set; }
        public DateTime IssuedDate { get; set; }
        public string IsPaid { get; set; }
        public string Chalan { get; set; }
        public int Amount { get; set; }
        public int Fine { get; set; }
        public int PaidAmount { get; set; }
        public int Balance { get; set; }
        public int Advance { get; set; }
        public bool IsLcm { get; set; }
        public int AnnualCharges { get; set; } 
    }
}
