﻿using System;
using System.Web.Mvc;
using System.Reflection;

namespace RemliCMS.Controllers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MultiButtonAttribute : ActionNameSelectorAttribute
    {
        public string MatchFormKey { get; set; }
        public string MatchFormValue1 { get; set; }
        public string MatchFormValue2 { get; set; }
        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            bool match = controllerContext.HttpContext.Request[MatchFormKey] != null &&
               (controllerContext.HttpContext.Request[MatchFormKey] == MatchFormValue1 || controllerContext.HttpContext.Request[MatchFormKey] == MatchFormValue2);


            var valueMatchForm = controllerContext.HttpContext.Request[MatchFormKey];


            return match;
        }
    }

}
    