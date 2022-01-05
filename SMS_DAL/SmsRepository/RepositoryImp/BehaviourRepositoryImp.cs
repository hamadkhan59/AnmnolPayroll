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
    public class BehaviourRepositoryImp : IBehaviourRepository
    {
        private readonly SC_WEBEntities2 dbContext;

        public BehaviourRepositoryImp(SC_WEBEntities2 context)
        {
            //dbContext.Configuration.LazyLoadingEnabled = false;            
            dbContext = context;
        }

        #region Categories
        public BehaviourCategory GetCategory(int id)
        {
            return dbContext.BehaviourCategories.Where(n => n.ID == id).FirstOrDefault();
        }

        public List<BehaviourCategory> GetCategoriesByBranch(int brachId)
        {
            return dbContext.BehaviourCategories.Where(n => n.BranchId == brachId).ToList();
        }

        public List<BehaviourCategoryViewModel> GetCategoryViewsByBranch(int brachId)
        {
            var cats = dbContext.BehaviourCategories.Where(n => n.BranchId == brachId).ToList();
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

        public BehaviourCategory CreateOrUpdateCategory(BehaviourCategory obj)
        {
            if (obj.ID > 0)
            {
                dbContext.Entry(obj).State = EntityState.Modified;
            }
            else
            {
                //var parameter = new BehaviourParameter();
                //parameter.BehaviourCategory = obj;
                //parameter.Name = "Others";
                //parameter.Rating = 100;
                //parameter.Description = "Other Aspects of Behaviour under " + obj.Name;
                //dbContext.BehaviourParameters.Add(parameter);
                dbContext.BehaviourCategories.Add(obj);
            }

            dbContext.SaveChanges();
            return obj;
        }

        public bool DeleteCategory(int id)
        {
            var category = dbContext.BehaviourCategories.Where(n => n.ID == id).FirstOrDefault();
            if (category != null)
            {
                dbContext.BehaviourCategories.Remove(category);
                dbContext.SaveChanges();
                return true;
            }

            return false;
        }
        #endregion

        #region Parameters
        public BehaviourParameter GetParameter(int id)
        {
            return dbContext.BehaviourParameters.Where(n => n.ID == id).FirstOrDefault();
        }

        public List<BehaviourParameter> GetParametersByBranch(int brachId)
        {
            return dbContext.BehaviourParameters.Where(n => n.BehaviourCategory.BranchId == brachId).OrderBy(n => n.CategoryId).ToList();
        }

        public List<BehaviourParameter> GetParametersByCategory(int catId)
        {
            return dbContext.BehaviourParameters.Where(n => n.CategoryId == catId).ToList();
        }

        public List<BehaviourParameterViewModel> GetParameterViewByCategory(int catId)
        {
            var parms = dbContext.BehaviourParameters.Where(n => n.CategoryId == catId).ToList();
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

        public BehaviourParameter CreateOrUpdateParameter(BehaviourParameter obj)
        {
            //var category = GetCategory(obj.CategoryId);
            //obj.Rating = obj.Rating / category.Rating * 100.0d;
            //var other = dbContext.BehaviourParameters.Where(n => n.CategoryId == obj.CategoryId && n.Name == "Others").FirstOrDefault();
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
                dbContext.BehaviourParameters.Add(obj);
            }

            dbContext.SaveChanges();
            return obj;
        }

        public bool DeleteParameter(int id)
        {
            var Parameter = dbContext.BehaviourParameters.Where(n => n.ID == id).FirstOrDefault();
            if (Parameter != null)
            {
                dbContext.BehaviourParameters.Remove(Parameter);
                dbContext.SaveChanges();
                return true;
            }

            return false;
        }
        #endregion

        #region Student_Behaviour
        public StudentBehaviourViewModel GetStudentBehaviour(int studentId, int staffId, int branchId)
        {
            var student = (new StudentRepositoryImp(dbContext)).GetStudentById(studentId);
            var categories = GetCategoriesByBranch(branchId);
            var parameters = GetParametersByBranch(branchId);
            var studentParams = dbContext.StudentBehaviours.Where(n => n.StaffId == staffId && n.StudentId == studentId);

            var viewModel = new StudentBehaviourViewModel();
            viewModel.StaffID = staffId;
            if (student != null)
            {
                viewModel.StudentID = student.id;
                viewModel.StudentName = student.Name;
                viewModel.FatherName = student.FatherName;
                viewModel.RollNumber = student.RollNumber;
                viewModel.BForm = student.BFormNo;
                viewModel.Class = student.ClassSection != null ? student.ClassSection.Class.Name : null;
                viewModel.Section = student.ClassSection != null ? student.ClassSection.Section.Name : null;
                viewModel.ImageSource = student.StdImage;
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
                    var behaviourParameterViewModel = new BehaviourParameterViewModel();
                    behaviourParameterViewModel.ParameterID = parameter.ID;
                    behaviourParameterViewModel.ParameterName = parameter.Name;
                    behaviourParameterViewModel.ParameterRating = parameter.Rating;
                    behaviourParameterViewModel.ParameterDescription = parameter.Description;
                    behaviourParameterViewModel.BehaviourDetails = new List<BehaviourDetails>();

                    var _studentParams = studentParams.Where(n => n.ParameterId == parameter.ID);
                    foreach (var studentParam in _studentParams)
                    {
                        var behaviourDetails = new BehaviourDetails();
                        behaviourDetails.ID = studentParam.ID;
                        behaviourDetails.StaffComment = studentParam.Comment;
                        behaviourDetails.StaffRating = studentParam.Rating;
                        behaviourDetails.Date = studentParam.CreatedOn.GetValueOrDefault();

                        behaviourParameterViewModel.BehaviourDetails.Add(behaviourDetails);
                    }

                    var newBehaviourDetails = new BehaviourDetails();
                    newBehaviourDetails.Date = DateTime.UtcNow;
                    newBehaviourDetails.StaffRating = -1;
                    behaviourParameterViewModel.BehaviourDetails.Add(newBehaviourDetails);

                    cat.Parameters.Add(behaviourParameterViewModel);
                }

                viewModel.Categories.Add(cat);
            }

            return viewModel;
        }

        public StudentBehaviourViewModel GetOverallStudentBehaviour(int studentId, int branchId)
        {
            var student = (new StudentRepositoryImp(dbContext)).GetStudentById(studentId);
            var categories = GetCategoriesByBranch(branchId);
            var parameters = GetParametersByBranch(branchId);
            var studentParams = dbContext.StudentBehaviours.Where(n => n.StudentId == studentId);

            var viewModel = new StudentBehaviourViewModel();
            if (student != null)
            {
                viewModel.StudentID = student.id;
                viewModel.StudentName = student.Name;
                viewModel.FatherName = student.FatherName;
                viewModel.RollNumber = student.RollNumber;
                viewModel.BForm = student.BFormNo;
                viewModel.Class = student.ClassSection != null ? student.ClassSection.Class.Name : null;
                viewModel.Section = student.ClassSection != null ? student.ClassSection.Section.Name : null;
                viewModel.ImageSource = student.StdImage;
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
                    var behaviourParameterViewModel = new BehaviourParameterViewModel();
                    behaviourParameterViewModel.ParameterID = parameter.ID;
                    behaviourParameterViewModel.ParameterName = parameter.Name;
                    behaviourParameterViewModel.ParameterRating = parameter.Rating;
                    behaviourParameterViewModel.ParameterDescription = parameter.Description;
                    behaviourParameterViewModel.BehaviourDetailsByStaff = new List<BehaviourDetailsByStaff>();

                    var _studentParams = studentParams.Where(n => n.ParameterId == parameter.ID);
                    var staffIds = _studentParams.Select(n => n.StaffId).Distinct();
                    foreach (var staffId in staffIds)
                    {
                        var behaviourDetailsByStaff = new BehaviourDetailsByStaff();
                        behaviourDetailsByStaff.BehaviourDetails = new List<BehaviourDetails>();
                        foreach (var studentParam in _studentParams.Where(n => n.StaffId == staffId))
                        {
                            behaviourDetailsByStaff.StaffId = staffId;
                            behaviourDetailsByStaff.StaffName = studentParam.Staff.Name;

                            var behaviourDetails = new BehaviourDetails();
                            behaviourDetails.ID = studentParam.ID;
                            behaviourDetails.StaffComment = studentParam.Comment;
                            behaviourDetails.StaffRating = studentParam.Rating;
                            behaviourDetails.Date = studentParam.CreatedOn.GetValueOrDefault();

                            behaviourDetailsByStaff.BehaviourDetails.Add(behaviourDetails);
                        }

                        behaviourParameterViewModel.BehaviourDetailsByStaff.Add(behaviourDetailsByStaff);
                    }

                    cat.Parameters.Add(behaviourParameterViewModel);
                }

                viewModel.Categories.Add(cat);
            }

            return viewModel;
        }

        public PerformanceChartData GetStudentPerformance(int studentId, int categoryId, int parameterId)
        {
            var result = dbContext.StudentBehaviours.Where(n => n.StudentId == studentId
                && (categoryId > 0 ? n.BehaviourParameter.CategoryId == categoryId : true)
                && (parameterId > 0 ? n.ParameterId == parameterId : true)).ToList();
            var response = new PerformanceChartData();
            response.Parameters = result.Select(n => n.BehaviourParameter.Name).Distinct().ToList();
            response.RatingsByStaff = new List<RatingsByStaff>();

            var parmIds = result.Select(n => n.ParameterId).Distinct();
            var staffIds = result.Select(n => n.StaffId).Distinct();

            foreach (var staffId in staffIds)
            {
                var obj = new RatingsByStaff();
                obj.Ratings = new List<float>();
                foreach (var parmId in parmIds)
                {

                    var ratingByStaff = result.Where(n => n.StaffId == staffId && n.ParameterId == parmId).ToList();
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
        public List<StudentBehaviour> GetStudentBehaviourByStudent(int studentId)
        {
            return null;
        }

        public bool CreateOrUpdateStudentBehaviour(List<StudentBehaviour> objs)
        {
            foreach (var obj in objs)
            {
                if (obj.ID > 0)
                {
                    dbContext.Entry(obj).State = EntityState.Modified;
                }
                else
                {
                    dbContext.StudentBehaviours.Add(obj);
                }
            }

            dbContext.SaveChanges();
            return true;
        }

        public bool DeleteStudentBehaviour(int id)
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
