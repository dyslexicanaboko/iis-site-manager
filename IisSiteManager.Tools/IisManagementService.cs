using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//C:\Windows\System32\inetsrv\Microsoft.Web.Administration.dll

namespace IisSiteManager.Tools
{
    public class IisManagementService : IDisposable
    {
        public const string DefaultDocument = "system.webServer/defaultDocument";

        private readonly ServerManager _manager;

        public IisManagementService()
        {
            _manager = new ServerManager();
        }

        public void Dispose()
        {
            _manager.Dispose();
        }

        public SiteCollection GetSites()
        {
            return _manager.Sites;
        }

        public List<string> GetDefaultDocuments(Site site)
        {
            var webConfig = site.GetWebConfiguration();
            var defaultDocumentSection = webConfig.GetSection(DefaultDocument);
            var filesCollection = defaultDocumentSection.GetCollection("files");

            //filesCollection[0].GetAttributeValue("value")
            return filesCollection.Select(x => Convert.ToString(x.GetAttributeValue("value"))).ToList();
        }

        public List<SiteInfo> GetSitesDirectory()
        {
            var sites = GetSites();

            return sites.Select(GetSiteInfoFromSite).ToList();
        }

        private bool DoesSitePhysicalPathExist(Site site)
        {
            return Directory.Exists(site.Applications[0].VirtualDirectories[0].PhysicalPath);
        }

        public SiteInfo GetSiteInfoFromSite(Site site)
        {
            var obj = new SiteInfo
            {
                Name = site.Name,
                State = ObjectState.Started
            };

            if (!DoesSitePhysicalPathExist(site)) return obj;

            obj.DefaultDocument = GetDefaultDocuments(site)[0]; //I am not sure if this makes sense yet

            foreach (var app in site.Applications)
            {
                var ap = _manager.ApplicationPools[app.ApplicationPoolName];

                obj.Applications.Add(new ApplicationInfo(site, app, ap)
                {
                    DefaultDocument = GetDefaultDocuments(site)[0] //I am not sure if this makes sense yet
                });
            }

            return obj;
        }

        public List<ApplicationPool> GetApplicationPools(ProcessModelIdentityType? identityType = null,
            string applicationPoolName = null)
        {
            IEnumerable<ApplicationPool> q = _manager.ApplicationPools;

            if (identityType.HasValue)
                q = q.Where(x => x.ProcessModel.IdentityType == identityType.Value);

            if (!q.Any())
                throw new ApplicationException(
                    $"No application pools were found for the ProcessModelIdentityType \"{identityType}\".");

            //If an application pool name is actually provided then search by that name
            if (!string.IsNullOrWhiteSpace(applicationPoolName))
                q = q.Where(x => x.Name == applicationPoolName);

            if (!q.Any())
                throw new ApplicationException(
                    $"There are no application pools that match the search criteria: \"{applicationPoolName}\" (Tip: Searches are an exact match).");

            return q.ToList();
        }

        public List<ApplicationPoolCredential> GetApplicationPoolCredentials(string applicationPoolName = null)
        {
            return GetApplicationPools(ProcessModelIdentityType.SpecificUser, applicationPoolName)
                .Select(x => new ApplicationPoolCredential(x))
                .ToList();
        }

        public void ChangeCredentials(string username, string password, string applicationPoolName = null)
        {
            var usernameOk = !string.IsNullOrWhiteSpace(username);
            var passwordOk = !string.IsNullOrWhiteSpace(password);

            if (!usernameOk && !passwordOk)
                throw new UsernamePasswordException();

            var lst = GetApplicationPoolCredentials(applicationPoolName);

            foreach (var pm in lst)
            {
                if (usernameOk)
                    pm.Username = username;

                if (passwordOk)
                    pm.Password = password;
            }

            _manager.CommitChanges();
        }
    }
}