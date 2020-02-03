using Microsoft.Web.Administration;
using System.Collections.Generic;

namespace IisSiteManager.Tools
{
    public class SiteInfo : ApplicationInfo
    {
        public SiteInfo()
        {
            Applications = new List<ApplicationInfo>();
        }

        public List<ApplicationInfo> Applications { get; set; }

        public ObjectState State { get; set; }
    }
}