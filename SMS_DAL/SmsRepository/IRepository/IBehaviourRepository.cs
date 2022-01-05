using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IBehaviourRepository : IDisposable
    {
        #region Category
        BehaviourCategory GetCategory(int id);
        List<BehaviourCategory> GetCategoriesByBranch(int brachId);
        List<BehaviourCategoryViewModel> GetCategoryViewsByBranch(int brachId);
        BehaviourCategory CreateOrUpdateCategory(BehaviourCategory obj);
        bool DeleteCategory(int id);
        #endregion

        #region Parameters
        BehaviourParameter GetParameter(int id);
        List<BehaviourParameter> GetParametersByCategory(int catId);
        List<BehaviourParameter> GetParametersByBranch(int brachId);
        List<BehaviourParameterViewModel> GetParameterViewByCategory(int catId);
        BehaviourParameter CreateOrUpdateParameter(BehaviourParameter obj);
        bool DeleteParameter(int id);
        #endregion

        #region Student_Behaviour
        StudentBehaviourViewModel GetStudentBehaviour(int studentId, int staffId, int branchId);
        StudentBehaviourViewModel GetOverallStudentBehaviour(int studentId, int branchId);
        PerformanceChartData GetStudentPerformance(int studentId, int categoryId, int parameterId);
        List<StudentBehaviour> GetStudentBehaviourByStudent(int studentId);
        bool CreateOrUpdateStudentBehaviour(List<StudentBehaviour> objs);
        bool DeleteStudentBehaviour(int id);
        #endregion
    }
}
