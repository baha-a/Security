using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ITech.Security.Models;
using ITech.Security.Business;
using View = ITech.Security.Models.View;

namespace ITech.Security.Controllers
{
    public class RolesController : BaseCommandController
    {
        [HttpGet]
        public ActionResult Index(int id = -1)
        {
            HasPermission("SeeAllRoles");
            return View();
            
            //var apps = new List<Application>();
            //apps.Add(Application.All);
            //apps.AddRange(ask.GetAllApplications());

            //ViewBag.Applications = apps;
            //ViewBag.SelectedApplicationId = id;

            //return View(ask.GetAllRoles(id));
        }



        [HttpGet]
        public JsonResult AllRoles()
        {
            HasPermission("SeeAllRoles");

            return JsonX(
                ask.GetAllRoles()
                .Include(x => x.Application)
                .ConvertToObjects());
        }


        [HttpGet]
        public ActionResult Details(int id, string tab)
        {
            HasPermission("SeeRoleDetails");
            ViewBag.Tab = tab;
            return View();
        }

        [HttpGet]
        public JsonResult DetailsJson(int id)
        {
            HasPermission("SeeRoleDetails");

            Role role = ask.FindRoleById(id);
            if (role == null)
                return JsonNotFound();
            return JsonX(buildDetailsData(role));
        }

        private object buildDetailsData(Role role)
        {
            var roleuser = ask.GetAllUsersOfRole(role.Id);
            var roleoperation = ask.GetAllOperationsOfRoleWithScope(role.Id);
            var roleViews = ask.GetAllViewsOfRole(role.Id);

            return new
            {
                id = role.Id,
                roleName = role.Name,
                applicationName = ask.FindApplicationById(role.ApplicationId).Name,
                description = role.Description,

                users = roleuser.ConvertToUserInfo().ConvertToObjects(),
                operations = roleoperation.ConvertToObjects(),
                views = roleViews.ConvertToObjects(),

                otherUsers = getReminingUserFromActiveDir(roleuser.Select(x => x.UserName)).ConvertToObjects(),
                otherOperations = getReminingOperations(role.ApplicationId, roleoperation.Select(x => x.Operation.Id))
                    .OrderBy(x => x.Name).ConvertToObjects(),

                otherViews = getReminginViews(role.ApplicationId, roleViews.Select(x => x.Id)).ConvertToObjects(),
            };
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            HasPermission("AddRole");

            var role = ask.FindRoleById(id);
            ViewBag.Id = id;
            ViewBag.Name = role.Name;
            ViewBag.Description = role.Description;
            ViewBag.SelectedApplicationName = ask.FindApplicationById(role.ApplicationId).Name;

            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, string name, string description)
        {
            HasPermission("AddRole");

            if (string.IsNullOrWhiteSpace(name))
                ModelState.AddModelError("name", new ArgumentException());

            var role = ask.FindRoleById(id);
            if (ModelState.IsValid)
            {
                try
                {
                    command.EditRole(role.Id ,name, description, UserName);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }
            else
            {
                ViewBag.Error = "Errors";                
            }

            ViewBag.Name = name;
            ViewBag.Description = description;
            ViewBag.SelectedApplicationName = ask.FindApplicationById(role.ApplicationId).Name;

            return View();
        }


        [HttpGet]
        public ActionResult Create()
        {
            HasPermission("AddRole");

            ViewBag.Applications = ask.GetAllApplications();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string name, string description, int applicationId)
        {
            HasPermission("AddRole");

            if (string.IsNullOrWhiteSpace(name))
                ModelState.AddModelError("name", new ArgumentException());

            if (ModelState.IsValid)
            {
                try
                {
                    command.AddRole(new Role() {Name = name, Description = description}, UserName, applicationId);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            ViewBag.Applications = ask.GetAllApplications();
            ViewBag.Name = name;
            ViewBag.Description = description;
            ViewBag.SelectedApplicationId = applicationId;

            return View();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            HasPermission("DeleteRole");

            return View();
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            HasPermission("DeleteRole");

            command.RemoveRole(id, UserName);
            return RedirectToAction("Index");
        }

        

        [HttpGet]
        public ActionResult AddUser(int id, string userName)
        {
            HasPermission("AssignUserToRole");

            var role = ask.FindRoleById(id);
            var user = ActiveDirectory.GetUser(userName, AppSettings.ActiveDirDomain);

            command.AddUserToRoleAndCreateUserIfNotExist(user.ToDbUser(), role.Id, UserName);
            return RedirectToAction("Details", new { id = id, tab = "user" });
        }
        [HttpGet]
        public ActionResult RemoveUser(int id, string userName)
        {
            HasPermission("UnAssignUserFromRole");

            var role = ask.FindRoleById(id);

            command.RemoveUserFromRole(userName, role.Name, UserName, role.ApplicationId);

            return RedirectToAction("Details", new { id = id, tab = "user" });
        }
        [HttpGet]
        public ActionResult AddOperation(int id, int operationId, string json)
        {
            HasPermission("AssignOperationToRole");

            var role = ask.FindRoleById(id);
            command.AddOperationToRole(operationId, role.Id, json, UserName, role.ApplicationId);

            return RedirectToAction("Details", new {id = id, tab = "operation"});
        }
        [HttpGet]
        public ActionResult RemoveOperation(int id, int operationId)
        {
            HasPermission("UnAssignOperationFromRole");

            var role = ask.FindRoleById(id);
            command.RemoveOperationFromRole(operationId, role.Id, UserName);

            return RedirectToAction("Details", new { id = id, tab = "operation" });
        }

        [HttpGet]
        public ActionResult AddView(int id, int viewId)
        {
            HasPermission("AssignViewToRole");
            command.AddViewToRole(viewId, id, UserName);

            return RedirectToAction("Details", new {id = id, tab = "view"});
        }
        [HttpGet]
        public ActionResult RemoveView(int id, int viewId)
        {
            HasPermission("UnAssignViewFromRole");
            command.RemoveViewFromRole(viewId, id, UserName);

            return RedirectToAction("Details", new { id = id, tab = "view" });
        }

        private IEnumerable<View> getReminginViews(int applicationId, IEnumerable<int> exceptedIds)
        {
            return ask.GetAllViews(applicationId)
                .Where(x => exceptedIds.Contains(x.Id) == false);
        }
        private IEnumerable<Operation> getReminingOperations(int applicationId, IEnumerable<int> exceptedIds)
        {
            return ask.GetAllOperations(applicationId)
                .Where(x => exceptedIds.Contains(x.Id) == false);
        }
        private IEnumerable<UserInfoViewModel> getReminingUserFromActiveDir(IEnumerable<string> exceptedUserNames)
        {
            return
                ActiveDirectory.GetUsers(AppSettings.ActiveDirDomain)
                    .Where(x => exceptedUserNames.Contains(x.UserName) == false)
                    .ConvertToUserInfo();
        }

    }
}
