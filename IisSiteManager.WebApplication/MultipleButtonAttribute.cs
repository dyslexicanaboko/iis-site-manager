using System;
using System.Reflection;
using System.Web.Mvc;

namespace IisSiteManager.WebApplication
{
    //http://stackoverflow.com/questions/442704/how-do-you-handle-multiple-submit-buttons-in-asp-net-mvc-framework
    [AttributeUsage(AttributeTargets.Method)]
    public class MultipleButtonAttribute : ActionNameSelectorAttribute
    {
        public string Name { get; set; }
        public string Argument { get; set; }

        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            var keyValue = $"{Name}:{Argument}";

            var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

            if (value == null) return false;

            controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;

            return true;
        }
    }
}