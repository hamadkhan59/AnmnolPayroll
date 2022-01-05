using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface ISecurityRepository : IDisposable
    {
        int AddUserGroup(UserGroup userGroup);
        UserGroup GetUserGroupById(int UserGroupId);
        UserGroup GetUserGroupByName(string UserGroupName, int branchId);
        UserGroup GetUserGroupByNameAndId(string UserGroupName, int UserGroupId, int branchId);
        List<UserGroup> GetAllUserGroups(int branchId);
        void UpdateUserGroup(UserGroup userGroup);
        void DeleteUserGroup(UserGroup userGroup);

        int AddUser(User user);
        User GetUserById(int UserId);
        User AuthenticateUser(string userName, string password, int branchId);
        User AuthenticateUser(string userName, string password);
        User GetUserByName(string UserName, int branchId);
        User GetUserByNameAndId(string UserName, int UserId, int branchId);
        List<User> GetAllUsers(int branchId);
        List<User> GetAllUsers();
        void UpdateUser(User user);
        void DeleteUser(User user);

        void UpdateSessionUser(SessionUser sessionUse);
        int AddSessionUser(SessionUser sessionUser);

        int AddPermission(Permission permission);
        GroupPermission GetPermissionById(int permissionId);
        //Permission GetPermission(int GroupId, string moduleName, string subModuleName);
        void UpdatePermission(Permission permission);
        void UpdateGroupPermission(GroupPermission permission);
        List<PermissionModel> GetPermissionsByGroup(int GroupId);

        YoutubeVideo GetVideoUrl(int videoId);

        int AddBracnh(Branch branch);
        Branch GetBranchById(int branchId);
        Branch AuthenticateBracnh(string branchName, string password);
        Branch GetBranchByName(string branchName);
        Branch GetBranchByCode(string branchCode);
        Branch GetBranchByLoginId(string loginId);
        Branch GetBranchByNameAndId(string branchName, int branchId);
        Branch GetBranchByCodeAndId(string branchCode, int branchId);
        Branch GetBranchByLoginIdAndId(string loginId, int branchId);
        List<Branch> GetAllBranches();
        void UpdateBranch(Branch bracnh);
        void DeleteBranch(Branch bracnh);

        Branch AuthenticateBranch(string loginId, string password);

        void AddSchoolConfig(SchoolConfig schoolConfig);
        void UpdateSchoolConfig(SchoolConfig schoolConfig);
        SchoolConfig GetSchoolConfigByBranchId(int branchId);
        int GetUserCount(int UserGroupId);
        int GetSessionUserCount(int UserId);

        string AddParent(AppUser parent);
        //Parent getParent(string cnic, string contact);
        AppUser AuthenticateParent(string userName, string password);
        void SetSession(string session, int parentId);

        AppUser AuthenticateStaff(string phoneNo, string password);

        string RegisterStaff(AppUser staffUser);

    }
}
