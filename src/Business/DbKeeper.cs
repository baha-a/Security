using ITech.Security.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;

namespace ITech.Security.Business
{
    public class DbKeeper
    {
        private readonly SecurityDB db;
        private readonly DbSearcher asker;

        public DbKeeper(SecurityDB _db, DbSearcher _asker)
        {
            db = _db;
            db.Configuration.ProxyCreationEnabled = false;

            asker = _asker;
        }


        public void AddApplication(string appName, string byAdmin, int appId)
        {
            var app = new Application()
            {
                Id = appId,

                Created = DateTime.Now,
                Modified = DateTime.Now,

                CreatedBy = byAdmin,
                ModifiedBy = byAdmin,

                IsDeleted = false,
                Name = appName,
            };
            db.Applications.Add(app);
            db.SaveChanges();
        }

        public Operation AddOperation(string p, string byAdmin, int applicationId)
        {
            var opr = new Operation()
            {
                ApplicationId = applicationId,

                Created = DateTime.Now,
                Modified = DateTime.Now,

                CreatedBy = byAdmin,
                ModifiedBy = byAdmin,

                IsDeleted = false,
                Name = p,
            };
            db.Operations.Add(opr);
            db.SaveChanges();

            return opr;
        }





        public Role AddRole(Role r, string byAdmin, int applicationId)
        {
            if (asker.FindRole(r.Name, applicationId) != null)
                throw new AreadyExistedException("Role with same name aready existed");

            r.ApplicationId = applicationId;
            r.Created = DateTime.Now;
            r.CreatedBy = byAdmin;
            r.Modified = DateTime.Now;
            r.ModifiedBy = byAdmin;
            r.IsDeleted = false;

            var res = db.Roles.Add(r);
            db.SaveChanges();

            OnRoleAddedEvent(res);

            return res;
        }
        public void RemoveRole(int roleId, string byAdmin)
        {
            var r = asker.FindRoleById(roleId);

            if (asker.IsRoleHaveAnyUser(roleId))
                throw new DeleteNestedResourceException("cann't delete role because it has users");

            if (asker.IsRoleHaveAnyOperation(roleId))
                throw new DeleteNestedResourceException("cann't delete role because it has operations");

            r.Modified = DateTime.Now;
            r.ModifiedBy = byAdmin;
            r.IsDeleted = true;

            db.SaveChanges();

            OnRoleDeletedEvent(roleId);
        }
        public void EditRole(int roleid, string name, string description, string byAdmin)
        {
            var role = asker.FindRoleById(roleid);
            var role2 = asker.FindRole(name, role.ApplicationId);
            if (role2 != null && role2.Id != role.Id)
                throw new AreadyExistedException("Role with same name aready existed");

            role.Name = name;
            role.Description = description;

            role.Modified = DateTime.Now;
            role.ModifiedBy = byAdmin;

            db.SaveChanges();
        }

        public User AddUserIfNotExisted(string userName, string firstname, string lastname, string byAdmin)
        {
            var user = new User()
            {
                UserName = userName, 
                FirstName = firstname, 
                LastName = lastname
            };

            return AddUserIfNotExisted(user, byAdmin);
        }
        public User AddUserIfNotExisted(User u, string byAdmin)
        {
            var user = asker.FindUser(u.UserName);
            if (user != null)
                return user;

            u.Created = DateTime.Now;
            u.CreatedBy = byAdmin;
            u.Modified = DateTime.Now;
            u.ModifiedBy = byAdmin;
            u.IsDeleted = false;

            var res = db.Users.Add(u);
            db.SaveChanges();

            OnUserAddedEvent(res);

            return res;
        }

        public void RemoveUser(int userId, string byAdmin)
        {
            var r = asker.FindUserById(userId);

            //removeAllUserRoleOfUser(userId, byAdmin);
            if(asker.IsUserHaveAnyRoles(userId))
                throw new DeleteNestedResourceException();
            
            r.IsDeleted = true;
            r.ModifiedBy = byAdmin;
            r.Modified = DateTime.Now;

            db.SaveChanges();

            OnUserDeletedEvent(userId);
        }

        private void removeAllUserRoleOfUser(int userId, string byAdmin)
        {
            asker.GetAllUserRoleOfUser(userId)
                .ForEach(x =>
                {
                    x.IsDeleted = true;
                    x.ModifiedBy = byAdmin;
                    x.Modified = DateTime.Now;

                    OnUserDeletedFromRoleEvent(x.UserId, x.RoleId);
                });
        }



        public void AddOperationToRole(int operationId, int roleId, string JsonScope, string byAdmin, int applicationId)
        {
            if (asker.IsRoleHaveOperation(roleId, operationId, applicationId))
                return;

            var oper = db.RoleOperations.Add(new RoleOperation()
            {
                Created = DateTime.Now,
                CreatedBy = byAdmin,
                Modified = DateTime.Now,
                ModifiedBy = byAdmin,
                IsDeleted = false,

                RoleId = roleId,
                OperationId = operationId,

                Scope = JsonScope,
            });

            db.SaveChanges();
            OnOperationAddedToRoleEvent(oper);
        }
        public void RemoveOperationFromRole(int operationId, int roleId, string byAdmin)
        {
            var ro = asker.FindRoleOperation(roleId, operationId);

            if (ro == null)
                return;

            ro.IsDeleted = true;
            ro.Modified = DateTime.Now;
            ro.ModifiedBy = byAdmin;

            db.SaveChanges();

            OnOperationDeletedFromRoleEvent(roleId, operationId);
        }


        public void AddViewToRole(int viewId, int roleId, string byAdmin)
        {
            if (asker.FindViewRole(viewId, roleId) != null)
                return;

            db.RoleViews.Add(new RoleView()
            {
                ViewId = viewId,
                RoleId = roleId,

                IsDeleted = false,
                Created = DateTime.Now,
                Modified = DateTime.Now,
                CreatedBy = byAdmin,
                ModifiedBy = byAdmin,
            });
            db.SaveChanges();
        }

        public void RemoveViewFromRole(int viewId, int roleId, string byAdmin)
        {
            var rv = asker.FindViewRole(viewId, roleId);
            rv.IsDeleted = true;
            rv.Modified = DateTime.Now;
            rv.ModifiedBy = byAdmin;
            db.SaveChanges();
        }


        public void AddUserToRoleAndCreateUserIfNotExist(User user, int roleId, string byAdmin)
        {
            user = AddUserIfNotExisted(user, byAdmin);
            var role = asker.FindRoleById(roleId);

            if (role == null)
                throw new ResourceNotFoundException("Role not found");

            addNewUserRole(user.Id, role.Id, byAdmin);
        }
        public void AddUserToRole(int userId, int roleId, string byAdmin)
        {
            var user = asker.FindUserById(userId);
            var role = asker.FindRoleById(roleId);

            if (role == null)
                throw new ResourceNotFoundException("Role not found");

            addNewUserRole(user.Id, role.Id, byAdmin);
        }
        private void addNewUserRole(int userId, int roleId, string byAdmin)
        {
            if (asker.IsUserInRole(userId, roleId))
                return;

            var ur = db.UserRoles.Add(new UserRole()
            {
                UserId = userId,
                RoleId = roleId,

                Created = DateTime.Now,
                CreatedBy = byAdmin,

                Modified = DateTime.Now,
                ModifiedBy = byAdmin,

                IsDeleted = false,
            });

            db.SaveChanges();

            OnUserAddedToRoleEvent(ur);
        }


        public void RemoveUserFromRole(string userName, string roleName, string byAdmin, int applicationId)
        {
            var user = asker.FindUser(userName);
            var role = asker.FindRole(roleName, applicationId);

            RemoveUserFromRole(user.Id, role.Id, byAdmin);
        }
        public void RemoveUserFromRole(int userId, int roleId, string byAdmin)
        {
            var ur = asker.FindUserRole(userId, roleId);
            removeUserRoleAndSave(ur, byAdmin);
        }

        public void RemoveUserRole(int userRoleId, string byAdmin)
        {
            var ur = asker.FindUserRoleById(userRoleId);
            removeUserRoleAndSave(ur, byAdmin);
        }
        private void removeUserRoleAndSave(UserRole ur, string byAdmin)
        {
            ur.IsDeleted = true;
            ur.ModifiedBy = byAdmin;
            ur.Modified = DateTime.Now;

            db.SaveChanges();

            OnUserDeletedFromRoleEvent(ur.UserId, ur.RoleId);
        }





        public event Action<User> UserAddedEvent;
        public event Action<int> UserDeletedEvent;

        public event Action<Role> RoleAddedEvent;
        public event Action<int> RoleDeletedEvent;

        public event Action<RoleOperation> OperationAddedToRoleEvent;
        public event Action<int, int> OperationDeletedFromRoleEvent;

        public event Action<UserRole> UserAddedToRoleEvent;
        public event Action<int, int> UserDeletedFromRoleEvent;


        protected virtual void OnUserAddedEvent(User user)
        {
            var handler = UserAddedEvent;
            if (handler != null)
                handler(user);
        }
        protected virtual void OnUserDeletedEvent(int userId)
        {
            var handler = UserDeletedEvent;
            if (handler != null) 
                handler(userId);
        }
        protected virtual void OnRoleAddedEvent(Role role)
        {
            var handler = RoleAddedEvent;
            if (handler != null) handler(role);
        }
        protected virtual void OnRoleDeletedEvent(int roleId)
        {
            var handler = RoleDeletedEvent;
            if (handler != null)
                handler(roleId);
        }
        protected virtual void OnOperationAddedToRoleEvent(RoleOperation opr)
        {
            var handler = OperationAddedToRoleEvent;
            if (handler != null)
                handler(opr);
        }
        protected virtual void OnOperationDeletedFromRoleEvent(int roleId, int operationId)
        {
            var handler = OperationDeletedFromRoleEvent;
            if (handler != null) 
                handler(roleId, operationId);
        }

        protected virtual void OnUserAddedToRoleEvent(UserRole userRole)
        {
            var handler = UserAddedToRoleEvent;
            if (handler != null)
                handler(userRole);
        }

        protected virtual void OnUserDeletedFromRoleEvent(int userId, int roleId)
        {
            var handler = UserDeletedFromRoleEvent;
            if (handler != null) 
                handler(userId, roleId);
        }
    }
}