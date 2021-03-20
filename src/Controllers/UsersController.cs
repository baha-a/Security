using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ITech.Security.Business;
using ITech.Security.Models;

namespace ITech.Security.Controllers
{
    public class UsersController : BaseCommandController
    {

        [HttpGet]
        public ActionResult Index()
        {
            HasPermission("SeeAllUsers");
            return View(getUsersViewModel());
        }

        [HttpGet]
        public JsonResult AllUsers()
        {
            HasPermission("SeeAllUsers");
            return JsonX(ask.GetAllUsers().ConvertToUserInfo().ConvertToObjects());
        }


        [HttpGet]
        public ActionResult AddUser(string userName)
        {
            HasPermission("AddUser");

            var user = ActiveDirectory
                .GetUser(userName, AppSettings.ActiveDirDomain)
                .ToDbUser();

            command.AddUserIfNotExisted(user, UserName);
            return RedirectToAction("Index");
        }

        

        [HttpGet]
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        public JsonResult DetailsJson(int id)
        {
            try
            {
                HasPermission("SeeUserDetails");
            }
            catch (HttpException ex)
            {
                return JsonAccessDenied(ex.Message);                
            }

            var user = ask.FindUserById(id);
            if (user == null)
                return JsonNotFound();

            user.Roles = order(ask.GetAllRolesOfUser(id));
            user.OtherRoles = order(getRemindRoles(user.Roles.Select(x => x.Id)));
            return JsonX(user.ConvertToObject());
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            HasPermission("DeleteUser");

            User user = ask.FindUserById(id);
            if (user == null)
                return HttpNotFound();

            ViewBag.UserHaveRole = false;
            if (ask.IsUserHaveAnyRoles(id))
                ViewBag.UserHaveRole = true;

            return View();
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            HasPermission("DeleteUser");

            command.RemoveUser(id, UserName);
            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult Assign(int id, int roleId)
        {
            HasPermission("AssignRoleToUser");


            command.AddUserToRole(id, roleId, UserName);

            return RedirectToAction("Details", new { id = id });
        }
        [HttpGet]
        public ActionResult UnAssign(int id, int roleId)
        {
            HasPermission("UnAssignRoleFromUser");


            command.RemoveUserFromRole(id, roleId, UserName);
            return RedirectToAction("Details", new { id = id });
        }



        private UsersViewModel getUsersViewModel()
        {
            var result = new UsersViewModel()
            {
                DbUsersInfo = ask.GetAllUsers().ConvertToUserInfo(),
                UsersInfo = getReminingUserFromActiveDir(ask.GetAllUsers().Select(x=>x.UserName).ToList()),
            };
            return result;
        }
        private IEnumerable<UserInfoViewModel> getReminingUserFromActiveDir(IEnumerable<string> exceptedUserNames)
        {
            return
                ActiveDirectory.GetUsers(AppSettings.ActiveDirDomain)
                    .Where(x => exceptedUserNames.Contains(x.UserName) == false)
                    .ConvertToUserInfo();
        }
        private IQueryable<Role> order(IQueryable<Role> t)
        {
            return t.Include(x => x.Application)
                .OrderBy(x => x.Application.Name)
                .ThenBy(x => x.Name);
        }
        private IQueryable<Role> getRemindRoles(IEnumerable<int> exceptedIds)
        {
            return ask.GetAllRoles().Where(x => exceptedIds.Contains(x.Id) == false);
        }
    }
}