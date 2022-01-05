using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class StaffBehaviourRepositoryImp : IStaffBehaviourRepository
    {
        private readonly SC_WEBEntities2 dbContext;

        public StaffBehaviourRepositoryImp(SC_WEBEntities2 context)
        {
            //dbContext.Configuration.LazyLoadingEnabled = false;            
            dbContext = context;
        }

        #region Categories
        public StaffBehaviourCategory GetCategory(int id)
        {
            return dbContext.StaffBehaviourCategories.Where(n => n.ID == id).FirstOrDefault();
        }

        public List<StaffBehaviourCategory> GetCategoriesByBranch(int brachId)
        {
            return dbContext.StaffBehaviourCategories.Where(n => n.BranchId == brachId).ToList();
        }

        public List<BehaviourCategoryViewModel> GetCategoryViewsByBranch(int brachId)
        {
            var cats = dbContext.StaffBehaviourCategories.Where(n => n.BranchId == brachId).ToList();
            var categories = new List<BehaviourCategoryViewModel>();
            foreach (var cat in cats)
            {
                var category = new BehaviourCategoryViewModel();
                category.CategoryID = cat.ID;
                category.CategoryName = cat.Name;
                category.CategoryRating = cat.Rating;
                category.CategoryDescription = cat.Description;

                categories.Add(category);
            }
            return categories;
        }

        public StaffBehaviourCategory CreateOrUpdateCategory(StaffBehaviourCategory obj)
        {
            if (obj.ID > 0)
            {
                dbContext.Entry(obj).State = EntityState.Modified;
            }
            else
            {
                //var parameter = new StaffBehaviourParameter();
                //parameter.StaffBehaviourCategory = obj;
                //parameter.Name = "Others";
                //parameter.Rating = 100;
                //parameter.Description = "Other Aspects of StaffBehaviour under " + obj.Name;
                //dbContext.StaffBehaviourParameters.Add(parameter);
                dbContext.StaffBehaviourCategories.Add(obj);
            }

            dbContext.SaveChanges();
            return obj;
        }

        public bool DeleteCategory(int id)
        {
            var category = dbContext.StaffBehaviourCategories.Where(n => n.ID == id).FirstOrDefault();
            if (category != null)
            {
                dbContext.StaffBehaviourCategories.Remove(category);
                dbContext.SaveChanges();
                return true;
            }

            return false;
        }
        #endregion

        #region Parameters
        public StaffBehaviourParameter GetParameter(int id)
        {
            return dbContext.StaffBehaviourParameters.Where(n => n.ID == id).FirstOrDefault();
        }

        public List<StaffBehaviourParameter> GetParametersByBranch(int brachId)
        {
            return dbContext.StaffBehaviourParameters.Where(n => n.StaffBehaviourCategory.BranchId == brachId).OrderBy(n => n.CategoryId).ToList();
        }

        public List<StaffBehaviourParameter> GetParametersByCategory(int catId)
        {
            return dbContext.StaffBehaviourParameters.Where(n => n.CategoryId == catId).ToList();
        }

        public List<BehaviourParameterViewModel> GetParameterViewByCategory(int catId)
        {
            var parms = dbContext.StaffBehaviourParameters.Where(n => n.CategoryId == catId).ToList();
            var paramters = new List<BehaviourParameterViewModel>();
            foreach (var p in parms)
            {
                var parm = new BehaviourParameterViewModel();
                parm.ParameterID = p.ID;
                parm.ParameterName = p.Name;
                parm.ParameterRating = p.Rating;
                parm.ParameterDescription = p.Description;

                paramters.Add(parm);
            }

            return paramters;
        }

        public StaffBehaviourParameter CreateOrUpdateParameter(StaffBehaviourParameter obj)
        {
            //var category = GetCategory(obj.CategoryId);
            //obj.Rating = obj.Rating / category.Rating * 100.0d;
            //var other = dbContext.StaffBehaviourParameters.Where(n => n.CategoryId == obj.CategoryId && n.Name == "Others").FirstOrDefault();
            //if (other != null)
            //{
            //    // rating wieghtage reached max
            //    if (other.Rating - obj.Rating < 0)
            //        return 400;
            //    else
            //    {
            //        //update other param
            //        other.Rating = other.Rating - obj.Rating;
            //        dbContext.Entry(other).State = EntityState.Modified;
            //    }
            //}

            if (obj.ID > 0)
            {
                dbContext.Entry(obj).State = EntityState.Modified;
            }
            else
            {
                dbContext.StaffBehaviourParameters.Add(obj);
            }

            dbContext.SaveChanges();
            return obj;
        }

        public bool DeleteParameter(int id)
        {
            var Parameter = dbContext.StaffBehaviourParameters.Where(n => n.ID == id).FirstOrDefault();
            if (Parameter != null)
            {
                dbContext.StaffBehaviourParameters.Remove(Parameter);
                dbContext.SaveChanges();
                return true;
            }

            return false;
        }
        #endregion

        #region StaffBehaviour
        public StaffBehaviourViewModel GetStaffBehaviour(int staffId, int evaluatingStaffId, int branchId)
        {
            var staff = (new StaffRepositoryImp(dbContext)).GetStaffById(staffId);
            var categories = GetCategoriesByBranch(branchId);
            var parameters = GetParametersByBranch(branchId);
            var staffParams = dbContext.StaffBehaviours.Where(n => n.StaffId == staffId && n.EvaluatingStaffId == evaluatingStaffId);

            var viewModel = new StaffBehaviourViewModel();
            viewModel.StaffID = staffId;
            viewModel.EvaluatingStaffID = evaluatingStaffId;
            if (staff != null)
            {
                var designtion = dbContext.Designations.Find(staff.DesignationId);
                viewModel.StaffID = staff.StaffId;
                viewModel.StaffName = staff.Name;
                viewModel.FatherName = staff.FatherName;
                viewModel.CNIC = staff.CNIC;
                viewModel.Designation = designtion.Name;
                viewModel.ImageSource = staff.StaffImage;
            }

            viewModel.Categories = new List<BehaviourCategoryViewModel>();
            foreach (var category in categories)
            {

                var cat = new BehaviourCategoryViewModel();
                cat.CategoryID = category.ID;
                cat.CategoryName = category.Name;
                cat.CategoryDescription = category.Description;
                cat.CategoryRating = category.Rating;

                cat.Parameters = new List<BehaviourParameterViewModel>();
                foreach (var parameter in parameters.Where(n => n.CategoryId == category.ID))
                {
                    var StaffBehaviourParameterViewModel = new BehaviourParameterViewModel();
                    StaffBehaviourParameterViewModel.ParameterID = parameter.ID;
                    StaffBehaviourParameterViewModel.ParameterName = parameter.Name;
                    StaffBehaviourParameterViewModel.ParameterRating = parameter.Rating;
                    StaffBehaviourParameterViewModel.ParameterDescription = parameter.Description;
                    StaffBehaviourParameterViewModel.BehaviourDetails = new List<BehaviourDetails>();

                    var _staffParams = staffParams.Where(n => n.ParameterId == parameter.ID);
                    foreach (var staffParam in _staffParams)
                    {
                        var StaffBehaviourDetails = new BehaviourDetails();
                        StaffBehaviourDetails.ID = staffParam.ID;
                        StaffBehaviourDetails.StaffComment = staffParam.Comment;
                        StaffBehaviourDetails.StaffRating = staffParam.Rating;
                        StaffBehaviourDetails.Date = staffParam.CreatedOn.GetValueOrDefault();

                        StaffBehaviourParameterViewModel.BehaviourDetails.Add(StaffBehaviourDetails);
                    }

                    var newStaffBehaviourDetails = new BehaviourDetails();
                    newStaffBehaviourDetails.Date = DateTime.UtcNow;
                    newStaffBehaviourDetails.StaffRating = -1;
                    StaffBehaviourParameterViewModel.BehaviourDetails.Add(newStaffBehaviourDetails);

                    cat.Parameters.Add(StaffBehaviourParameterViewModel);
                }

                viewModel.Categories.Add(cat);
            }

            return viewModel;
        }

        public StaffBehaviourViewModel GetOverallStaffBehaviour(int staffId, int branchId)
        {
            var staff = (new StaffRepositoryImp(dbContext)).GetStaffById(staffId);
            var categories = GetCategoriesByBranch(branchId);
            var parameters = GetParametersByBranch(branchId);
            var staffParams = dbContext.StaffBehaviours.Where(n => n.StaffId == staffId);

            var viewModel = new StaffBehaviourViewModel();
            if (staff != null)
            {
                var designtion = dbContext.Designations.Find(staff.DesignationId);
                viewModel.StaffID = staff.StaffId;
                viewModel.StaffName = staff.Name;
                viewModel.FatherName = staff.FatherName;
                viewModel.CNIC = staff.CNIC;
                viewModel.ImageSource = staff.StaffImage;
                viewModel.Designation = designtion.Name;
            }

            viewModel.Categories = new List<BehaviourCategoryViewModel>();
            foreach (var category in categories)
            {

                var cat = new BehaviourCategoryViewModel();
                cat.CategoryID = category.ID;
                cat.CategoryName = category.Name;
                cat.CategoryDescription = category.Description;
                cat.CategoryRating = category.Rating;

                cat.Parameters = new List<BehaviourParameterViewModel>();
                foreach (var parameter in parameters.Where(n => n.CategoryId == category.ID))
                {
                    var StaffBehaviourParameterViewModel = new BehaviourParameterViewModel();
                    StaffBehaviourParameterViewModel.ParameterID = parameter.ID;
                    StaffBehaviourParameterViewModel.ParameterName = parameter.Name;
                    StaffBehaviourParameterViewModel.ParameterRating = parameter.Rating;
                    StaffBehaviourParameterViewModel.ParameterDescription = parameter.Description;
                    StaffBehaviourParameterViewModel.BehaviourDetailsByStaff = new List<BehaviourDetailsByStaff>();

                    var _staffParams = staffParams.Where(n => n.ParameterId == parameter.ID);
                    var staffIds = _staffParams.Select(n => n.EvaluatingStaffId).Distinct();
                    foreach (var _evaluatingStaffId in staffIds)
                    {
                        var StaffBehaviourDetailsByStaff = new BehaviourDetailsByStaff();
                        StaffBehaviourDetailsByStaff.BehaviourDetails = new List<BehaviourDetails>();
                        foreach (var staffParam in _staffParams.Where(n => n.EvaluatingStaffId == _evaluatingStaffId))
                        {
                            StaffBehaviourDetailsByStaff.StaffId = _evaluatingStaffId;
                            StaffBehaviourDetailsByStaff.StaffName = staffParam.Staff.Name;

                            var StaffBehaviourDetails = new BehaviourDetails();
                            StaffBehaviourDetails.ID = staffParam.ID;
                            StaffBehaviourDetails.StaffComment = staffParam.Comment;
                            StaffBehaviourDetails.StaffRating = staffParam.Rating;
                            StaffBehaviourDetails.Date = staffParam.CreatedOn.GetValueOrDefault();

                            StaffBehaviourDetailsByStaff.BehaviourDetails.Add(StaffBehaviourDetails);
                        }

                        StaffBehaviourParameterViewModel.BehaviourDetailsByStaff.Add(StaffBehaviourDetailsByStaff);
                    }

                    cat.Parameters.Add(StaffBehaviourParameterViewModel);
                }

                viewModel.Categories.Add(cat);
            }

            return viewModel;
        }

        public PerformanceChartData GetPerformance(int staffId, int categoryId, int parameterId)
        {
            var result = dbContext.StaffBehaviours.Where(n => n.StaffId == staffId
                && (categoryId > 0 ? n.StaffBehaviourParameter.CategoryId == categoryId : true)
                && (parameterId > 0 ? n.ParameterId == parameterId : true)).ToList();
            var response = new PerformanceChartData();
            response.Parameters = result.Select(n => n.StaffBehaviourParameter.Name).Distinct().ToList();
            response.RatingsByStaff = new List<RatingsByStaff>();

            var parmIds = result.Select(n => n.ParameterId).Distinct();
            var staffIds = result.Select(n => n.EvaluatingStaffId).Distinct();

            foreach (var _evaluatingStaffId in staffIds)
            {
                var obj = new RatingsByStaff();
                obj.Ratings = new List<float>();
                foreach (var parmId in parmIds)
                {

                    var ratingByStaff = result.Where(n => n.EvaluatingStaffId == _evaluatingStaffId && n.ParameterId == parmId).ToList();
                    if (ratingByStaff.Count() > 0)
                    {
                        obj.StaffName = ratingByStaff.FirstOrDefault().Staff.Name;
                        var avg = (float)ratingByStaff.Select(n => n.Rating).Sum() / ratingByStaff.Count();
                        obj.Ratings.Add(avg);
                    }
                    else
                    {
                        obj.Ratings.Add(0.0f);
                    }
                }

                response.RatingsByStaff.Add(obj);
            }

            return response;
        }
        public List<StaffBehaviour> GetStaffBehaviourBy(int staffId)
        {
            return null;
        }

        public bool CreateOrUpdateStaffBehaviour(List<StaffBehaviour> objs)
        {
            foreach (var obj in objs)
            {
                if (obj.ID > 0)
                {
                    dbContext.Entry(obj).State = EntityState.Modified;
                }
                else
                {
                    dbContext.StaffBehaviours.Add(obj);
                }
            }

            dbContext.SaveChanges();
            return true;
        }

        public bool DeleteStaffBehaviour(int id)
        {
            return false;
        }
        #endregion
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
