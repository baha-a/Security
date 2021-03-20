using System;
using System.Collections.Generic;
using System.Linq;
using ITech.Security.Models;

namespace ITech.Security.Business
{
    public static class Cacher
    {
        public static CacheContainer<object> Views { get; private set; }
        public static CacheContainer<object> Permissions { get; private set; }

        static Cacher()
        {
            Views = new CacheContainer<object>();
            Permissions = new CacheContainer<object>();
        }


        public static void BuildCache(DbSearcher ask = null)
        {
            if (ask == null)
                ask = new DbSearcher(new SecurityDB());

            resetAllCaches();
            loadAllData(ask);
        }

        private static void loadAllData(DbSearcher ask)
        {
            var users = ask.GetAllUsers();
            var appIds = ask.GetAllApplications().Select(x => x.Id);

            foreach (var appId in appIds)
                loadUserViewAndPermissions(ask, users, appId, ask.GetAllOperations(appId));
        }
        private static void resetAllCaches()
        {
            Views.Reset();
            Permissions.Reset();
        }
        private static void loadUserViewAndPermissions(DbSearcher ask, IQueryable<User> users, int appId, IQueryable<Operation> operations)
        {
            foreach (var user in users)
            {
                addView(ask, appId, user);
                loadPermissions(ask, operations, user, appId);
            }
        }
        private static void addView(DbSearcher ask, int appId, User user)
        {
            Views.Add(
                key: (user.UserName + appId),
                value: (ask.GetAllUserViews(user.Id).ConvertToObjects())
            );
        }
        private static void loadPermissions(DbSearcher ask, IQueryable<Operation> operations, User user, int appId)
        {
            foreach (var oper in operations)
                addPermission(ask, user, appId, oper);
        }
        private static void addPermission(DbSearcher ask, User user, int appId, Operation oper)
        {
            Permissions.Add(
                key: (user.UserName + oper.Name + appId),
                value: (ask.IsAllow(user.UserName, oper.Name, appId).ConvertToObject())
            );
        }
    }


    public class CacheContainer<TValue>
    {
        private Dictionary<string, TValue> cache;

        public CacheContainer()
        {
            if (cache == null)
                Reset();
        }

        public void Reset()
        {
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<string, TValue>();
        }

        public TValue this[string i]
        {
            get
            {
                return cache[i.ToLower()];
            }
        }

        public bool ContiansKey(string i)
        {
            return cache.ContainsKey(i.ToLower());
        }
        public bool ContiansValue(TValue v)
        {
            return cache.ContainsValue(v);
        }


        public void Add(string key, TValue value)
        {
            cache[key.ToLower()] = value;
        }
    }
}