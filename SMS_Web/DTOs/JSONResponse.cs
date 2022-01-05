using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS_Web.DTOs
{
    public class JSONResponse
    {
        public bool Success{get; set;}
        public string Message { get; set; }
        public dynamic Source { get; set; }
    }
}