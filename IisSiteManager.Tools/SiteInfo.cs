using Microsoft.Web.Administration;
using System.Collections.Generic;

namespace IisSiteManager.Tools
{
    public class SiteInfo 
        : ApplicationInfo
    {
        public List<ApplicationInfo> Applications { get; set; } = new List<ApplicationInfo>();

        public ObjectState State { get; set; }
    }
}