using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Objects;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class SecurityRepositoryImp : ISecurityRepository
    {
        private SC_WEBEntities2 dbContext1;

        IStudentRepository studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());
        public SecurityRepositoryImp(SC_WEBEntities2 context)
        {
            dbContext1 = context;
        }

        SC_WEBEntities2 dbContext
        {
            get
            {
                if (dbContext1 == null || this.disposed == true)
                    dbContext1 = new SC_WEBEntities2();
                this.disposed = false;
                return dbContext1;
            }
        }

        #region USER_GROUP_FUNCTIONS
        public int AddUserGroup(UserGroup userGroup)
        {
            int result = -1;
            if (userGroup != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.UserGroups.Add(userGroup);
                dbContext.SaveChanges();
                result = userGroup.Id;
            }

            return result;
        }

        public UserGroup GetUserGroupById(int UserGroupId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.UserGroups.Where(x => x.Id == UserGroupId).FirstOrDefault();
        }

        public UserGroup GetUserGroupByName(string UserGroupName, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.UserGroups.Where(x => x.Name == UserGroupName && x.BranchId == branchId).FirstOrDefault();
        }

        public UserGroup GetUserGroupByNameAndId(string UserGroupName, int UserGroupId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.UserGroups.Where(x => x.Name == UserGroupName && x.Id != UserGroupId && x.BranchId == branchId).FirstOrDefault();
        }

        public List<UserGroup> GetAllUserGroups(int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.UserGroups.Where(x => x.BranchId == branchId).ToList();
        }

        public void UpdateUserGroup(UserGroup userGroup)
        {
            if (userGroup != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(userGroup).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteUserGroup(UserGroup userGroup)
        {
            if (userGroup != null)
            {
                var permissions = dbContext.GroupPermissions.Where(x => x.GroupId == userGroup.Id).ToList();
                if (permissions != null && permissions.Count > 0)
                {
                    foreach (var permission in permissions)
                    {
                        dbContext.GroupPermissions.Remove(permission);
                    }
                }

                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.UserGroups.Remove(userGroup);
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region USER_FUNCTIONS
        public int AddUser(User user)
        {
            int result = -1;
            if (user != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                result = user.Id;
            }

            return result;
        }

        public User GetUserById(int UserId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Users.Where(x => x.Id == UserId).FirstOrDefault();
        }

        public User AuthenticateUser(string userName, string password, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Users.Where(x => x.UserName == userName && x.Password == password && x.BranchId == branchId).FirstOrDefault();
        }

        public User AuthenticateUser(string userName, string password)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            //SC_WEBEntities2 context = new SC_WEBEntities2();
            //var query = from user in context.Users
            //            where user.LoginId == userName && user.Password == password
            //            select user;

            //return query.FirstOrDefault();
            var user =  dbContext.Users.Where(x => x.LoginId == userName && x.Password == password).FirstOrDefault();
            if(user != null)
                dbContext.Entry<User>(user).Reload();
            return user;
        }

        public Branch AuthenticateBranch(string loginId, string password)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Branches.Where(x => x.LOGIN_ID == loginId && x.PASSWORD == password).FirstOrDefault();
        }

        public User GetUserByName(string UserName, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Users.Where(x => x.LoginId == UserName && x.BranchId == branchId).FirstOrDefault();
        }

        public User GetUserByNameAndId(string UserName, int UserId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Users.Where(x => x.LoginId == UserName && x.Id != UserId && x.BranchId == branchId).FirstOrDefault();
        }

        public List<User> GetAllUsers(int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            if (branchId == 1)
                return dbContext.Users.ToList();
            else
                return dbContext.Users.Where(x => x.BranchId == branchId).ToList();
        }

        public List<User> GetAllUsers()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Users.ToList();
        }

        public void UpdateUser(User user)
        {
            if (user != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(user).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteUser(User user)
        {
            if (user != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region SESSION_USER_FUNCTIONS
        public int AddSessionUser(SessionUser sessionUser)
        {
            int result = -1;
            if (sessionUser != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.SessionUsers.Add(sessionUser);
                dbContext.SaveChanges();
                result = sessionUser.ID;
            }

            return result;
        }

        public void UpdateSessionUser(SessionUser sessionUser)
        {
            if (sessionUser != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(sessionUser).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region PERMISSION_FUNCTION

        public int AddPermission(Permission permission)
        {
            int result = -1;
            if (permission != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Permissions.Add(permission);
                dbContext.SaveChanges();
                result = permission.Id;
            }

            return result;
        }

        //public Permission GetPermission(int GroupId, string moduleName, string subModuleName)
        //{
        //    dbContext.Configuration.LazyLoadingEnabled = false;
        //    return dbContext.Permissions.Where(x => x.GroupId == GroupId && x.ModuleName == moduleName
        //                                && x.SubModuleName == subModuleName).FirstOrDefault();
        //}

        public GroupPermission GetPermissionById(int permissionId)
        {
            return dbContext.GroupPermissions.Find(permissionId);
        }

        public void UpdatePermission(Permission permission)
        {
            if (permission != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(permission).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void UpdateGroupPermission(GroupPermission permission)
        {
            if (permission != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(permission).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
        //public List<Permission> GetPermissionsByGroup(int GroupId)
        //{
        //    dbContext.Configuration.LazyLoadingEnabled = false;
        //    return dbContext.Permissions.Where(x => x.GroupId == GroupId).ToList();
        //}

        public List<PermissionModel> GetPermissionsByGroup(int GroupId)
        {
            List<PermissionModel> modelList = null;

            using (SC_WEBEntities2 conn = new SC_WEBEntities2())
            {
                var query = from grpPerm in conn.GroupPermissions
                            join perm in conn.Permissions on grpPerm.PermissionId equals perm.Id
                            join grp in conn.UserGroups on grpPerm.GroupId equals grp.Id
                            where grpPerm.GroupId == GroupId
                            select new PermissionModel
                            {
                                Id = grpPerm.Id,
                                GroupId = (int)grpPerm.GroupId,
                                PermissionId = (int)grpPerm.PermissionId,
                                Granted = (bool)grpPerm.IsGranted,
                                GroupName = grp.Name,
                                ModuleName = perm.ModuleName,
                                SubModuleName = perm.SubModuleName,
                            };
                modelList = query.ToList().OrderBy(x => x.ModuleName).ToList();
            }

            return modelList;
        }

        #endregion

        #region USER_FUNCTIONS
        public int AddBracnh(Branch branch)
        {
            int result = -1;
            if (branch != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Branches.Add(branch);
                dbContext.SaveChanges();
                result = branch.ID;
            }

            return result;
        }

        public YoutubeVideo GetVideoUrl(int videoId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.YoutubeVideos.Where(x => x.Id == videoId).FirstOrDefault();
        }

        public Branch GetBranchById(int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Branches.Where(x => x.ID == branchId).FirstOrDefault();
        }

        public Branch AuthenticateBracnh(string branchName, string password)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Branches.Where(x => x.LOGIN_ID == branchName && x.PASSWORD == password).FirstOrDefault();
        }

        public Branch GetBranchByName(string branchName)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Branches.Where(x => x.BRANCH_NAME == branchName).FirstOrDefault();
        }

        public Branch GetBranchByCode(string branchCode)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Branches.Where(x => x.BRANCH_CODE == branchCode).FirstOrDefault();
        }

        public Branch GetBranchByLoginId(string loginId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Branches.Where(x => x.LOGIN_ID == loginId).FirstOrDefault();
        }

        public Branch GetBranchByNameAndId(string branchName, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Branches.Where(x => x.BRANCH_NAME == branchName && x.ID != branchId).FirstOrDefault();
        }

        public Branch GetBranchByCodeAndId(string branchCode, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Branches.Where(x => x.BRANCH_CODE == branchCode && x.ID != branchId).FirstOrDefault();
        }

        public Branch GetBranchByLoginIdAndId(string loginId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Branches.Where(x => x.LOGIN_ID == loginId && x.ID != branchId).FirstOrDefault();
        }
        public List<Branch> GetAllBranches()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Branches.ToList();
            //return dbContext.Branches.Select(c => new Branch
            //{
            //    BRANCH_NAME = c.BRANCH_NAME,
            //    BRANCH_CODE = c.BRANCH_CODE,
            //    EMAIL = c.EMAIL,
            //    PHONE_NUMBER = c.PHONE_NUMBER,
            //    ADDRESS = c.ADDRESS,
            //    ID = c.ID
            //}).ToList();
        }

        public void UpdateBranch(Branch bracnh)
        {
            if (bracnh != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(bracnh).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteBranch(Branch bracnh)
        {
            if (bracnh != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Branches.Remove(bracnh);
                dbContext.SaveChanges();
            }
        }
        #endregion


        public void AddSchoolConfig(SchoolConfig schoolConfig)
        {
            dbContext.SchoolConfigs.Add(schoolConfig);
            dbContext.SaveChanges();
        }

        public void UpdateSchoolConfig(SchoolConfig schoolConfig)
        {
            dbContext.Entry(schoolConfig).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public SchoolConfig GetSchoolConfigByBranchId(int branchId)
        {
            return dbContext.SchoolConfigs.Where(x => x.BranchId == branchId).FirstOrDefault();
        }

        public int GetUserCount(int UserGroupId)
        {
            return dbContext.Users.Where(x => x.UserGroupId == UserGroupId).Count();
        }

        public int GetSessionUserCount(int UserId)
        {
            return dbContext.SessionUsers.Where(x => x.USER_ID == UserId).Count();

        }

        #region PARENTS_FUNCTIONS
        public string AddParent(AppUser parent)
        {
            int result = -1;
            List<Student> student = studentRepo.GetStudentByParentCnic(parent.ContactNo);
            List<AppUser> Parent = getParent(parent.ContactNo);
            if (parent != null && student.Count > 0 && Parent.Count < 1)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.AppUsers.Add(parent);
                dbContext.SaveChanges();
                result = parent.Id;
                return AppConstHelper.LOGIN_SUCCESS;
            }
            else
            {
                if (Parent.Count >= 1)
                {
                    return AppConstHelper.LOGIN_ALREADY_EXIST;
                }
                else
                {
                    return AppConstHelper.LOGIN_NO_DATA;
                }
            }

        }
        public List<AppUser> getParent(string contact)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.AppUsers.Where(x => x.ContactNo == contact).ToList();
        }
        public AppUser AuthenticateParent(string userName, string password)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.AppUsers.Where(x => (x.ContactNo == userName) && x.Password == password).FirstOrDefault();
        }
        public void SetSession(string session, int parentId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            AppUser parent = dbContext.AppUsers.Where(x => x.Id == parentId).FirstOrDefault();
            parent.Session = session;
            dbContext.Entry(parent).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
        #endregion

        public string RegisterStaff(AppUser staffUser)
        {
            string errorCode = AppConstHelper.LOGIN_SUCCESS;
            var staff = dbContext.Staffs.Where(x => x.PhoneNumber == staffUser.ContactNo).FirstOrDefault();

            if (staff == null)
            {
                errorCode = AppConstHelper.LOGIN_NO_DATA;
            }
            else
            {
                var loginExist = dbContext.AppUsers.Where(x => x.ContactNo == staffUser.ContactNo && x.LoginType == 2).FirstOrDefault();
                if (loginExist != null)
                {
                    errorCode = AppConstHelper.LOGIN_ALREADY_EXIST;
                }
                else
                {
                    staffUser.LoginType = 2;

                    dbContext.AppUsers.Add(staffUser);
                    dbContext.SaveChanges();
                }
            }
            return errorCode;
        }

        public AppUser AuthenticateStaff(string phoneNo, string password)
        {
            return dbContext.AppUsers.Where(x => x.ContactNo == phoneNo && x.Password == password && x.LoginType == 2).FirstOrDefault();
        }

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
