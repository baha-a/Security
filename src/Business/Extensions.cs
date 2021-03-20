using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITech.Security.Models;

namespace ITech.Security.Business
{
    public static class Extensions
    {
        public static IEnumerable<View> TopMenuItems(this WebViewPage x)
        {
            string username = x.User.Identity.Name.FixUserName();
            return
                new DbSearcher(new SecurityDB())
                    .GetAllUserViewsOfTheSectionTopMenu(username, AppSettings.ApplicationId);
        }


        public static IEnumerable<SectionViewModel> GroupBySection(this IEnumerable<View> views)
        {
            return views.GroupBy(
                p => p.Section,
                (key, g) => new SectionViewModel()
                {
                    Section = key, Views = g.OrderBy(x=>x.ZIndex == null).ThenBy(x => x.ZIndex)
                });
        }

        public static IEnumerable<SectionViewModel> Order(this IEnumerable<SectionViewModel> sections)
        {
            return sections.OrderBy(x=>x.Section.ZIndex == null).ThenBy(x => x.Section.ZIndex);
        }

        public static IEnumerable<T> MargeWith<T>(this IEnumerable<T> x, IEnumerable<T> y)
        {
            List<T> data = new List<T>();
            data.AddRange(x);
            data.AddRange(y);
            return data;
        }

        public static void AddTheEssentialDataIfDeleteFromDB(this SecurityDB db)
        {
            var asker = new DbSearcher(db);
            var keeper = new DbKeeper(db, asker);

            string superadmin = AppSettings.SuperAdmin;
            string appName = AppSettings.ApplicationName;
            string userManagerRoleName = AppSettings.UserManagerRoleName;
            string rolerManagerRoleName = AppSettings.RoleManagerRoleName;
            int appId = AppSettings.ApplicationId;


            if (asker.FindApplicationById(appId) == null)
            {
                keeper.AddApplication(appName, superadmin, appId);
            }

            Role userManager = asker.FindRole(userManagerRoleName, appId);
            if (userManager == null)
                userManager = keeper.AddRole(new Role() { Name = userManagerRoleName }, superadmin, appId);

            string[] userOperations = 
            {
                "AddUser",
                "DeleteUser",
                "AssignRoleToUser",
                "UnAssignRoleFromUser",
                "SeeUserDetails",
                "SeeAllUsers",
            };

            foreach (var p in userOperations)
            {
                var tmp = asker.FindOperation(p, appId);
                if (tmp == null)
                    tmp = keeper.AddOperation(p, superadmin, appId);

                keeper.AddOperationToRole(tmp.Id, userManager.Id, "", superadmin, appId);
            }

            Role roleManager = asker.FindRole(rolerManagerRoleName, appId);
            if (roleManager == null)
                roleManager = keeper.AddRole(new Role() { Name = rolerManagerRoleName }, superadmin, appId);

            string[] roleOperations =
            {
                "AddRole",
                "DeleteRole",
                "AssignUserToRole",
                "UnAssignUserFromRole",
                "SeeRoleDetails",
                "SeeAllRoles",
                "UnAssignOperationFromRole",
                "AssignOperationToRole"
            };

            foreach (var p in roleOperations)
            {
                var tmp = asker.FindOperation(p, appId);
                if (tmp == null)
                    tmp = keeper.AddOperation(p, superadmin, appId);

                keeper.AddOperationToRole(tmp.Id, roleManager.Id, "", superadmin, appId);
            }

            User superAdmin = asker.FindUser(superadmin);
            if (superAdmin == null)
                superAdmin = keeper.AddUserIfNotExisted(superadmin, "", "", superadmin);

            keeper.AddUserToRole(superAdmin.Id, userManager.Id, superadmin);
            keeper.AddUserToRole(superAdmin.Id, roleManager.Id, superadmin);

            db.SaveChanges();
        }

        public static User ToDbUser(this ActiveDirectoryUser user)
        {
            return new User()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }

        public static object ConvertToObject(this UsersViewModel d)
        {
            return new
            {
                dbUsersInfo = d.DbUsersInfo.ConvertToObjects(),
                usersInfo = d.UsersInfo.ConvertToObjects(),
            };
        }
        public static IEnumerable<object> ConvertToObjects(this IEnumerable<UserInfoViewModel> users)
        {
            List<object> res = new List<object>();
            foreach (var u in users)
            {
                res.Add(u.ConvertToObject());
            }
            return res;
        }

        private static object ConvertToObject(this UserInfoViewModel u)
        {
            return new
            {
                id = u.Id,
                userName = u.UserName,
                firstName = u.FirstName,
                lastName = u.LastName,
                domain = u.Domain,
            };
        }


        public static IEnumerable<UserInfoViewModel> ConvertToUserInfo(this IEnumerable<ActiveDirectoryUser> users)
        {
            List<UserInfoViewModel> res = new List<UserInfoViewModel>();
            foreach (var u in users)
            {
                res.Add(new UserInfoViewModel()
                {
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                });
            }
            return res;
        }

        public static IEnumerable<UserInfoViewModel> ConvertToUserInfo(this IEnumerable<User> users)
        {
            List<UserInfoViewModel> res = new List<UserInfoViewModel>();
            foreach (var u in users)
            {
                res.Add(new UserInfoViewModel()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                });
            }
            return res;
        }

        public static object ConvertToObject(this User user)
        {
            return new
            {
                id = user.Id,
                userName = user.UserName,
                lastName = user.LastName,
                firstName = user.FirstName,
                roles = user.Roles.ConvertToObjects(),
                views = user.Views.ConvertToObjects(),
                otherRoles = user.OtherRoles.ConvertToObjects(),
            };
        }

        public static IEnumerable<object> ConvertToObjects(this IEnumerable<Operation> oper)
        {
            List<object> objs = new List<object>();
            if (oper != null && oper.Any())
                foreach (var r in oper)
                    objs.Add(r.ConvertToObject());
            return objs;
        }
        public static object ConvertToObject(this Operation r)
        {
            return new
            {
                id = r.Id,
                name = r.Name,
                applicationId = r.ApplicationId,
                description = r.Description,
                modelId = r.ModelId,
                modelName = (r.Model == null ? null : r.Model.Name),
                modelDescription = (r.Model == null ? null : r.Model.Description),
            };
        }

        public static IEnumerable<object> ConvertToObjects(this IEnumerable<ScopedOperation> oper)
        {
            List<object> objs = new List<object>();
            if (oper != null && oper.Any())
                foreach (var r in oper)
                    objs.Add(r.ConvertToObject());
            return objs;
        }

        public static object ConvertToObject(this ScopedOperation r)
        {
            return new
            {
                id = r.Operation.Id,
                name = r.Operation.Name,
                applicationId = r.Operation.ApplicationId,
                description = r.Operation.Description,
                modelId = r.Operation.ModelId,
                modelName = (r.Operation.Model == null ? null : r.Operation.Model.Name),
                modelDescription = (r.Operation.Model == null ? null : r.Operation.Model.Description),
                scope = r.Scope,
            };
        }

        public static IEnumerable<object> ConvertToObjects(this IEnumerable<Role> roles)
        {
            List<object> objs = new List<object>();
            if (roles != null)
                foreach (var r in roles)
                    objs.Add(r.ConvertToObject());
            return objs;
        }

        public static object ConvertToObject(this Role r)
        {
            return new
            {
                id = r.Id,
                name = r.Name,
                applicationId = r.ApplicationId,
                applicationName = (r.Application == null ? "" : r.Application.Name),
                description = r.Description,
                operations = r.ScopedOperations.ConvertToObjects(),
            };
        }

        public static IEnumerable<object> ConvertToObjects(this IEnumerable<View> views)
        {
            List<object> r = new List<object>();
            if (views!=null)
                foreach (var v in views)
                    r.Add(v.ConvertToObject());
            return r;
        }

        public static object ConvertToObject(this View v)
        {
            return new
            {
                id = v.Id,
                name = v.Name,
                url = v.Url,
                description = v.Description,
                sectionId = v.SectionId,
                sectionName = v.Section.Name,
                applicationId = v.ApplicationId
            };
        }


        public static object ConvertToObject(this ScopedBoolean v)
        {
            return new
            {
                isAllowed = v.IsAllowed,
                scopes = v.Scopes,
            };
        }





        public static string FixUserName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                int slash = name.LastIndexOf("\\") + 1;
                if (slash < name.Length)
                    name = name.Substring(slash);
            }
            return name;
        }
    }
}