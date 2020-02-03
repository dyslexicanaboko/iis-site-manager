using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IisSiteManager.Tools
{
    public class ApplicationInfo
    {
        public const string Localhost = "localhost";

        private readonly Regex _regex = new Regex(@"\d");

        internal ApplicationInfo()
        {

        }

        public ApplicationInfo(Site site, Application application, ApplicationPool applicationPool)
        {
            ApplicationPool = new ApplicationPoolInfo(applicationPool);

            Name = GetApplicationName(site, application);

            Uris = GetUris(site, application);

            //DefaultDocument = GetDefaultDocuments(site)[0], //I am not sure if this makes sense yet
        }

        public string Name { get; set; }

        public string DefaultDocument { get; set; }

        public ApplicationPoolInfo ApplicationPool { get; set; }

        public List<Uri> Uris { get; set; }

        private string GetApplicationName(Site site, Application application)
        {
            var strPath = application.Path.Replace("/", string.Empty);

            return string.IsNullOrWhiteSpace(strPath) ? "Root" : strPath;
        }

        private List<Uri> GetUris(Site site, Application application)
        {
            var lst = new List<Uri>();

            foreach (var b in site.Bindings.Where(HasPortNumber).ToList())
            {
                var ub = new UriBuilder();
                ub.Scheme = b.Protocol;
                ub.Host = GetHost(b);
                ub.Port = GetPort(b);
                ub.Path = application.Path;

                lst.Add(ub.Uri);
            }

            return lst;
        }

        private string GetHost(Binding binding)
        {
            return !string.IsNullOrWhiteSpace(binding.Host) ? binding.Host : Localhost;
        }

        private int GetPort(Binding binding)
        {
            return binding.EndPoint?.Port ?? Convert.ToInt32(binding.BindingInformation.Replace(":", string.Empty).Replace("*", string.Empty));
        }

        private bool HasPortNumber(Binding binding)
        {
            return _regex.IsMatch(binding.BindingInformation);
        }
    }
}