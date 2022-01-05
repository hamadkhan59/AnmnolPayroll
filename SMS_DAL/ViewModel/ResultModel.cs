using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class ResultModel
    {
        public List<DataSet> DatasetList { get; set; }
        public List<Exam> examList { get; set; }
    }
}
