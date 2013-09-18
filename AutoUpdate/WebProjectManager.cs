using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Web.WebPages.Administration.PackageManager;
using NuGet;

namespace AutoUpdate
{
    internal class WebProjectManager
    {
        // Fields
        private readonly IProjectManager _projectManager;

        // Methods
        public WebProjectManager(string remoteSource, string siteRoot)
        {
            string webRepositoryDirectory = GetWebRepositoryDirectory(siteRoot);
            var sourceRepository = PackageRepositoryFactory.Default.CreateRepository(remoteSource);
            var pathResolver = new DefaultPackagePathResolver(webRepositoryDirectory);
            var localRepository = PackageRepositoryFactory.Default.CreateRepository(webRepositoryDirectory);
            IProjectSystem project = new WebProjectSystem(siteRoot);
            _projectManager = new ProjectManager(sourceRepository, pathResolver, project, localRepository);
        }

        public IQueryable<IPackage> GetInstalledPackages(string searchTerms)
        {
            return GetPackages(LocalRepository, searchTerms);
        }

        private static IEnumerable<IPackage> GetPackageDependencies(IPackage package, IPackageRepository localRepository, IPackageRepository sourceRepository)
        {
            var walker = new InstallWalker(
                localRepository,
                sourceRepository,
                new FrameworkName(".NET Framework, Version=4.0"),
                NullLogger.Instance,
                ignoreDependencies: false,
                allowPrereleaseVersions: true);
            return (from operation in walker.ResolveOperations(package)
                    where operation.Action == PackageAction.Install
                    select operation.Package);
        }

        internal static IQueryable<IPackage> GetPackages(IPackageRepository repository, string searchTerm)
        {
            return GetPackages(repository.GetPackages(), searchTerm);
        }

        internal static IQueryable<IPackage> GetPackages(IQueryable<IPackage> packages, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                packages = packages.Find(searchTerm);
            }
            return packages;
        }

        internal IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IPackage package)
        {
            IPackageRepository localRepository = LocalRepository;
            IPackageRepository sourceRepository = SourceRepository;
            return GetPackagesRequiringLicenseAcceptance(package, localRepository, sourceRepository);
        }

        internal static IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IPackage package, IPackageRepository localRepository, IPackageRepository sourceRepository)
        {
            return (from p in GetPackageDependencies(package, localRepository, sourceRepository)
                    where p.RequireLicenseAcceptance
                    select p);
        }

        public IQueryable<IPackage> GetPackagesWithUpdates(string searchTerms)
        {
            return GetPackages(LocalRepository.GetUpdates(
                SourceRepository.GetPackages(),
                includePrerelease: true,
                includeAllVersions: true)
            .AsQueryable(), searchTerms);
        }

        public IQueryable<IPackage> GetRemotePackages(string searchTerms)
        {
            return GetPackages(SourceRepository, searchTerms);
        }

        public IPackage GetUpdate(IPackage package)
        {
            return SourceRepository.GetUpdates(
                LocalRepository.GetPackages(),
                includePrerelease: true,
                includeAllVersions: true)
            .FirstOrDefault(p => (package.Id == p.Id));
        }

        internal static string GetWebRepositoryDirectory(string siteRoot)
        {
            return Path.Combine(siteRoot, "App_Data", "packages");
        }

        public IEnumerable<string> InstallPackage(IPackage package)
        {
            return PerformLoggedAction(
                () => _projectManager.AddPackageReference(
                    package,
                    ignoreDependencies: false,
                    allowPrereleaseVersions: true));
        }

        public bool IsPackageInstalled(IPackage package)
        {
            return LocalRepository.Exists(package);
        }

        private IEnumerable<string> PerformLoggedAction(Action action)
        {
            var logger = new ErrorLogger();
            _projectManager.Logger = logger;
            try
            {
                action();
            }
            finally
            {
                _projectManager.Logger = null;
            }
            return logger.Errors;
        }

        public IEnumerable<string> UninstallPackage(IPackage package, bool removeDependencies)
        {
            return PerformLoggedAction(
                () =>
                    _projectManager.RemovePackageReference(package.Id,
                        forceRemove: false,
                        removeDependencies: removeDependencies));
        }

        public IEnumerable<string> UpdatePackage(IPackage package)
        {
            return PerformLoggedAction(
                () =>
                    _projectManager.UpdatePackageReference(
                        package.Id,
                        package.Version,
                        updateDependencies: true,
                        allowPrereleaseVersions: true));
        }

        // Properties
        public IPackageRepository LocalRepository
        {
            get
            {
                return _projectManager.LocalRepository;
            }
        }

        public IPackageRepository SourceRepository
        {
            get
            {
                return _projectManager.SourceRepository;
            }
        }

        // Nested Types
        private class ErrorLogger : ILogger
        {
            // Fields
            private readonly IList<string> _errors = new List<string>();

            // Methods
            public void Log(MessageLevel level, string message, params object[] args)
            {
                if (level == MessageLevel.Warning)
                {
                    _errors.Add(string.Format(CultureInfo.CurrentCulture, message, args));
                }
            }

            // Properties
            public IEnumerable<string> Errors
            {
                get
                {
                    return _errors;
                }
            }

            public FileConflictResolution ResolveFileConflict(string message)
            {
                // TODO: Whatever I'm supposed to do here.
                throw new NotImplementedException();
            }
        }
    }



}