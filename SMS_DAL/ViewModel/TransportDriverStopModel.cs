using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class TransportDriverStopModel
    {
        public int Id { get; set; }
        public Nullable<int> DriverId { get; set; }
        public Nullable<int> StopId { get; set; }
        public string StopName { get; set; }
        public int StopRent { get; set; }


    }
}
