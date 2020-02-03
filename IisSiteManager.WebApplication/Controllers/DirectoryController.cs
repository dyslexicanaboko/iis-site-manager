﻿using IisSiteManager.Tools;
using IisSiteViewerWebApp.Models;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace IisSiteManager.WebApplication.Controllers
{
    //[Authorize]
    public class DirectoryController : Controller
    {
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "UpdatePassword")]
        public ActionResult UpdatePassword(DirectoryModel model) //FormCollection form
        {
            using (var svc = new IisManagementService())
            {
                foreach (var ap in model.SelectedApplicationPools)
                    svc.ChangeCredentials(model.NewUsername, model.NewPassword, ap);
            }

            return Filter(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Filter")]
        public ActionResult Filter(DirectoryModel model) //FormCollection form
        {
            var m = model;

            var lst = GetAllSites();

            //Doing a lambda query was difficult
            if (m.IdentityType.HasValue)
            {
                IEnumerable<ApplicationInfo> remove = null;

                if (m.ServiceAccount == null)
                    remove = lst.SelectMany(x =>
                        x.Applications.Where(y => y.ApplicationPool.IdentityType != m.IdentityType.Value));
                else
                    remove = lst.SelectMany(x => x.Applications.Where(y =>
                        y.ApplicationPool.IdentityType != m.IdentityType.Value &&
                        y.ApplicationPool.IdentityUser != m.ServiceAccount));

                var lstRemove = remove.ToList();

                SiteInfo si = null;

                for (var i = lst.Count - 1; i > -1; i--)
                {
                    si = lst[i];

                    foreach (var a in lstRemove)
                        si.Applications.Remove(a);

                    if (si.Applications.Count == 0)
                        lst.RemoveAt(i);
                }
            }

            m.Data = lst;

            return GetIndexView(m);
        }

        private ActionResult GetIndexView(DirectoryModel model)
        {
            return View("Index", model);
        }

        private void GetModelFromFormCollection(FormCollection form)
        {
            ProcessModelIdentityType? identityType = null;
            var it = ProcessModelIdentityType.ApplicationPoolIdentity;

            if (Enum.TryParse(Convert.ToString(form["ddlIdentityType"]), out it))
                identityType = it;

            var serviceAccount = Convert.ToString(form["ddlServiceAccount"]);
        }

        private DirectoryModel GetModel(List<SiteInfo> data)
        {
            return new DirectoryModel {Data = data};
        }

        public ActionResult Index()
        {
            return View(GetModel(GetAllSites()));
        }

        private List<SiteInfo> GetAllSites()
        {
            using (var svc = new IisManagementService())
            {
                return svc.GetSitesDirectory();
            }
        }
    }
}