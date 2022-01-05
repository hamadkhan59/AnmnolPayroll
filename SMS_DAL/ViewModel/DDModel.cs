using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class ClassDD
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SectionDD
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public partial class SessionDD
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public partial class StaffDD
    {
        public int StaffId { get; set; }
        public string Name { get; set; }
    }

    public class FeeHeadDD
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class DesignationCatagoryDD
    {
        public int Id { get; set; }
        public string CatagoryName { get; set; }
    }

}
