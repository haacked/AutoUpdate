using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace AutoUpdate
{
    public class PackageIdFilter : IActionFilter
    {
        // Methods
        public PackageIdFilter(string packageId)
        {
            this.PackageId = packageId;
        }

        private string GetPackageId(RequestContext context)
        {
            string name = context.RouteData.DataTokens["PackageId"] as string;
            if (name == null)
            {
                name = new DirectoryInfo(HostingEnvironment.ApplicationPhysicalPath).Name;
            }
            return name;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.PackageId == null)
            {
                this.PackageId = this.GetPackageId(filterContext.RequestContext);
            }
            filterContext.ActionParameters["PackageId"] = this.PackageId;
        }

        // Properties
        public string PackageId { get; private set; }
    }

 

}