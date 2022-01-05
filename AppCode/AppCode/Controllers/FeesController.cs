using Newtonsoft.Json;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace SMSApi.Controllers
{
    public class FeesController : ApiController
    {
        IFeePlanRepository feeRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
        [HttpGet]
        public Object getFeeStatus(DateTime fromDate, DateTime toDate, int studentId)
        {
            string json = "";
            FeeResponse response = new FeeResponse();
            try
            {
                response.FeeDetail = feeRepo.GetFeeStatus(studentId, fromDate, toDate);
                response.StatusCode = ((response.FeeDetail != null && response.FeeDetail.Count > 0)
                                     ? AppConstHelper.SUCCESS : AppConstHelper.FEE_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch(Exception e)
            {
                response.StatusCode = AppConstHelper.FEE_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }
    }
}
