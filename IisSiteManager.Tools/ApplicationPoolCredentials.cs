using Microsoft.Web.Administration;

namespace IisSiteManager.Tools
{
    public class ApplicationPoolCredential
    {
        public ApplicationPoolCredential(ApplicationPool pool)
        {
            ApplicationPoolRef = pool;
        }

        public ApplicationPoolProcessModel ProcessModel => ApplicationPoolRef.ProcessModel;

        public ApplicationPool ApplicationPoolRef { get; }

        public string ApplicationPoolName => ApplicationPoolRef.Name;

        public string Username
        {
            get => ProcessModel.UserName;
            set => ProcessModel.UserName = value;
        }

        public string Password
        {
            get => ProcessModel.Password;
            set => ProcessModel.Password = value;
        }
    }
}