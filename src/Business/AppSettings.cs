using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ITech.Security.Business
{
    public static class AppSettings
    {
        public static int ApplicationId
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["ApplicationId"]);
            }
        }
        public static string ActiveDirDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["ActiveDirectoryDomain"];
            }
        }
        public static string SuperAdmin
        {
            get
            {
                return ConfigurationManager.AppSettings["SuperAdmin"];
            }
        }
        public static string ApplicationName
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationName"];
            }
        }
        public static string RoleManagerRoleName
        {
            get
            {
                return ConfigurationManager.AppSettings["RoleManagerRoleName"];
            }
        }
        public static string UserManagerRoleName
        {
            get
            {
                return ConfigurationManager.AppSettings["UserManagerRoleName"];
            }
        }
    }
}