using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using NuGet;

namespace AutoUpdate.Controllers
{
    [Authorize]
    public class UpdatesController : Controller
    {
        // Methods
        public ActionResult Check(string packageId)
        {
            var projectManager = GetProjectManager();
            var installedPackage = GetInstalledPackage(projectManager, packageId);
            var update = projectManager.GetUpdate(installedPackage);
            var model = new InstallationState
            {
                Installed = installedPackage,
                Update = update
            };
            if (Request.IsAjaxRequest())
            {
                var data = new
                {
                    Version = (update != null) ? update.Version.ToString() : null,
                    UpdateAvailable = update != null
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return View(model);
        }

        private static IPackage GetInstalledPackage(WebProjectManager projectManager, string packageId)
        {
            var package = (from p in projectManager.GetInstalledPackages(packageId)
                                where p.Id == packageId
                                select p).ToList<IPackage>().FirstOrDefault<IPackage>();
            if (package == null)
            {
                throw new InvalidOperationException(string.Format("The package for package ID '{0}' is not installed in this website. Copy the package into the App_Data/packages folder.", packageId));
            }
            return package;
        }

        private WebProjectManager GetProjectManager()
        {
            string remoteSource = ConfigurationManager.AppSettings["PackageSource"];
            return new WebProjectManager(remoteSource, Request.MapPath("~/"));
        }

        public ActionResult Upgrade(string packageId)
        {
            var projectManager = GetProjectManager();
            var installedPackage = GetInstalledPackage(projectManager, packageId);
            var update = projectManager.GetUpdate(installedPackage);
            projectManager.UpdatePackage(update);
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true, Version = update.Version.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return View(update);
        }
    }

}
