using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IStaffBehaviourRepository : IDisposable
    {
        #region Category
        StaffBehaviourCategory GetCategory(int id);
        List<StaffBehaviourCategory> GetCategoriesByBranch(int brachId);
        List<BehaviourCategoryViewModel> GetCategoryViewsByBranch(int brachId);
        StaffBehaviourCategory CreateOrUpdateCategory(StaffBehaviourCategory obj);
        bool DeleteCategory(int id);
        #endregion

        #region Parameters
        StaffBehaviourParameter GetParameter(int id);
        List<StaffBehaviourParameter> GetParametersByCategory(int catId);
        List<StaffBehaviourParameter> GetParametersByBranch(int brachId);
        List<BehaviourParameterViewModel> GetParameterViewByCategory(int catId);
        StaffBehaviourParameter CreateOrUpdateParameter(StaffBehaviourParameter obj);
        bool DeleteParameter(int id);
        #endregion

        #region _StaffBehaviour
        StaffBehaviourViewModel GetStaffBehaviour(int Id, int staffId, int branchId);
        StaffBehaviourViewModel GetOverallStaffBehaviour(int Id, int branchId);
        PerformanceChartData GetPerformance(int Id, int categoryId, int parameterId);
        List<StaffBehaviour> GetStaffBehaviourBy(int Id);
        bool CreateOrUpdateStaffBehaviour(List<StaffBehaviour> objs);
        bool DeleteStaffBehaviour(int id);
        #endregion
    }
}
