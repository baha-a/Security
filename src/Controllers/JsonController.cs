using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Web.Caching;
using System.Web.Mvc;
using ITech.Security.Business;
using ITech.Security.Models;

namespace ITech.Security.Controllers
{
    public class JsonController : BaseController
    {

        [HttpGet]
        public JsonResult Check(string username, string operation, int applicationId)
        {
            try
            {
                string cacheKey = username + operation + applicationId;
                if (Cacher.Permissions.ContiansKey(cacheKey))
                    return JsonX(Cacher.Permissions[cacheKey]);

                var res = ask.IsAllow(username, operation, applicationId).ConvertToObject();

                Cacher.Permissions.Add(cacheKey, res);
                return JsonX(res);
            }
            catch
            {
                return JsonBadResult();
            }
        }

        [HttpGet]
        public JsonResult CheckModel(string username,string operation, string model, int applicationId)
        {
            try
            {
                string cacheKey = username + operation + model + applicationId;
                if (Cacher.Permissions.ContiansKey(cacheKey))
                    return JsonX(Cacher.Permissions[cacheKey]);

                var res = ask.IsAllowedWithModel(username, operation, model, applicationId).ConvertToObject();

                Cacher.Permissions.Add(cacheKey, res);
                return JsonX(res);
            }
            catch
            {
                return JsonBadResult();                
            }
        }

        [HttpGet]
        public JsonResult Views(string username, int applicationId)
        {
            try
            {
                string cacheKey = username + applicationId;
                if (Cacher.Views.ContiansKey(cacheKey))
                    return JsonX(Cacher.Views[cacheKey]);

                var objects = ask.GetAllUserViews(ask.FindUser(username).Id).ConvertToObjects();

                Cacher.Views.Add(cacheKey, objects);
                return JsonX(objects);
            }
            catch
            {
                return JsonBadResult();                
            }
        }





        [HttpGet]
        public JsonResult Info(string userName)
        {
            try
            {
                var user = ask.FindUser(userName);
                user.Roles = getRoleWithScopedOperationsOfUser(user.Id);
                user.Views = ask.GetAllUserViews(user.Id);

                return JsonOk(user.ConvertToObject());
            }
            catch
            {
                return JsonBadResult();
            }
        }


        private IEnumerable<Role> getRoleWithScopedOperationsOfUser(int userId)
        {
            var roles = ask.GetAllRolesOfUser(userId);
            foreach (var r in roles)
                r.ScopedOperations = fillOperationsWithModel(ask.GetAllOperationsOfRoleWithScope(r.Id));
            return roles;
        }
        private IEnumerable<ScopedOperation> fillOperationsWithModel(IEnumerable<ScopedOperation> operations)
        {
            foreach (var p in operations)
                if (p.Operation.ModelId != null)
                    p.Operation.Model = ask.FindModelById((int)p.Operation.ModelId);
            return operations;
        }


        private JsonResult JsonRaw(object data, int statusCode)
        {
            Response.StatusCode = statusCode;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        private JsonResult JsonOk(object data)
        {
            return JsonRaw(data, 200);
        }
        public JsonResult JsonNotFound(string msg = "")
        {
            return JsonRaw(msg, 404);
        }
        public JsonResult JsonBadResult(string msg = "")
        {
            return JsonRaw(msg, 400);
        }
    }
}