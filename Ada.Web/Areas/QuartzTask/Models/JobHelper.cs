﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Quartz;

namespace QuartzTask.Models
{
    public class JobHelper
    {
        public static List<string> GetJobs()
        {
            List<string> list = new List<string>();
            var assemblies = BuildManager.GetReferencedAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass && typeof(IJob).IsAssignableFrom(type))
                    {
                        list.Add(type.Name);
                    }
                }
            }
            return list;
        }
        public static Type GetJobType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return null;
            }
            var assemblies = BuildManager.GetReferencedAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass && typeof(IJob).IsAssignableFrom(type) && string.Equals(type.Name, typeName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return type;
                    }

                }
            }
            return null;
        }
        public static string GetFullErrorText(DbEntityValidationException exc)
        {
            var msg = string.Empty;
            foreach (var validationErrors in exc.EntityValidationErrors)
            foreach (var error in validationErrors.ValidationErrors)
                msg += $"属性: {error.PropertyName} 错误: {error.ErrorMessage}" + Environment.NewLine;
            return msg;
        }
    }
}