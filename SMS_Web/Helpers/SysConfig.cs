using SMS_DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS_Web.Helpers
{
    public class SysConfig
    {
        static SC_WEBEntities2 db = new SC_WEBEntities2();
        static List<ConfigDD> configDD = new List<ConfigDD>();
        public const string EC_GRADE_FLAG = "ExamGradeFlag";
        public const string EC_REMARKS_FLAG = "ExamRemarksFlag";
        public const string EC_POSITION_FLAG = "ExamPositionFlag";
        public const string EC_PERCENTAGE_FLAG = "ExamPercentageFlag";
        public const string EC_PARENT_SIGN_FLAG = "ParentSignatureFlag";
        public const string EC_PRICIPLE_SIGN_FLAG = "PrinciplSignatureFlag";
        public const string EC_TEACHER_SIGN_FLAG = "TeacherSignatureFlag";
        public const string EC_PASSING_PER_NOTE_FLAG = "PassingPercntageNoteFlag";
        public const string EC_PASSING_PER_NOTE = "PassingPercntageNote";

        public static SystemConfig GetSystemParam(string paramName)
        {
            var param = SessionHelper.SystemConfigList().Where(x => x.ParamName == paramName).FirstOrDefault();
            if (param == null)
            {
                param = AddSystemParam(paramName);
            }

            return param;
        }

        private static SystemConfig AddSystemParam(string paramName)
        {
            SystemConfig param = new SystemConfig();
            param.ParamName = paramName;
            param.ParamValue = string.Empty;
            db.SystemConfigs.Add(param);
            db.SaveChanges();

            SessionHelper.InvalidateSystemConfigCache = false;
            return param;
        }

        public static void UpdateSystemParam(SystemConfig param)
        {
            SessionHelper.InvalidateSystemConfigCache = false;
            db.Entry(param).State = System.Data.EntityState.Modified;
            db.SaveChanges();
        }

        public static SystemConfig GetSystemConfigById(int Id)
        {
            return db.SystemConfigs.Where(x => x.Id == Id).FirstOrDefault();
        }

        public static List<ConfigDD> GetYesNoDD()
        {
            configDD.RemoveAll(x => x.Value != null);
            configDD.Add(new ConfigDD(1, "Yes"));
            configDD.Add(new ConfigDD(2, "No"));

            return configDD;
        }
    }

    public class ConfigDD
    {
        public ConfigDD(int id, string value)
        {
            Id = id;
            Value = value;
        }
        public int Id { get; set; }
        public string Value { get; set; }
    }
}