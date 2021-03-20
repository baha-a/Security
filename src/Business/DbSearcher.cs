using ITech.Security.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Web.WebSockets;
using Microsoft.Ajax.Utilities;
using Expression = System.Linq.Expressions.Expression;


namespace ITech.Security.Business
{
    public class DbSearcher
    {
        public SecurityDB db { get; private set; }
        public DbSearcher(SecurityDB dataBase)
        {
            db = dataBase;
            //db.Configuration.ProxyCreationEnabled = false;
        }

        private static Expression<Func<Role, bool>> IsRoleAlive(int applicationId = -1)
        {
            if (applicationId == -1)
                return x => x.IsDeleted == false;
            return x => x.IsDeleted == false && x.ApplicationId == applicationId;
        }
        private static Expression<Func<Operation, bool>> IsOperationAlive(int applicationId = -1)
        {
            if (applicationId == -1)
                return x => x.IsDeleted == false;
            return x => x.IsDeleted == false && x.ApplicationId == applicationId;
        }
        private static Expression<Func<Model, bool>> IsModelAlive(int applicationId = -1)
        {
            if (applicationId == -1)
                return x => x.IsDeleted == false;
            return x => x.IsDeleted == false && x.ApplicationId == applicationId;
        }

      



        public IEnumerable<Application> GetAllApplications()
        {
            return db.Applications.ToList();
        }

        public IQueryable<Role> GetAllRoles(int applicationId = -1)
        {
            return db.Roles.Where(IsRoleAlive(applicationId));
        }

        public IQueryable<User> GetAllUsers()
        {
            return db.Users.Where(x => x.IsDeleted == false);
        }

        public IQueryable<Operation> GetAllOperations(int applicationId)
        {
            return db.Operations.Where(IsOperationAlive(applicationId));
        }

        public IQueryable<Model> GetAllModels(int applicationId)
        {
            return db.Models.Where(IsModelAlive(applicationId));
        }

        public IQueryable<Operation> GetAllOperationsOfModel(int modelId)
        {
            return db.Models
                .Include(x => x.Operations)
                .SingleOrDefault(x => x.Id == modelId)
                .Operations
                .AsQueryable()
                .Where(IsOperationAlive());
        }

        public IQueryable<Operation> GetAllOperationsOfRole(int roleId)
        {
            return getRoleOperationIncludOperation(roleId).Select(x => x.Operation);
        }

        public IQueryable<ScopedOperation> GetAllOperationsOfRoleWithScope(int roleId)
        {
            List<ScopedOperation> res = new List<ScopedOperation>();
            var role = FindRoleById(roleId);
            foreach (var p in getRoleOperationIncludOperation(roleId))
            {
                res.Add(new ScopedOperation()
                {
                    Role = role,
                    Operation = p.Operation,
                    Scope = p.Scope,
                });
            }
            return res.AsQueryable();
        }

        private IQueryable<RoleOperation> getRoleOperationIncludOperation(int roleId)
        {
            return db.RoleOperations
                .Include(x => x.Operation)
                .Where(x => x.RoleId == roleId)
                .Where(x => x.IsDeleted == false && x.Operation.IsDeleted == false);
        }

        public IQueryable<Role> GetAllRolesOfOperations(int operationId)
        {
            return db.RoleOperations
                .Include(x => x.Role)
                .Where(x => x.OperationId == operationId && x.IsDeleted == false)
                .Select(x => x.Role)
                .Where(IsRoleAlive());
        }

        public IQueryable<User> GetAllUsersOfRole(int roleId)
        {
            return db.UserRoles
                .Include(x => x.User)
                .Where(x => x.RoleId == roleId && x.IsDeleted == false && x.User.IsDeleted == false)
                .Select(x => x.User)
                .Where(x => x.IsDeleted == false);
        }

        public IQueryable<Role> GetAllRolesOfUser(int userId)
        {
            return db.UserRoles
                .Include(x => x.Role)
                .Include(x => x.User)
                .Where(x => x.UserId == userId &&
                            x.IsDeleted == false &&
                            x.User.IsDeleted == false &&
                            x.Role.IsDeleted == false)
                .Select(x => x.Role);
        }

        public IEnumerable<View> GetAllUserViewsOfSection(string userName, string sectionName, int applicationId)
        {
            return GetAllUserViews(userName)
                .Where(x => x.Section.Name == sectionName && x.Section.ApplicationId == applicationId);
        }
        public IEnumerable<View> GetAllUserViewsOfTheSectionTopMenu(string userName, int applicationId)
        {
            return GetAllUserViews(userName)
                .Where(x => x.Section.IsTopMenu && x.Section.ApplicationId == applicationId);
        }

        public IEnumerable<View> GetAllUserViews(string username)
        {
            return GetAllUserViews(FindUser(username).Id);
        }

        public IEnumerable<View> GetAllUserViews(int userId)
        {
            var roleIds = GetAllRolesOfUser(userId).Select(x => x.Id);

            List<View> views = new List<View>();

            foreach (var r in roleIds)
                views.AddRange(GetAllViewsOfRole(r));

            return views.Distinct();
        }

        public IEnumerable<View> GetAllViewsOfRole(int roleId)
        {
            var viewIds = getAllViewIdsOfRole(roleId);

            List<View> views = new List<View>();
            foreach (var id in viewIds)
                views.Add(FindViewById(id));

            return views;
        }
        private IQueryable<int> getAllViewIdsOfRole(int roleId)
        {
            return db.RoleViews
                .Where(x => x.RoleId == roleId && x.IsDeleted == false)
                .Select(x => x.ViewId);
        }

        public IQueryable<UserRole> GetAllUserRoleOfUser(int userId)
        {
            return db.UserRoles.Where(x => x.IsDeleted == false && x.UserId == userId);
        }

        public IQueryable<View> GetAllViews(int applicationId)
        {
            return db.Views.Include(x => x.Section)
                .Where(x => x.IsDeleted == false && x.Section.IsDeleted == false && x.ApplicationId == applicationId);
        }



        public Application FindApplication(string appName)
        {
            return db.Applications
                .FirstOrDefault(x => x.IsDeleted == false && x.Name == appName);
        }
        public Application FindApplicationById(int appId)
        {
            return db.Applications
                .SingleOrDefault(x => x.IsDeleted == false && x.Id == appId);
        }

        public Role FindRole(string roleName, int applicationId)
        {
            return db.Roles.Where(x => x.Name == roleName)
                .Include(x => x.Application)
                .SingleOrDefault(IsRoleAlive(applicationId));
        }
        public Role FindRoleById(int id)
        {
            return db.Roles.Where(x => x.Id == id)
                .SingleOrDefault(IsRoleAlive());
        }

        public User FindUser(string userName)
        {
            return db.Users.Where(x => x.UserName == userName)
                .SingleOrDefault(x => x.IsDeleted == false);
        }
        public User FindUserById(int userId)
        {
            return db.Users.Where(x => x.Id == userId)
                .SingleOrDefault(x => x.IsDeleted == false);
        }

        public UserRole FindUserRoleById(int userRoleId)
        {
            return db.UserRoles
                .Where(x => x.Id == userRoleId)
                .SingleOrDefault(x => x.IsDeleted == false && x.User.IsDeleted == false);
        }
        public UserRole FindUserRole(int userId, int roleId)
        {
            return db.UserRoles
                .Where(x => x.UserId == userId && x.RoleId == roleId)
                .FirstOrDefault(x => x.IsDeleted == false && x.User.IsDeleted == false);
        }


        public Operation FindOperation(string operationName, int applicationId)
        {
            return db.Operations.Where(x => x.Name == operationName)
                .SingleOrDefault(IsOperationAlive(applicationId));
        }
        public Operation FindOperationById(int id)
        {
            return db.Operations.Where(x => x.Id == id)
                .SingleOrDefault(IsOperationAlive());
        }
        public RoleOperation FindRoleOperation(int roleId, int operationId)
        {
            var role = FindRoleById(roleId);
            var oper = FindOperationById(operationId);

            if (role == null || oper == null)
                throw new ResourceNotFoundException("Role or Operation not found");

            return db.RoleOperations
                .Include(x=>x.Role)
                .Include(x=>x.Operation)
                .Where(x => x.RoleId == role.Id && x.OperationId == oper.Id)
                .SingleOrDefault(x => x.IsDeleted == false && x.Operation.IsDeleted == false && x.Role.IsDeleted == false);
        }

        public Model FindModelById(int id)
        {
            return db.Models.Where(x => x.Id == id)
                .SingleOrDefault(IsModelAlive());
        }
        private Model FindModel(string modelName, int applicationId)
        {
            return db.Models.Where(x => x.Name == modelName)
                .SingleOrDefault(IsModelAlive(applicationId));
        }

        public View FindViewById(int viewId)
        {
            return db.Views
                .Include(x => x.Section)
                .Where(x => x.Id == viewId)
                .SingleOrDefault(x => x.IsDeleted == false && x.Section.IsDeleted == false);
        }

        public RoleView FindViewRole(int viewId, int roleId)
        {
            return db.RoleViews.SingleOrDefault(x => x.IsDeleted == false && x.ViewId == viewId && x.RoleId == roleId);
        }


        public bool IsRoleHaveOperation(int roleId, int operationId, int applicationId)
        {
            string tmp = "";
            return IsRoleHaveOperation(roleId, operationId, ref tmp);
        }
        public bool IsRoleHaveOperation(int roleId, int operationId, ref string scope)
        {
            var res = FindRoleOperation(roleId, operationId);
            if (res == null)
                return false;

            scope = res.Scope;
            return true;
        }


        public bool IsUserInRole(string userName, string role, int applicationId)
        {
            return IsUserInRole(FindUser(userName).Id, FindRole(role, applicationId).Id);
        }

        public bool IsUserInRole(int userId, int roleId)
        {
            return FindUserRole(userId, roleId) != null;
        }

        public ScopedBoolean IsAllowedWithModel(string userName, string operationName, string modelName, int applicationId)
        {
            foreach (var r in GetAllRolesOfUser(FindUser(userName).Id))
                if (GetRoleOperationOfModel(r.Id,operationName, modelName, applicationId).Any())
                    return ScopedBoolean.Allowed;

            return ScopedBoolean.Denied;
        }

        private IEnumerable<Operation> GetRoleOperationOfModel(int roleId,string operationName, string modelName, int applicationId)
        {
            return GetAllOperationsOfRole(roleId)
                .Include(x => x.Model)
                .Where(x => x.Name == operationName)
                .Where(x => x.Model != null && x.Model.IsDeleted == false &&
                                     x.Model.Name == modelName &&
                                     x.Model.ApplicationId == applicationId);
        }

        public ScopedBoolean IsAllow(string userName, string operationName, int applicationId)
        {
            var user = FindUser(userName);
            if (user == null)
                return ScopedBoolean.Denied;

            var operation = FindOperation(operationName, applicationId);
            if (operation == null)
                return ScopedBoolean.Denied;

            var roles = GetAllRolesOfUser(user.Id);

            bool isAllowed;
            var scopes = searchForPermissionsAndScopes(roles, operation.Id, out isAllowed);

            return new ScopedBoolean()
            {
                Scopes = scopes,
                IsAllowed = isAllowed,
            };
        }
        private List<string> searchForPermissionsAndScopes(IQueryable<Role> roles, int operationId, out bool isPermissionFound)
        {
            List<string> scopes = new List<string>();
            isPermissionFound = false;
            foreach (var r in roles)
            {
                var tmpScope = "";
                if (IsRoleHaveOperation(r.Id, operationId, ref tmpScope))
                {
                    isPermissionFound = true;
                    scopes.Add(tmpScope);
                }
            }

            return scopes;
        }

        public bool IsRoleHaveAnyUser(int roleId)
        {
            return db.UserRoles
                .Include(x => x.User)
                .Any(x => x.RoleId == roleId && x.IsDeleted == false && x.User.IsDeleted == false);
        }

        public bool IsRoleHaveAnyOperation(int roleId)
        {
            return GetAllOperationsOfRole(roleId).Any();
        }

        public bool IsUserHaveAnyRoles(int id)
        {
            return GetAllRolesOfUser(id).Any();
        }


    }
}