using System.Collections.Generic;
using IisSiteManager.Tools;
using Microsoft.Web.Administration;

namespace IisSiteManager.WebApplication.Models
{
    public class DirectoryModel
    {
        public ProcessModelIdentityType? IdentityType { get; set; }

        public string ServiceAccount { get; set; }

        public string[] SelectedApplicationPools { get; set; }

        public string NewUsername { get; set; }

        public string NewPassword { get; set; }

        public IEnumerable<SiteInfo> Data { get; set; }
    }
}