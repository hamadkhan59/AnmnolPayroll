using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.Reports
{
    public class DAL_Exam_Reports : DAL.DABase
    {
        public DataSet GetFeeCollectionReportDataGraphical()
        {
//            string sql = @"select st.Name as TeacherName1, et.Name as ExamName1, concat(cls.Name, ', ', sec.Name, ', ', subj.Name) as ClassDetail,  
//                            (sum(case when ((er.ObtainedMarks * 100)/ex.TotalMarks) >= ex.PassPercentage then 1 else 0 end) * 100) / count(*) as Percentage
//                            from ExamResults er, Exam ex, Subjects subj, Classes cls, ClassSection clsec,
//                            Staff st, SessionSubject ssub, Section sec, ExamType et
//                            where ex.Id = er.ExamId and ex.CourseId = subj.Id  and clsec.ClassSectionId = ex.ClassSectionId
//                            and clsec.ClassId = cls.Id and ssub.TeacherId = st.StaffId and sec.Id = clsec.SectionId
//                            and ssub.ClassSectionId = clsec.ClassSectionId 
//                            and ex.CourseId = ssub.SubjectId and ex.ExamTypeId = et.Id
//                            group by st.Name, et.Name, concat(cls.Name, ', ', sec.Name, ', ', subj.Name )
//                            order by st.Name, Percentage desc";

            string sql = @"select st.Name as TeacherName1, et.Name as ExamName1, concat(cls.Name, ', ', sec.Name, ', ', subj.Name) as ClassDetail, eterm.TermName, yrs.Year,
                            (sum(case when ((er.ObtainedMarks * 100)/ex.TotalMarks) >= ex.PassPercentage then 1 else 0 end) * 100) / count(*) as Percentage
                            from ExamResults er, Exam ex, Subjects subj, Classes cls, ClassSection clsec,
                            Staff st, SessionSubject ssub, Section sec, ExamType et, ExamTerm eterm, Years yrs
                            where ex.Id = er.ExamId and ex.CourseId = subj.Id  and clsec.ClassSectionId = ex.ClassSectionId
                            and clsec.ClassId = cls.Id and ssub.TeacherId = st.StaffId and sec.Id = clsec.SectionId
                            and ssub.ClassSectionId = clsec.ClassSectionId and et.TermId = eterm.Id
                            and ex.CourseId = ssub.SubjectId and ex.ExamTypeId = et.Id and eterm.Year = yrs.Id
                            group by st.Name, et.Name, concat(cls.Name, ', ', sec.Name, ', ', subj.Name ), eterm.TermName, yrs.Year
                            order by st.Name, Percentage desc";


            return ExecuteDataSet(sql);   
        }

        public DataSet GetFeeCollectionReportData()
        {
            //            string sql = @"select st.Name as TeacherName1, et.Name as ExamName1, concat(cls.Name, ', ', sec.Name, ', ', subj.Name) as ClassDetail,  
            //                            (sum(case when ((er.ObtainedMarks * 100)/ex.TotalMarks) >= ex.PassPercentage then 1 else 0 end) * 100) / count(*) as Percentage
            //                            from ExamResults er, Exam ex, Subjects subj, Classes cls, ClassSection clsec,
            //                            Staff st, SessionSubject ssub, Section sec, ExamType et
            //                            where ex.Id = er.ExamId and ex.CourseId = subj.Id  and clsec.ClassSectionId = ex.ClassSectionId
            //                            and clsec.ClassId = cls.Id and ssub.TeacherId = st.StaffId and sec.Id = clsec.SectionId
            //                            and ssub.ClassSectionId = clsec.ClassSectionId 
            //                            and ex.CourseId = ssub.SubjectId and ex.ExamTypeId = et.Id
            //                            group by st.Name, et.Name, concat(cls.Name, ', ', sec.Name, ', ', subj.Name )
            //                            order by st.Name, Percentage desc";

            string sql = @"select st.Name as TeacherName1, et.Name as ExamName1, concat(cls.Name, ', ', sec.Name, ', ', subj.Name) as ClassDetail, eterm.TermName, yrs.Year,
                            cls.Name as ClassName, sec.Name as SectionName, subj.Name as SubjectName,
							(sum(case when ((er.ObtainedMarks * 100)/ex.TotalMarks) >= ex.PassPercentage then 1 else 0 end) * 100) / count(*) as Percentage
                            from ExamResults er, Exam ex, Subjects subj, Classes cls, ClassSection clsec,
                            Staff st, SessionSubject ssub, Section sec, ExamType et, ExamTerm eterm, Years yrs
                            where ex.Id = er.ExamId and ex.CourseId = subj.Id  and clsec.ClassSectionId = ex.ClassSectionId
                            and clsec.ClassId = cls.Id and ssub.TeacherId = st.StaffId and sec.Id = clsec.SectionId
                            and ssub.ClassSectionId = clsec.ClassSectionId and et.TermId = eterm.Id
                            and ex.CourseId = ssub.SubjectId and ex.ExamTypeId = et.Id and eterm.Year = yrs.Id
                            group by st.Name, et.Name, cls.Name, sec.Name, subj.Name, eterm.TermName, yrs.Year
                            order by st.Name, Percentage desc";


            return ExecuteDataSet(sql);
        }

        public DataSet SubjectWiseTeacherAnalysisData()
        {

            string sql = @"select st.Name as TeacherName1, et.Name as ExamName1, concat(cls.Name, ', ', sec.Name, ', ', subj.Name) as ClassDetail, eterm.TermName, yrs.Year,
                            cls.Name as ClassName, sec.Name as SectionName, subj.Name as SubjectName,
							(sum(case when ((er.ObtainedMarks * 100)/ex.TotalMarks) >= ex.PassPercentage then 1 else 0 end) * 100) / count(*) as Percentage
                            from ExamResults er, Exam ex, Subjects subj, Classes cls, ClassSection clsec,
                            Staff st, SessionSubject ssub, Section sec, ExamType et, ExamTerm eterm, Years yrs
                            where ex.Id = er.ExamId and ex.CourseId = subj.Id  and clsec.ClassSectionId = ex.ClassSectionId
                            and clsec.ClassId = cls.Id and ssub.TeacherId = st.StaffId and sec.Id = clsec.SectionId
                            and ssub.ClassSectionId = clsec.ClassSectionId and et.TermId = eterm.Id
                            and ex.CourseId = ssub.SubjectId and ex.ExamTypeId = et.Id and eterm.Year = yrs.Id
                            group by subj.Name, st.Name, et.Name, cls.Name, sec.Name,  eterm.TermName, yrs.Year
                            order by subj.Name, st.Name, Percentage desc";


            return ExecuteDataSet(sql);
        }
    }
}
